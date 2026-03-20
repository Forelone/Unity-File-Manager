using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerGrab : MonoBehaviour
{
    public KeyCode GrabKey;
    [SerializeField] PlayerHands PH;
    [SerializeField] Rigidbody RG;
    [SerializeField] Transform ArmToAim;
    [SerializeField] Rigidbody HandToGrab;
    [SerializeField] Transform HeadToRotate;
    [SerializeField] Joint CurrentlyGrabbing;

    Quaternion DefaultRot;

    void Awake()
    {
        PH = GetComponent<PlayerHands>();
        RG = GetComponent<Rigidbody>();
        DefaultRot = ArmToAim.localRotation;
    }

    /*void FixedUpdate()
    {
        bool GrabAveiwable /*Huh*/ /*= PH.HeadRayDidHit && PH.HeadRayHit.rigidbody && CurrentlyGrabbing == null;
        bool IsGrabbing = CurrentlyGrabbing != null;

        Quaternion RDesired = Quaternion.Euler(Vector3.zero);
        if (GrabAveiwable)
        {
            var Dir = PH.HeadRayHit.point - ArmToAim.position;
            RDesired = Quaternion.LookRotation(Dir);
            ArmToAim.rotation = Quaternion.Lerp(ArmToAim.rotation, RDesired, 0.1f);
        }
        else if (IsGrabbing)
        {
            RDesired = HeadToRotate.rotation;
            ArmToAim.rotation = Quaternion.Lerp(ArmToAim.rotation, RDesired, 0.1f);
        }
        else
        {
            RDesired = DefaultRot;
            ArmToAim.localRotation = Quaternion.Lerp(ArmToAim.localRotation, RDesired, 0.1f);
        }

        //Check if player is willing to grab it.
        if (Input.GetKey(GrabKey))
        {
            //Check if player is looking at a rigidbody
            if (GrabAveiwable)
            {
                //Grab it.
                CurrentlyGrabbing = PH.HeadRayHit.transform.AddComponent<SpringJoint>();
                CurrentlyGrabbing.anchor = PH.HeadRayHit.transform.InverseTransformPoint(PH.HeadRayHit.point);
                CurrentlyGrabbing.connectedBody = HandToGrab;
                SpringJoint SJ = CurrentlyGrabbing as SpringJoint;
                SJ.spring = 100f;
            }
        }
        else if (CurrentlyGrabbing != null)
        {
            Destroy(CurrentlyGrabbing);
        }
    }*/
}
