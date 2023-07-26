namespace global

open Microsoft.FSharp.Core
open System
open System.Security.Cryptography

module StringUtil =

    let nullOption (sV:string option) =
        match sV with
        |Some sv -> sv
        |None -> null


    let stringOption 
            (emptyVal:string) 
            (strVal: string option) 
        =
        match strVal with
        | Some sv -> sv
        | None -> emptyVal

    let toCsvLine<'a> 
            (strFetch:'a->string) 
            (lineData:'a seq)
        =
        lineData 
        |> Seq.fold 
                (fun st t ->
                    let cv = strFetch t 
                    if (st = "") then cv else $"{st}\t{cv}" 
                )
                ""