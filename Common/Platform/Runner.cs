#region

using System.Diagnostics;
using System.Text;

#endregion

namespace Common.Platform;

public static class Runner
{
    public static RunResult Run(string command, bool showWindow = false)
    {
        // Handle commands with spaces in the executable path by considering quotes
        string executable;
        string arguments;

        if (command.StartsWith("\"") && command.IndexOf("\"", 1, StringComparison.Ordinal) > 1)
        {
            // Command is quoted, extract executable and arguments
            int closingQuoteIndex = command.IndexOf("\"", 1, StringComparison.Ordinal);
            executable = command.Substring(1, closingQuoteIndex - 1);
            arguments = command.Length > closingQuoteIndex + 1 ? command.Substring(closingQuoteIndex + 2) : String.Empty;
        }
        else
        {
            // Command is not quoted, split by first space
            var parts = command.Split(' ', 2);
            executable = parts[0];
            arguments = parts.Length > 1 ? parts[1] : String.Empty;
        }

        using var process = new Process();
        process.StartInfo = new ProcessStartInfo
        {
            FileName = executable,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = !showWindow
        };

        var stdOut = new StringBuilder();
        var stdErr = new StringBuilder();
        
        process.OutputDataReceived += (_, args) => stdOut.AppendLine(args.Data);
        process.ErrorDataReceived += (_, args) => stdErr.AppendLine(args.Data);
            
        try
        {
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
            int exitCode = process.ExitCode;
            return new RunResult(exitCode, stdOut.ToString(), stdErr.ToString());
        }
        catch (Exception ex)
        {
            return new RunResult(-1, String.Empty, ex.Message);
        }
    }
}

public record RunResult(int ExitCode, string StdOut, string StdErr);