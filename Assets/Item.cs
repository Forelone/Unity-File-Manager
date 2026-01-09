using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] float Pickup_Delay = 1f;
    public float PickupDelay { get { return Pickup_Delay; } }

    Useable Use;
    Rigidbody RG;
    Collider Col;
    Collider[] Cols;

    void Awake()
    {
        RG = GetComponent<Rigidbody>();
        Col = GetComponent<Collider>();
        Cols = GetComponents<Collider>(); //Yes.
        Use = GetComponent<Useable>();

        if (RG == null) Debug.LogError("Rigidbody not found on Item! If you're making a button, just use Useable script.", this.gameObject);
        if (Col == null) Debug.LogError("Collider not found on Item! Kill yourself!", this.gameObject);
    }

    public void PrimaryUse()
    {
        if (Use == null) return;

        Use.Use_();
    }

    public void SecondaryUse()
    {
        if (Use == null) return;

        Use.Use_Alternative();
    }

    public Rigidbody RigidBody_ { get { return RG; } }
    public Collider Collider_ { get { return Col; } }
    public Collider[] Colliders { get { return Cols; } }

}
