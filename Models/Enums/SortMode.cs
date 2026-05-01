namespace PhotoSort.Models.Enums;

/// <summary>
/// Definuje režimy třídění fotografií podle typu souboru.
/// </summary>
public enum SortMode
{
    /// <summary> Standardní formáty (JPG, PNG, atd.). </summary>
    Normal,
    
    /// <summary> Pouze RAW formáty (CR2, NEF, ARW, atd.). </summary>
    Raw,
    
    /// <summary> Třídit oba typy souborů. </summary>
    Both
}