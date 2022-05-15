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
        int x = 0, y = 1;
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
            AnsiConsole.Cursor.SetPosition(x + 1, y + 1);
            var keyInfo = Console.ReadKey();

            var column = x - 1;
            var row = y - 1;

            var buffer = fileText.Split('\n').ToList();

            switch (keyInfo.Key)
            {
                case ConsoleKey.Enter:
                    if ((keyInfo.Modifiers & ConsoleModifiers.Control) != 0)
                        continue;

                    AnsiConsole.Cursor.SetPosition(0, buffer.Count + 1);

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

                    buffer[row] = buffer[row].Remove(column, 1);
                    break;

                case ConsoleKey.Delete:
                    if (fileText.Length == 0) continue;

                    buffer[row] = buffer[row].Remove(column + 1, 1);
                    break;
                case ConsoleKey.UpArrow:
                    AnsiConsole.Cursor.MoveUp();
                    break;
                case ConsoleKey.DownArrow:
                    AnsiConsole.Cursor.MoveDown();
                    break;
                case ConsoleKey.LeftArrow:
                    AnsiConsole.Cursor.MoveLeft();
                    break;
                case ConsoleKey.RightArrow:
                    AnsiConsole.Cursor.MoveRight();
                    break;
                default:

                    if (!char.IsControl(keyInfo.KeyChar))
                    {
                        while (buffer.Count <= row)
                        {
                            buffer.Add("");
                        }

                        if (buffer[row].Length <= column)
                        {
                            for (int i = 0; i <= column; i++)
                            {
                                buffer[row] += " ";
                            }
                        }
                        buffer[row] = buffer[row].Insert(column + 1, keyInfo.KeyChar.ToString());
                    }
                    break;
            }

            fileText = string.Join('\n', buffer);
            (x, y) = Console.GetCursorPosition();
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