using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManipulationTool : MonoBehaviour
{
    public string Description;

    Joint Grabbing;
    Transform PastParent = null;
    [SerializeField] Rigidbody Grabber;

    [SerializeField] float MaxDistance = 5f,Mul = 10f;

    float Dist;
    bool CanGrab = true;

    public void Fire()
    {
        Grab();
    }

    void Grab() //TO DO: Make this work only when pressing LMB so we don't have to enable and disable this shit.
    {
        if (!CanGrab) return;
        //Check if we already grabbing something.
        if (Grabbing == null)
        {
            //If so check if we have a rigidbody front of us
            Dist = 0f;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance) && hit.rigidbody != null)
            {
                Grabber.transform.position = hit.point;
                //Grab the rigidbody
                Grabbing = hit.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
                ConfigurableJoint Conf = Grabbing as ConfigurableJoint;
                Conf.autoConfigureConnectedAnchor = false;
                Vector3 Hold = hit.transform.InverseTransformPoint(hit.point);
                StartCoroutine(GrabHandle(hit.rigidbody,Hold));
                Dist = hit.distance;
            }
            return;
        }
    }

    bool HandlingGrab = false;
    IEnumerator GrabHandle(Rigidbody Object, Vector3 HoldPoint)
    {
        HandlingGrab = true;
        GrabIt(Object,HoldPoint);

        while (Input.GetAxisRaw("Fire1") == 1)
            yield return new WaitForFixedUpdate();

        CanGrab = false;
        DropIt();

        yield return new WaitForFixedUpdate();
        CanGrab = true;
        HandlingGrab = false;
    }
    
    void GrabIt(Rigidbody Object, Vector3 HoldPoint)
    {
                Grabbing = Object.AddComponent<ConfigurableJoint>();
                ConfigurableJoint Conf = Grabbing as ConfigurableJoint; 

                Conf.connectedBody = Grabber;
                Conf.anchor = HoldPoint; //What
                
                Conf.angularXMotion = ConfigurableJointMotion.Locked;
                Conf.angularYMotion = ConfigurableJointMotion.Locked;
                Conf.angularZMotion = ConfigurableJointMotion.Locked;
                Conf.xMotion = ConfigurableJointMotion.Locked;
                Conf.yMotion = ConfigurableJointMotion.Locked;
                Conf.zMotion = ConfigurableJointMotion.Locked;
    }

    void DropIt()
    {
        Destroy(Grabbing);
    }
}
