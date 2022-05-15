using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Spectre.Console;
using Spectre.Console.Cli;

namespace MoriEditor;

public class EditorCommandApp : Command<EditorCommandApp.Settings>
{
    private static void Editor(string filePath)
    {
        var fileReader = File.OpenText(filePath);
        var fileText = fileReader.ReadToEnd();
        fileReader.Close();
        while (true)
        {
            AnsiConsole.Clear();

            var path = new TextPath(filePath)
            {
                RootStyle = new Style(Color.Red),
                SeparatorStyle = new Style(Color.Green),
                StemStyle = new Style(Color.Blue),
                LeafStyle = new Style(Color.Yellow)
            };

            AnsiConsole.Write(path);
            AnsiConsole.Write(new Text(fileText));
            var keyInfo = Console.ReadKey();
            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    AnsiConsole.Status()
                        .Start("Saving...", ctx =>
                        {
                            ctx.Status("Saving...");
                            ctx.SpinnerStyle(Style.Parse("green"));
                            var fileStream = File.CreateText(filePath);
                            fileStream.Write(fileText);
                            fileStream.WriteLine();
                            fileStream.Close();
                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine("[green bold]File has been saved[/]");
                        });
                    return;
                case ConsoleKey.Backspace:
                    if (fileText.Length == 0) continue;
                    fileText = fileText.Remove(fileText.Length - 1);
                    break;
                default:
                    if (!char.IsControl(keyInfo.KeyChar))
                        fileText += keyInfo.KeyChar;
                    break;
            }
        }
    }

    private static string FileSelect(string currentDirectory)
    {
        var directoryInfos = new DirectoryInfo(currentDirectory).GetFiles();

        var fileName = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[green bold] Select a file for editing[/]:")
                .PageSize(10)
                .AddChoices(directoryInfos.Select(e => e.Name)));

        return fileName;
    }

    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(
            new FigletText("MoriEditor")
                .Color(Color.Green));
        var dirName = settings.DirName ?? Directory.GetCurrentDirectory();

        var directoryInfo = new DirectoryInfo(dirName);

        var fileName = settings.FileName ?? (settings.LogMode
            ? DateTime.Now.ToLocalTime().ToString(CultureInfo.CurrentCulture) + ".txt"
            : FileSelect(dirName));
        var filePath = Path.Combine(directoryInfo.FullName, fileName);

        var fileInfo = new FileInfo(filePath);
        if (!fileInfo.Exists) File.Create(filePath).Close();

        Editor(filePath);
        return 0;
    }

    public class Settings : CommandSettings
    {
        public Settings(string? dirName, string? fileName, bool logMode = false)
        {
            DirName = dirName;
            FileName = fileName;
            LogMode = logMode;
        }

        [CommandOption("-o|--open")]
        [Description("The name of your file. If null, a selection from the selected folder will be opened.")]
        public string? FileName { get; set; }

        [CommandOption("-d|--dir")]
        [Description("The path of your folder. If null, the current directory will be used.")]
        public string? DirName { get; set; }

        [CommandOption("-l|--logmode")]
        [Description("Instead of the file name, there will be a date and time")]
        [DefaultValue(false)]
        public bool LogMode { get; set; }
    }
}