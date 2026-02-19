using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ColorTool : MonoBehaviour
{
    public string Description = "Colors files.";
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
    public bool DebugMode = true;
    public void Fire()
    {
        if (FireReady)
            Apply();
        else
            Configure();
    }

    [SerializeField] Renderer Renderer;
    [SerializeField] float MaxDistance;
    [SerializeField] Color ApplyColor;


    bool Configuring;
    bool FireReady = false;

    public void Apply()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance) && 
        hit.transform != transform && 
        hit.transform.TryGetComponent(out Renderer R))
        {
            R.material.color = ApplyColor;
            FireReady = false;
            Renderer.gameObject.SetActive(false);
            Renderer.material.color = Color.black;
        }
    }

    public void Configure()
    {
        if (!Configuring)
        StartCoroutine(Configuration());
    }

    IEnumerator Configuration()
    {
        Configuring = true;

        bool RedOK = false,BlueOK = false,GreenOK = false,ColorCreated = false;
        int R = 0,G = 0,B = 0;
        while(!ColorCreated)
        {
            string Str = string.Empty;
            if (!RedOK)
            {
                Desc = "Please enter RED color value (0-255)\n and press 'Submit' key.";
                while (Input.GetAxisRaw("Submit") == 0)
                {
                    Str += Input.inputString;
                    yield return new WaitUntil(() => Input.anyKey);
                    if (DebugMode) print(Str);
                }
                if (DebugMode) print("Submitted!");
                if (int.TryParse(Str,out R)) RedOK = true;
                else { yield return new WaitForFixedUpdate(); continue; }
            }
            
            UpdateColor(R,G,B);
            Str = string.Empty;
            if (!BlueOK)
            {
                Desc = "Please enter BLUE color value (0-255)\n and press 'Submit' key.";
                while (Input.GetAxisRaw("Submit") == 0)
                {
                    Str += Input.inputString;
                    yield return new WaitUntil(() => Input.anyKey);
                    if (DebugMode) print(Str);
                }
                if (DebugMode) print("Submitted!");
                if (int.TryParse(Str,out B)) BlueOK = true;
                else { yield return new WaitForFixedUpdate(); continue; }
            }

            UpdateColor(R,G,B);
            Str = string.Empty;
            if (!GreenOK)
            {
                Desc = "Please enter GREEN color value (0-255)\n and press 'Submit' key.";
                while (Input.GetAxisRaw("Submit") == 0)
                {
                    Str += Input.inputString;
                    yield return new WaitUntil(() => Input.anyKey);
                    if (DebugMode) print(Str);
                }
                if (DebugMode) print("Submitted!");
                if (int.TryParse(Str,out G)) GreenOK = true;
                else { yield return new WaitForFixedUpdate(); continue; }
            }

            if (RedOK && BlueOK && GreenOK) { UpdateColor(R,G,B,true); ColorCreated = true; }
            yield return new WaitForFixedUpdate();
        }
        Configuring = false;
    }

    void UpdateColor(int R,int G,int B,bool SetTo = false)
    {
        Color color = new Color((float)R / 255f,(float)G / 255f,(float)B / 255f);
        Renderer.material.color = color;

        if (SetTo)
        {
            ApplyColor = color;
            FireReady = true;
        } 
    }

}
