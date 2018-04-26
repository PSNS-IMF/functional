module ParsingTests

open NUnit.Framework
open FsUnit

type ex = Psns.Common.Functional.Prelude

[<Test>]
let ``it should parse a valid string as an integer.`` () =
    let result = ex.ParseInt("12").Match((fun i -> i), (fun () -> -1))
    result |> should equal 12

[<Test>]
let ``it should fail to parse an invalid string as an integer.`` () =
    let result = ex.ParseInt("a12").Match((fun i -> i), (fun () -> -1))
    result |> should equal -1