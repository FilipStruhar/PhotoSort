using System.Xml.Serialization;
using PhotoSort.Models;
using Spectre.Console;

namespace PhotoSort.Services;

/// <summary>
/// Zajišťuje načítání a ukládání uživatelského nastavení do souboru XML.
/// </summary>
public class XmlConfigService
{
    private readonly string _fileName = "settings.xml"; // Deklarace názvu XML souboru pro uložení nastavení

    /// <summary>
    /// Uloží aktuální nastavení do souboru settings.xml.
    /// </summary>
    /// <param name="settings">Instance nastavení k uložení.</param>
    public void SaveSettings(UserSettings settings)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            using StreamWriter writer = new StreamWriter(_fileName);
            serializer.Serialize(writer, settings);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]Critical error in XmlConfigService:[/]");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
        }
    }

    /// <summary>
    /// Načte nastavení ze souboru. Pokud soubor neexistuje, vrátí výchozí nastavení.
    /// </summary>
    /// <returns>Objekt UserSettings s načtenými nebo výchozími daty.</returns>
    public UserSettings LoadSettings()
    {
        if (!File.Exists(_fileName))
        {
            // Soubor neexistuje, vrátíme objekt s defaultními hodnotami
            return new UserSettings();
        }

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            using StreamReader reader = new StreamReader(_fileName);
            
            var settings = (UserSettings?)serializer.Deserialize(reader);
            
            // Pokud se deserializace povedla, vrátíme výsledek, jinak nové nastavení
            return settings ?? new UserSettings();
        }
        catch (Exception ex)
        {
            // Při jakékoliv chybě (např. poškozený soubor) raději vrátíme default
            AnsiConsole.MarkupLine("[red]Critical error in XmlConfigService:[/]");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
            return new UserSettings();
        }
    }
}