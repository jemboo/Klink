namespace global
open System
open System.IO


type runCfg =
    | Shc of shcRunCfg
    | Ga of gaRunCfg


module RunCfg =

    let procRunCfg 
            (projectFolderPath:string)
            (up:useParallel)
            (runCfg:runCfg)
        =
            match runCfg with
            | Shc shc -> 
                    shc |> ShcRunCfg.procShcRunCfg projectFolderPath up

            //| Ga ga -> 
            //        failwith "not implemnted"
                    // ga |> GaRunCfg.procGaRunCfg projectFolderPath up |> Ok
            //match res with
            //| Ok msg ->  Console.WriteLine($"success: {msg}" )
            //| Error msg ->  Console.WriteLine($"error: {msg}" )
    


    //let procRunCfg 
    //        (projectFolderPath:string)
    //        (up:useParallel)
    //        (runCfg:runCfg)
    //    =
    //        let res = 
    //            match runCfg with
    //            | Shc shc -> 
    //                    shc |> ShcRunCfg.procShcRunCfg projectFolderPath up

    //            | Ga ga -> 
    //                   ga |> GaRunCfg.procGaRunCfg projectFolderPath up |> Ok
    //        Console.WriteLine(res)