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
        Vector2 TargetPos = Cursor.visible ? Input.mousePosition : new Vector2(Screen.width / 2, Screen.height / 2); 
        RT.position = new Vector3(TargetPos.x, TargetPos.y, 0);
    }
}
