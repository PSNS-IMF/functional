module IEnumerableTests

open NUnit.Framework
open FsUnit
open System
open Psns.Common.Functional
open System.Collections.Generic

type ex = Psns.Common.Functional.Prelude

let vals = seq {
    yield 1
    yield 5
    yield 2
}

[<Test>]
let ``it should match an IEnumerable correctly.`` () =
    let res = vals.Match(Func<string>(fun () -> "empty"), Func<int, IEnumerable<int>, string>(fun _ _ -> "some"))
    res |> should equal "some"

[<Test>]
let ``it should append an item to the end.`` () =
    let res = vals.Append 6
    res |> should equal [1;5;2;6]

[<Test>]
let ``it should prepend an item to the beginning.`` () =
    int(7).Cons(vals) |> should equal [7;1;5;2]