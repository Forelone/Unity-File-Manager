using System;
using System.Collections;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using UnityEngine;

public class ImageHandler : MonoBehaviour
{
    ObjectFileInfo OFI;
    Renderer ImageRen;

    string PathToFile;
    public string PTFile
    {
        get { return PathToFile; }
        set
        {
            if (PathToFile != value)
            {
                PathToFile = value;
                DisplayPictureAsync();
            }
        }
    }

    public bool ImageEnabled
    {
        get { return enabled; }
        set
        {
            if (enabled != value)
            {
                enabled = value;

                if (enabled == true)
                {
                    DisplayPictureAsync(); //WHAT COULD GO WRONG :DDD ????
                }
                else
                    ErasePicture();
            }
        }
    }

    public void Toggle() => ImageEnabled = !ImageEnabled;

    async Task DisplayPictureAsync()
    {
        OFI = OFI ?? GetComponent<ObjectFileInfo>();
        ImageRen = ImageRen ?? GetComponent<Renderer>();
        PathToFile = PathToFile ?? OFI.Path;

        byte[] FileData = await GetTextureDataAsync(PathToFile);

        Texture2D Texture = new Texture2D(2, 2);
        bool SuccessfullyLoadedData = Texture.LoadImage(FileData);
        if (SuccessfullyLoadedData)
        {
            Texture.Apply();
            ImageRen.material.mainTexture = Texture;
        }
        else
        Debug.LogError("Something went wrong while trying to load this Image.");
    }

    static Task<byte[]> GetTextureDataAsync(string Path)
    {
        return File.ReadAllBytesAsync(Path);
    }

    void ErasePicture()
    {
        Destroy(ImageRen.material.mainTexture);
    }
}
