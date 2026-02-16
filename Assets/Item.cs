using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    Useable Use;
    Rigidbody RG;
    [SerializeField] Collider Col; //This Collider is for Grabbing. If not specified first one will be grabbed. Creatures will grab item from center of this collider
    Collider[] Cols;

    void Awake()
    {
        RG = GetComponent<Rigidbody>();
        Col = Col == null ? GetComponent<Collider>() : Col;
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

    public event Action OnDrop;
    public event Action OnEquip;

    public void Equip() => OnEquip?.Invoke();
    public void Drop() => OnDrop?.Invoke();

    public Rigidbody RigidBody_ { get { return RG; } }
    public Collider Collider_ { get { return Col; } }
    public Collider[] Colliders { get { return Cols; } }

}
