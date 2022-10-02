using System.Globalization;
using System.Text;
using System;

namespace DAMTool.Helpers;

internal static class PathAutoComplete
{
    public static string PathAutoCompleteReadLine()
    {
        string path = "";
        var input = Console.ReadKey(intercept: true);

        while (input.Key != ConsoleKey.Enter)
        {
            var currentInput = path.ToString();
            if (input.Key == ConsoleKey.Tab)
            {
                var childrenOfPath = GetChildrenOfPath(path);

                var matches = childrenOfPath?.Where(item => item != currentInput && item.StartsWith(currentInput, StringComparison.InvariantCultureIgnoreCase)).ToArray();
                string? match = matches?.Count() == 1 ? matches[0] : GetConsecutiveIntersectingSubstringFromStart(matches ?? Array.Empty<string>());
                if (string.IsNullOrEmpty(match))
                {
                    input = Console.ReadKey(intercept: true);
                    continue;
                }

                ClearCurrentLine();
                path = "";

                Console.Write(match);
                path += match;
            }
            else
            {
                if (input.Key == ConsoleKey.Backspace && currentInput.Length > 0)
                {
                    path = path.Remove(path.Length - 1);
                    ClearCurrentLine();

                    currentInput = currentInput.Remove(currentInput.Length - 1);
                    Console.Write(currentInput);
                }
                else
                {
                    var key = input.KeyChar;
                    path += key;
                    Console.Write(key);
                }
            }

            input = Console.ReadKey(intercept: true);
        }

        Console.Write(Environment.NewLine);
        return path;
    }

    private static void ClearCurrentLine()
    {
        var currentLine = Console.CursorTop;
        Console.SetCursorPosition(0, currentLine);
        Console.Write(new string(' ', Console.WindowWidth - 1));
        Console.SetCursorPosition(0, currentLine);
    }

    private static HashSet<string>? GetChildrenOfPath(string path)
    {
        path = path.Contains('/') ? path.Substring(0, path.LastIndexOf('/') + 1) : "/";
        HashSet<string> result = new();

        try
        {
            result = result.Concat(Directory.GetFiles(path, "*.*", SearchOption.TopDirectoryOnly)).ToHashSet();
        }
        catch (System.Exception)
        { }

        try
        {
            var directories = Directory.GetDirectories(path, "*.*", searchOption: SearchOption.TopDirectoryOnly).ToArray();

            for (int i = 0; i < directories.Length; i++)
            {
                directories[i] = directories[i] + '/';
            }
            result = result.Concat(directories).ToHashSet();

        }
        catch (System.Exception)
        { }

        return result;
    }

    public static string GetConsecutiveIntersectingSubstringFromStart(params string[] values)
    {
        if(values == null || values.Length == 0 || values.Any(x=>string.IsNullOrWhiteSpace(x)))
        {return string.Empty;}

        StringBuilder result = new();

        for (int i = 0; i < values.Min(x=>x.Length); i++)
        {
            if(values.All(x=>Char.ToLowerInvariant(x[i]) == char.ToLowerInvariant(values[0][i])))
            { result.Append(values[0][i]); }
            else
            { break; }
        }
        return result.ToString();
    }

}