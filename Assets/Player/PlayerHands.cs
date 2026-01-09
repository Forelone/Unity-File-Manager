using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerHands : MonoBehaviour
{
    Item Equipped, Holding;
    //Equipped one is you're currently using.
    //Holding one is you have to hold Fire1.

    [SerializeField] Transform RightHand, LeftHand, Head, LeftArm, RightArm;
    [SerializeField] PlayerInput PInput;
    [SerializeField] LayerMask HandInteractLayer;

    int RStatus; //0 is idle, 1 is picking up, 2 is holding
    bool DidHit; RaycastHit hit;
    public bool IsEquipped { get { return Equipped != null && RStat != 1; } }
    
    public Item EquippedItem { get { return Equipped; } }

    public bool HeadRayDidHit { get { return DidHit; } }
    public int RStat { get { return RStatus; } }
    public RaycastHit HeadRayHit { get { return hit; } }
    public float TLeft { get { return TimeLeft; } }
    float TimeLeft = 1f;

    public void HandDrop() //Use only for inventory or storage management etc. 
    {
        Equipped = null;
        RStatus = 0;
    }

    public void HandEquip(Item Item) //Also, use only for inventory or storage management etc.
    {
        Equipped = Item;
        RStatus = 2;
    }

    void FixedUpdate()
    {
             hit = new RaycastHit(); Useable HitUse = null; Item HitItem = null;
             DidHit = Physics.Raycast(Head.position, Head.forward, out hit, 2f, HandInteractLayer);
        bool HitUseable = DidHit && hit.transform.TryGetComponent(out HitUse);
        bool HitAItem = DidHit && hit.transform.TryGetComponent(out HitItem);

        Quaternion RDesired = Quaternion.Euler(transform.eulerAngles + new Vector3(90,0,0));

        if (Equipped != null)
        {
            //DO NOT CHANGE BELOW LINE. IF THE ITEM IS LOOKING AT NON-DESIRED ROTATION OPEN BLENDER AND FUCKING FIX IT. FUCK YOU. DO WHAT I SAY.
            Equipped.transform.SetPositionAndRotation(RightHand.position, RightHand.rotation);
            Equipped.RigidBody_.velocity = Vector3.zero;
            Equipped.RigidBody_.angularVelocity = Vector3.zero;
        }

        switch (RStatus)
        {
            default: //Not equipped with a item
                if (DidHit)
                {
                    var Dir = hit.point - RightArm.position;
                    RDesired = Quaternion.LookRotation(Dir);
                }

                if (PInput.PrimaryHandUse)
                {
                    if (HitAItem)
                        StartCoroutine(PickupHandle(HitItem));
                    else if (HitUseable)
                        HitUse.Use_();
                }
                else if (PInput.SecondaryHandUse)
                {
                    if (HitAItem && HitUseable) HitUse.Use_();
                    else if (HitUseable) HitUse.Use_Alternative();
                }
                break;

            case 1: //Equipping a item
                if (DidHit)
                {
                    var Dir = hit.point - RightArm.position;
                    RDesired = Quaternion.LookRotation(Dir);
                }
                break;

            case 2: //Equipped with a item
                if (!HitUseable && PInput.InteractiveUse) StartCoroutine(DropHandle());
                if (PInput.PrimaryHandUse) Equipped.PrimaryUse();
                if (PInput.SecondaryHandUse) Equipped.SecondaryUse();

                RDesired = Head.rotation;
                break;
        }


        RightArm.rotation = Quaternion.Lerp(RightArm.rotation, RDesired, 0.1f);
    }

    IEnumerator DropHandle()
    {
        float TimePassed = 0, RequiredTime = 1f;
        while (PInput.InteractiveUse && TimePassed < RequiredTime)
        {
            TimePassed += Time.deltaTime;
            if (TimePassed >= RequiredTime)
            {
                Equipped = null;
                RStatus = 0;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator PickupHandle(Item I)
    {
        RStatus = 1;
        TimeLeft = 0; float RequiredTime = I.PickupDelay;
        while (PInput.PrimaryHandUse && TimeLeft < RequiredTime)
        {
            TimeLeft += Time.deltaTime;
            if (TimeLeft >= RequiredTime)
            {
                Equipped = I;
                break;
            }
            yield return new WaitForEndOfFrame();
        }
        while (PInput.PrimaryHandUse) yield return new WaitForFixedUpdate();
        RStatus = Equipped == null ? 0 : 2;
    }
}
