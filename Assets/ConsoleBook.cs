using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ConsoleBook : MonoBehaviour
{
    [SerializeField] int Index,MaxIndex;
    string[] LastLogs;
    TextMesh TextMesh;
    void Start()
    {
        TextMesh = GetComponentInChildren<TextMesh>();
        LastLogs = new string[MaxIndex];
        Application.logMessageReceived += Log;

        DrawText();
    }

    void Log(string LogSt, string StackTr, LogType LType)
    {
        DateTime Time = DateTime.Now;
        string Output = string.Empty;
        switch (LType)
        {
            case LogType.Error:
                Output += "<color=red>";
            break;
            case LogType.Warning:
                Output += "<color=yellow>";
            break;
            case LogType.Log:
                Output += "<color=black>";
            break;
            default:
                Output += "<color=black>";
            break;
        }
        Output += $"[{Time.ToShortTimeString()}] {LogSt}</color>";

        LastLogs[Index] = Output;
        Index = (Index + 1 > MaxIndex - 1) ? 0 : Index + 1;

        DrawText();
    }

    void DrawText()
    {
        string Text = string.Empty;

        foreach (var Log in LastLogs)
        {
            Text += $"{Log}\n";
        }

        TextMesh.text = Text;
    }

    public void DebugError()
    {
        Debug.LogError("This is a error!",gameObject);
    }

    public void DebugWarning()
    {
        Debug.LogWarning("This is a warning",gameObject);
    }
}
