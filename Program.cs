using PhotoSort.Services;
using PhotoSort.UI;
using Spectre.Console;

try
{
    // NASTAVENÍ KÓDOVÁNÍ
    Console.OutputEncoding = System.Text.Encoding.UTF8;

    // INICIALIZACE SLUŽEB
    // Vytvoříme instanci ConfigService, která se bude starat o XML soubor
    var configService = new XmlConfigService();

    // SPUŠTĚNÍ UI
    // Předáme ConfigService hlavnímu menu a zavoláme jeho zobrazení
    var mainMenu = new MainMenu(configService);
    mainMenu.Show();
}
catch (Exception ex)
{
    // CENTRÁLNÍ FAILOVER
    AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
    
    AnsiConsole.MarkupLine("\n[red]Application encountered a critical error and must be terminated.[/]");
    Console.ReadKey();
}