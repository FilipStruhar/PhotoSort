using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using PhotoSort.Models;
using PhotoSort.Models.Enums;
using Spectre.Console;
using Directory = System.IO.Directory;

namespace PhotoSort.Services;

/// <summary>
/// Handles scanning files and extracting metadata for sorting.
/// </summary>
public class PhotoAnalyzer
{
    private readonly UserSettings _settings; // Loaded settings used by class methods
    
    // RAW file extensions recognized by the app
    private readonly string[] _rawExtensions = 
    { 
        // Canon, Nikon, Sony
        ".crw", ".cr2", ".cr3", ".nef", ".nrw", ".arw", ".srf", ".sr2", 
        // Olympus, Panasonic, Fujifilm
        ".orf", ".rw2", ".raf", 
        // Pentax, Samsung, Minolta
        ".pef", ".ptx", ".srw", ".mrw", 
        // Universal (Adobe, Leica, mobile) and other
        ".dng", ".raw" 
    };

    private readonly string[] _normalExtensions = 
    { 
        ".jpg", ".jpeg", ".png", ".heic", ".heif", ".webp", ".tif", ".tiff", ".bmp"
    };

    public PhotoAnalyzer(UserSettings settings)
    {
        _settings = settings;
    }

    /// <summary>
    /// Checks whether the source path is configured.
    /// </summary>
    public bool HasSourcePath()
    {
        return !string.IsNullOrWhiteSpace(_settings.SourcePath);
    }

    /// <summary>
    /// Scans the source folder and returns a sorted photo archive.
    /// </summary>
    /// <returns>PhotoArchive filled with data.</returns>
    public PhotoArchive GetAnalyzedFiles()
    {
        var archive = new PhotoArchive();

        if (!HasSourcePath() || !Directory.Exists(_settings.SourcePath))
        {
            AnsiConsole.MarkupLine("[yellow]Warning: Source path does not exist or is not accessible.[/]");
            AnsiConsole.MarkupLine("[yellow]Please check the source path in the settings.[/]\n");
            return archive;
        }
        try
        {
            // Get all files in the source folder
            string[] files = Directory.GetFiles(_settings.SourcePath);

            foreach (string filePath in files)
            {
                try
                {
                    string extension = Path.GetExtension(filePath).ToLower();
                    bool isRaw = _rawExtensions.Contains(extension);
                    bool isNormal = _normalExtensions.Contains(extension);

                    if (!isRaw && !isNormal)
                    {
                        continue;
                    }

                    // Filtering: decide whether to process this file
                    if (!ShouldProcess(isRaw)) continue;

                    // Extract date: EXIF -> fallback to file creation time
                    DateTime? dateTaken = GetDateFromExif(filePath);
                    if (!dateTaken.HasValue)            
                    {
                        dateTaken ??= File.GetCreationTime(filePath);
                    }

                    // Create photo model
                    var photo = new PhotoFile(
                        filePath, // OriginalPath
                        Path.GetFileName(filePath), // FileName
                        dateTaken, // DateTaken
                        isRaw // IsRaw
                    );

                    // Add to archive
                    archive.AddPhoto(photo);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]Failed to process {Path.GetFileName(filePath)}:[/] {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex, ExceptionFormats.ShortenEverything | ExceptionFormats.ShowLinks);
        }

        return archive;
    }

    /// <summary>
    /// Helper method to decide whether a file should be processed based on settings.
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
    /// Attempts to read the capture date from EXIF metadata using MetadataExtractor.
    /// </summary>
    private DateTime? GetDateFromExif(string filePath)
    {
        try
        {
            var directories = ImageMetadataReader.ReadMetadata(filePath); // Read metadata from the photo
            var subIfdDirectory = directories.OfType<ExifSubIfdDirectory>().FirstOrDefault(); // Look for "EXIF SubIFD" where capture date is usually stored

            if (subIfdDirectory != null && subIfdDirectory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime)) // Try to read "TagDateTimeOriginal"
            {
                return dateTime;
            }

            return null; // If EXIF has no date, return null and use fallback
        }
        catch
        {
            return null; // On error return null and use fallback
        }
    }
}