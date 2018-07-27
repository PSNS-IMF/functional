module PreludeTests

open System
open NUnit.Framework
open FsUnit

type ex = Psns.Common.Functional.Prelude

[<Test>]
let ``it should create a defined range.`` () =
    ex.range(1, 5) |> should equal [1..5]

[<Test>]
let ``it should only execute a memo'd function once.`` () =
    let func callCount = Func<_, _>(fun _ ->
        callCount :=  !callCount + 1
        Unchecked.defaultof<_>)
    
    let callCount = ref 0
    let cached = ex.Memo (func callCount)

    cached.Invoke(1)
    cached.Invoke(1)

    !callCount |> should equal 1

[<Test>]
let ``it should return the last executed value of a memo'd function.`` () =
    let func callCount = Func<_, _>(fun _ ->
        callCount := !callCount + 1
        !callCount * !callCount)
    
    let callCount = ref 0
    let cached = ex.MemoRead (func callCount)
    let struct (toCall, getState) = cached

    toCall.Invoke(1) |> ignore
    toCall.Invoke(1) |> ignore
    toCall.Invoke(2) |> ignore

    !callCount |> should equal 2
    getState.Invoke() |> should equal 4
