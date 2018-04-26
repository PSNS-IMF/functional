module TryTests

open System
open Psns.Common.Functional
open NUnit.Framework
open FsUnit
open System.Threading.Tasks

type ex = Psns.Common.Functional.Prelude

let mutable executed = false

let regardlessFunc = Func<Task<string>> (fun () ->
    executed <- true
    "regardless".AsTask())
let regardless = ex.TryAsync(regardlessFunc)

let okTry = ex.TryAsync(Func<Task<string>> (fun () -> "result".AsTask()))
let failTry = ex.TryAsync(Func<Task<string>> (failwith "failure"))
let doTry (tryable: TryAsync<string>) (rgdless: TryAsync<string>) =
    tryable.Regardless(rgdless).Match((fun s -> s), (fun e -> e.Message)).Result

[<Test>]
let ``it should execute a post action when Try succeeds.`` () =
    doTry okTry regardless |> should equal "regardless"
    executed |> should equal true

[<Test>]
let ``it should execute a post action even if the current Try fails.`` () =
    doTry failTry regardless |> should equal "failure"
    executed |> should equal true

[<Test>]
let ``it should return the failed result of then regardless try if it fails.`` () =
    doTry okTry failTry |> should equal "failure"