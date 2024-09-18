using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Launcher;

public static class Log4JProcessor
{
    private static List<string> _messageParts = new();
    private static object _messagePartsLock = new();
    const string CloseTag = "</log4j:Event>";
    const string OpenTag = "<log4j:Event";
    
    private static bool _isOpen;
    public static bool IsLog4JOutput(string msg)
    {
        var trim = msg.Trim();
        return _isOpen || trim.StartsWith("<log4j:") || trim.StartsWith("</log4j:");
    }

    public static void ProcessLog4JOutput(string msg)
    {
        lock (_messagePartsLock)
        {
            var trim = msg.Trim();
            if (trim.StartsWith(OpenTag))
            {
                if (_messageParts.Any())
                {
                    _messageParts.Add(CloseTag);
                    TryPrintMessage();
                    Close();
                }
                _isOpen = true;
            }
        
            _messageParts.Add(trim);
        
            if (trim.Trim().StartsWith("</log4j:Event>"))
            {
                TryPrintMessage();
            }
        }
    }

    private static void TryPrintMessage()
    {
        var sb = new StringBuilder();
        _messageParts.ForEach(x => sb.AppendLine(x));
        var xmlText = sb.ToString();
        var nslessText = xmlText.Replace("log4j:", "");
        var fineText = nslessText.Trim();

        try
        {
            XDocument doc = XDocument.Parse(nslessText);
            // Retrieve the 'Event' element using XName.Get to ignore namespace
            var logEvent = doc.Root;

            if (logEvent != null)
            {
                // Retrieve the 'thread' and 'level' attributes
                string thread = logEvent.Attribute(XName.Get("thread"))?.Value;
                string level = logEvent.Attribute(XName.Get("level"))?.Value;

                // Retrieve the message content from the 'Message' element
                string message = logEvent.Element(XName.Get("Message", ""))?.Value;

                if(level == "INFO")
                    Log.Info($"[{thread}/{level}]: {message}");
                else if(level == "WARN")
                    Log.Warning($"[{thread}/{level}]: {message}");
                else if(level == "ERROR")
                    Log.Error($"[{thread}/{level}]: {message}");

                Close();
            }
            else
            {
                Close();
            }
        }
        catch (Exception e)
        {
            Log.Warning($"Failed to parse log message: {e.Message}\n{xmlText}");
            _messageParts.RemoveAt(_messageParts.Count - 1);
            //_messageParts.Clear();
        }
    }

    private static void Close()
    {
        _messageParts.Clear();
        _isOpen = false;
    }
}