using PhotoSort.Models;
using Spectre.Console;
using System.Globalization;

namespace PhotoSort.Services;

/// <summary>
/// Handles copying files into a structured archive on disk.
/// </summary>
public class PhotoCopier
{
    private readonly UserSettings _settings;

    /// <summary>
    /// Initializes a new instance of the PhotoCopier class.
    /// </summary>
    /// <param name="settings">Current user settings containing the destination path.</param>
    public PhotoCopier(UserSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Walks the analyzed archive and copies files to target folders.
    /// </summary>
    /// <param name="archive">Filled data structure with sorted photos.</param>
    public void CopyFiles(PhotoArchive archive)
    {
        if (string.IsNullOrWhiteSpace(_settings.DestinationPath))
        {
            AnsiConsole.MarkupLine("[red]Error: Destination path is not set![/]");
            return;
        }

        try
        {
            Directory.CreateDirectory(_settings.DestinationPath);
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine("[red]Error: Destination path is not accessible or cannot be created.[/]");
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
            return;
        }

        AnsiConsole.Status()
            .Start("Copying files...", ctx =>
            {
                foreach (var monthEntry in archive.Data)
                {
                    // monthEntry.Key e.g. "2026-05"
                    foreach (var dayEntry in monthEntry.Value)
                    {
                        // Use the first photo of the day to determine the month name
                        var samplePhoto = dayEntry.Value[0];
                        if (!samplePhoto.DateTaken.HasValue) continue;

                        DateTime date = samplePhoto.DateTaken.Value;

                        // Build folder names
                        // [YYYY-MM_MonthName] -> 2026-05_May
                        string monthFolderName = $"{date:yyyy-MM}_{date:MMMM}";
                        // [YYYY-MM-DD] -> 2026-05-13
                        string dayFolderName = date.ToString("yyyy-MM-dd");

                        // Safe path combine
                        string targetDirectory = Path.Combine(
                            _settings.DestinationPath, 
                            monthFolderName, 
                            dayFolderName
                        );

                        // Create the folder if missing (including parents)
                        if (!Directory.Exists(targetDirectory))
                        {
                            try
                            {
                                Directory.CreateDirectory(targetDirectory);
                            }
                            catch (Exception ex)
                            {
                                AnsiConsole.MarkupLine($"[red]Failed to create folder {targetDirectory}:[/] {ex.Message}");
                                continue;
                            }
                        }

                        // Copy all photos for the day
                        foreach (var photo in dayEntry.Value)
                        {
                            string destFile = Path.Combine(targetDirectory, photo.FileName);

                            // Copy only if the destination file does not exist (avoid overwrites)
                            if (!File.Exists(destFile))
                            {
                                try 
                                {
                                    File.Copy(photo.OriginalPath, destFile);
                                    AnsiConsole.MarkupLine($"[grey]Copying:[/] {photo.FileName} -> [green]{dayFolderName}[/]");
                                }
                                catch (Exception ex)
                                {
                                    // Log the failure but don't stop the rest of the files from copying
                                    AnsiConsole.MarkupLine($"[red]Failed to copy {photo.FileName}:[/] {ex.Message}");
                                }
                            }
                        }
                    }
                }
            });

        AnsiConsole.MarkupLine("[bold green]All files were successfully copied![/]");
    }
}