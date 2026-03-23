using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManipulationTool : MonoBehaviour
{
    public string Description;

    Joint Grabbing;
    [SerializeField] Rigidbody Grabber;

    [SerializeField] float MaxDistance = 5f;

    bool CanGrab = true;

    public void Fire()
    {
        Grab();
    }

    void Grab() //TO DO: Make this work only when pressing LMB so we don't have to enable and disable this shit. DONE
    {
        if (!CanGrab) return;
        //Check if we already grabbing something.
        if (Grabbing == null)
        {
            //If so check if we have a rigidbody front of us
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
            {
                if (hit.rigidbody != null)
                {
                    Grabber.transform.position = hit.point;
                    //Grab the rigidbody
                    Grabbing = hit.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
                    ConfigurableJoint Conf = Grabbing as ConfigurableJoint;
                    Conf.autoConfigureConnectedAnchor = false;
                    Vector3 Hold = hit.transform.InverseTransformPoint(hit.point);
                    StartCoroutine(GrabHandle(hit.rigidbody,Hold));
                }
            }
            return;
        }
    }

    IEnumerator GrabHandle(Rigidbody Object, Vector3 HoldPoint)
    {
        GrabIt(Object,HoldPoint);

        while (Input.GetAxisRaw("Fire1") == 1)
            yield return new WaitForFixedUpdate();

        CanGrab = false;
        DropIt();

        yield return new WaitForFixedUpdate();
        CanGrab = true;
    }
    
    bool WasKinematicBefore = false;
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

                if (Object.isKinematic)
                {
                    WasKinematicBefore = Object.isKinematic;
                    Object.isKinematic = false;
                }
    }

    void DropIt()
    {
            Transform T = Grabbing.transform;
            Vector3 Rot = T.eulerAngles;
            string X = T.position.x.ToString(),
                   Y = T.position.y.ToString(),
                   Z = T.position.z.ToString(),
                   XR = Rot.x.ToString(),
                   YR = Rot.y.ToString(),
                   ZR = Rot.z.ToString();

        if (Grabbing.gameObject.TryGetComponent(out ObjectFileInfo OFI))
        {
            OFI.AddTag("Position", new string[]{X, Y, Z});
            OFI.AddTag("Rotation", new string[]{XR,YR,ZR});
        }
        else if (Grabbing.gameObject.TryGetComponent(out GATEProtocol GATE))
        {
            GATE.AddTag("Position", new string[]{X, Y, Z});
            GATE.AddTag("Rotation", new string[]{XR,YR,ZR});
        }
        Grabbing.GetComponent<Rigidbody>().isKinematic = WasKinematicBefore;
        
        Destroy(Grabbing);
    }
}
