using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioHandler : MonoBehaviour
{
    ObjectFileInfo OFI;
    AudioSource ASS; //*Xaxxaxaxaxaxa

    public void Toggle()
    {
        enabled = !enabled;
    }

    void OnEnable() => StartCoroutine(EnableHandle());

    void OnDisable()
    {
        ASS.Stop();
        ASS.time = 0f;
        OFI.AddTag("AF:Play",new string[]{false.ToString()});
    }

    IEnumerator EnableHandle()
    {
        OFI = OFI ?? GetComponent<ObjectFileInfo>();
        ASS = ASS ?? GetComponent<AudioSource>();
        if (ASS.clip == null)
        {
            //First we get full path
            PathProtocol PP = GetComponentInParent<PathProtocol>();
            string Name = transform.name;
            string FullPath = PP.GetPath() + Name;
            //Then we extract the juice
            string HTML = $"file://{FullPath}";
            UnityWebRequest MrWorldwide = UnityWebRequestMultimedia.GetAudioClip(HTML, AudioType.UNKNOWN);
            yield return MrWorldwide.SendWebRequest();

            if (MrWorldwide.result == UnityWebRequest.Result.Success)
            {
                AudioClip Clip = DownloadHandlerAudioClip.GetContent(MrWorldwide);
                ASS.clip = Clip;
            }
            else
            {
                Debug.LogError("I couldn't load from " + HTML);
            }
        }
        ASS.Play();
        OFI.AddTag("AF:Play",new string[]{true.ToString()});
    } //This is the most stable code i've ever wrote. Damn.
}
