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