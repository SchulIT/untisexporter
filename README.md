# UntisExport

[![NuGet version (SchulIT.SchildExport)](https://img.shields.io/nuget/v/SchulIT.UntisExport.svg?style=flat-square)](https://www.nuget.org/packages/SchulIT.UntisExport/)
[![Build Status](https://dev.azure.com/schulit/UntisExport/_apis/build/status/SchulIT.UntisExport?branchName=master)](https://dev.azure.com/schulit/UntisExport/_build/latest?definitionId=1&branchName=master)

Mithilfe dieser Bibliothek kann der Vertretungs- und Klausurplan aus Untis ausgelesen werden. Dazu wird das Info-Stundenplan-Modul von Untis benötigt. Die zu verwendende Ausgabe-Vorlage wird ebenfalls bereitgestellt.

## Installation

Die Installation erfolgt via [NuGet](https://www.nuget.org/packages/SchulIT.UntisExport/).

## Nutzung

Zunächst muss die HTML-Datei eingelesen werden. **Wichtig:** Da Untis nicht UTF-8 sondern ISO-8859-1 verwendet, muss dies berücksichtigt werden:

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

### Vertretungen, Infotexte und Absenzen

```csharp
var exporter = new SubstitutionExporter();
var settings = new SubstitutionExportSettings();

var result = await exporter.ParseHtmlAsync(html);
```

### Klausurplan

```csharp
var exporter = new ExamExporter();
var settings = new ExamExportSettings();

var result = await exporter.ParseHtmlAsync(html);
```

## Einrichtung von Untis

Die Einrichtung von Untis ist [hier](doc/untis.md) beschrieben.

## Anpassungen

Mithilfe der `SubstitutionExportSettings` können folgende Dinge eingestellt werden:

* ob die nicht geschlossenen `<p>`-Tags gefixt werden sollen
* das Datumsformat
* Liste von Werten, die als leer gewertet werden sollen (bspw. "---")
* ob Werte, die als absent gekennzeichnet werden (in Untis durch ergänzen von Klammern, also bspw. "(05A"), als leer gewertet werden sollen
* mit den `ColumnSettings` lassen sich noch die Spaltennamen anpassen
* mit den `AbsenceSettings` lassen sich auch Absenzen parsen

Mithilfe der `ExamExportSettings` können folgende Dinge eingestellt werden:

* das Datumsformat
* mit den `ColumnSettings` lassen sich die Spaltennamen anpassen

## Lizenz

[MIT](./LICENSE.md)