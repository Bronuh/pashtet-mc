#region

using System.Net.Http;
using System.Threading.Tasks;
using HttpClient = System.Net.Http.HttpClient;

#endregion

namespace IO;


public class HttpResponse
{
    public string Body { get; set; }
    public Dictionary<string, IEnumerable<string>> Headers { get; set; }
}

public static class HttpHelper
{
    public static double HttpGetTimeoutSeconds { get; set; } = 2;

    /// <summary>
    /// Synchronous GET request with additional headers.
    /// </summary>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="customHeaders">Optional custom headers to include in the request.</param>
    /// <param name="timeoutSeconds"></param>
    /// <param name="ignoreException"></param>
    /// <returns>HttpResponse object containing body and headers.</returns>
    public static HttpResponse Get(string url, Dictionary<string, string> customHeaders = null, double timeoutSeconds = 0, bool ignoreException = false)
    {
        var client = new HttpClient();
        client.Timeout = timeoutSeconds == 0 ? TimeSpan.FromSeconds(HttpGetTimeoutSeconds) : TimeSpan.FromSeconds(timeoutSeconds);
        try
        {
            // Add custom headers if any
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = client.GetAsync(url).Result; // Sync call
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
            if(!ignoreException)
                throw new Exception($"Error during GET request: {ex.Message}", ex);
            
            return null;
        }
    }

    /// <summary>
    /// Asynchronous GET request with additional headers.
    /// </summary>
    /// <param name="url">The URL to send the request to.</param>
    /// <param name="customHeaders">Optional custom headers to include in the request.</param>
    /// <returns>HttpResponse object containing body and headers.</returns>
    public static async Task<HttpResponse> GetAsync(string url, Dictionary<string, string> customHeaders = null, double timeoutSeconds = 0, bool ignoreException = false)
    {
        var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(HttpGetTimeoutSeconds);
        client.Timeout = timeoutSeconds == 0 ? TimeSpan.FromSeconds(HttpGetTimeoutSeconds) : TimeSpan.FromSeconds(timeoutSeconds);
        try
        {
            // Add custom headers if any
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    client.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }

            HttpResponseMessage response = await client.GetAsync(url); // Async call
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
            if(!ignoreException)
                throw new Exception($"Error during GET request: {ex.Message}", ex);
            
            return null;
        }
    }
}