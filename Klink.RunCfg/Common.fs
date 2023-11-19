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


type cfgPlexItemValue =
    | Order of order
    | SortableSetCfg of (sortableSetCfgType*stageCount*sorterEvalMode)
    | MutationRate of mutationRate
    | NoiseFraction of noiseFraction
    | RngGen of rngGen
    | ParentAndChildCounts of (sorterCount*sorterCount)
    | SorterSetPruneMethod of sorterSetPruneMethod
    | StageWeight of stageWeight
    | SwitchGenMode of switchGenMode


module CfgPlexItemValue =
    let toArrayOfStrings (cfgPlexItemValue: cfgPlexItemValue) =
        match cfgPlexItemValue with
        | Order o ->
                [|
                    "Order";
                    o |> Order.value |> string
                |]
        | SortableSetCfg (sect, sc, sem) ->
                [|
                  "SortableSetCfg";
                  sect |> string;
                  sc |> StageCount.value |> string;
                  sem |> string
                |]
        | MutationRate o ->
                [|
                    "MutationRate";
                    o |> MutationRate.value |> string
                |]
        | NoiseFraction nf ->
                [|
                    "NoiseFraction";
                    nf |> NoiseFraction.value |> string
                |]
        | RngGen rg ->
                [|
                    "RngGen";
                    rg |> RngGen.getType |> string;
                    rg |> RngGen.getSeed |> string;
                |]
        | ParentAndChildCounts (sc1, sc2) ->
                [|
                     "ParentAndChildCounts";
                     sc1 |> SorterCount.value |> string;
                     sc2 |> SorterCount.value |> string;
                |]
        | SorterSetPruneMethod ssp ->
                [|
                    "SorterSetPruneMethod";
                    ssp |> SorterSetPruneMethod.toReport
                |]
        | StageWeight sw ->
                [|
                    "StageWeight";
                    sw |> StageWeight.value |> string
                |]
        | SwitchGenMode sgm ->
                [|
                        "SwitchGenMode";
                        sgm |> string
                |]

    let fromArrayOfStrings (lst: string array) : Result<cfgPlexItemValue, string> =
        match lst with
        | [|"Order"; o|] ->
            result {
                let! ov = StringUtil.parseInt o
                return Order.createNr ov |> cfgPlexItemValue.Order
            }
        | [|"SortableSetCfg"; sct; sc; sem|] -> 
            result {
                let! sortableSetCfgType = SortableSetCfgType.fromString sct
                let! scValue = StringUtil.parseInt sc
                let! semValue = SorterEvalMode.fromString sem
                return
                    (
                       sortableSetCfgType ,
                       scValue |> StageCount.create ,
                       semValue 
                     ) |> cfgPlexItemValue.SortableSetCfg
            }
        | [|"MutationRate"; o|] ->
            result {
                let! ov = StringUtil.parseFloat o
                return ov |> MutationRate.create |> cfgPlexItemValue.MutationRate
            }
        | [|"NoiseFraction"; nf|] ->
            result {
                let! nfValue = StringUtil.parseFloat nf
                return nfValue |> NoiseFraction.create |> cfgPlexItemValue.NoiseFraction
            }
        | [|"RngGen"; rgt; seed|] ->
            result {
                let! rngGen = RngGen.fromStrings rgt seed
                return rngGen |> cfgPlexItemValue.RngGen
            }
        | [|"ParentAndChildCounts"; sc1; sc2|] ->
            result {
                let! sc1Value = StringUtil.parseInt sc1
                let! sc2Value = StringUtil.parseInt sc2
                return (
                        sc1Value |> SorterCount.create,
                        sc2Value |> SorterCount.create
                        ) |> cfgPlexItemValue.ParentAndChildCounts
            }
        | [|"SorterSetPruneMethod"; ssp|] ->
            result {
                let! ov = StringUtil.parseFloat ssp
                let! spm = SorterSetPruneMethod.fromReport ssp
                return spm |> cfgPlexItemValue.SorterSetPruneMethod
            }
        | [|"StageWeight"; o|] ->
            result {
                let! swv = StringUtil.parseFloat o
                let sw = swv |> StageWeight.create
                return sw |> cfgPlexItemValue.StageWeight
            }
        | [|"SwitchGenMode"; sgm|] ->
            result {
                let! smv = sgm |> SwitchGenMode.fromString
                return smv |> cfgPlexItemValue.SwitchGenMode
            }
            | uhv -> $"not handled in CfgPlexType.fromList %A{uhv}" |> Error
            


type cfgPlexItem = 
    private 
        { 
            name: cfgPlexItemName
            cfgPlexItemRank: cfgPlexItemRank
            cfgPlexItemValues: cfgPlexItemValue[]
        }


module CfgPlexItem =

    let create 
            (name: string) 
            (rank: int)
            (cfgPlexItemValues: cfgPlexItemValue[])
        =
        {
            cfgPlexItem.name = name |> CfgPlexItemName.create;
            cfgPlexItem.cfgPlexItemRank = rank |> CfgPlexItemRank.create;
            cfgPlexItem.cfgPlexItemValues = cfgPlexItemValues;
        }
    let getName (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem.name
        
    let getRank (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem.cfgPlexItemRank
        
    let getCfgPlexItemValues (cfgPlexItem:cfgPlexItem) =
        cfgPlexItem.cfgPlexItemValues
    

    let enumerateItems (cfgPlexItems: cfgPlexItem[]) =
        let listList =
                cfgPlexItems
                |> Array.sortBy(fun it -> it.cfgPlexItemRank |> CfgPlexItemRank.value)
                |> Array.map(fun it -> it.cfgPlexItemValues |> Array.toList)
                |> Array.toList
        CollectionOps.crossProduct listList
        

type cfgPlex =
     private 
        { 
            name: cfgPlexName
            rngGen: rngGen
            cfgPlexItems: cfgPlexItem[]
        }

module CfgPlex =
    let load
            (name:cfgPlexName)
            (rngGen:rngGen)
            (cfgPlexItems:cfgPlexItem[])
         =
        { 
            name = name
            rngGen = rngGen
            cfgPlexItems = cfgPlexItems
        }
        
    let getName (cfgPlex:cfgPlex) =
        cfgPlex.name
        
    let getRngGen (cfgPlex:cfgPlex) =
        cfgPlex.rngGen
        
    let getCfgPlexItems (cfgPlex:cfgPlex) =
        cfgPlex.cfgPlexItems