# PhotoSort

PhotoSort is a cross-platform console app for organizing and exporting photos by date. It scans a source folder, reads EXIF metadata to determine the capture date. You can then select specific months and days for an export into a structured photo archive.

## Features

- Interactive terminal UI powered by Spectre.Console
- EXIF date extraction with safe fallback to file creation time
- Supports RAW and common image formats
- Export by month and day selection
- Simple XML-based settings file

## Supported Formats

Normal:
- .jpg, .jpeg, .png, .heic, .heif, .webp, .tif, .tiff, .bmp

RAW:
- .crw, .cr2, .cr3, .nef, .nrw, .arw, .srf, .sr2, .orf, .rw2, .raf, .pef, .ptx, .srw, .mrw, .dng, .raw

## Requirements

- .NET 10 SDK
- macOS, Linux, or Windows

## Getting Started

Clone the repository and run:

```bash
dotnet restore
dotnet run
```

## Configuration

Open the Settings menu to set:

- Source Path: folder containing your photos
- Destination Path: export folder
- Sort Mode: Normal, Raw, or Both

Settings are stored in `settings.xml` in the working directory.

## Usage

1. Run the app.
2. Open Settings and configure paths and sort mode.
3. Choose Export photos.
4. Select months and days to export.
5. Confirm and copy files.

## Project Structure

- Program.cs: application entry point
- Services: analysis, copying, and settings persistence
- UI: terminal menus
- Models: data models and enums
