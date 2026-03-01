using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModelTool : MonoBehaviour
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

    string ModelCopyPath = string.Empty;
    bool FireReady;
    public void Fire()
    {
        if (FireReady)
            Paste();
        else
            Copy();
    }

    [SerializeField] float MaxDistance = 5;

    string DefaultText = "Applies models to file. \nCurrent Model:\n"; 

    public void Paste()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit,MaxDistance) && ModelCopyPath != string.Empty)
        {
            hit.transform.GetOrAddComponent<ModelFile>().OverrideModel(ModelCopyPath);

            ModelCopyPath = string.Empty; 
            Desc = $"{DefaultText} {ModelCopyPath}";
            FireReady = false;
        }
    }

    public void Copy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit,MaxDistance) && hit.transform.TryGetComponent(out MeshRenderer mR))
        {
            ModelCopyPath = hit.collider.GetComponent<ObjectFileInfo>().Path;
            Desc = $"{DefaultText} {ModelCopyPath}";
            FireReady = true;
        }
    }
}
