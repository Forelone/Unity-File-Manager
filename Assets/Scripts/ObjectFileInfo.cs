using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectFileInfo : MonoBehaviour
{
    [HideInInspector]
    public PathProtocol.CustomFileSave CFS;
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

    public void Setup(string Path)
    {
        if (SetupComplete) return;

        FileInfo fileInfo = new FileInfo(Path);

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

    public void AddTag(string Tag,string[] Vars)
    {
        CFS = CFS == null ? new PathProtocol.CustomFileSave() : CFS;
        CFS.Tags = CFS.Tags == null ? new List<string>() : CFS.Tags;
        CFS.Name = Name;
        string T = Tag + ' ' + string.Join(' ',Vars);
        
        bool Exists = false;
        for (int i = 0; i < CFS.Tags.Count; i++)
        {
            string[] Str = CFS.Tags[i].Split(' ');
            if (Tag == Str[0])
            {
                CFS.Tags[i] = T;
                Exists = true;
                break;
            } 
            else continue;
        }
        if (!Exists) CFS.Tags.Add(T);
    }

    public void LoadTags(string[] Tags_)
    {
        CFS = CFS == null ? new PathProtocol.CustomFileSave() : CFS;
        CFS.Tags = CFS.Tags ?? Tags_.ToList();
        CFS.Name = Name;
    }
}
