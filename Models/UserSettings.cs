using PhotoSort.Models.Enums;

namespace PhotoSort.Models;

/// <summary>
/// Represents user settings for export and sorting.
/// </summary>
public class UserSettings
{
    /// <summary>
    /// Source path where unprocessed photos are located.
    /// </summary>
    public string SourcePath { get; set; } = string.Empty;

    /// <summary>
    /// Destination path for the organized archive.
    /// </summary>
    public string DestinationPath { get; set; } = string.Empty;

    /// <summary>
    /// Sorting mode (Normal, Raw, or Both).
    /// </summary>
    public SortMode Mode { get; set; } = SortMode.Normal;
}