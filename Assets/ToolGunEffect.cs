using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolGunEffect : MonoBehaviour
{
    LineRenderer LR;
    [SerializeField] float HitDistance = 5f;
    
    Vector3 FollowPoint;
    
    void Start()
    {
        LR = GetComponent<LineRenderer>();
        FollowPoint = transform.position;
    }

    void Update()
    {
        if (LR == null) { enabled = false; return; }

        Ray ray = new Ray(transform.position,transform.forward);
        RaycastHit hit;
        bool DidHit = Physics.Raycast(ray,out hit,HitDistance);
        LR.enabled = DidHit;

        Vector3 Pos = DidHit ? hit.point : transform.position;
        LR.SetPosition(0,Pos);
        LR.SetPosition(1,FollowPoint);

        FollowPoint = Vector3.Lerp(FollowPoint,Pos,0.1f);
    }

    public void FireEffect() => FollowPoint = transform.position;
}