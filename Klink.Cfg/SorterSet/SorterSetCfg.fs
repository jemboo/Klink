namespace global
      
type sorterSetCfg = 
     | Rnd of sorterSetRndCfg

module SorterSetCfg =

    let makeSorterSet
            (sscfg: sorterSetCfg) 
        =
        match sscfg with
        | Rnd rdsg -> rdsg |> SorterSetRndCfg.makeSorterSet