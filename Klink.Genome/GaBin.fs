namespace global
open System

type binIndex = private BinIndex of int

module BinIndex =
    let value (BinIndex v) = v
    let create dex = BinIndex dex


type binSetId = private BinSetId of Guid
module BinSetId =
    let value (BinSetId v) = v
    let create v = BinSetId v


type binSet<'a when 'a:comparison> = 
            private 
                {
                    id:binSetId
                    binMap:Map<'a,int>; 
                    binCount:int; 
                    binSize:int
                }

module BinSet =

    let makeId<'a when 'a:comparison>  (binMap:Map<'a,int>) =
        binMap 
            |> Map.toArray
            |> Array.map(fun tup -> tup :> obj) 
            |> GuidUtils.guidFromObjs
            |> BinSetId.create
        

    let make<'a when 'a:comparison> 
            (rngGen:rngGen)
            (binSize:int)
            (binCount:int)
            (items:'a seq)
        =
        let randy = Rando.fromRngGen rngGen
        let modulus = binSize*binCount
        let binMap = 
                items
                    |> Seq.map(fun key -> (key, randy.NextInt modulus) )
                    |> Map.ofSeq
        {
            binSet.id = makeId binMap;
            binSet.binMap = binMap;
            binCount = binCount;
            binSize = binSize;
        }

    let update<'a when 'a:comparison>
                (rngGen:rngGen)
                (binSet:binSet<'a>)
        =
        let randy = Rando.fromRngGen rngGen

        let modulus = binSet.binSize * binSet.binCount


        let _nextVal (r:IRando) (pv:int) =
            let bump = 2 * (r.NextInt 2) - 1
            (pv + modulus + bump) % modulus

        let binMapNew = 
                binSet.binMap |> Map.toSeq
                    |> Seq.map(fun kvp -> (fst kvp, kvp |> snd |> _nextVal randy) )
                    |> Map.ofSeq

        {
            binSet.id = makeId binMapNew;
            binSet.binMap = binMapNew;
            binCount = binSet.binCount;
            binSize = binSet.binSize;
        }

    let getBinIndexGroups<'a when 'a:comparison>
            (binSet:binSet<'a>)
        =
        let modulus = binSet.binSize * binSet.binCount
        let _toBinIndex (ival:int) = 
            ((ival - (ival % binSet.binSize) ) / binSet.binSize)
            |> BinIndex.create

        let yab = 
            binSet.binMap 
            |> Map.toArray
            |> Array.map(fun kvp -> (fst kvp, kvp |> snd |> _toBinIndex))
            
        let doink = yab |> Array.groupBy(snd)

        doink |> Array.map(fun kvp -> (fst kvp, kvp |> snd |> Array.map(fst)))


