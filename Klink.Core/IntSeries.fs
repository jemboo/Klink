namespace global

open System

module IntSeries =
    // returns true with an exp decreasing frequency
    let expoB (ticsPerLog: float) (value: int) =
        if (ticsPerLog |> int) > value then
            true
        else
            let logo = (ticsPerLog * Math.Log2(value |> float)) |> int
            let logm = (ticsPerLog * Math.Log2((value - 1) |> float)) |> int
            logo > logm

    // 141 tics per log gives 10% of 10K
    // 70 tics per log give 10% of 5K
    let logTics (ticsPerLog: float) (endVal: int) =
        seq {
            let mutable dex = 0

            while dex < endVal do
                if (expoB ticsPerLog dex) then
                    yield dex

                dex <- dex + 1
        }
