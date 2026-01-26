using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegs : MonoBehaviour
{
    [SerializeField] Rigidbody RG;
    [SerializeField] Transform LeftPelvis, RightPelvis, RDownLeg, LDownLeg;
    [SerializeField] float Divide = 10f;
    [SerializeField] float MinSpeed = 5f;
    [SerializeField] float Angle = 45;
    float Sin, S;

    Camera Eyes;
    void Awake()
    {
        Eyes = GetComponentInChildren<Camera>();
    }

    void FixedUpdate()
    {
        var Speed = Mathf.Abs(RG.velocity.magnitude + RG.angularVelocity.magnitude);
        if (Speed > MinSpeed)
        {
            Sin += RG.velocity.magnitude / Divide;
            S = Mathf.Sin(Sin);
        }
        else
        {
            Sin = 0;
            S = 0f;
        }
        RightPelvis.localRotation = Quaternion.Euler(Vector3.right * (90 + S * Angle) + Vector3.forward * -90);
        LeftPelvis.localRotation = Quaternion.Euler(Vector3.right * (90 - S * Angle) + Vector3.forward * 90);
        RDownLeg.localRotation = Quaternion.Euler(Vector3.right * (+S * Angle));
        LDownLeg.localRotation = Quaternion.Euler(Vector3.right * (-S * Angle));

        RaycastHit hit;
        if (Physics.Raycast(transform.position,-transform.up,out hit,2) && hit.transform.TryGetComponent(out PathProtocol PeePee))
        {
            Eyes.backgroundColor = PeePee.GetBGColor();
        }
    }

    void OnDisable()
    {
        Sin = 0f;
        S = 0f;
    }
}
