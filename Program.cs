using PhotoSort.Services;
using PhotoSort.UI;
using Spectre.Console;

try
{
    // Encoding setup
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    // Service initialization
    // Create ConfigService instance for XML settings
    var configService = new XmlConfigService();

    // UI startup
    // Pass ConfigService to the main menu and show it
    var mainMenu = new MainMenu(configService);
    mainMenu.Show();
}
catch (Exception ex)
{
    // Global failover
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
    
    AnsiConsole.MarkupLine("\n[red]Application encountered a critical error and must be terminated.[/]");
    Console.ReadKey();
}