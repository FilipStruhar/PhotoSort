using System.Xml.Serialization;
using PhotoSort.Models;
using Spectre.Console;

namespace PhotoSort.Services;

/// <summary>
/// Handles loading and saving user settings to an XML file.
/// </summary>
public class XmlConfigService
{
    private readonly string _fileName = "settings.xml"; // XML file name used to store settings

    /// <summary>
    /// Saves the current settings to settings.xml.
    /// </summary>
    /// <param name="settings">Settings instance to save.</param>
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
    /// Loads settings from a file. If the file does not exist, returns default settings.
    /// </summary>
    /// <returns>UserSettings object with loaded or default data.</returns>
    public UserSettings LoadSettings()
    {
        if (!File.Exists(_fileName))
        {
            // File does not exist, return a default instance
            return new UserSettings();
        }

        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(UserSettings));
            using StreamReader reader = new StreamReader(_fileName);
            
            var settings = (UserSettings?)serializer.Deserialize(reader);
            
            // If deserialization succeeded, return the result; otherwise return default settings
            return settings ?? new UserSettings();
        }
        catch (Exception ex)
        {
            // On any error (e.g., corrupted file) return defaults
            AnsiConsole.MarkupLine("[red]Critical error in XmlConfigService:[/]");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
            return new UserSettings();
        }
    }
}