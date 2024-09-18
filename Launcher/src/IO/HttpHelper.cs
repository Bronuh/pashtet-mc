using System.Net.Http;
using System.Threading.Tasks;
using HttpClient = System.Net.Http.HttpClient;

namespace IO;


public class HttpResponse
{
    public string Body { get; set; }
    public Dictionary<string, IEnumerable<string>> Headers { get; set; }
}


public static class HttpHelper
{

    /// <summary>
    /// Synchronous GET request.
    /// </summary>
    /// <param name="url">The URL to send the request to.</param>
    /// <returns>HttpResponse object containing body and headers.</returns>
    public static HttpResponse Get(string url)
    {
        var _client = new HttpClient();
        try
        {
            HttpResponseMessage response = _client.GetAsync(url).Result; // Sync call
            response.EnsureSuccessStatusCode();
            string body = response.Content.ReadAsStringAsync().Result;  // Get response body
            var headers = response.Headers;  // Get headers

            // Convert headers to a dictionary
            var headersDict = new Dictionary<string, IEnumerable<string>>();
            foreach (var header in headers)
            {
                headersDict[header.Key] = header.Value;
            }

            return new HttpResponse
            {
                Body = body,
                Headers = headersDict
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during GET request: {ex.Message}");
        }
    }

    /// <summary>
    /// Asynchronous GET request.
    /// </summary>
    /// <param name="url">The URL to send the request to.</param>
    /// <returns>HttpResponse object containing body and headers.</returns>
    public static async Task<HttpResponse> GetAsync(string url)
    {
        var _client = new HttpClient();
        try
        {
            HttpResponseMessage response = await _client.GetAsync(url); // Async call
            response.EnsureSuccessStatusCode();
            string body = await response.Content.ReadAsStringAsync();  // Get response body
            var headers = response.Headers;  // Get headers

            // Convert headers to a dictionary
            var headersDict = new Dictionary<string, IEnumerable<string>>();
            foreach (var header in headers)
            {
                headersDict[header.Key] = header.Value;
            }

            return new HttpResponse
            {
                Body = body,
                Headers = headersDict
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during GET request: {ex.Message}");
        }
    }
}