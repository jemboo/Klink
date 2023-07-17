namespace global
open System

type jsonDataMapId = private JsonDataMapId of Guid
module JsonDataMapId =
    let value (JsonDataMapId v) = v
    let create vl = JsonDataMapId vl

type jsonDataMap =
    private 
        { id: jsonDataMapId; 
          data: Map<string,string> }

module JsonDataMap =
    let load 
            (id:jsonDataMapId) 
            (data: Map<string,string>)
        =
        {
            id = id;
            data = data;
        }

    let make (data: Map<string,string>) =
        let nextId = 
            data |> Map.toArray |> Array.map(fun tup -> tup :> obj)
            |> GuidUtils.guidFromObjs 
            |> JsonDataMapId.create

        load nextId data

    let getId (jsonDataMap:jsonDataMap) =
        jsonDataMap.id

    let getData (jsonDataMap:jsonDataMap) =
        jsonDataMap.data


    let addItem (key:string) (cereal:string) (jsonDataMap:jsonDataMap) =
        let newMap = jsonDataMap.data |> Map.add key cereal
        make newMap

    let addItems 
            (kvps:(string*string) seq) 
            (jsonDataMap:jsonDataMap) =
        Seq.fold (fun wp tup -> addItem (fst tup) (snd tup) wp) jsonDataMap kvps
