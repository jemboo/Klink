namespace Klink.Core.Test

open System
open Microsoft.VisualStudio.TestTools.UnitTesting

[<TestClass>]
type BitPackFixture() =

    [<TestMethod>]
    member this.fromIntArrays() =
        let arrayLen = 7 |> ArrayLength.createNr
        let bitsPerSymbl = (1111111uL) |> SymbolSetSize.createNr
                                       |> BitsPerSymbol.fromSymbolSetSize

        let sortedArrays =
            [| [| 0; 1; 3; 4; 5; 6; 9 |]
               [| 110; 221; 333; 444; 5555; 6666; 7779 |]
               [| 10; 21; 43; 64; 85; 96; 109 |]
               [| 1; 11; 31; 41; 51; 61; 91 |] |]

        let bitPack =
            sortedArrays |> BitPack.fromIntArrays bitsPerSymbl

        let sortedArraysBack =
            bitPack |> BitPack.toIntArrays arrayLen

        Assert.IsTrue(CollectionProps.areEqual sortedArrays sortedArraysBack)


    [<TestMethod>]
    member this.fromBoolArrays() =
        let arrayLen = 5 |> ArrayLength.createNr

        let boolArrays =
            [| [| true; true; false; false; true |]
               [| false; true; false; false; false |]
               [| false; true; true; false; true |]
               [| true; true; false; false; true |]
               [| false; true; false; false; false |]
               [| false; true; true; false; true |]
               [| true; true; false; false; true |]
               [| false; true; false; false; false |]
               [| false; true; true; false; true |] |]

        let bytePack = boolArrays |> BitPack.fromBoolArrays


        let boolArraysBack =
            bytePack |> BitPack.toBoolArrays arrayLen

        Assert.IsTrue(CollectionProps.areEqual boolArrays boolArraysBack)



    [<TestMethod>]
    member this.byteArrayToString() =
        let bitsPerSymbl = (1111111uL) |> SymbolSetSize.createNr
                                       |> BitsPerSymbol.fromSymbolSetSize

        let sortedArrays =
            [| [| 0; 1; 3; 4; 5; 6; 9 |]
               [| 110; 221; 333; 444; 5555; 6666; 7779 |]
               [| 10; 21; 43; 64; 85; 96; 109 |]
               [| 1; 11; 31; 41; 51; 61; 91 |] |]

        let bitPack =
            sortedArrays |> BitPack.fromIntArrays bitsPerSymbl

        let bites = bitPack |> BitPack.getData |> Seq.toList

        let strVal = Convert.ToBase64String(bites |> Seq.toArray)

        let bitesBack = Convert.FromBase64String(strVal) |> Array.toList

        Assert.AreEqual(bites, bitesBack)

     //   let strVal = System.Text.Encoding.UTF8.GetString()
