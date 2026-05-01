using PhotoSort.Services;
using Spectre.Console;

namespace PhotoSort.UI;

/// <summary>
/// Main UI hub for the application.
/// </summary>
public class MainMenu
{
    private readonly XmlConfigService _configService;

    public MainMenu(XmlConfigService configService)
    {
        _configService = configService;
    }

    public void Show()
    {
        bool exit = false;

        while (!exit)
        {
            AnsiConsole.Clear();
            
            // Render the large PhotoSort title
            AnsiConsole.Write(
                new FigletText("PhotoSort")
                    .LeftJustified()
                    .Color(Color.Blue));

            AnsiConsole.MarkupLine("[blue]Welcome to PhotoSort![/]");
            AnsiConsole.MarkupLine("[grey]Sort memories fast. Keep originals safe. Export with confidence.[/]\n");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Main menu:")
                    .AddChoices(
                        "Export photos",
                        "Settings",
                        "Exit"
                    ));

            switch (choice)
            {
                case "Export photos":
                    var currentSettings = _configService.LoadSettings();
                    
                    var analyzer = new PhotoAnalyzer(currentSettings);
                    var copier = new PhotoCopier(currentSettings);
                    var exportMenu = new ExportMenu(analyzer, copier);
                    
                    exportMenu.Show();
                    break;

                case "Settings":
                    var settingsMenu = new SettingsMenu(_configService);
                    settingsMenu.Show();
                    break;

                case "Exit":
                    exit = true;
                    AnsiConsole.MarkupLine("\n[blue]Thank you for using PhotoSort. Have a great day![/]");
                    break;
            }
        }
    }
}