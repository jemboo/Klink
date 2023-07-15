namespace global

open Microsoft.FSharp.Core
open Newtonsoft.Json

module Json =

    type Marker =
        interface
        end

    let serialize obj = JsonConvert.SerializeObject obj

    let deserialize<'a> str : Result<'a, string> =
        try
            JsonConvert.DeserializeObject<'a> str |> Ok
        with ex ->
            Result.Error ex.Message

    let deserializeOption<'a> str =
        match str with
        | Some s -> (deserialize<'a> s)
        | None -> Result.Error "option was none"


    let deserializeMapEntry<'a> 
            key (strMap:Map<string,string>) =
        result {
            if strMap.ContainsKey(key) then
               return! deserialize<'a> (strMap.[key])
            else
               return! $"the key: {key} was not found in the map" 
                        |> Error   
        }