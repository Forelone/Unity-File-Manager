using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModelTool : MonoBehaviour
{
    [TextArea]
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
        bool HitSuccess = Physics.Raycast(transform.position,transform.forward,out hit,MaxDistance);
        if (!HitSuccess) return;
        if (ModelCopyPath == string.Empty) return;

        Mesh mesh = transform.AddComponent<ModelFile>().GetMeshFromPath(ModelCopyPath); 
        if (hit.transform.TryGetComponent(out BoxCollider B)) {Destroy(B); hit.transform.AddComponent<MeshCollider>().convex = true;}
        if (hit.transform.TryGetComponent(out MeshFilter MF)) MF.mesh = mesh;
        if (hit.transform.TryGetComponent(out MeshCollider MC)) MC.sharedMesh = mesh;

        if (hit.transform.TryGetComponent(out ObjectFileInfo OFI))
        {
            OFI.AddTag("Model",new string[]{$"'{ModelCopyPath}'"});
        }
        FireReady = false;
    }

    public void Copy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit,MaxDistance) && hit.transform.TryGetComponent(out ModelFile MF))
        {
            ModelCopyPath = hit.collider.GetComponent<ObjectFileInfo>().Path;
            Desc = $"{DefaultText} {ModelCopyPath}";
            FireReady = true;
        }
    }
}
