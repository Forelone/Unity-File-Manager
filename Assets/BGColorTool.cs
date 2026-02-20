using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGColorTool : MonoBehaviour
{
    public string Description = "Colors folder backgrounds. \n0,0,0";
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
        hit.transform.TryGetComponent(out PathProtocol PP))
        {
            PP.SetBGColor(ApplyColor);
            FireReady = false;
            Renderer.gameObject.SetActive(false);
            Renderer.material.color = Color.black;

            if (Input.GetAxisRaw("Sprint") == 0)
            {
                Renderer.gameObject.SetActive(false);
                Renderer.material.color = Color.black;                
                FireReady = false;   
            }
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
        Renderer.gameObject.SetActive(true);
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
        Renderer.material.color = color;

        if (SetTo)
        {
            ApplyColor = color;
            FireReady = true;
            Desc = $"Colors folder backgrounds. {R},{G},{B}";
        } 
    }

}
