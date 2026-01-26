using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGColorTool : MonoBehaviour
{
[SerializeField] Renderer Renderer;
    [SerializeField] TextMesh TxMesh;
    [SerializeField] float MaxDistance;
    [SerializeField] Color ApplyColor;

    bool Configuring;

    public void Apply()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance) && 
        hit.transform != transform && 
        hit.transform.TryGetComponent(out PathProtocol PP))
        {
            PP.SetBGColor(ApplyColor);
        }
    }

    public void Configure()
    {
        if (!Configuring)
        StartCoroutine(Configuration());
    }

    IEnumerator Configuration()
    {
        ClearText();
        Configuring = true;
        AddText("Configuration started, \nEscape to exit.\n");
        ApplyColor = Color.black;
        int RI,GI,BI;
        float R,G,B;
        while (!Input.GetKey(KeyCode.Escape))
        {
            AddText("Please input red color value\nbetween 255 and 0\n and press Keypad Enter:");
            string RStr = "0";
            while (!Input.GetKey(KeyCode.KeypadEnter))
            {
                if (Input.anyKey)
                {
                    RStr += Input.inputString;
                }
                yield return new WaitUntil(() => Input.anyKey);
            }
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.KeypadEnter));
            if (int.TryParse(RStr,out RI))
            {
                R = RI / 255f;
                ApplyColor.r = R;
                UpdateColor();
            }
            else { ClearText(); continue;}
            ClearText();
            
            AddText("Please input green color value\nbetween 255 and 0\n and press Keypad Enter:");
            string GStr = "0";
            while (!Input.GetKey(KeyCode.KeypadEnter))
            {
                if (Input.anyKey)
                {
                    GStr += Input.inputString;
                }
                yield return new WaitUntil(() => Input.anyKey);
            }
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.KeypadEnter));
            if (int.TryParse(GStr,out GI))
            {
                G = RI / 255f;
                ApplyColor.g = G;
                UpdateColor();
            }
            else { ClearText(); continue;}
            ClearText();

            AddText("Please input blue color value\nbetween 255 and 0\n and press Keypad Enter:");
            string BStr = "0";
            while (!Input.GetKey(KeyCode.KeypadEnter))
            {
                if (Input.anyKey)
                {
                    BStr += Input.inputString;
                }
                yield return new WaitUntil(() => Input.anyKey);
            }
            yield return new WaitUntil(() => !Input.GetKey(KeyCode.KeypadEnter));
            if (int.TryParse(BStr,out BI))
            {
                B = BI / 255f;
                ApplyColor.b = B;
                UpdateColor();
                break;
            }
            else{ ClearText(); continue;}
        } //This is fine.
        Configuring = false;
        ResetText();
    }

    void AddText(string Text)
    {
        TxMesh.text += $"{Text}\n";
    }

    void ClearText() => TxMesh.text = string.Empty;

    void ResetText() => TxMesh.text = $"BG Color Tool \n\nLMB = Apply Color\nRMB = Configure Color\n\nCurrent: {Mathf.RoundToInt(ApplyColor.r * 255)}, {Mathf.RoundToInt(ApplyColor.g * 255)}, {Mathf.RoundToInt(ApplyColor.b * 255)}";

    void UpdateColor() => Renderer.material.color = ApplyColor;

}
