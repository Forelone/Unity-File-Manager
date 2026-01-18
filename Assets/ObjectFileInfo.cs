using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ObjectFileInfo : MonoBehaviour
{
    [HideInInspector]
    public string Name, Extension, Path, DirPath;
    [HideInInspector]
    public long Size;
    [HideInInspector]
    public DateTime CreationDate, ModificationDate;
    [HideInInspector]
    public bool Protected;
    bool SetupComplete = false;
    public void Setup(string Name,string Extension, string Path, long Size, DateTime CreationDate, DateTime ModificationDate, bool Protected)
    {
        if (SetupComplete) return;
        this.Name = Name;
        this.Extension = Extension;
        this.Size = Size;
        this.CreationDate = CreationDate;
        this.ModificationDate = ModificationDate;
        this.Path = Path;
        this.Protected = Protected;

        string[] S = Path.Split('/');
        S[S.Length - 1] = string.Empty;
        DirPath = string.Join('/',S);

        SetupComplete = true;
    }

    public void Setup(FileInfo fileInfo)
    {
        if (SetupComplete) return;
        this.Name = fileInfo.Name;
        this.Extension = fileInfo.Extension;
        this.Size = fileInfo.Length;
        this.CreationDate = fileInfo.CreationTime;
        this.ModificationDate = fileInfo.LastWriteTime;
        this.Path = fileInfo.FullName;
        this.Protected = fileInfo.IsReadOnly;

        string[] S = Path.Split('/');
        S[S.Length - 1] = string.Empty;
        DirPath = string.Join('/',S);

        SetupComplete = true;
    }

    public void Setup(ObjectFileInfo OFI)
    {
        if (SetupComplete) return;

        this.Name = OFI.Name;
        this.Extension = OFI.Extension;
        this.Size = OFI.Size;
        this.CreationDate = OFI.CreationDate;
        this.ModificationDate = OFI.ModificationDate;
        this.Path = OFI.Path;
        this.Protected = OFI.Protected;

        string[] S = Path.Split('/');
        S[S.Length - 1] = string.Empty;
        DirPath = string.Join('/',S);

        SetupComplete = true;
    }

    public FileInfo ToFileInfo()
    {
        FileInfo FI = new FileInfo(Path);
        return FI;
    }
}
