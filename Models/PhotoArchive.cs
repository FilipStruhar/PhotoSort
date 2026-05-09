using System.Collections.Generic;

namespace PhotoSort.Models;

/// <summary>
/// Represents a structured photo archive sorted by date.
/// Structure: [Key: YYYY-MM] -> [Key: Day] -> List of photos.
/// </summary>
public class PhotoArchive
{
    /// <summary>
    /// Internal data storage for the archive.
    /// </summary>
    public Dictionary<string, Dictionary<int, List<PhotoFile>>> Data { get; private set; } // Dictionary shape: Year-Month -> Day -> List of photos

    public PhotoArchive()
    {
        Data = new Dictionary<string, Dictionary<int, List<PhotoFile>>>(); // Initialize empty archive
    }

    /// <summary>
    /// Adds a photo to the archive in the correct location by capture date.
    /// </summary>
    /// <param name="photo">Photo to add.</param>
    public void AddPhoto(PhotoFile photo)
    {
        // If the photo has no date, ignore it
        if (!photo.DateTaken.HasValue) return;

        DateTime date = photo.DateTaken.Value;
        string monthKey = date.ToString("yyyy-MM");
        int dayKey = date.Day;

        // Level 1: Year-Month
        if (!Data.ContainsKey(monthKey))
        {
            Data[monthKey] = new Dictionary<int, List<PhotoFile>>();
        }

        // Level 2: Day
        if (!Data[monthKey].ContainsKey(dayKey))
        {
            Data[monthKey][dayKey] = new List<PhotoFile>();
        }

        // Level 3: Add the photo to the list in the correct slot
        Data[monthKey][dayKey].Add(photo);
    }
}