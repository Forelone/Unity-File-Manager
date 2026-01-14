using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFileInfo : MonoBehaviour
{
    [HideInInspector]
    public string Name, Extension, Path;
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
        SetupComplete = true;
    }
}
