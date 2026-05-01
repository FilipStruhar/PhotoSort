namespace PhotoSort.Models;

/// <summary>
/// Represents a photo to be processed.
/// </summary>
public class PhotoFile
{
    /// <summary> Original full path to the file. </summary>
    public string OriginalPath { get; set; } = string.Empty;

    /// <summary> File name (e.g., DSC_001.jpg). </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary> Capture date from EXIF or file creation time. </summary>
    public DateTime? DateTaken { get; set; } // Can be null when no date is available

    /// <summary> Flag indicating whether the file is RAW. </summary>
    public bool IsRaw { get; set; }

    /// <summary>
    /// Creates a new photo representation instance.
    /// </summary>
    public PhotoFile(string originalPath, string fileName, DateTime? dateTaken, bool isRaw)
    {
        OriginalPath = originalPath;
        FileName = fileName;
        DateTaken = dateTaken;
        IsRaw = isRaw;
    }
}