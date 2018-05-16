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

[<Test>]
let ``it should execute all Trys when all succeed.`` () =
    let create (i: int) = i.ToTry() 
    let attempts = [create 1; create 2; create 10]

    Seq.reduce (fun (a: Try<int>) (b: Try<int>) -> a.Append(b)) attempts 
    |> fun res -> res.Match((fun i -> i), (fun e -> 0))
    |> should equal 10

[<Test>]
let ``it should not execute all Trys when one fails.`` () =
    let exeCount = ref 0
    let create f = ex.Try(Func<'a>(f))
    let attempts = [create (fun () -> incr exeCount; 1);create (fun() -> failwith "fail"); create (fun () -> incr exeCount; 3)]

    Seq.reduce (fun (a: Try<int>) (b: Try<int>) -> a.Append(b)) attempts 
    |> fun res -> res.Match((fun i -> i), (fun _ -> 0))
    |> should equal 0

    !exeCount |> should equal 1

[<Test>]
let ``it should generate a Try with the default value of some object type.`` () =
    ex.OfTry<int>().Match((fun i -> i), (fun _ -> -1)) |> should equal 0
    int(7).ToTryAsync().Match((fun i -> i), (fun _ -> -1)).Result |> should equal 7
    ex.FailWithAsync<int>("fail").Match((fun _t -> "ok"), (fun e -> e.Message)).Result |> should equal "fail"