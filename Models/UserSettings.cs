using PhotoSort.Models.Enums;

namespace PhotoSort.Models;

/// <summary>
/// Reprezentuje uživatelské nastavení aplikace pro export a třídění.
/// </summary>
public class UserSettings
{
    /// <summary>
    /// Zdrojová cesta, kde se nacházejí nezpracované fotografie.
    /// </summary>
    public string SourcePath { get; set; } = string.Empty;

    /// <summary>
    /// Cílová cesta pro organizovaný archiv.
    /// </summary>
    public string DestinationPath { get; set; } = string.Empty;

    /// <summary>
    /// Režim třídění (Normální, Raw, nebo obojí).
    /// </summary>
    public SortMode Mode { get; set; } = SortMode.Normal;
}