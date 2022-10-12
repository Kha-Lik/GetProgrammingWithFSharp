module Scripts.lesson_31

open FSharp.Data
open XPlot.GoogleCharts

type Comments = JsonProvider<"https://jsonplaceholder.typicode.com/comments">
let comment = Comments.GetSamples().[0]
printfn $"%A{comment}"

type Films = HtmlProvider<"https://en.wikipedia.org/wiki/Robert_De_Niro_filmography">
let deNiro = Films.GetSample()
deNiro.Tables.FilmsEdit.Rows
|> Array.countBy (fun f -> f.Year)
|> Array.map (fun (year, count) -> year.ToString(), count)
|> Chart.SteppedArea
|> Chart.Show

type Package = HtmlProvider<"Data\sample-package.html">
let nUnit = Package.Load "https://www.nuget.org/packages/nunit"
let xUnit = Package.Load "https://www.nuget.org/packages/xunit"
let efcore = Package.Load "https://www.nuget.org/packages/microsoft.entityframeworkCore"
let nJson = Package.Load "https://www.nuget.org/packages/newtonsoft.json"

[ nUnit; xUnit; efcore; nJson ]
|> Seq.collect (fun package -> package.Tables.Table4.Rows)
|> Seq.sortByDescending (fun row -> row.Downloads)
|> Seq.take 10
|> Seq.map (fun row -> row.Version, row.Downloads)
|> Chart.Column
|> Chart.Show