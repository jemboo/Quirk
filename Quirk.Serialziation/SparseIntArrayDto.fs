namespace Quirk.Serialization

open System
open Microsoft.FSharp.Core
open FSharp.UMX
open Quirk.Core


type sparseIntArrayDto = 
    { emptyVal:int; 
      length:int; 
      indexes:int[];
      values:int[] }

module SparseIntArrayDto =

    let fromDto 
            (dto:sparseIntArrayDto) 
        =
        result {
            return 
                SparseArray.create
                    dto.length
                    dto.indexes
                    dto.values
                    dto.emptyVal
        }

    let toDto 
            (sia:sparseArray<int>) 
        =
        { 
            emptyVal = sia |> SparseArray.getEmptyVal; 
            length = sia |> SparseArray.getLength; 
            indexes = sia |> SparseArray.getIndexes;
            values =  sia |> SparseArray.getValues
        }
        
    let fromJson 
            (cereal:string)
        =
        result {
            let! dto = Json.deserialize<sparseIntArrayDto> cereal
            return! fromDto dto
        }

    let toJson 
            (sia:sparseArray<int>) 
        = 
        sia |> toDto |> Json.serialize

