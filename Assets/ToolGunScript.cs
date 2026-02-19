using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ToolGunScript : MonoBehaviour
{
    [HideInInspector]
    public TextMesh TText,THelp;
    
    public Component[] Tools;
    string[] ComponentNames;
    int CurrentTool;

    void Awake()
    {
        TText = transform.Find("TypeText").GetComponent<TextMesh>();
        THelp = transform.Find("HelpText").GetComponent<TextMesh>();
        ComponentNames = new string[Tools.Length];
        for (int i = 0; i < Tools.Length; i++)
        {
            ComponentNames[i] = $"{i} - {Tools[i].GetType()}";
        }
    }

    public void Fire()
    {
            Type ThisTool = Tools[CurrentTool].GetType();
            var Method = ThisTool.GetMethod("Fire");
            if (Method != null && Method.IsPublic)
                Method.Invoke(Tools[CurrentTool],null);
            else
                Debug.LogError("Make the function you fat fuck!");

            UpdateTexts();
    }

    bool OnSelectionScreen,Selected;
    public void BackToMenu() 
    {
        if (!OnSelectionScreen)
        StartCoroutine(HandleMenuSelection());
        else
        UpdateTexts();
    }

    void UpdateTexts()
    {
            Component ThisTool = Tools[CurrentTool];

            Selected = true;
            TText.text = ThisTool.GetType().Name;
            FieldInfo FInfo = ThisTool.GetType().GetField("Description");
            if (FInfo != null)
            {
                object Value = FInfo.GetValue(ThisTool);
                if (Value is string Str)
                {
                THelp.text = Str;
                }
            }
            else
            Debug.LogError(FInfo.GetType());
    }

    IEnumerator HandleMenuSelection()
    {
        Selected = false;
        OnSelectionScreen = true;
        while (!Selected)
        {
            string Text = string.Empty;
            for (int i = 0; i < Tools.Length; i++)
            {
                string S = CurrentTool == i ? "<\n" : "\n";
                Text += $"{ComponentNames[i]} {S}"; 
            }
            THelp.text = Text;

            int V = CurrentTool + Mathf.RoundToInt(Input.GetAxisRaw("MMWheel"));
            CurrentTool = Mathf.Clamp(V,0,Tools.Length - 1);

            yield return new WaitForFixedUpdate();
        }
        THelp.text = string.Empty;
        OnSelectionScreen = false;
        UpdateTexts();
    }

    /*Bu nedir?

        Bu script bütün aletleri tek bir alette toplayan bir scripttir.

        Nasıl çalışır?

            Oyuncu menüye girmek için sağ tıklar
            Oyuncu menü'den almak istediği alete mouse tekerleği ile gidip sol tıklar
            alet'in çalışma şekline göre ilerler.
        
        Kaç tane alet olacak?
            
            ekleyebildiğimiz kadar!

        Bununla nasıl başa çıkacaksın?

            I dunno?

    */
}
