using PhotoSort.Models;
using PhotoSort.Models.Enums;
using PhotoSort.Services;
using Spectre.Console;

namespace PhotoSort.UI;

public class SettingsMenu
{
    private readonly XmlConfigService _configService;
    private UserSettings _settings;

    public SettingsMenu(XmlConfigService configService)
    {
        _configService = configService;
        _settings = _configService.LoadSettings();
    }

    public void Show()
    {
        bool backToMain = false;
        while (!backToMain)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]PhotoSort Settings[/]"));

            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("Option");
            table.AddColumn("Value");
            table.AddRow("Source", string.IsNullOrEmpty(_settings.SourcePath) ? "[red]Not Set[/]" : _settings.SourcePath);
            table.AddRow("Destination", string.IsNullOrEmpty(_settings.DestinationPath) ? "[red]Not Set[/]" : _settings.DestinationPath);
            table.AddRow("Sort Mode", _settings.Mode.ToString());
            AnsiConsole.Write(table);

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("What would you like to change?")
                    .AddChoices("Modify Source Path", "Modify Destination Path", "Modify Sort Mode", "Save and Return"));

            switch (choice)
            {
                case "Modify Source Path":
                    _settings.SourcePath = PromptForPath("Enter source folder:", true);
                    break;

                case "Modify Destination Path":
                    _settings.DestinationPath = PromptForPath("Enter destination folder:", false);
                    break;

                case "Modify Sort Mode":
                    _settings.Mode = AnsiConsole.Prompt(
                        new SelectionPrompt<SortMode>()
                            .Title("Select mode:")
                            .AddChoices(Enum.GetValues<SortMode>()));
                    break;

                case "Save and Return":
                    _configService.SaveSettings(_settings);
                    backToMain = true;
                    break;
            }
        }
    }

    /// <summary>
    /// Pomocná metoda pro získání a validaci cesty od uživatele.
    /// </summary>
    private string PromptForPath(string message, bool mustExist)
    {
        // 1. KROK: Získáme textový vstup od uživatele.
        // Spectre.Console se postará o validaci přímo během psaní.
        string rawInput = AnsiConsole.Prompt(
            new TextPrompt<string>(message)
                .Validate(path =>
                {
                    string expandedPath = ResolvePath(path); // Přeložíme cestu, aby se validovala v absolutní formě
                    
                    if (mustExist && !Directory.Exists(expandedPath))
                    {
                        return ValidationResult.Error("[red]Error: Folder does not exist![/]");
                    }

                    try
                    {
                        // Test na nesmyslné znaky v cestě
                        new DirectoryInfo(expandedPath);
                        return ValidationResult.Success();
                    }
                    catch
                    {
                        return ValidationResult.Error("[red]Error: Invalid path format![/]");
                    }
                }));

        string resolvedPath = ResolvePath(rawInput); // Přeložíme cestu, aby se uložila v absolutní formě
        return resolvedPath;
    }

    /// <summary>
    /// Převede relativní cesty a unixové '~' na absolutní cestu.
    /// </summary>
    private string ResolvePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return path;

        // Podpora pro Unix/Mac home directory '~'
        if (path.StartsWith("~"))
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            path = Path.Combine(homeDir, path.TrimStart('~', '/', '\\'));
        }

        // Převedení na absolutní cestu (vyřeší i relativní cesty jako "./photos")
        return Path.GetFullPath(path);
    }
}