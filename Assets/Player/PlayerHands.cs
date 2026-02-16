using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerHands : MonoBehaviour
{
    [SerializeField] Transform RightHand, LeftHand;
    PlayerInput PInput;
    [SerializeField] float EyeDist = 2f;
    Camera Eye;
    public Camera Eye_
    { get { return Eye; } }
    [SerializeField] LayerMask HandInteractLayer;

    [SerializeField] bool DebugMode;

    public bool Lefty {get{ return PInput.IsPlayerLefty; }}

    void Awake()
    {
        PInput = GetComponent<PlayerInput>();
        Eye = transform.GetComponentInChildren<Camera>();
    }

    void OnEnable()
    {
        PInput.OnPrimaryClick += PrimaryHandle;
        PInput.OnSecondaryClick += SecondaryHandle;
        PInput.OnInteractClick += InteractHandle;
        PInput.OnRefuseClick += DropHandle;
        PInput.OnMouseMovement += HandItemHandle;
        PInput.OnInspectClick += InspectHandle;

        if (DebugMode)
            print("PH Active!");
    }

    void OnDisable()
    {
        PInput.OnPrimaryClick -= PrimaryHandle;
        PInput.OnSecondaryClick -= SecondaryHandle;
        PInput.OnInteractClick -= InteractHandle;
        PInput.OnRefuseClick -= DropHandle;
        PInput.OnMouseMovement -= HandItemHandle;
        PInput.OnInspectClick -= InspectHandle;

        if (DebugMode)
            print("PH De-Active!");
    }

    void FixedUpdate()
    {
        if (DebugMode)
        {
            Ray ray = Eye.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool A = Physics.Raycast(ray,out hit,EyeDist,HandInteractLayer);
            Color color = Color.white;
            if (A)
            {
                color = Color.green;
                if (hit.transform.GetComponent<Item>()) color = Color.blue;
                else if (hit.transform.GetComponent<Useable>()) color = Color.red;
            }
            Debug.DrawRay(ray.origin,ray.direction * EyeDist,color);
        }
    }

    Item ItemInMainHand;
    public Item ItemOnHand {get { return ItemInMainHand; }}
    [SerializeField] Rigidbody RigidbodyInOtherHand;
    float RGDist = 0;
    public bool IsHandFull { get { return ItemInMainHand != null; } }
    public bool IsOHandFull { get { return RigidbodyInOtherHand != null; } }
    public bool Inspecting { get { return PInput.Inspect; } }

    void HandItemHandle(Vector2 _)
    {
        if (!IsHandFull) return;

        Transform Hand = PInput.IsPlayerLefty ? LeftHand : RightHand;
        ItemInMainHand.transform.SetPositionAndRotation(Hand.position,Hand.rotation);
    }

    void PrimaryHandle()
    {
        if (DebugMode) print("Primary Call");
        
        if (Inspecting)
        {
            (bool IsSomethingInFrontOfMe,RaycastHit Hit) = EyeRay();
            bool HasRG = Hit.rigidbody != null;
            if (!IsOHandFull)
            {
                if (!IsSomethingInFrontOfMe) return;
                if (!HasRG) return;

                RigidbodyInOtherHand = Hit.rigidbody;
                RGDist = Hit.distance;
            }
            else
            {
                Vector3 MousePos = Input.mousePosition;
                Ray ray = Eye.ScreenPointToRay(MousePos);
                Vector3 SpacePos = ray.origin + ray.direction * RGDist;
                RigidbodyInOtherHand.position = SpacePos;
                RigidbodyInOtherHand.rotation = LeftHand.rotation;
                if (!RigidbodyInOtherHand.isKinematic)
                {
                RigidbodyInOtherHand.angularVelocity = Vector3.zero;
                RigidbodyInOtherHand.velocity = Vector3.zero;
                }
            }

            if (Hit.collider != null && Hit.collider.transform.TryGetComponent(out Useable U)) U.Use_();
        }
        else
        {
            RigidbodyInOtherHand = null;

            if (IsHandFull)
                ItemInMainHand.PrimaryUse();
            else
                Punch();   
        }
    }

    void SecondaryHandle()
    {
        if (DebugMode) print("Secondary Call");
        
        if (IsHandFull)
            ItemInMainHand.SecondaryUse();
    }

    void InteractHandle()
    {
        if (DebugMode) print("Interact Call");
        (bool IsSomethingInFrontOfMe,RaycastHit Hit) = EyeRay();

        if (!IsSomethingInFrontOfMe) return;

        if (Hit.transform.TryGetComponent(out Item I))
            if (IsHandFull) 
                SwitchHandItem(I);
            else
                PickupItem(I);
        else if (Hit.transform.TryGetComponent(out Useable U))
            U.Use_();
    }

    void DropHandle()
    {
        if (DebugMode) print("Drop Call");

        DropHandItem();
    }

    public void DropHandItem()
    {
        if (!IsHandFull) return;

        Item OldItem = ItemInMainHand;

        OldItem.RigidBody_.isKinematic = false;
        OldItem.transform.SetParent(null);
        ItemInMainHand = null;
    }

    void InspectHandle()
    {
        if (!PInput.PrimaryHandUse) RigidbodyInOtherHand = null;
    }

    public (bool,RaycastHit) EyeRay()
    {
        Vector3 MousePos = Input.mousePosition;
        Ray CamRay = Eye.ScreenPointToRay(MousePos);
        bool DidHit = Physics.Raycast(CamRay,out RaycastHit hit,EyeDist,HandInteractLayer);
        return (DidHit,hit);
    }

    void SwitchHandItem(Item NewItem)
    {
        DropHandle();
        PickupItem(NewItem);
    }

    public void PickupItem(Item ItemToPickup)
    {
        if (IsHandFull) return;

        Transform Hand = PInput.IsPlayerLefty ? LeftHand : RightHand;

        ItemToPickup.transform.SetParent(null);
        ItemToPickup.RigidBody_.isKinematic = true;
        ItemToPickup.transform.position = Hand.position;
        ItemToPickup.transform.rotation = Hand.rotation;
        //ItemToPickup.transform.SetPositionAndRotation(Hand.position,Hand.rotation);
        ItemToPickup.transform.SetParent(Hand);

        ItemInMainHand = ItemToPickup;
        ItemToPickup.Equip();
    }

    void Punch()
    {
        
    }
    /*
        Modern Yöntem


            OnPrimaryClick() <- Bu Oyuncu LMB tuşuna bastığında çalışır.
                -Oyuncu Inspect tuşuna basıyor mu?
                    Evet:
                        Oyuncu bir Rigidbody'e bakıyor mu?
                            Evet:
                                Tut ve Mouse kordinatlarına sürükle
                -Oyuncunun ana elinde item var mı?
                    Evet:
                        Oyuncunun elindeki itemi kullan
                    Hayır:
                        Yumruk at
            
            OnSecondaryClick() <- Bu Oyuncu RMB tuşuna bastığında çalışır.
                - Oyuncunun ana elinde item var mı?
                    Evet:
                        item'i diğer şekilde kullan.
                    Hayır:
                        - Oyuncunun diğer elinde item var mı?
                            Evet:
                                diğer itemi kullan.
            
            OnInteractClick() <- Bu oyuncu MMB veya E tuşuna bastığında çalışır.
                - Oyuncunun önünde bir şey var mı?
                    Evet:
                        - Bu şey bir item mi?
                            Evet:
                                - Oyuncunun elinde herhangi bir item var mı?
                                    Evet:
                                        Oyuncunun elindeki item ile değiştir.
                                    Hayır:
                                        Item'i eline al
                        - Bu şey bir eşya mı?
                            Evet:
                                Eşyayı kullan.
            OnDropClick() -> Bu oyuncu Q tuşuna bastığında çalışır.
                - Oyuncunun elinde item var mı?
                    Evet:
                        Elindeki itemi bırak.
            OnInspectClick() -> Bu oyuncu R tuşuna bastığında çalışır.

        Eşya: Kullanılabilir öğe (Useable.cs)
    */


    /*
        Klasik Yöntem.
        + Herşeyi tek bir loopta hallediyor
        - Çok kaynak kullanıyor.

        FixedUpdate()
            - Oyuncu bir item'e bakıyor mu?
                Evet:
                    -Elinde item var mı?
                    -İtem'i almak istiyor mu?
                        Evet:
                            İtemi Al.
            - Oyuncu eline bakmak istiyor mu?
                Evet:
                    Elini özgür bırak
            - Oyuncu bir eşya ile etkileşimde bulunmak istiyor mu?
                Evet:
                    - Eşya zaman istiyor mu?
                    - Eşya dinlenme sürecinde mi_
                        Evet:
                            Eşyayı kullan
    */
}
