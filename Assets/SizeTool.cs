using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeTool : MonoBehaviour
{
    public string Description = "Colors things. \n0,0,0";
    public string Desc
    {
        get {return Description;}
        set
        {
            if (Description != value)
            {
                Description = value; OnDescriptionChange.Invoke();
            }
        }
    }
    public event Action OnDescriptionChange;

    [SerializeField] float MaxDistance;
    Vector3 ApplySize;
    bool FireReady = false;

    public void Fire()
    {
        if (FireReady)
            Apply();
        else
            Configure();
    }

    public void Apply()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
        {
            var Parent = hit.collider.transform.parent;
            Transform T = hit.collider.transform;
            T.SetParent(null);
            T.localScale = ApplySize;
            T.SetParent(Parent);
            FireReady = false;
        }
    }

    public void Configure()
    {
        if (!Configuring)
        StartCoroutine(Configuration());
    }

    bool Configuring;
    
    IEnumerator Configuration()
    {
        ClearText();
        Configuring = true;
        AddText("Configuration started, \nEscape to exit.\n");
        float X,Y,Z;
        while (!Input.GetKey(KeyCode.Escape))
        {
            AddText("Please input x value and press Keypad Enter:");
            string XStr = "0";
            while (!Input.GetKey(KeyCode.KeypadEnter))
            {
                if (Input.anyKey)
                {
                    XStr += Input.inputString;
                }
                yield return new WaitUntil(() => Input.anyKey);
            }
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.KeypadEnter));
            if (!float.TryParse(XStr,out X)) { ClearText(); continue;}
            else {}
            ClearText();
            
            AddText("Please input y value and press Keypad Enter:");
            string YStr = "0";
            while (!Input.GetKey(KeyCode.KeypadEnter))
            {
                if (Input.anyKey)
                {
                    YStr += Input.inputString;
                }
                yield return new WaitUntil(() => Input.anyKey);
            }
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.KeypadEnter));
            if (!float.TryParse(YStr,out Y)) { ClearText(); continue;}
            else {}
            ClearText();

            AddText("Please input z value and press Keypad Enter:");
            string ZStr = "0";
            while (!Input.GetKey(KeyCode.KeypadEnter))
            {
                if (Input.anyKey)
                {
                    ZStr += Input.inputString;
                }
                yield return new WaitUntil(() => Input.anyKey);
            }
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.KeypadEnter));
            if (!float.TryParse(ZStr,out Z)) { ClearText(); continue;}
            else {
                ApplySize = new Vector3(X,Y,Z);
                FireReady = true; 
                break;
            }
        } //This is fine.
        Configuring = false;
        ResetText();
    }

    void AddText(string Text)
    {
        Desc += $"{Text}\n";
    }

    void ClearText() => Desc = string.Empty;

    void ResetText() => Desc = $"Sizes things. \n\nLMB = Configure (if not)\nSize it. {ApplySize.x}, {ApplySize.y}, {ApplySize.z}";

}
