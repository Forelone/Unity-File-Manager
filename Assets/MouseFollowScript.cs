using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollowScript : MonoBehaviour
{
    Camera cam;
    RectTransform RT;
    void Awake()
    {
        RT = GetComponent<RectTransform>();
        cam = Camera.current;
    }

    void Update()
    {
        var mousePosition = Input.mousePosition;
        RT.position = new Vector3(mousePosition.x, mousePosition.y, 0);
    }
}
