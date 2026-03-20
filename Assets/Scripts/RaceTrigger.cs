using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceTrigger : MonoBehaviour
{
    [SerializeField] bool IsFinishLine;

    void OnTriggerEnter(Collider other)
    {
        RaceTrack RT = GetComponentInParent<RaceTrack>();

        if (IsFinishLine)
        RT.RaceEnd();
        else 
        RT.RaceStart();
    }
}
