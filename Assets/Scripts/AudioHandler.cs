using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AudioHandler : MonoBehaviour
{
    [SerializeField] AudioSource ASS; //*Xaxxaxaxaxaxa

    void OnEnable() => StartCoroutine(EnableHandle());

    void OnDisable()
    {
        ASS.Stop();
        ASS.time = 0f;
    }

    IEnumerator EnableHandle()
    {
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
    }
}
