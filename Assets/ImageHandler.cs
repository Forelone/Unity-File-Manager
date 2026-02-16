using System.IO;
using UnityEngine;

public class ImageHandler : MonoBehaviour
{
    PathProtocol PP;
    public void SetPeePee(PathProtocol P) => PP = P;

    public void Toggle()
    {
        enabled = !enabled;
    }

    void OnEnable()
    {
        PP = transform.parent.GetComponentInParent<PathProtocol>();
        if (PP == null) Debug.LogError("Can't find my peenie :/", gameObject);
        //First we get full path
        string Name = gameObject.transform.name;
        string FullPath = PP.GetPath() + Name;
        print(FullPath);
        //Then we check if we can convert it
        byte[] FileData = File.ReadAllBytes(FullPath);
        Texture2D Texture = new Texture2D(2, 2);
        bool SuccessfullyLoadedData = Texture.LoadImage(FileData);
        //Then we create material with this image (or not)
        if (SuccessfullyLoadedData)
        {
        transform.GetComponentInChildren<Renderer>().material.mainTexture = Texture;
        int W = Mathf.Clamp(Texture.width / 100, 1, 4), H = Mathf.Clamp(Texture.height / 100, 1, 4);
        //???
        transform.localScale = new Vector3(W, 0.03f, H);
        //Profit!!!
        }
        else
        Debug.LogError("Something went wrong while trying to load this Image.");
    }

    void OnDisable()
    {
        Destroy(transform.GetComponentInChildren<Renderer>().material.mainTexture);
    }
}
