using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoHandler : MonoBehaviour
{
    VideoPlayer VP;
    void OnEnable()
    {
        VP = GetComponent<VideoPlayer>();
        PathProtocol PP = GetComponentInParent<PathProtocol>();
        string Name = transform.name;
        string FullPath = PP.GetPath() + Name;

        string HTML = $"file://{FullPath}";
        try
        {
            VP.url = HTML;
            VP.Play();
        }
        catch (System.Exception)
        {
            /*deez nuts.*/
        }
    }
    
    void OnDisable()
    {
        VP.Stop();
    }
}
