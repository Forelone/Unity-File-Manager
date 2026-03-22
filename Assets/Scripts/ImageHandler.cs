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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                DisplayPictureAsync();
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
            } //WITH THE POWER OF FLEX TAPE, I SAWED THIS CODE IN HALF!
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
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    DisplayPictureAsync(); //WHAT COULD GO WRONG :DDD ????
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
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
        OFI.AddTag("IMG:Open", new string[]{true.ToString()});
    }

    static Task<byte[]> GetTextureDataAsync(string Path)
    {
        return File.ReadAllBytesAsync(Path);
    }

    void ErasePicture()
    {
        Destroy(ImageRen.material.mainTexture);
        OFI.AddTag("IMG:Open", new string[]{false.ToString()});
    }
}
