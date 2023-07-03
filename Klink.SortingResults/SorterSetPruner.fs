namespace global

open System

type sorterSetPrunerId = private SorterSetPrunerId of Guid
module SorterSetPrunerId =
    let value (SorterSetPrunerId v) = v
    let create vl = SorterSetPrunerId vl


type sorterSetPrunerWhole = 
        private {
        id: sorterSetPrunerId;
        selectionFraction:selectionFraction;
        stageWeight:stageWeight; }


module SorterSetPrunerWhole =

    let getId
            (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.id


    let getSelectionFraction
             (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.selectionFraction


    let getStageWeight
                (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.stageWeight


    let load
            (id:sorterSetPrunerId)
            (selectionFraction:selectionFraction)
            (stageWeight:stageWeight)
        =
        {   
            id=id
            selectionFraction=selectionFraction
            stageWeight=stageWeight
        }

    let makeId
            (selectionFraction:selectionFraction)
            (stageWeight:stageWeight)
        =
        [|
            "sorterSetPrunerWhole" :> obj
            stageWeight |> StageWeight.value :> obj; 
            selectionFraction |> SelectionFraction.value :> obj
        |] 
        |> GuidUtils.guidFromObjs
        |> SorterSetPrunerId.create

    let make (selectionFraction:selectionFraction)  
             (stageWeight:stageWeight) 
        =
        {
            id = makeId selectionFraction stageWeight;
            selectionFraction = selectionFraction;
            stageWeight =  stageWeight; 
        }



type sorterSetPruner =
    | Whole of sorterSetPrunerWhole


module SorterSetPruner =

    let getId
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.id


    let getSelectionFraction
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.selectionFraction


    let getStageWeight
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.stageWeight