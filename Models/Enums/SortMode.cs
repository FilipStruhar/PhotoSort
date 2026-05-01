namespace PhotoSort.Models.Enums;

/// <summary>
/// Defines sorting modes by file type.
/// </summary>
public enum SortMode
{
    /// <summary> Standard formats (JPG, PNG, etc.). </summary>
    Normal,
    
    /// <summary> RAW formats only (CR2, NEF, ARW, etc.). </summary>
    Raw,
    
    /// <summary> Sort both file types. </summary>
    Both
}