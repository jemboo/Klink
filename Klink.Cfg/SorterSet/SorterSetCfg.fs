namespace global
      
type sorterSetCfg = 
     | Rnd of sorterSetRndCfg
     | RndMutated of sorterSetMutatedFromRndCfg
     | SelfAppend of sorterSetSelfAppendCfg

module SorterSetCfg =
    ()