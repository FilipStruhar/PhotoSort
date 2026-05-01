using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PhotoSort.Models;
using PhotoSort.Models.Enums;
using Spectre.Console;

namespace PhotoSort.Services;

/// <summary>
/// Zajišťuje procházení souborů a extrakci metadat pro účely třídění.
/// </summary>
public class PhotoAnalyzer
{
    private readonly UserSettings _settings; // Načtení nastavení pro použití v metodách třídy
    
    // Definice RAW přípon, které aplikace rozpoznává
    private readonly string[] _rawExtensions = 
    { 
        // Canon, Nikon, Sony
        ".crw", ".cr2", ".cr3", ".nef", ".nrw", ".arw", ".srf", ".sr2", 
        // Olympus, Panasonic, Fujifilm
        ".orf", ".rw2", ".raf", 
        // Pentax, Samsung, Minolta
        ".pef", ".ptx", ".srw", ".mrw", 
        // Univerzální (Adobe, Leica, Mobily) a ostatní
        ".dng", ".raw" 
    };

    public PhotoAnalyzer(UserSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Analyzuje zdrojovou složku a vrací roztříděný archiv fotografií.
    /// </summary>
    /// <returns>Objekt PhotoArchive naplněný daty.</returns>
    public PhotoArchive GetAnalyzedFiles()
    {
        var archive = new PhotoArchive();

        if (!Directory.Exists(_settings.SourcePath))
        {
            AnsiConsole.MarkupLine("[yellow]Varování: Zdrojová cesta neexistuje nebo není přístupná.[/]");
            return archive;
        }

        try
        {
            // Získáme všechny soubory ve zdrojové složce
            string[] files = Directory.GetFiles(_settings.SourcePath);

            foreach (string filePath in files)
            {
                string extension = Path.GetExtension(filePath).ToLower();
                bool isRaw = _rawExtensions.Contains(extension);

                // Filtrování: Rozhodneme, zda chceme tento soubor zpracovat
                if (!ShouldProcess(isRaw)) continue;

                // Extrakce data: EXIF -> Fallback na datum vytvoření souboru
                DateTime? dateTaken = GetDateFromExif(filePath);
                if (!dateTaken.HasValue)            
                {
                    dateTaken ??= File.GetCreationTime(filePath);
                }

                // Vytvoření modelu fotky
                var photo = new PhotoFile(
                    filePath, // OriginalPath
                    Path.GetFileName(filePath), // FileName
                    dateTaken, // DateTaken
                    isRaw // IsRaw
                );

                // Zařazení do archivu
                archive.AddPhoto(photo);
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
        }

        return archive;
    }

    /// <summary>
    /// Pomocná metoda pro určení, zda má být soubor zpracován na základě nastavení.
    /// </summary>
    private bool ShouldProcess(bool isRaw)
    {
        switch (_settings.Mode)
        {
            case SortMode.Normal:
                return !isRaw;
            case SortMode.Raw:
                return isRaw;
            case SortMode.Both:
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Pokusí se přečíst datum pořízení snímku z EXIF metadat pomocí MetadataExtractoru.
    /// </summary>
    private DateTime? GetDateFromExif(string filePath)
    {
        try
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath); // Načtení metadat z fotografie
            var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault(); // Hledáme "EXIF SubIFD", kde se obvykle nachází datum pořízení

            if (subIfdDirectory != null && subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime)) // Pokus o získání data pořízení z "TagDateTimeOriginal"
            {
                return dateTime;
            }

            return null; // Pokud EXIF neobsahuje datum, vrátíme null a použijeme fallback
        }
        catch
        {
            return null; // Při chybě vrátíme null a použijeme fallback
        }
    }
}