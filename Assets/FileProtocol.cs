using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileProtocol : MonoBehaviour
{
    public string CharacterPath = ""; //THIS CAN NOT BE EMPTY.
    public string StartPath = "";
    [SerializeField] GameObject FolderPrefab;

    List<GameObject> OpenPaths;

    void Awake()
    {
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
}
