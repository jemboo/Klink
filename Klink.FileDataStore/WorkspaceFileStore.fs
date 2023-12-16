namespace global
open System
open System.IO


type WorkspaceFileStore (wsRootDir:string) =

    member this.wsRootDir = wsRootDir
    member this.fileExt = "txt"


    member this.getFolderName (wsCompType:workspaceComponentType option) =
        match wsCompType with
        | Some v -> v |> string
        | None -> ""

    member this.writeToFileIfMissing (wsCompType:workspaceComponentType option) (fileName:string) (data: string) =
        TextIO.writeToFileIfMissing this.fileExt (Path.Combine(this.wsRootDir, this.getFolderName wsCompType)) fileName data

    member this.writeToFileOverwrite (wsCompType:workspaceComponentType option) (fileName:string) (data: string) =
        TextIO.writeToFileOverwrite this.fileExt (Path.Combine(this.wsRootDir, this.getFolderName wsCompType)) fileName data

    member this.writeLinesEnsureHeader (wsCompType:workspaceComponentType option) (fileName:string) (hdr: seq<string>) (data: string seq) =
        TextIO.writeLinesEnsureHeader this.fileExt (Path.Combine(this.wsRootDir, this.getFolderName wsCompType)) fileName hdr data

    member this.appendLines (wsCompType:workspaceComponentType option) (fileName:string) (data: string seq) =
        TextIO.appendLines this.fileExt (Path.Combine(this.wsRootDir, this.getFolderName wsCompType)) fileName data

    member this.fileExists (wsCompType:workspaceComponentType option) (fileName:string) =
        TextIO.fileExists this.fileExt (Path.Combine(this.wsRootDir, this.getFolderName wsCompType)) fileName

    member this.readAllText (wsCompType:workspaceComponentType option) (fileName:string) =
        TextIO.readAllText this.fileExt (Path.Combine(this.wsRootDir, this.getFolderName wsCompType)) fileName

    member this.readAllLines (wsCompType:workspaceComponentType option) (fileName:string) =
        TextIO.readAllLines this.fileExt (Path.Combine(this.wsRootDir, this.getFolderName wsCompType)) fileName

    member this.getAllFiles (wsCompType:workspaceComponentType option) =
           let filePath = Path.Combine(this.wsRootDir, this.getFolderName wsCompType)
           Directory.GetFiles(filePath)

    member this.markLastWorkspaceId (wsId:workspaceId) = 
           TextIO.writeToFileOverwrite 
                    this.fileExt 
                    this.wsRootDir
                    "lastUpdate" 
                    (wsId |> WorkspaceId.value |> string )

    member this.getLastWorkspaceId () =
           TextIO.readAllText this.fileExt this.wsRootDir "lastUpdate"
           |> Result.map (Guid.Parse >> WorkspaceId.create)


    member this.compStore (wsComp:workspaceComponent) 
        = 
        result {
            let wsCompType = wsComp |> WorkspaceComponent.getWorkspaceComponentType
            let cereal = wsComp |> WorkspaceComponentDto.toJsonT
            let fileName = wsComp |> WorkspaceComponent.getId |> string
            let! res = this.writeToFileIfMissing (Some wsCompType) fileName cereal
            return fileName
        }


    member this.compRetreive
                (compId:Guid) 
                (wsCompType:workspaceComponentType) 
        = 
        result {
            let fileName = compId |> string
            let! cereal = this.readAllText (Some wsCompType) fileName
            return! cereal |> WorkspaceComponentDto.fromJson wsCompType
        }


    member this.getAllComponents
               (wsCompType:workspaceComponentType) 
        = 
        this.getAllFiles (Some wsCompType)
            |> Array.map(Path.GetFileNameWithoutExtension >> Guid.Parse)
            |> Array.map(fun gu -> this.compRetreive gu wsCompType)


    member this.getComponent
            (wscn:wsComponentName)
            (wsd:workspaceDescription)
        =
        result {
            let yab = wsd |> WorkspaceDescription.getComponents
            if yab.ContainsKey wscn then
                let compDescr = yab.[wscn]
                let compId = compDescr |> WorkspaceComponentDescr.getId
                let compType = compDescr |> WorkspaceComponentDescr.getCompType
                return! (this.compRetreive compId compType)
            else 
                return! $"missing {wscn.ToString()}" |> Error
        }

    member this.getComponentWithParams
                (wscn:wsComponentName)
                (wsd:workspaceDescription)
        =
        result {
            let yab = wsd |> WorkspaceDescription.getComponents
            if yab.ContainsKey wscn then
                let compDescr = yab.[wscn]
                let compDescrParams = yab.[WsConstants.workSpaceComponentNameForParams]
                let compId = compDescr |> WorkspaceComponentDescr.getId
                let compType = compDescr |> WorkspaceComponentDescr.getCompType
                let compIdParams = compDescrParams |> WorkspaceComponentDescr.getId
                let compTypeParams = compDescrParams |> WorkspaceComponentDescr.getCompType

                let! wsComp = this.compRetreive compId compType
                let! wsParams = this.compRetreive compIdParams compTypeParams
                                    |> Result.bind(WorkspaceComponent.asWorkspaceParams)
                let rv = (wsComp, wsParams)
                return Some rv
            else 
                return None
               // return! $"{wscn |> WsComponentName.value} not found (403)" |> Error
        }


    member this.getAllComponentsByName
                     (wscn:wsComponentName)
        = 
        result {
            let! wsDescrs = 
                    this.getAllComponents workspaceComponentType.WorkspaceDescription
                      |> Array.map(Result.bind(WorkspaceComponent.asWorkspaceDescription))
                      |> Array.toList
                      |> Result.sequence

            return!  wsDescrs 
                            |> List.map(this.getComponent wscn)
                            |> Result.sequence
        }


    member this.getAllWorkspaceDescriptionsWithParams()
        = 
        result {
            let! wsDescrs = 
                    this.getAllComponents workspaceComponentType.WorkspaceDescription
                      |> Array.map(Result.bind(WorkspaceComponent.asWorkspaceDescription))
                      |> Array.toList
                      |> Result.sequence


            let! yab = wsDescrs 
                            |> List.map(this.getComponent WsConstants.workSpaceComponentNameForParams)
                            |> Result.sequence


            let yab2 = 
                wsDescrs 
                |> List.map(
                    fun descr -> 
                    (descr, 
                     descr |> this.getComponent WsConstants.workSpaceComponentNameForParams)
                           )
                |> List.map(
                    fun (descr, prams) -> 
                        (
                            descr,
                            prams |> Result.bind(WorkspaceComponent.asWorkspaceParams)
                        )
                           )

            return 
                yab2 
                |> List.filter(fun (descr, pramsR) -> pramsR |> Result.isOk)
                |> List.map(
                    fun (descr, prams) -> 
                    (
                        descr,
                        prams |> Result.ExtractOrThrow
                    )
                           )
        }


    member this.getAllComponentsWithParams
                     (evalCompName:wsComponentName)
        = 
        result {
            let! wsDescrs = 
                    this.getAllComponents workspaceComponentType.WorkspaceDescription
                      |> Array.map(Result.bind(WorkspaceComponent.asWorkspaceDescription))
                      |> Array.toList
                      |> Result.sequence

            return! wsDescrs 
                        |> List.map(this.getComponentWithParams evalCompName)
                        |> Result.sequence

        }

    member this.getAllSorterSetEvalsWithParams
                     (evalCompName:wsComponentName)
                     (fF:workspaceParams -> bool)
        = 
        result {
            let! wsDescrs = 
                    this.getAllComponents workspaceComponentType.WorkspaceDescription
                      |> Array.map(Result.bind(WorkspaceComponent.asWorkspaceDescription))
                      |> Array.toList
                      |> Result.sequence

            let! tupOpts = 
                    wsDescrs
                      |> List.map(this.getComponentWithParams evalCompName)
                      |> Result.sequence
                            
            let foundComps = tupOpts 
                             |> List.filter(Option.isSome) 
                             |> List.map(Option.get)
                             |> List.filter(fun (cp, wsps) -> wsps |> fF)


            return! foundComps 
                       |> List.map(fun (wsC, wsP) -> (wsC |> WorkspaceComponent.asSorterSetEval, wsP))
                       |> List.map(Result.tupLeft)
                       |> Result.sequence

        }


    member this.getAllSpeedSetBinsWithParams
                     (wscn:wsComponentName)
                     (fF:workspaceParams -> bool)
        = 
        result {
            let! wsDescrs = 
                    this.getAllComponents workspaceComponentType.WorkspaceDescription
                      |> Array.map(Result.bind(WorkspaceComponent.asWorkspaceDescription))
                      |> Array.toList
                      |> Result.sequence

            let! tupOpts = 
                    wsDescrs
                      |> List.map(this.getComponentWithParams wscn)
                      |> Result.sequence
                            
            let foundComps = tupOpts 
                             |> List.filter(Option.isSome) 
                             |> List.map(Option.get)
                             |> List.filter(fun (cp, wsps) -> wsps |> fF)


            return! foundComps 
                       |> List.map(fun (wsC, wsP) -> (wsC |> WorkspaceComponent.asSorterSpeedBinSet, wsP))
                       |> List.map(Result.tupLeft)
                       |> Result.sequence

        }


    member this.workSpaceExists (id:workspaceId) =
        result {
            let fileName = id |> WorkspaceId.value |> string
            return!                   
                this.fileExists (Some workspaceComponentType.WorkspaceDescription) fileName
        }

    member this.loadWorkSpace (id:workspaceId) =
        result {
            let fileName = id |> WorkspaceId.value |> string
            let! cereal = this.readAllText (Some workspaceComponentType.WorkspaceDescription) fileName
            let! wsd = cereal |> WorkspaceDescriptionDto.fromJson
            return! wsd |> Workspace.ofWorkspaceDescription this.compRetreive
        }

    member this.loadParameters (id:workspaceId) =
        result {
            let fileName = id |> WorkspaceId.value |> string
            let! cereal = this.readAllText (Some workspaceComponentType.WorkspaceDescription) fileName
            let! wsd = cereal |> WorkspaceDescriptionDto.fromJson
            return! wsd |> Workspace.ofWorkspaceDescription this.compRetreive
        }

    member this.saveWorkSpace (workspace:workspace) (filter:wsComponentName -> bool)  =
        result {
            let fileName = workspace |> Workspace.getId |> WorkspaceId.value |> string
            let cereal = workspace |> Workspace.toWorkspaceDescription
                                   |> WorkspaceDescriptionDto.toJson
            let! res = this.writeToFileIfMissing (Some workspaceComponentType.WorkspaceDescription) fileName cereal
            let! res2 = this.markLastWorkspaceId (workspace |> Workspace.getId)
            let _, comps = workspace 
                            |> Workspace.getWsComponents 
                            |> Map.toArray
                            |> Array.filter (fst >> filter )
                            |> Array.unzip
            let! _ = comps |> Array.toList |> List.map(this.compStore) |> Result.sequence
            return fileName
        }

    interface IWorkspaceStore with
        member this.SaveWorkSpace ws filter = this.saveWorkSpace ws filter
        member this.LoadWorkSpace wsId = this.loadWorkSpace wsId
        member this.WorkSpaceExists wsId = this.workSpaceExists wsId
        member this.GetLastWorkspaceId() = this.getLastWorkspaceId ()
        member this.WriteLinesEnsureHeader wsc folder hdr data = this.writeLinesEnsureHeader wsc folder hdr data
        member this.GetAllSorterSetEvalsWithParams evalCompName fF = this.getAllSorterSetEvalsWithParams evalCompName fF
        member this.GetAllSpeedSetBinsWithParams evalCompName fF = this.getAllSpeedSetBinsWithParams evalCompName fF
        member this.GetAllWorkspaceDescriptionsWithParams () = this.getAllWorkspaceDescriptionsWithParams ()
        member this.GetComponent wsCompName wsDescr = this.getComponent wsCompName wsDescr 