using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuVersionCheckerScriptHandlerBullshitPleaseHelpMe : MonoBehaviour
{
    void Awake()
    {
        GetComponent<TextMesh>().text = Application.version;
    }
}
