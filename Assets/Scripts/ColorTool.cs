using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class ColorTool : MonoBehaviour
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
    public bool DebugMode = true;
    public void Fire()
    {
        if (FireReady)
            Apply();
        else
            Configure();
    }

    [SerializeField] Renderer HoloRender;
    [SerializeField] float MaxDistance;
    [SerializeField] Color ApplyColor;


    bool Configuring;
    bool FireReady = false;

    public void Apply()
    {
        RaycastHit hit;
        bool HitSuccess = Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance);
        if (hit.transform == transform) return;

         string R = ApplyColor.r.ToString(),
                G = ApplyColor.g.ToString(),
                B = ApplyColor.b.ToString(),
                A = ApplyColor.a.ToString();

        string[] Save = new string[]{R,G,B,A};

        if (hit.transform.TryGetComponent(out Renderer Ren))
        {
            Ren.material.color = ApplyColor;

            FireReady = false;
            HoloRender.gameObject.SetActive(false);
            HoloRender.material.color = Color.black;

            if (hit.transform.TryGetComponent(out ObjectFileInfo OFI))
            {
                OFI.AddTag("Color",Save);
            }

            if (hit.transform.TryGetComponent(out GroundProtocol GP))
            {
                GP.AddTag("Color",Save);                
            }
        }

        if (hit.transform.TryGetComponent(out GATEProtocol GATE))
        {
            GATE.AddTag("Color",Save);
            GATE.ApplyCustomColor(ApplyColor);
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
        HoloRender.gameObject.SetActive(true);
        int R = 0,G = 0,B = 0;
        while(!ColorCreated)
        {
            string Str = string.Empty;
            if (!RedOK)
            {
                var S = "Please enter RED color value (0-255)\n and press 'Submit' key."; 
                Desc = S;
                while (Input.GetAxisRaw("Submit") == 0)
                {
                    Str += Input.inputString;
                    yield return new WaitUntil(() => Input.anyKey);
                    if (DebugMode) print(Str);
                    Desc = $"{S} \n{Str}";
                }
                if (DebugMode) print("Submitted!");
                Desc = "Submitted!";
                if (int.TryParse(Str,out R)) RedOK = true;
                else { yield return new WaitForFixedUpdate(); continue; }
            }
            
            UpdateColor(R,G,B);
            Str = string.Empty;
            if (!BlueOK)
            {
                var S = "Please enter BLUE color value (0-255)\n and press 'Submit' key."; 
                Desc = S;
                while (Input.GetAxisRaw("Submit") == 0)
                {
                    Str += Input.inputString;
                    yield return new WaitUntil(() => Input.anyKey);
                    if (DebugMode) print(Str);
                    Desc = $"{S} \n{Str}";
                }
                if (DebugMode) print("Submitted!");
                Desc = "Submitted!";
                if (int.TryParse(Str,out B)) BlueOK = true;
                else { yield return new WaitForFixedUpdate(); continue; }
            }

            UpdateColor(R,G,B);
            Str = string.Empty;
            if (!GreenOK)
            {
                var S = "Please enter GREEN color value (0-255)\n and press 'Submit' key."; 
                Desc = S;
                while (Input.GetAxisRaw("Submit") == 0)
                {
                    Str += Input.inputString;
                    yield return new WaitUntil(() => Input.anyKey);
                    if (DebugMode) print(Str);
                    Desc = $"{S} \n{Str}";
                }
                if (DebugMode) print("Submitted!");
                Desc = "Submitted!";
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
        HoloRender.material.color = color;

        if (SetTo)
        {
            ApplyColor = color;
            FireReady = true;
            Desc = $"Colors things. {R},{G},{B}";
        } 
    }

}
