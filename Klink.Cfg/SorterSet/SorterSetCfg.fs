namespace global
      
type sorterSetCfg = 
     | Rnd of sorterSetRndCfg

module SorterSetCfg =

    let makeSorterSet            
            (rngGenProvider: rngGenProvider)
            (sscfg: sorterSetCfg) 
        =
        match sscfg with
        | Rnd rdsg -> rdsg |> SorterSetRndCfg.makeSorterSet rngGenProvider