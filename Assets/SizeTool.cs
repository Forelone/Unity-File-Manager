using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeTool : MonoBehaviour
{
    TextMesh TextMesh;
    [SerializeField] float MaxDistance;
    Vector3 ApplySize;
    // Start is called before the first frame update
    void Start()
    {
        TextMesh = GetComponentInChildren<TextMesh>();
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
            if (!float.TryParse(YStr,out Z)) { ClearText(); continue;}
            else {
                ApplySize = new Vector3(X,Y,Z); 
                break;
            }
        } //This is fine.
        Configuring = false;
        ResetText();
    }

    void AddText(string Text)
    {
        TextMesh.text += $"{Text}\n";
    }

    void ClearText() => TextMesh.text = string.Empty;

    void ResetText() => TextMesh.text = $"Size Tool \n\nLMB = Apply Size\nRMB = Configure Size\n\nCurrent: {ApplySize.x}, {ApplySize.y}, {ApplySize.z}";

}
