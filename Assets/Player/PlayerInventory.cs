using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// This is a standalone plugin for SCC (Simple Character Controller) by Forelone.
// You can disable or delete this from your Scripts and use the rest like normal.
public class PlayerInventory : MonoBehaviour
{
   [SerializeField] List<Item> Inventory;
    [SerializeField] Animation RightArmAnim;

    [SerializeField] PlayerHands PHands;

    bool SwitchingItem = false;
    void Update()
    {
        if (!Input.anyKey) return;
        if (SwitchingItem) return;
        
        int PressedKey = -1;
        for (int Key = 0; Key <= 9; Key++)
        {
            if (Input.GetKey(KeyCode.Alpha0 + Key))
            {
                PressedKey = Key;
                print(PressedKey);
                break;
            }
        }

        if (PHands.IsHandFull && PressedKey != -1) //Haul equipped item to inventory
        {
            StartCoroutine(HaulToInv(PressedKey));
        }
        else if (!PHands.IsHandFull && PressedKey != -1) //Equip hauled item to right hand.
        {
            StartCoroutine(EquipToHand(PressedKey));
        }
    }

    IEnumerator HaulToInv(int Index)
    {
        SwitchingItem = true;
        RightArmAnim.Play("PlyHaul");

        bool HauledIt = false;
        while (RightArmAnim.isPlaying)
        {
            if (!HauledIt && RightArmAnim["PlyHaul"].time > 0.5f)
            {
                Item Haul = PHands.ItemOnHand;
                Haul.gameObject.SetActive(false);

                if (Inventory[Index] != null)
                {
                    Item Drop = Inventory[Index];
                    Drop.gameObject.SetActive(true);
                    Drop.transform.SetPositionAndRotation(transform.position + transform.forward + transform.up, transform.rotation);
                }

                PHands.DropHandItem(); //Allows player to instantly drop item without animation.
                Inventory[Index] = Haul;

                HauledIt = true;
            }
            yield return new WaitForEndOfFrame();
        }
        SwitchingItem = false;
    }
    
    IEnumerator EquipToHand(int Index)
    {
        SwitchingItem = true;
        RightArmAnim.Play("PlyEquip");

        bool EquippedIt = false;
        while (RightArmAnim.isPlaying)
        {
            if (!EquippedIt && RightArmAnim["PlyEquip"].time > 0.5f)
            {
                if (Inventory[Index] != null)
                {
                    Item Equip = Inventory[Index];
                    Equip.gameObject.SetActive(true);
                    Inventory[Index] = null;
                    PHands.PickupItem(Equip);
                }

                EquippedIt = true;
            }
            yield return new WaitForEndOfFrame();
        }
        SwitchingItem = false;
    }
}
