using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class FileProtocol : MonoBehaviour
{
    public static FileProtocol Instance { get; private set; }
    public string StartPath = "";

    [SerializeField] GameObject FolderPrefab;

    List<GameObject> OpenPaths;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        var OS = SystemInfo.operatingSystem;
        OS.ToLower();

        if (OS.Contains("Windows"))
        {
            StartPath = "C:";
        }
        else if (OS.Contains("Linux"))
        {
            StartPath = "/";
        }
    }

    [SerializeField]
    List<PathProtocol> CurrentlyOpenFolders;

    void Start()
    {
        CurrentlyOpenFolders = new List<PathProtocol>();
        GameObject StartPrefab = Instantiate(FolderPrefab);
        StartPrefab.GetComponent<PathProtocol>().SetupPath(StartPath);
        StartPrefab.name = "";
    }

    public void OpenFolder(string Path,GATEProtocol RequestedBy)
    {
        GameObject RequestFolder = Instantiate(FolderPrefab);
        PathProtocol pathProtocol = RequestFolder.GetComponent<PathProtocol>();
        Transform Parent = RequestedBy.transform;

        DirectoryInfo Info = new DirectoryInfo(Path);
        int FileCount = Info.GetFiles().Length, FolderCount = Info.GetDirectories().Length, TotalCount = FileCount + FolderCount;
        Vector3 SpawnPos = new Vector3(Parent.position.x, -50, Parent.position.z) + Parent.transform.forward * FolderCount;
        RequestFolder.transform.position = SpawnPos;
        RequestFolder.transform.rotation = RequestedBy.transform.rotation;
        RequestFolder.GetComponent<PathProtocol>().SetupPath(Path);
        CurrentlyOpenFolders.Add(pathProtocol);
    }

    public void CloseFolder(string gatePath, GATEProtocol gATEProtocol)
    {
        PathProtocol FoundPeePee = CurrentlyOpenFolders.Find(x => x.GetPath() == gatePath);
        if (FoundPeePee != null)
        {
            FoundPeePee.KillYourSelf();
            CurrentlyOpenFolders.Remove(FoundPeePee);
        }
        else
            Debug.LogError($"Path not found: {gatePath}", gATEProtocol.gameObject);
    }

    public Vector3 StringToVector3(string Input)
    {
        Input = Input.Trim('(', ')');
        string[] Values = Input.Split(',');
        float x = float.Parse(Values[0], CultureInfo.InvariantCulture),
              y = float.Parse(Values[1], CultureInfo.InvariantCulture),
              z = float.Parse(Values[2], CultureInfo.InvariantCulture);
        return new Vector3(x, y, z);
    }
    
    public Color StringToColor3(string Input)
    {
        Input = Input.Trim('(', ')');
        string[] Values = Input.Split(',');
        float x = float.Parse(Values[0], CultureInfo.InvariantCulture),
              y = float.Parse(Values[1], CultureInfo.InvariantCulture),
              z = float.Parse(Values[2], CultureInfo.InvariantCulture);
        return new Color(x,y,z);
    }
}
