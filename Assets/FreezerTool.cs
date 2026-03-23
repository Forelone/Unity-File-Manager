using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezerTool : MonoBehaviour
{
    public string Description = "Colors things. \n0,0,0";
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

    [SerializeField] float MaxDistance;
    Vector3 ApplySize;
    bool FireReady = false;

    public void Fire()
    {
            Apply();
    }

    public void Apply()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance) && hit.rigidbody != null)
        {
            hit.rigidbody.isKinematic = !hit.rigidbody.isKinematic;

            if (hit.rigidbody.TryGetComponent(out ObjectFileInfo OFI))
            {
                OFI.AddTag("Kinematic",new string[]{hit.rigidbody.isKinematic.ToString()});
            }
        }
    }
}
