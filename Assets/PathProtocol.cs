using System;
using System.Collections;
using System.Collections.Generic;
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

        Vector3 StartScale = transform.localScale, EndScale = Vector3.up * 100 + (Vector3.right * DirsInside + Vector3.forward * DirsInside);
        for (int F = 0; F < 100; F++)
        {
            transform.localScale = Vector3.Lerp(StartScale, EndScale, F / 100f); //Be sure to add 'f' to near static intreger numbers for no accidents.
            yield return new WaitForEndOfFrame();
        }

        //End of animation

        List<Vector3> GatePos = new List<Vector3>();
        List<Quaternion> GateRot = new List<Quaternion>();

        float Hayflayf = DirsInside / 2f;
        Vector3 Line1Start = transform.position - transform.right * Hayflayf - transform.forward * Hayflayf + transform.up * 50,
                Line1End = transform.position - transform.right * Hayflayf + transform.forward * Hayflayf+ transform.up * 50,
                Line2Start = Line1End,
                Line2End = transform.position + transform.right * Hayflayf + transform.forward * Hayflayf+ transform.up * 50,
                Line3Start = Line2End,
                Line3End = transform.position + transform.right * Hayflayf - transform.forward * Hayflayf+ transform.up * 50;

        int LineCount = DirsInside / 3;

        for (int i = 0; i < LineCount; i++)
        {
            float Status = i / (float)LineCount;
            GatePos.Add(Vector3.Lerp(Line1Start, Line1End, Status));
            GateRot.Add(Quaternion.Euler(transform.eulerAngles + Vector3.up * 90f));
            yield return new WaitForEndOfFrame();
        }
        for (int i = 1; i < LineCount; i++)
        {
            float Status = i / (float)LineCount;
            GatePos.Add(Vector3.Lerp(Line2Start, Line2End, Status));
            GateRot.Add(Quaternion.Euler(transform.eulerAngles + Vector3.up * 0f));
            yield return new WaitForEndOfFrame();
        }
        for (int i = 0; i <= LineCount; i++)
        {
            float Status = i / (float)LineCount;
            GatePos.Add(Vector3.Lerp(Line3Start, Line3End, Status));
            GateRot.Add(Quaternion.Euler(transform.eulerAngles + Vector3.up * -90f));
            yield return new WaitForEndOfFrame();
        }
        //TODO: Fix that shit that causes overflow on gates.

        GameObject[] Gates = new GameObject[DirsInside];
        for (int G = 0; G < Gates.Length; G++)
        {
            GameObject GateFab = Instantiate(GatePrefab);
            GateFab.transform.SetPositionAndRotation(GatePos[G], GateRot[G]);
            //Todo: add a gate script to make new paths.
            Gates[G] = GateFab;
        }
    }

    [Serializable]
    public class Extensions
    {
        public string Extension;
        public GameObject Prefab;
    }
}
