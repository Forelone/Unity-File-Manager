using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileInfoFetcher : MonoBehaviour
{
    public string Description;

    public void Fire()
    {
        enabled = !enabled;
    }

    void FixedUpdate()
    {
        //Check if player is still holding, else close yourself.
        bool RaycastDidHit = Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f);
        string Text = "";
        //Check raycast is hitting something
        if (RaycastDidHit)
        {
            //Check it is a file or a directory
            if (hit.transform.TryGetComponent(out ObjectFileInfo OFI))
            {
                Text = $"{OFI.Name}\nSize: {OFI.Size}b\nCreate Date: {OFI.CreationDate.ToShortDateString()}\nMod Date: {OFI.ModificationDate.ToShortDateString()}\nExtension: {OFI.Extension}\nAdmin: {OFI.Protected}";
                //Display Info
            }
            else if (hit.transform.TryGetComponent(out GATEProtocol Gate))
            {
                //Display Info
            }
            else
            {
                Text = $"{hit.transform.gameObject.name}";
            }
        }

        var Current = DateTime.Now;
        Text += $"\n\nDate: {Current.ToShortDateString()}\nTime: {Current.ToShortTimeString()}";
        Description = Text;
    }

    void OnDisable()
    {
        Description = string.Empty;
    }
}
