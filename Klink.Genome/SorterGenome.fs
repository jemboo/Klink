namespace global


type epiSite<'y> = private {payloadA:'y; payloadB:'y; activation:boundedFloat}

module EpiSite =
    
    let create<'y> 
            (payloadA:'y) 
            (payloadB:'y) 
            (activation:boundedFloat)
        =
        {payloadA=payloadA; payloadB = payloadB; activation = activation}

    let getActivation (site: epiSite<'y>) =
        site.activation


    let applyDelta (site: epiSite<'y>)
                   (selectDelta:float)
        =
        { site with activation = BoundedFloat.addValue site.activation selectDelta}



    let _selectA<'y> 
            (site: epiSite<'y>)
            (selectVal:float)
            (selectDelta:float)
        =
            if ((site.activation |> BoundedFloat.value) < selectVal) then
                (site.payloadA, applyDelta site (- selectDelta) )
            else
                (site.payloadB, applyDelta site ( selectDelta) )



    let _selectB<'y> 
            (site: epiSite<'y>)
            (selectVal:float)
            (selectDelta:float)
        =
            if ((site.activation |> BoundedFloat.value) + selectVal < 0) then
                (site.payloadA, applyDelta site (- selectDelta) )
            else
                (site.payloadB, applyDelta site ( selectDelta) )




    let epiSelectA
            (randy:IRando)
            (dev:float)
            (selectDelta:float)
            (sites: epiSite<'y> array)
        =
        let mean = 0.0
        let selectVals =  
                RandVars.gaussianDistribution mean dev randy
                |> Seq.take (sites.Length)
                |> Seq.toArray
        
        let siteSelections = 
               sites
                |> Array.mapi(fun dex site -> _selectA site selectVals.[dex] selectDelta)

        siteSelections




    let epiSelectB
            (randy:IRando)
            (dev:float)
            (selectDelta:float)
            (sites: epiSite<'y> array)
        =
        let mean = 0.0
        let selectVals =  
                RandVars.gaussianDistribution mean dev randy
                |> Seq.take (sites.Length)
                |> Seq.toArray
        
        let siteSelections = 
               sites
                |> Array.mapi(fun dex site -> _selectB site selectVals.[dex] selectDelta)

        siteSelections




type activated<'y,'r> = private {payload:'y; activationParam:'r}

module Activated =

    let make y r =  { payload = y; activationParam = r }

    let getParam av = av.activationParam

    let filter (avs : activated<'y,'r> seq) actFun =
        avs |> Seq.filter(fun av -> actFun (av |> getParam))

    let mutatePayload av mutator rate (randy:IRando) =
        if ( randy.NextFloat < (rate |> MutationRate.value)) then
           { payload = mutator av.payload; activationParam = av.activationParam }
           else
           { payload = av.payload; activationParam = av.activationParam }


    let mutateParameter av mutator rate (randy:IRando) =
        if ( randy.NextFloat < (rate |> MutationRate.value)) then
           { payload = av.payload; activationParam = mutator av.activationParam }
           else
           { payload = av.payload; activationParam = av.activationParam }


type chromoIndex = private ChromoIndex of int

module ChromoIndex =
    let value (ChromoIndex v) = v
    let create dex = ChromoIndex dex


type permutationChromo = private {index: chromoIndex; chromo:permutation[]}

module PermutationChromo
       =
    let getIndex (pc:permutationChromo) = pc.index

    let getChromo (pc:permutationChromo) = pc.chromo

    let mutate 
        (rate:mutationRate) 
        (randy:IRando) 
        (pc:permutationChromo)
        =
        let _mutate rndy p =
            if ( randy.NextFloat < (rate |> MutationRate.value)) then
                p |> Permutation.mutate rndy
                else p
        {
            index = pc.index;
            chromo = pc.chromo |> Array.map(_mutate randy)
        }