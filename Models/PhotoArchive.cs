using System.Collections.Generic;

namespace PhotoSort.Models;

/// <summary>
/// Reprezentuje strukturovaný archiv fotografií roztříděný podle data.
/// Struktura: [Klíč: RRRR-MM] -> [Klíč: Den] -> Seznam fotek.
/// </summary>
public class PhotoArchive
{
    /// <summary>
    /// Vnitřní datové úložiště archivu.
    /// </summary>
    public Dictionary<string, Dictionary<int, List<PhotoFile>>> Data { get; private set; } // Tvorba dictionary pro roztřídění: Rok-Měsíc -> Den -> Seznam fotek

    public PhotoArchive()
    {
        Data = new Dictionary<string, Dictionary<int, List<PhotoFile>>>(); // Inicializace prázdného archivu
    }

    /// <summary>
    /// Přidá fotografii do archivu na správné místo podle jejího data pořízení.
    /// </summary>
    /// <param name="photo">Objekt fotografie k zařazení.</param>
    public void AddPhoto(PhotoFile photo)
    {
        // Pokud fotka nemá datum, v této verzi aplikace ji ignorujeme (bezpečný fallback)
        if (!photo.DateTaken.HasValue) return;

        DateTime date = photo.DateTaken.Value;
        string monthKey = date.ToString("yyyy-MM");
        int dayKey = date.Day;

        // 1. Úroveň: Rok-Měsíc (vytvoření "krabice", pokud neexistuje)
        if (!Data.ContainsKey(monthKey))
        {
            Data[monthKey] = new Dictionary<int, List<PhotoFile>>();
        }

        // 2. Úroveň: Den (vytvoření "přihrádky", pokud neexistuje)
        if (!Data[monthKey].ContainsKey(dayKey))
        {
            Data[monthKey][dayKey] = new List<PhotoFile>();
        }

        // 3. Úroveň: Samotné přidání fotky do seznamu ve správné přihrádce
        Data[monthKey][dayKey].Add(photo);
    }
}