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
                    _settings.DestinationPath = PromptForPath("Enter destination folder:", true);
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
    /// Helper method to get and validate a path from the user.
    /// </summary>
    private string PromptForPath(string message, bool mustExist)
    {
        // Step 1: Get text input from the user.
        // Spectre.Console validates while the user types.
        string rawInput = AnsiConsole.Prompt(
            new TextPrompt<string>(message)
                .Validate(path =>
                {
                    string expandedPath = ResolvePath(path); // Normalize to an absolute path before validating
                    
                    if (mustExist && !Directory.Exists(expandedPath))
                    {
                        return ValidationResult.Error("[red]Error: Folder does not exist![/]");
                    }

                    try
                    {
                        // Basic check for invalid path characters
                        new DirectoryInfo(expandedPath);
                        return ValidationResult.Success();
                    }
                    catch
                    {
                        return ValidationResult.Error("[red]Error: Invalid path format![/]");
                    }
                }));

        string resolvedPath = ResolvePath(rawInput); // Normalize to an absolute path before saving
        return resolvedPath;
    }

    /// <summary>
    /// Converts relative paths and unix '~' to an absolute path.
    /// </summary>
    private string ResolvePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return path;

        path = path.Trim('\'', '"'); // Remove wrapping quotes used around paths with spaces
        path = path.Replace("\\ ", " "); // Undo shell-escaped spaces (e.g., "/Volumes/NIKON\ D3300")

        // Support for Unix/Mac home directory '~'
        if (path.StartsWith("~"))
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            path = Path.Combine(homeDir, path.TrimStart('~', '/', '\\'));
        }

        // Convert to an absolute path (also resolves relative paths like "./photos")
        return Path.GetFullPath(path);
    }
}