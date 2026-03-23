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

        if (StartPath == "")
        {
            StartPath = Application.dataPath;
        }
    }

    [SerializeField]
    List<PathProtocol> CurrentlyOpenFolders;

    void Start()
    {
        CurrentlyOpenFolders = new List<PathProtocol>();
        GameObject StartPrefab = Instantiate(FolderPrefab);
        StartPrefab.GetComponent<PathProtocol>().SetupPath(StartPath);
        StartPrefab.name = $"Folder: {StartPath}";
    }

    public void OpenFolder(string Path,GATEProtocol RequestedBy)
    {
        var ExistingPath = CurrentlyOpenFolders.Find(x => x.GetPath() == Path);

        if (ExistingPath) { Debug.LogError("This path is already open!",ExistingPath.gameObject); return; }

        Transform Parent = RequestedBy.transform;

        DirectoryInfo Info = new DirectoryInfo(Path);
        int FileCount = Info.GetFiles().Length, FolderCount = Info.GetDirectories().Length, TotalCount = FileCount + FolderCount;

        Vector3 SpawnPos = RequestedBy.transform.position + RequestedBy.transform.forward * (TotalCount / 2);

        GameObject RequestFolder = Instantiate(FolderPrefab);
        RequestFolder.transform.position = SpawnPos;
        RequestFolder.transform.rotation = RequestedBy.transform.rotation;
        PathProtocol pathProtocol = RequestFolder.GetComponent<PathProtocol>();
        pathProtocol.SetupPath(Path);
        RequestFolder.name = $"Folder: {Path}";
        CurrentlyOpenFolders.Add(pathProtocol);
    }

    public void CloseFolder(string gatePath, GATEProtocol gATEProtocol)
    {
        PathProtocol FoundPeePee = CurrentlyOpenFolders.Find(x => x.GetPath() == gatePath);
        if (FoundPeePee != null)
        {
            FoundPeePee.Save();
            CurrentlyOpenFolders.Remove(FoundPeePee);
        }
        else
            Debug.LogError($"Path not found or is already closed: {gatePath}", gATEProtocol.gameObject);
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
