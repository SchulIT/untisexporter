# UntisExport

[![NuGet version (SchulIT.UntisExport)](https://img.shields.io/nuget/v/SchulIT.UntisExport.svg?style=flat-square)](https://www.nuget.org/packages/SchulIT.UntisExport/)
[![Build Status](https://dev.azure.com/schulit/UntisExport/_apis/build/status/SchulIT.UntisExport?branchName=master)](https://dev.azure.com/schulit/UntisExport/_build/latest?definitionId=1&branchName=master)

Mithilfe dieser Bibliothek können Daten aus Untis eingelesen werden. Möchte man die HTML-Daten nutzen, wird das Info-Stundenplan-Modul benötigt. Die zu verwendeten Vorlagen für die Ausgabe werden bereitgestellt und müssen zuvor importiert werden.

## Features

* Stundenplan
    * HTML-Import
* Vertretungsplan
    * GPU014.txt-Import (ohne Absenzen und Infotexte)
    * HTML-Import (inkl. Absenzen und Infotexte)
* Klausurplan
    * GPU017.txt-Import (in Kombination mit den Unterrichten aus GPU002.txt)
    * HTML-Import (funktioniert ohne GPU002.txt)
* Aufsichten
    * GPU009.txt-Import
* Unterricht
    * GPU002.txt-Import
* Räume
    * GPU005.txt-Import

## Installation

Die Installation erfolgt via [NuGet](https://www.nuget.org/packages/SchulIT.UntisExport/).

## Nutzung

Zunächst muss die GPU- oder HTML-Datei eingelesen werden. 

**Wichtig:** Da Untis bei HTML nicht UTF-8 sondern ISO-8859-1 verwendet, muss dies berücksichtigt werden! GPU-Dateien werden standardmäßig in UTF-8 exportiert.

```csharp
var inputEncoding = Encoding.GetEncoding("iso-8859-1");

using (StreamReader reader = new StreamReader(stream, inputEncoding))
{
    var html = reader.ReadToEnd(); // Alternativ kann natürlich auch ReadToEndAsync() verwendet werden
    var bytes = inputEncoding.GetBytes(html);
    var utf8bytes = Encoding.Convert(inputEncoding, Encoding.UTF8, bytes);

    return Encoding.UTF8.GetString(utf8bytes);
}
```

### GPU-Dateien

Die GPU-Dateien werden jeweils mithilfe der `FileHelper` geparst und anschließend in Objekte umgewandelt.

#### Vertretungen

```csharp
var exporter = new SubstitutionExporter();
var settings = new SubstitutionExportSettings();

var substitutions = await exporter.ParseGpuAsync(csv, settings);
```

#### Unterrichte

```csharp
var exporter = new TuitionExporter();
var settings = new TuitionExportSettings();

var tuitions = await exporter.ParseGpuAsync(csv, settings);
```

#### Exams

Um die Klasse aufzulösen, muss zuvor der Unterricht eingelesen werden und dem Exporter übergeben werden.

```csharp
var exporter = new ExamExporter();
var settings = new ExamExportSettings();

var exams = await exporter.ParseGpuAsync(csv, settings, tuitions);
```

### Räume

```csharp
var exporter = new RoomExporter();
var settings = new RoomExportSettings();

var rooms = await exporter.ParseGpuAsync(csv, settings);
```

### Aufsichten

Da in Untis Aufsichten pro Woche angegeben werden können, erhält man bei Aufsichten die Wochen, in denen die Aufsicht laut Untis stattfinden soll. Wenn keine Wochen angegeben sind, ist jede Woche gemeint.

```csharp
var exporter = new SupervisionExporter();
var settings = new SupervisionExportSettings();

var exams = await exporter.ParseGpuAsync(csv, settings);
```

### HTML-Dateien

Die HTML-Dateien werden jeweils mithilfe des `HtmlAgilityPack` geparst und anschließend in Objekte umgewandelt.

#### Vertretungen, Infotexte und Absenzen

```csharp
var exporter = new SubstitutionExporter();
var settings = new SubstitutionExportSettings();

var result = await exporter.ParseHtmlAsync(html, settings);
```

#### Klausurplan

```csharp
var exporter = new ExamExporter();
var settings = new ExamExportSettings();

var result = await exporter.ParseHtmlAsync(html, settings);
```

#### Stundenplan

```csharp
var exporter = new TimetableExporter();
var settings = new TimetableExportSettings();

var result = await exporter.ParseHtmlAsync(html, settings);
```

### Anpassungen

Beim Einlesen lassen sich verschiedene Parameter spezifieren. Diese sind im Quellcode beschrieben und erklärt.

### Einrichtung von Untis

Die Einrichtung von Untis ist [hier](doc/untis.md) beschrieben.

## Lizenz

[MIT](./LICENSE.md)