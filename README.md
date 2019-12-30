# UntisExport

[![NuGet version (SchulIT.SchildExport)](https://img.shields.io/nuget/v/SchulIT.UntisExport.svg?style=flat-square)](https://www.nuget.org/packages/SchulIT.UntisExport/)
[![Build Status](https://dev.azure.com/schulit/UntisExport/_apis/build/status/SchulIT.UntisExport?branchName=master)](https://dev.azure.com/schulit/UntisExport/_build/latest?definitionId=1&branchName=master)

Mithilfe dieser Bibliothek kann der Untis-Vertretungsplan ausgelesen werden. Dazu wird das Info-Stundenplan-Modul von Untis benötigt. Die zu verwendende Ausgabe-Vorlage wird ebenfalls bereitgestellt.

## Installation

Die Installation erfolgt via [NuGet](https://www.nuget.org/packages/SchulIT.UntisExport/).

## Nutzung

```csharp
var exporter = new UntisExporter();
var settings = new ExportSettings();

var result = await exporter.ParseHtmlAsync(html);
```

## Einrichtung von Untis

Die Einrichtung von Untis ist [hier](doc/untis.md) beschrieben.

### Anpassungen

Mithilfe der `ExportSettings` können folgende Dinge eingestellt werden:

* ob die nicht geschlossenen `<p>`-Tags gefixt werden sollen
* das Datumsformat
* Liste von Werten, die als leer gewertet werden sollen (bspw. "---")
* ob Werte, die als absent gekennzeichnet werden (in Untis durch ergänzen von Klammern, also bspw. "(05A"), als leer gewertet werden sollen
* mit den `ColumnSettings` lassen sich noch die Spaltennamen anpassen
* mit den `AbsenceSettings` lassen sich auch Absenzen parsen

## Lizenz

[MIT](./LICENSE.md)