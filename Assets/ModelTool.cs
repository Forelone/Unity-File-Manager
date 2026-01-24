using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ModelTool : MonoBehaviour
{
    string ModelCopyPath = string.Empty;
    [SerializeField] float MaxDistance = 5;

    TextMesh TM;
    string DefaultText;
    void Start()
    {
        TM = GetComponentInChildren<TextMesh>();
        DefaultText = TM.text;
    }

    public void Paste()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit,MaxDistance) && ModelCopyPath != string.Empty)
        {
            hit.transform.GetOrAddComponent<ModelFile>().OverrideModel(ModelCopyPath);

            ModelCopyPath = string.Empty; 
            TM.text = DefaultText + ModelCopyPath;
        }
    }

    public void Copy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit,MaxDistance) && hit.transform.TryGetComponent(out MeshRenderer mR))
        {
            ModelCopyPath = hit.collider.GetComponent<ObjectFileInfo>().Path;
            TM.text = DefaultText + ModelCopyPath;
        }
    }
}
