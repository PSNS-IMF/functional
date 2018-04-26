module MatchingTests

open NUnit.Framework
open FsUnit
open System
open Psns.Common.Functional

type ex = Psns.Common.Functional.Prelude

let trueMatch value = ex.AsEqual(value, Func<bool, bool> (fun b -> b))
let catcher = Func<bool, Maybe<bool>> (fun b -> ex.Some(b))
let matchFun cond value = ex.Match(cond, trueMatch value, catcher)

[<Test>]
let ``it should match a bool correctly.`` () =
    matchFun (1 > 0) true |> should equal true