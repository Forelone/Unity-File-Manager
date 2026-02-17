using System.Collections;
using System.IO;
using UnityEngine;

public class ImageHandler : MonoBehaviour
{
    ObjectFileInfo OFI;
    Renderer ImageRen;

    bool Want = false;

    string PathToFile = string.Empty;

    public void Toggle()
    {
        enabled = !enabled;
    }

    public void Start() => StartCoroutine(GetPath());

    IEnumerator GetPath()
    {
        print("Start!");
        ImageRen = transform.GetChild(0).GetComponent<Renderer>();
        while (PathToFile == string.Empty)
        {
            if (TryGetComponent(out OFI))
            { 
                PathToFile = OFI.Path;
            }
            yield return new WaitForEndOfFrame();
        }

        if (Want) DisplayPicture();
    }

    void DisplayPicture()
    {
        if (PathToFile == string.Empty) { Debug.LogError("Path is empty!"); Want = true; return; }

        byte[] FileData = File.ReadAllBytes(PathToFile);
        Texture2D Texture = new Texture2D(2, 2);
        bool SuccessfullyLoadedData = Texture.LoadImage(FileData);
        if (SuccessfullyLoadedData)
        {
        ImageRen.material.mainTexture = Texture;
        int W = Mathf.Clamp(Texture.width / 100, 1, 4), H = Mathf.Clamp(Texture.height / 100, 1, 4);
        transform.localScale = new Vector3(W, 0.03f, H);
        }
        else
        Debug.LogError("Something went wrong while trying to load this Image.");
    }

    void ErasePicture()
    {
        Destroy(ImageRen.material.mainTexture);
    }

    void OnEnable() => DisplayPicture();

    void OnDisable() => ErasePicture();
}
