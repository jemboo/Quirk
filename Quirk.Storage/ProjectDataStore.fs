namespace Quirk.Storage


type ProjectFileStore (wsRootDir:string) =

    member this.wsRootDir = wsRootDir
    member this.fileExt = "txt"
