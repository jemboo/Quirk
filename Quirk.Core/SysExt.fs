﻿
module SysExt

let inline zero_of (target:'a) : 'a = LanguagePrimitives.GenericZero<'a>

let inline one_of (target:'a) : 'a = LanguagePrimitives.GenericOne<'a>


(*
2s complement, i.e. invert each bit and add 1
ex. 5 = 00101 (5) -> 11010 -> 11010 + 1 - > 11011 = -5 
ex. 16 = 0b10000 -> ~16+1=-16 = -0b10000 (11111111111111111111111111110000 count=28)
*)
type System.Int32 with // x=this
    (* bit manipulation methods *)
    member x.shrl = int (uint32 x >>> 1) // logical right shift
    member x.shrln i = int (uint32 x >>> i) // logical right shift by n positions
    member x.isset i = x &&& (1 <<< i) <> 0 // test if bit set at a specified position
    member x.get i = x &&& (1 <<< i) <> 0
    member x.set i = x ||| (1 <<< i) // set bit to 1
    member x.unset i = x &&& ~~~(1 <<< i) // set bit from 0
    member x.flip i = (x ^^^ (1 <<< i)) // change bit

    member x.rev = // reverse bits
        let rec go b i acc =
            if i = 32 then
                acc
            else
                go (b >>> 1) (i + 1) ((acc <<< 1) ||| (b &&& 1))

        go x 0 0

    member x.count = // count bits set to 1
        let rec go b acc =
            if b = 0 then acc else go (b &&& (b - 1)) (acc + 1) //sparse count
        //if b = 0 then acc else go (b.shrl) (acc + (b &&& 1)) //add res of calc
        go x 0

    member x.count_dense = // The loop will execute once for each unset bit
        32 - ((~~~x).count) //do sparse count

    //as opposed to diff: 0101&(~1100)=0101&0011=0001 xor returns mutual difference: 0101^1100=1001
    member x.diff y = x &&& (~~~y) // subtract y from x
    member x.subset y = (x &&& y) = x // x &&& (~~~ super) = 0  //must be in Bitmap module
    member x.propersubset y = (x < y) && ((x &&& y) = x)
    member x.rotateLeft r = (x <<< r) ||| (x >>> (32 - r))
    member x.rotateRight r = (x >>> r) ||| (x <<< (32 - r))

    member x.rotate r =
        if r > 0 then x.rotateLeft r else x.rotateRight (-r)

    member x.contains_zero_byte = ((x - 0x01010101) ^^^ x) &&& (~~~x) &&& 0x80808080
    member x.rightmost_one = x &&& (-x)
    member x.rightmost_zero = (x ^^^ (x + 1)) &&& ~~~x

    member x.leftmost_one = //??
        let mutable y = 0
        y <- x ||| (x >>> 1)
        y <- y ||| (y >>> 2)
        y <- y ||| (y >>> 4)
        y <- y ||| (y >>> 8)
        y <- y ||| (y >>> 16)
        //y <- y ||| (y>>>32)
        y ^^^ (y >>> 1)

    member x.leftmost_zero = x.leftmost_one <<< 1 //??

    member x.rightmost_index = // index of lowest bit set
        let mutable r = 0
        let y = x &&& -x // isolate lowest bit

        if y &&& 0xffff0000 <> 0 then
            r <- r + 16

        if y &&& 0xff00ff00 <> 0 then
            r <- r + 8

        if y &&& 0xf0f0f0f0 <> 0 then
            r <- r + 4

        if y &&& 0xcccccccc <> 0 then
            r <- r + 2

        if y &&& 0xaaaaaaaa <> 0 then
            r <- r + 1

        r

    member x.leftmost_index =
        if 0 = x then
            0
        else
            let mutable r = 0
            let mutable y = x

            if y &&& 0xffff0000 <> 0 then
                y <- y >>> 16
                r <- r + 16

            if y &&& 0x0000ff00 <> 0 then
                y <- y >>> 8
                r <- r + 8

            if y &&& 0x000000f0 <> 0 then
                y <- y >>> 4
                r <- r + 4

            if y &&& 0x0000000c <> 0 then
                y <- y >>> 2
                r <- r + 2

            if y &&& 0x00000002 <> 0 then
                r <- r + 1

            r

    (* bit coersion methods *)
    member x.toHex = sprintf "0x%x" x // to hexadecimal
    member x.toBits = System.Convert.ToString(x, 2).PadLeft(32, '0') // to binary

    member x.toBoolArray (length:int) =
        let aRet = Array.create length false
        for i = 0 to (length - 1) do
            if x.isset i then
                aRet.[i] <- true
        aRet


    member x.applyBoolArray (bA:bool[]) =
        let mutable iRet = x
        for i = 0 to (bA.Length - 1) do
            if bA.[i] then
                iRet <- iRet.set i
        iRet


    member x.toResizeArray = // to Resizable array of positions set to 1
        let array = ResizeArray()
        for i = 0 to 31 do
            if x.isset i then
                array.Add(i)
        array

    member x.toArray = // to array of positions set to 1
        let res = x.toResizeArray
        res.ToArray()

    member x.toList = // to list of positions set to 1
        let res = x.toResizeArray
        Array.toList (res.ToArray())

    member x.toSeq = // to seq of positions set to 1
        let res = x.toResizeArray
        Array.toSeq (res.ToArray())

    (* bit print methods *)
    member x.print = printf "%A" x

    member x.display = // helper to show bits
        x.toArray |> Seq.iter (fun i -> printf "%A " i)

    (* misc methods *)
    member x.abs = (x ^^^ (x >>> 31)) - (x >>> 31) //3000% faster than standard math.abs


type System.UInt16 with // x=this
    (* bit manipulation methods *)
    member x.isset i = x &&& (1us <<< i) <> 0us // test if bit set at a specified position
    member x.get i = x &&& (1us <<< i) <> 0us
    member x.set i = x ||| (1us <<< i) // set bit to 1
    member x.unset i = x &&& ~~~(1us <<< i) // set bit from 0
    member x.flip i = (x ^^^ (1us <<< i)) // change bit


type System.UInt64 with // x=this
    (* bit manipulation methods *)
    member x.isset i = x &&& (1uL <<< i) <> 0uL // test if bit set at a specified position
    member x.get i = x &&& (1uL <<< i) <> 0uL
    member x.set i = x ||| (1uL <<< i) // set bit to 1
    member x.unset i = x &&& ~~~(1uL <<< i) // set bit from 0
    member x.flip i = (x ^^^ (1uL <<< i)) // change bit

    member x.rev = // reverse bits
        let rec go b i acc =
            if i = 64 then
                acc
            else
                go (b >>> 1) (i + 1) ((acc <<< 1) ||| (b &&& 1uL))

        go x 0 0uL

    member x.count = // count bits set to 1
        let rec go b acc =
            if b = 0uL then acc else go (b &&& (b - 1uL)) (acc + 1uL) //sparse count
        //if b = 0 then acc else go (b.shrl) (acc + (b &&& 1)) //add res of calc
        go x 0uL

    member x.count_dense = // The loop will execute once for each unset bit
        64uL - ((~~~x).count) //do sparse count

    //as opposed to diff: 0101&(~1100)=0101&0011=0001 xor returns mutual difference: 0101^1100=1001
    member x.diff y = x &&& (~~~y) // subtract y from x
    member x.subset y = (x &&& y) = x // x &&& (~~~ super) = 0  //must be in Bitmap module
    member x.propersubset y = (x < y) && ((x &&& y) = x)
    member x.rotateLeft r = (x <<< r) ||| (x >>> (64 - r))
    member x.rotateRight r = (x >>> r) ||| (x <<< (64 - r))

    member x.rotate r =
        if r > 0 then x.rotateLeft r else x.rotateRight (-r)
    //member x.contains_zero_byte = ((x-0x01010101)^^^x) &&& (~~~x) &&& 0x80808080

    member x.leftmost_one = //??
        let mutable y = 0uL
        y <- x ||| (x >>> 1)
        y <- y ||| (y >>> 2)
        y <- y ||| (y >>> 4)
        y <- y ||| (y >>> 8)
        y <- y ||| (y >>> 16)
        //y <- y ||| (y>>>32)
        y ^^^ (y >>> 1)

    member x.leftmost_zero = x.leftmost_one <<< 1 //??
    //member x.rightmost_index = // index of lowest bit set
    //  let mutable r = 0
    //  let y = x &&& -x // isolate lowest bit
    //  if y &&& 0xffff0000 <> 0 then r <- r + 16
    //  if y &&& 0xff00ff00 <> 0 then r <- r + 8
    //  if y &&& 0xf0f0f0f0 <> 0 then r <- r + 4
    //  if y &&& 0xcccccccc <> 0 then r <- r + 2
    //  if y &&& 0xaaaaaaaa <> 0 then r <- r + 1
    //  r
    member x.leftmost_index =
        if 0uL = x then
            0
        else
            let mutable r = 0
            let mutable y = x

            if y &&& 18446744069414584320UL <> 0uL then
                y <- y >>> 32
                r <- r + 32

            if y &&& 4294901760UL <> 0uL then
                y <- y >>> 16
                r <- r + 16

            if y &&& 65280UL <> 0uL then
                y <- y >>> 8
                r <- r + 8

            if y &&& 240UL <> 0uL then
                y <- y >>> 4
                r <- r + 4

            if y &&& 12UL <> 0uL then
                y <- y >>> 2
                r <- r + 2

            if y &&& 2UL <> 0uL then
                r <- r + 1

            r

    (* bit coersion methods *)
    member x.toHex = sprintf "0x%x" x // to hexadecimal
    // member x.toBits = System.Convert.ToString(x, 2).PadLeft(32, '0') // to binary
    member x.toResizeArray = // to Resizable array of positions set to 1
        let array = ResizeArray()

        for i = 0 to 63 do
            if x.isset i then
                array.Add(i)

        array

    member x.toArray = // to array of positions set to 1
        let res = x.toResizeArray
        res.ToArray()

    member x.toList = // to list of positions set to 1
        let res = x.toResizeArray
        Array.toList (res.ToArray())

    member x.toSeq = // to seq of positions set to 1
        let res = x.toResizeArray
        Array.toSeq (res.ToArray())

    (* bit print methods *)
    member x.print = printf "%A" x

    member x.display = // helper to show bits
        x.toArray |> Seq.iter (fun i -> printf "%A " i)

    (* misc methods *)
    member x.abs = (x ^^^ (x >>> 31)) - (x >>> 31) //3000% faster than standard math.abs


type System.Byte with // x=this
    (* bit manipulation methods *)
    member x.isset i = x &&& (1uy <<< i) <> 0uy // test if bit set at a specified position
    member x.get i = x &&& (1uy <<< i) <> 0uy
    member x.set i = x ||| (1uy <<< i) // set bit to 1
    member x.unset i = x &&& ~~~(1uy <<< i) // set bit from 0
    member x.flip i = (x ^^^ (1uy <<< i)) // change bit
