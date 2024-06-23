using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;
namespace Coincap.Helpers;

public class HttpConnection
{
    public async Task<T?> WebConnection<T>(string url, string requestType, object? payload = null, 
        Dictionary<string, string>? headers = null, string? authuser = null, string? authpassword = null
        ) where T : new()
    {
        T? result = new();  //Anonymous object with a Generic class T
		try
		{ //We make use of Using to dispose or close unnecessary resources after opening 
         // a connection using HttpClient
          using HttpClient client = new HttpClient();
          HttpRequestMessage requestMessage = new HttpRequestMessage
          {
              RequestUri = new Uri( url )
          };
            switch (requestType.ToLower())
            {
                case "post":
                    requestMessage.Method = HttpMethod.Post;
                    break;
                case "get":
                    requestMessage.Method = HttpMethod.Get;
                    break;
                case "put":
                    requestMessage.Method = HttpMethod.Put;
                    break;
                case "delete":
                    requestMessage.Method = HttpMethod.Delete;
                    break;
                case "patch":
                    requestMessage.Method = HttpMethod.Patch;
                    break;
                default:
                    throw new ArgumentException("Invalid request type");
               
            }
            if (payload != null)
            {
                var jsonPayload = JsonSerializer.Serialize(payload);
                requestMessage.Content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                    
            }
            if (headers != null)
            {
                foreach (var item in headers)
                {
                    requestMessage.Headers.Add(item.Key, item.Value);
                }
            }
            if (authuser != null && authpassword != null)
            {
                var byteArray = Encoding.ASCII.GetBytes($"{authuser}:{authpassword}");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            }
            HttpResponseMessage response = await client.SendAsync(requestMessage);
            var responseContent = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode)
            {
                result = JsonSerializer.Deserialize<T>(responseContent);
            }

        }
		catch (Exception)
		{

			throw;
		}
    }
}
