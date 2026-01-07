using System.IO;
using UnityEngine;

public class ImageHandler : MonoBehaviour
{
    void OnEnable()
    {
        //First we get full path
        PathProtocol PP = GetComponentInParent<PathProtocol>();
        string Name = transform.name;
        string FullPath = PP.GetPath() + Name;
        print(FullPath);
        //Then we check if we can convert it
        byte[] FileData = File.ReadAllBytes(FullPath);
        Texture2D Texture = new Texture2D(2, 2);
        Texture.LoadImage(FileData);
        //Then we create material with this image (or not)
        transform.GetChild(0).GetComponent<Renderer>().material.mainTexture = Texture;
        int W = Mathf.Clamp(Texture.width / 100, 1, 4), H = Mathf.Clamp(Texture.height / 100, 1, 4);
        //???
        transform.localScale = new Vector3(W, 0.03f, H);
        //Profit!!!
    }

    void OnDisable()
    {
        Destroy(transform.GetChild(0).GetComponent<Renderer>().material.mainTexture);
    }
}
