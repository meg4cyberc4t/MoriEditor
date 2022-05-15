// __  __                  _   _____       _   _   _                             
//|  \/  |   ___    _ __  (_) | ____|   __| | (_) | |_    ___    _ __            
//| |\/| |  / _ \  | '__| | | |  _|    / _` | | | | __|  / _ \  | '__|           
//| |  | | | (_) | | |    | | | |___  | (_| | | | | |_  | (_) | | |              
//|_|  |_|  \___/  |_|    |_| |_____|  \__,_| |_|  \__|  \___/  |_|    
//
// The code is published for demonstration. According to the license,
// the modification of the code requires the indication of the author.
// Love to all.
//                                              by Igor Molchanov

using MoriEditor;
using Spectre.Console;
using Spectre.Console.Cli;

try
{
    var app = new CommandApp<EditorCommandApp>();
    return app.Run(args);
}
catch (Exception error)
{
    AnsiConsole.Write(new Markup($"[red bold]{error.Message}[/]"));
    return error.HResult;
}
