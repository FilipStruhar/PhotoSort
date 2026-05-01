namespace PhotoSort.Models;

/// <summary>
/// Reprezentuje konkrétní fotografii určenou ke zpracování.
/// </summary>
public class PhotoFile
{
    /// <summary> Původní plná cesta k souboru. </summary>
    public string OriginalPath { get; set; } = string.Empty;

    /// <summary> Název souboru (např. DSC_001.jpg). </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary> Datum pořízení snímku získané z EXIFu nebo data vytvoření souboru. </summary>
    public DateTime? DateTaken { get; set; } // Může být null, pokud není datum k dispozici - "DateTime?"

    /// <summary> Příznak, zda se jedná o RAW formát. </summary>
    public bool IsRaw { get; set; }

    /// <summary>
    /// Vytvoří novou instanci reprezentace fotografie.
    /// </summary>
    public PhotoFile(string originalPath, string fileName, DateTime? dateTaken, bool isRaw)
    {
        OriginalPath = originalPath;
        FileName = fileName;
        DateTaken = dateTaken;
        IsRaw = isRaw;
    }
}