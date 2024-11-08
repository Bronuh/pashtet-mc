using IO;
using KludgeBox.Events.EventTypes;

namespace PatchApi.Events;

public class HttpGetRequestSendingEvent : IEvent
{
    public string Url { get; set; }
    public Dictionary<string, string> CustomHeaders { get; set; }
    public double TimeoutSeconds { get; set; }
    public bool IgnoreException { get; set; }

    public HttpGetRequestSendingEvent(string url, Dictionary<string, string> customHeaders = null,
        double timeoutSeconds = 0, bool ignoreException = false)
    {
        Url = url;
        CustomHeaders = customHeaders;
        TimeoutSeconds = timeoutSeconds;
        IgnoreException = ignoreException;
    }
}

public class HttpGetResponseReceivedEvent : IEvent
{
    public string Url { get; }
    public Dictionary<string, string> CustomHeaders { get; }
    public double TimeoutSeconds { get; }
    public bool IgnoreException { get; }
    public HttpResponse HttpResponse { get; set; }

    public HttpGetResponseReceivedEvent(string url, Dictionary<string, string> customHeaders = null,
        double timeoutSeconds = 0, bool ignoreException = false, HttpResponse httpResponse = null)
    {
        Url = url;
        CustomHeaders = customHeaders;
        TimeoutSeconds = timeoutSeconds;
        IgnoreException = ignoreException;
        HttpResponse = httpResponse;
    }
}