module PreludeTests

open NUnit.Framework
open FsUnit

type ex = Psns.Common.Functional.Prelude

[<Test>]
let ``it should create a defined range.`` () =
    ex.range(1, 5) |> should equal [1..5]