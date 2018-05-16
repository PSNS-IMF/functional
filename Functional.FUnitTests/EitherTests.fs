module EitherTests

open NUnit.Framework
open FsUnit
open System
open Psns.Common.Functional

type ex = Psns.Common.Functional.Prelude

[<Test>]
let ``it should return all Rights.`` () =
    String.Join(" ", [ex.Right<unit, string> "one"; ex.Right<unit, string> "two";].Rights()) |> should equal "one two"

[<Test>]
let ``it should return all Lefts.`` () =
    String.Join(" ", [ex.Left<string, unit> "one"; ex.Left<string, unit> "two";].Lefts()) |> should equal "one two"

[<Test>]
let ``it should return empty Rights.`` () =
    String.Join(" ", Seq.empty<Either<string, unit>>.Rights()) |> should equal ""

[<Test>]
let ``it should convert to a Try when asynchronous.`` () =
    (ex.Right<exn, int> 1).AsTask().AsTry().Match((fun i -> i), (fun _ -> -1)).Result |> should equal 1
    (ex.Left<exn, int> (exn "fail")).AsTask().AsTry().Match((fun _ -> "ok"), (fun e -> e.Message)).Result |> should equal "fail"