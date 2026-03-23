using System;
using UnityEditor.Search;
using UnityEngine;

public class TextureTool : MonoBehaviour
{
public string Description = "Applies textures to files. \nClick on a image file to copy";
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

    ObjectFileInfo TextureCopyFile = null;
    bool FireReady;
    public void Fire()
    {
        if (FireReady)
            Paste();
        else
            Copy();
    }

    [SerializeField] float MaxDistance = 5;

    string DefaultText = "Applies textures to files. \nCurrent Model:\n"; 

    public void Paste()
    {
        RaycastHit hit;
        bool HitSuccess = Physics.Raycast(transform.position,transform.forward,out hit,MaxDistance);
        if (!HitSuccess) return;
        if (TextureCopyFile == null) return;

            Material ApplyMaterial = new Material(Shader.Find("Unlit/Texture"));
            ApplyMaterial.mainTexture = TextureCopyFile.transform.GetComponent<Renderer>().material.mainTexture;
            
        if (hit.transform.TryGetComponent(out Renderer R))
        {

            R.material = ApplyMaterial;
        }

        if (hit.transform.TryGetComponent(out ObjectFileInfo OFI))
        {
            OFI.AddTag("Texture",new string[]{$"'{TextureCopyFile.Path}'"});
        }
        if (hit.transform.TryGetComponent(out GATEProtocol GATE))
        {
            GATE.ApplyCustomTexture(ApplyMaterial);
            GATE.AddTag("Texture",new string[]{$"'{TextureCopyFile.Path}'"});
        }
        FireReady = false;
    }

    public void Copy()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position,transform.forward,out hit,MaxDistance) && hit.transform.TryGetComponent(out ImageHandler IH))
        {
            TextureCopyFile = hit.collider.GetComponent<ObjectFileInfo>();
            Desc = $"{DefaultText} {TextureCopyFile.Path}";
            FireReady = true;
        }
    }
}
