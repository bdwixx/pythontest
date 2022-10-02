namespace DAMTool;

internal sealed class Program{
    static async Task Main()
    {
        Console.Write(Environment.NewLine);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(new string(' ',(Console.WindowWidth/2-4)) + "DAMTool" + new string(' ',(Console.WindowWidth/2-4)));
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("Pls enter path");
        var readLine = Helpers.PathAutoComplete.PathAutoCompleteReadLine();
        Console.WriteLine("starting ffmpeg process");
        Helpers.ProcessHandler ph = new();
        float treshold = 0.3f;
        await ph.StartProcessAndWaitForExitAsync( ph.FFMPEG_START_CALL,$" -hide_banner -i \"{readLine}\" -filter_complex \"select='gt(scene,\"{treshold}\")',metadata=print:file=-\" -f null -",printOutputToConsoleInRealTime:true);
        Console.ReadKey();
    }

}