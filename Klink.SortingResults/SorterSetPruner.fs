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
        temp:temp;
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

    let getTemp
             (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.temp

    let getStageWeight
                (sorterSetPruner:sorterSetPrunerWhole) 
         =
         sorterSetPruner.stageWeight


    let load
            (id:sorterSetPrunerId)
            (selectionFraction:selectionFraction)
            (temp:temp)
            (stageWeight:stageWeight)
        =
        {   
            id=id
            selectionFraction=selectionFraction
            temp=temp
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
             (temp:temp)
             (stageWeight:stageWeight) 
        =
        {
            id = makeId selectionFraction stageWeight;
            selectionFraction = selectionFraction;
            temp=temp;
            stageWeight =  stageWeight; 
        }



type sorterSetPrunerShc = 
        private {
        id: sorterSetPrunerId;
        selectionFraction:selectionFraction;
        temp:temp;
        stageWeight:stageWeight; }


module SorterSetPrunerShc =

    let getId
            (sorterSetPruner:sorterSetPrunerShc) 
         =
         sorterSetPruner.id


    let getSelectionFraction
             (sorterSetPruner:sorterSetPrunerShc) 
         =
         sorterSetPruner.selectionFraction

    let getTemp
             (sorterSetPruner:sorterSetPrunerShc) 
         =
         sorterSetPruner.temp


    let getStageWeight
                (sorterSetPruner:sorterSetPrunerShc) 
         =
         sorterSetPruner.stageWeight


    let load
            (id:sorterSetPrunerId)
            (selectionFraction:selectionFraction)
            (temp:temp)
            (stageWeight:stageWeight)
        =
        {   
            id=id
            selectionFraction=selectionFraction
            temp=temp
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
             (temp:temp)
             (stageWeight:stageWeight) 
        =
        {
            id = makeId selectionFraction stageWeight;
            selectionFraction = selectionFraction;
            temp=temp;
            stageWeight =  stageWeight; 
        }




type sorterSetPruner =
    | Whole of sorterSetPrunerWhole
    | Lhc of sorterSetPrunerShc


module SorterSetPruner =

    let getId
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.id
         | Lhc ssphW ->  ssphW.id


    let getSelectionFraction
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.selectionFraction
         | Lhc ssphW ->  ssphW.selectionFraction

    let getTemp
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.temp
         | Lhc ssphW ->  ssphW.temp

    let getStageWeight
            (sorterSetPruner:sorterSetPruner) 
         =
         match sorterSetPruner with
         | Whole ssphW ->  ssphW.stageWeight
         | Lhc ssphW ->  ssphW.stageWeight
