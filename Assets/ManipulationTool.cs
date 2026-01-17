using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ManipulationTool : MonoBehaviour
{
    Joint Grabbing;
    
    [SerializeField] Rigidbody Grabber;

    [SerializeField] float MaxDistance = 5f,Mul = 10f;

    float Dist;

    void FixedUpdate()
    {
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
                GrabIt(hit.rigidbody,Hold);
                Dist = hit.distance;
            }
            return;
        }
        else
        {
            int Left = Input.GetKey(KeyCode.Keypad4) ? 1 : 0;
            int Right = Input.GetKey(KeyCode.Keypad6) ? 1 : 0;
            int Up = Input.GetKey(KeyCode.Keypad8) ? 1 : 0;
            int Down = Input.GetKey(KeyCode.Keypad2) ? 1 : 0;
            int Forward = Input.GetKey(KeyCode.KeypadPlus) ? 1 : 0;
            int Back = Input.GetKey(KeyCode.KeypadMinus) ? 1 : 0;

            float Horizontal = Right - Left;
            float Vertical = Down - Up;
            float Linear = Back - Forward;
            if (Horizontal + Vertical != 0)
            {
                ConfigurableJoint Conf = Grabbing as ConfigurableJoint;
                Vector3 HoldPoint = Conf.anchor;
                Rigidbody RG = Conf.GetComponent<Rigidbody>();
                Destroy(Conf);
                RG.transform.RotateAround(RG.transform.position + HoldPoint,transform.up,Horizontal);
                RG.transform.RotateAround(RG.transform.position + HoldPoint,transform.right,Vertical);
                GrabIt(RG,HoldPoint);
            }

            if (Linear != 0)
            {
                Dist -= Time.fixedDeltaTime * Linear;
                Grabber.transform.position = transform.position + transform.forward * Dist;
            }
        }
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

    void OnDisable()
    {
        Destroy(Grabbing);
    }
}
