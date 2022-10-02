namespace DAMTool.Helpers;
using System.Diagnostics;

internal sealed class ProcessHandler
{
    public readonly string FFMPEG_START_CALL;
    public ProcessHandler()
    {
        if (Environment.OSVersion.Platform == PlatformID.Win32NT)
        { FFMPEG_START_CALL = "ffmpeg.exe "; }
        else if (Environment.OSVersion.Platform == PlatformID.Unix)
        { FFMPEG_START_CALL = "ffmpeg"; }
        // { FFMPEG_START_CALL = "nice -n 19 ffmpeg "; }
        else
        { throw new PlatformNotSupportedException(); }


    }


    public async Task<List<string>> StartProcessAndWaitForExitAsync(string procFileName, string procArgs, CancellationToken token = default, bool redirectStandardError = true, bool printOutputToConsoleInRealTime = false, bool returnOutputAtEnd = true)
    {
        List<string> result = Array.Empty<string>().ToList();
        
        Process proc = new();
        ProcessStartInfo psi = new();
        psi.RedirectStandardOutput = true;
        psi.RedirectStandardError = redirectStandardError;
        psi.FileName = procFileName;
        psi.Arguments = procArgs;
        psi.UseShellExecute = false;

        proc.StartInfo = psi;

        if (printOutputToConsoleInRealTime || returnOutputAtEnd)
        {
            proc.Start();
            proc.BeginOutputReadLine();
            proc.BeginErrorReadLine();
            DataReceivedEventHandler dataReceivedEventHandler = (s, e) =>
            {
                if (e.Data != null && e.Data.GetType() != typeof(string) || string.IsNullOrWhiteSpace(e.Data))
                { return; }

                if (printOutputToConsoleInRealTime)
                { Console.WriteLine(e.Data); }
                if (returnOutputAtEnd)
                { result.Add(e.Data); }
            };

            proc.ErrorDataReceived += dataReceivedEventHandler;
            proc.OutputDataReceived += dataReceivedEventHandler;

        }
        else
        {
            proc.Start();
        }

        await proc.WaitForExitAsync(token);
        return result;
    }

}