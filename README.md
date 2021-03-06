# UntisExport

[![NuGet version (SchulIT.UntisExport)](https://img.shields.io/nuget/v/SchulIT.UntisExport.svg?style=flat-square)](https://www.nuget.org/packages/SchulIT.UntisExport/)
[![Build Status](https://dev.azure.com/schulit/UntisExport/_apis/build/status/SchulIT.UntisExport?branchName=master)](https://dev.azure.com/schulit/UntisExport/_build/latest?definitionId=1&branchName=master)
![.NET Standard 2.1](https://img.shields.io/badge/.NET%20Standard-2.1-brightgreen?style=flat-square)

Mithilfe dieser Bibliothek kann die Untis GPN-Datei (in Teilen) ausgelesen werden.

## Features

Folgende Dinge werden aktuell von der Bibliothek ausgelesen:

* Einstellungen zum Schuljahr
    * Start & Ende des Schuljahres 
    * Periodizität & Startwoche
    * Anzahl der Stunden pro Tag
    * Anzahl der Tage pro Woche
    * Erste Stunde
* Ferien & Feiertage
* Perioden
* Fächer
* Lehrkräfte*
* Unterrichte*
* Stundenplan*
* Pausenaufsichten* (inkl. Flure)
* Veranstaltungen
* Vertretungen
* Tagesinformationen
    * Tagestexte
    * Unterrichtsfreie Stunden
* Klausuren
    * Kurse
    * Lernende
    * Aufsichten
* Absenzen (Klasse, Lehrkräfte, Räume)
* Räume

(*) Diese Funktion ist nur möglich, wenn Perioden genutzt werden.

## Installation

Die Installation erfolgt via [NuGet](https://www.nuget.org/packages/SchulIT.UntisExport/).

## Nutzung

```csharp
// Imports
using SchulIT.UntisExport;
using SchulIT.UntisExport.Model;

// Blockierende Methode:
var result = UntisExporter.ParseFile(pathToGpnFile);

// Alternativ auch asynchron:
var result = await UntisExporter.ParseFileAsync(pathToGpnFile);
```

Anschließend kann über die `UntisExportResult`-Klasse auf alle Entitäten zugegriffen werden.

## Wichtige Anmerkungen

* Die Bibliothek wurde gegen eine Untis-Datei mit einer Mutterperiode und vielen Unterperioden getestet.
* Es werden nicht alle Informationen ausgelesen. Aktuell beschränkt sich die Bibliothek auf die wichtigsten Informationen. Bei Bedarf kann sie gerne erweitert werden.
* Die Auflösung der Vertretungsart leider nicht immer der Vertretungsart, die in Untis angezeigt wird. Diese wird nicht direkt in der GPN-Datei abgespeichert sondern von Untis berechnet. Eine eigene Implementierung findest sich in der Klasse `SubstitutionTypeResolver`.

## Credits

Diese Bibliothek nutzt den Parser-Bibliothek [Sprache](https://github.com/sprache/sprache).

## Lizenz

[MIT](./LICENSE.md)