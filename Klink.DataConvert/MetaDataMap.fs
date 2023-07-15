namespace global
open System

type metaDataMapId = private MetaDataMapId of Guid
module MetaDataMapId =
    let value (MetaDataMapId v) = v
    let create vl = MetaDataMapId vl

type metaDataMap =
    private 
        { id: metaDataMapId; 
          data: Map<string,string> }

module MetaDataMap =
    let load 
            (id:metaDataMapId) 
            (data: Map<string,string>)
        =
        {
            id = id;
            data = data;
        }

    let getId (metaDataMap:metaDataMap) =
        metaDataMap.id

    let getData (metaDataMap:metaDataMap) =
        metaDataMap.data

