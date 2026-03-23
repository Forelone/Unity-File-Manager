using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTool : MonoBehaviour
{
        [TextArea]
    public string Description = "";
    public string Desc
    {
        get {return Description;}
        set
        {
            if (Description != value)
            {
                Description = value; OnDescriptionChange.Invoke();
            }
        }
    }
    public event Action OnDescriptionChange;
    [SerializeField] float MaxDistance = 5;
    Vector3 ApplySize;
    bool FireReady = false;

    public void Fire()
    {
            Apply();
    }

    public void Apply()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
        {
            if (hit.transform.TryGetComponent(out GroundProtocol GP))
            {
                GP.Save();
                print("Trying to save..");
            }
        }
    }
}
