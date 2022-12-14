module Lesson30.lesson_30

open FSharp.Data
open XPlot.GoogleCharts

[<Literal>]
let ResolutionFolder = __SOURCE_DIRECTORY__

type Football = CsvProvider<"Data\FootballResults.csv", ResolutionFolder=ResolutionFolder>
let data = Football.GetSample().Rows

data
|> Seq.filter (fun row -> row.``Full Time Home Goals`` > row.``Full Time Away Goals``)
|> Seq.countBy (fun row -> row.``Home Team``)
|> Seq.sortByDescending snd
|> Seq.take 10
|> Chart.Column
|> Chart.Show
