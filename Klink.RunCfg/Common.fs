namespace global
open System
open System.IO

type cfgPlexName = private CfgPlexName of string
module CfgPlexName =
    let value (CfgPlexName v) = v
    let create (value: string) =
        value |> CfgPlexName


type cfgPlexItemName = private CfgPlexItemName of string
module CfgPlexItemName =
    let value (CfgPlexItemName v) = v
    let create (value: string) =
        value |> CfgPlexItemName


type cfgPlexItemRank = private CfgPlexItemRank of int
module CfgPlexItemRank =
    let value (CfgPlexItemRank v) = v
    let create (value: int) =
        value |> CfgPlexItemRank


type cfgPlexType =
    | Order of order
    | SortableSetCfg of (sortableSetCfgType*stageCount*sorterEvalMode)
    | MutationRate of mutationRate
    | NoiseFraction of noiseFraction
    | RngGen of rngGen
    | ParentAndChildCounts of (sorterCount*sorterCount)
    | SorterSetPruneMethod of sorterSetPruneMethod
    | StageWeight of stageWeight
    | SwitchGenMode of switchGenMode


module CfgPlexType =
    let asStringList (cfgPlexType: cfgPlexType) =
        match cfgPlexType with
        | Order o -> ["Order"; o |> Order.value |> string]
        | SortableSetCfg (sect, sc, sem) ->
                [
                  "SortableSetCfg";
                  sect |> string;
                  sc |> StageCount.value |> string;
                  sem |> string
                ]
        | MutationRate o -> ["MutationRate"; o |> MutationRate.value |> string]
        | NoiseFraction nf -> ["NoiseFraction"; nf |> NoiseFraction.value |> string]
        | RngGen rg ->
                [
                    "RngGen"
                    rg |> RngGen.getType |> string
                    rg |> RngGen.getSeed |> string
                ]
        | ParentAndChildCounts (sc1, sc2) ->
                [
                     "ParentAndChildCounts";
                     sc1 |> SorterCount.value |> string;
                     sc2 |> SorterCount.value |> string;
                ]
        | SorterSetPruneMethod ssp ->
                [
                    "SorterSetPruneMethod";
                    ssp |> SorterSetPruneMethod.toReport
                ]
        | StageWeight sw ->
                [
                    "StageWeight";
                    sw |> StageWeight.value |> string
                ]
        | SwitchGenMode sgm ->
                [
                        "SwitchGenMode";
                        sgm |> string
                ]

    let fromList (lst: string list) : Result<cfgPlexType, string> =
        match lst with
        | ["Order"; o] ->
            result {
                let! ov = StringUtil.parseInt o
                return Order.createNr ov |> cfgPlexType.Order
            }
        | ["SortableSetCfg"; sct; sc; sem] -> 
            result {
                let! sortableSetCfgType = SortableSetCfgType.fromString sct
                let! scValue = StringUtil.parseInt sc
                let! semValue = SorterEvalMode.fromString sem
                return
                    (
                       sortableSetCfgType ,
                       scValue |> StageCount.create ,
                       semValue 
                     ) |> cfgPlexType.SortableSetCfg
            }
        | ["MutationRate"; o] ->
            result {
                let! ov = StringUtil.parseFloat o
                return ov |> MutationRate.create |> cfgPlexType.MutationRate
            }
        | ["NoiseFraction"; nf] ->
            result {
                let! nfValue = StringUtil.parseFloat nf
                return nfValue |> NoiseFraction.create |> cfgPlexType.NoiseFraction
            }
        | ["RngGen"; rgt; seed] ->
            result {
                let! rngGen = RngGen.fromStrings rgt seed
                return rngGen |> cfgPlexType.RngGen
            }
        | ["ParentAndChildCounts"; sc1; sc2] ->
            result {
                let! sc1Value = StringUtil.parseInt sc1
                let! sc2Value = StringUtil.parseInt sc2
                return (
                        sc1Value |> SorterCount.create,
                        sc2Value |> SorterCount.create
                        ) |> cfgPlexType.ParentAndChildCounts
            }
        | ["SorterSetPruneMethod"; ssp] ->
            result {
                let! ov = StringUtil.parseFloat ssp
                let! spm = SorterSetPruneMethod.fromReport ssp
                return spm |> cfgPlexType.SorterSetPruneMethod
            }
        | ["StageWeight"; o] ->
            result {
                let! swv = StringUtil.parseFloat o
                let sw = swv |> StageWeight.create
                return sw |> cfgPlexType.StageWeight
            }
        | ["SwitchGenMode"; sgm] ->
            result {
                let! smv = sgm |> SwitchGenMode.fromString
                return smv |> cfgPlexType.SwitchGenMode
            }
            | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            


type cfgPlexItem = 
    private 
        { 
            name: cfgPlexItemName
            rank: cfgPlexItemRank
            items: cfgPlexType[]
        }


module CfgPlexItem =

    let create 
            (name: string) 
            (rank: int)
            (items: cfgPlexType[])
        =
        {
            cfgPlexItem.name = name |> CfgPlexItemName.create;
            cfgPlexItem.rank = rank |> CfgPlexItemRank.create;
            cfgPlexItem.items = items;
        }


    let enumerateItems (cfgPlexItems: cfgPlexItem[]) =
        let listList =
                cfgPlexItems
                |> Array.sortBy(fun it -> it.rank |> CfgPlexItemRank.value)
                |> Array.map(fun it -> it.items |> Array.toList)
                |> Array.toList
        CollectionOps.crossProduct listList
        
