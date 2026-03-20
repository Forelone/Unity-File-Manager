using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RaceTrack : MonoBehaviour
{
    [SerializeField]
    Collider Start, End;

    Stopwatch Watch;

    public void RaceStart()
    {
        if (Watch != null) return;

        Watch = Stopwatch.StartNew();
        print("Race start");
    }

    public void RaceEnd()
    {
        if (Watch == null) return;

        Watch.Stop();
        print($"Won in {Watch.Elapsed.Seconds}.{Watch.Elapsed.Milliseconds} seconds!");
        Watch = null;
    }
}
