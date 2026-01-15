using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulationTool : MonoBehaviour
{
    Joint Grabbing;
    
    [SerializeField] Rigidbody Grabber;

    [SerializeField] float MaxDistance = 5f;

    void FixedUpdate()
    {
        //Check if we already grabbing something.
        if (Grabbing == null)
        {
            //If so check if we have a rigidbody front of us
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance) && hit.rigidbody != null)
            {
                Grabber.transform.position = hit.point;
                //Grab the rigidbody
                Grabbing = hit.rigidbody.gameObject.AddComponent<ConfigurableJoint>();
                ConfigurableJoint Conf = Grabbing as ConfigurableJoint;
                Conf.autoConfigureConnectedAnchor = false;

                Grabbing.connectedBody = Grabber;
                Grabbing.anchor = hit.transform.InverseTransformPoint(hit.point);

                Conf.xMotion = ConfigurableJointMotion.Locked;
                Conf.yMotion = ConfigurableJointMotion.Locked;
                Conf.zMotion = ConfigurableJointMotion.Locked;
                Conf.angularXMotion = ConfigurableJointMotion.Locked;
                Conf.angularYMotion = ConfigurableJointMotion.Locked;
                Conf.angularZMotion = ConfigurableJointMotion.Locked;
            }
            return;
        }
        else
        {
            int Left = Input.GetKey(KeyCode.Keypad4) ? 1 : 0;
            int Right = Input.GetKey(KeyCode.Keypad6) ? 1 : 0;
            int Up = Input.GetKey(KeyCode.Keypad8) ? 1 : 0;
            int Down = Input.GetKey(KeyCode.Keypad2) ? 1 : 0;

            Vector3 Horizontal = transform.up * (Right - Left);
            Vector3 Vertical = transform.right * (Up - Down);
            Vector3 Torque = Horizontal + Vertical;
            if (Torque != Vector3.zero)
            {
                ConfigurableJoint Conf = Grabbing as ConfigurableJoint;
                Conf.angularXMotion = ConfigurableJointMotion.Free;
                Conf.angularYMotion = ConfigurableJointMotion.Free;
                Conf.angularZMotion = ConfigurableJointMotion.Free;
                Conf.GetComponent<Rigidbody>().AddTorque(Torque, ForceMode.VelocityChange);
                Conf.angularXMotion = ConfigurableJointMotion.Locked;
                Conf.angularYMotion = ConfigurableJointMotion.Locked;
                Conf.angularZMotion = ConfigurableJointMotion.Locked; //To do: make things rotate or some shit idk.
            }
        }
    }

    void OnEnable()
    {
        
    }

    void OnDisable()
    {
        Destroy(Grabbing);
    }
}
