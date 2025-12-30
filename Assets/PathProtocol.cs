using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class PathProtocol : MonoBehaviour
{
    [SerializeField] string Path;
    [SerializeField] long SizeInBytes;
    [SerializeField] int FilesInside;
    [SerializeField] int DirsInside = 1;
    [SerializeField] bool Debug = false;

    [Header("Prefabs")]
    [SerializeField] GameObject GatePrefab;
    [SerializeField] Extensions[] OtherPrefabs;

    void Awake() //Debug
    {
        if (Debug)
        SetupPath(Path);
    }

    public void SetupPath(string Path)
    {
        this.Path = Path;

        StartCoroutine(PathHandle());
    }

    IEnumerator PathHandle()
    {
        DirectoryInfo DirInfo = new DirectoryInfo(Path);
        FileInfo[] Files = DirInfo.GetFiles();
        DirectoryInfo[] Directories = DirInfo.GetDirectories();

        long BiggestFileSize = 0;
        float GateForwardBound = GatePrefab.GetComponent<Collider>().bounds.size.z;

        foreach (var File in Files)
        {
            SizeInBytes += File.Length;
            FilesInside++;

            if (File.Length > BiggestFileSize)
                BiggestFileSize = File.Length;
        }

        foreach (var Directory in Directories)
        {
            DirsInside++;
        }

        int TotalObjects = FilesInside + DirsInside;
        print($"Objects count is {TotalObjects} with biggest File size being {BiggestFileSize / 1048576}MB");

        //End of gathering directory info

        Vector3 StartScale = transform.localScale, EndScale = Vector3.up * 100 + (Vector3.right * DirsInside * GateForwardBound + Vector3.forward * DirsInside * GateForwardBound);
        for (int F = 0; F < 100; F++)
        {
            transform.localScale = Vector3.Lerp(StartScale, EndScale, F / 100f); //Be sure to add 'f' to near static intreger numbers for no accidents.
            yield return new WaitForEndOfFrame();
        }

        //End of animation

        float TotalArea = DirsInside * 3;
    }
    
    [Serializable] public class Extensions
    {
        public string Extension;
        public GameObject Prefab;
    }
}
