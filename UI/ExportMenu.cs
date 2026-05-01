using PhotoSort.Models;
using PhotoSort.Services;
using Spectre.Console;

namespace PhotoSort.UI;

public class ExportMenu
{
    private readonly PhotoAnalyzer _analyzer;
    private readonly PhotoCopier _copier;

    public ExportMenu(PhotoAnalyzer analyzer, PhotoCopier copier)
    {
        _analyzer = analyzer;
        _copier = copier;
    }

    public void Show()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[green]Photo Export[/]"));

        PhotoArchive fullArchive = new PhotoArchive();
        
        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .Start("Analyzing source folder...", ctx =>
            {
                fullArchive = _analyzer.GetAnalyzedFiles();
            });

        if (fullArchive.Data.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No photos found matching the current criteria.[/]");
            WaitForExit();
            return;
        }

        // Month selection
        // Show photo counts per month in the menu
        var monthChoices = fullArchive.Data.Keys.OrderBy(x => x).ToList();
        var selectedMonths = AnsiConsole.Prompt(
            new MultiSelectionPrompt<string>()
                .Title("Select [green]months[/] to export:")
                .NotRequired()
                .PageSize(10)
                .InstructionsText("[grey](Press <space> to select, <enter> to confirm)[/]")
                .UseConverter(m => $"{m} [grey]({GetTotalPhotosInMonth(fullArchive, m)} photos)[/]")
                .AddChoices(monthChoices));

        if (selectedMonths.Count == 0) return;

        var filteredArchive = new PhotoArchive();
        int totalPhotosToCopy = 0;

        // Day selection
        foreach (var month in selectedMonths)
        {
            var daysInMonth = fullArchive.Data[month].Keys.OrderBy(x => x).ToList();
            
            var dayPrompt = new MultiSelectionPrompt<int>()
                .Title($"Which [blue]days[/] from [yellow]{month}[/] do you want to export?")
                .InstructionsText("[grey](Space to toggle, Enter to confirm)[/]")
                .UseConverter(d => $"{d}. [grey]({fullArchive.Data[month][d].Count} photos)[/]")
                .AddChoices(daysInMonth);

            // Preselect all days
            foreach (var day in daysInMonth)
            {
                dayPrompt.Select(day);
            }

            var selectedDays = AnsiConsole.Prompt(dayPrompt);

            foreach (var day in selectedDays)
            {
                var photos = fullArchive.Data[month][day];
                totalPhotosToCopy += photos.Count;

                foreach (var photo in photos)
                {
                    filteredArchive.AddPhoto(photo);
                }
            }
        }

        if (totalPhotosToCopy > 0)
        {
            AnsiConsole.WriteLine();
            var summaryTable = new Table().Border(TableBorder.Rounded);
            summaryTable.AddColumn("Summary");
            summaryTable.AddColumn("Count");
            summaryTable.AddRow("Selected Months", selectedMonths.Count.ToString());
            summaryTable.AddRow("Total Photos", $"{totalPhotosToCopy}");
            
            AnsiConsole.Write(summaryTable);
            
            if (AnsiConsole.Confirm("Do you want to start copying these files?"))
            {
                _copier.CopyFiles(filteredArchive);
            }
        }

        WaitForExit();
    }

    /// <summary>
    /// Helper method to count all photos across all days in a month.
    /// </summary>
    private int GetTotalPhotosInMonth(PhotoArchive archive, string monthKey)
    {
        return archive.Data[monthKey].Values.Sum(dayList => dayList.Count); // Sum photos for each day to get the monthly total
    }

    private void WaitForExit()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to return to the menu...[/]");
        Console.ReadKey(true);
    }
}