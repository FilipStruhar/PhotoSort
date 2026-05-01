using PhotoSort.Models;
using Spectre.Console;
using System.Globalization;

namespace PhotoSort.Services;

/// <summary>
/// Zajišťuje fyzické kopírování souborů do strukturovaného archivu na disku.
/// </summary>
public class PhotoCopier
{
    private readonly UserSettings _settings;

    /// <summary>
    /// Inicializuje novou instanci třídy PhotoCopier.
    /// </summary>
    /// <param name="settings">Aktuální uživatelské nastavení obsahující cílovou cestu.</param>
    public PhotoCopier(UserSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Projde analyzovaný archiv a zkopíruje soubory do cílových složek.
    /// </summary>
    /// <param name="archive">Naplněná datová struktura s roztříděnými fotkami.</param>
    public void CopyFiles(PhotoArchive archive)
    {
        if (string.IsNullOrWhiteSpace(_settings.DestinationPath))
        {
            AnsiConsole.MarkupLine("[red]Error: Destination path is not set![/]");
            return;
        }

        AnsiConsole.Status()
            .Start("Copying files...", ctx =>
            {
                foreach (var monthEntry in archive.Data)
                {
                    // monthEntry.Key je např. "2026-05"
                    foreach (var dayEntry in monthEntry.Value)
                    {
                        // Vezmeme první fotku v daném dni, abychom zjistili název měsíce
                        var samplePhoto = dayEntry.Value[0];
                        if (!samplePhoto.DateTaken.HasValue) continue;

                        DateTime date = samplePhoto.DateTaken.Value;

                        // Sestavení názvů složek podle zadání
                        // [YYYY-MM_MonthName] -> 2026-05_May
                        string monthFolderName = $"{date:yyyy-MM}_{date:MMMM}";
                        // [YYYY-MM-DD] -> 2026-05-13
                        string dayFolderName = date.ToString("yyyy-MM-dd");

                        // Bezpečné složení cesty
                        string targetDirectory = Path.Combine(
                            _settings.DestinationPath, 
                            monthFolderName, 
                            dayFolderName
                        );

                        // Pokud složka neexistuje, vytvoříme ji (i s nadřazenými složkami)
                        if (!Directory.Exists(targetDirectory))
                        {
                            Directory.CreateDirectory(targetDirectory);
                        }

                        // Kopírování všech fotek daného dne
                        foreach (var photo in dayEntry.Value)
                        {
                            string destFile = Path.Combine(targetDirectory, photo.FileName);

                            // Kopírujeme jen pokud soubor v cíli neexistuje (prevence přepsání)
                            if (!File.Exists(destFile))
                            {
                                File.Copy(photo.OriginalPath, destFile);
                                AnsiConsole.MarkupLine($"[grey]Copying:[/] {photo.FileName} -> [green]{dayFolderName}[/]");
                            }
                        }
                    }
                }
            });

        AnsiConsole.MarkupLine("[bold green]All files were successfully copied![/]");
    }
}