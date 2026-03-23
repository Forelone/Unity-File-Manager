using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ConsoleBook : MonoBehaviour
{
    [SerializeField] int Index,MaxIndex;
    string[] LastLogs;
    [SerializeField] TextMesh Logs,CommandLine;
    [SerializeField] Command[] Commands;
    void Awake()
    {
        LastLogs = new string[MaxIndex];
        Application.logMessageReceived += Log;
        float Size = PlayerPrefs.GetFloat("tsize",0.005f);
        Logs.characterSize = Size;
        CommandLine.characterSize = Size;
        DrawText();
    }

    PlayerInput PI;

    public void UseConsoleBook()
    {
        if (PI == null)
        {
            PI = GetComponentInParent<PlayerInput>();
            StartCoroutine(HandleInput());
        }
    }

    string InputStr = string.Empty;

    void Update()
    {
        if (PI == null) return;

        foreach (var Char in Input.inputString)
        {
            if (Char == '\b' && InputStr.Length != 0) // has backspace/delete been pressed?
                InputStr = InputStr.Substring(0, InputStr.Length - 1);
            else if ((Char == '\n') || (Char == '\r'))
                BreakItDown(InputStr);
            else
                InputStr += Char;
        }
        CommandLine.text = $">{InputStr}";
    }

    IEnumerator HandleInput()
    {
        PI.enabled = false;
        print("press ESC (Escape) key to retake control.");
        while (!Input.GetKey(KeyCode.Escape))
        {
            //I was gonna handle it here but void Update() does it better.
            yield return new WaitForEndOfFrame();
        }
        PI.enabled = true;
        PI = null;
        print("You retook control.");
    }

    void BreakItDown(string RawCommand)
    {
        print($">{RawCommand}");
        InputStr = string.Empty;

        if (RawCommand.Length == 0) return;
        List<string> Args = RawCommand.Split().ToList();
        string Command = Args.First();

        Args.RemoveAt(0);

        if (Args.Count == 0) Args = null;

        HandleCommand(Command,Args);
    }

    void HandleCommand(string Command, List<string> Vars = null)
    {
        switch (Command)
        {
            default:
                Debug.LogError($"Unknown command '{Command}'.\nUse '?' to list all avaiable commands,\n'? <ENTER COMMAND HERE>' for details of one.");
            break;

            case "?":
                if (Vars == null)
                {
                    var Str = "All Commands:\n";
                    foreach (var Cmm in Commands)
                    {
                        Str += $"{Cmm.Name}, ";
                    }
                    print(Str);
                }
                else
                {
                    Command FoundCommand = Commands.ToList().Find(x => x.Name == Vars[0]);

                    if (FoundCommand != null)
                    print($"\n {FoundCommand.Name}\n{FoundCommand.CommandDescription}");
                }
            break;
            
            case "cls":
                LastLogs = new string[MaxIndex];
                Index = 0;
                Logs.text = string.Empty;
            break;

            case "zoom":
                var T = PI.GetComponentInChildren<Camera>().transform;
                transform.position = T.position + T.forward / 2 - T.up / 4;
            break;

            case "credits":
                var S = "Made by Forelone in 2025!\n\nUsed OBJImporter extension made by Dummiesman!\nThank you for your contrubution!";
                print(S);
            break;

            case "tsize":
                if (Vars != null && float.TryParse(Vars[0],out float Size))
                {
                    Logs.characterSize = Size / 1000;
                    CommandLine.characterSize = Size / 1000;
                    PlayerPrefs.SetFloat("tsize",Size / 1000);
                }
            break;

            case "quit":
                Debug.LogWarning("Quitting...");
                Application.Quit();
            break;

            case "sens":
                if (Vars != null && float.TryParse(Vars[0],out float Sens))
                PI.Sens = Sens;
                else
                PI.Sens = 1;
            break;
        }
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
        if (Logs == null) {Debug.LogError("Logs are not present!",gameObject); return;}
        string Text = string.Empty;

        foreach (var Log in LastLogs)
        {
            Text += $"{Log}\n";
        }

        Logs.text = Text;
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

[Serializable]
public class Command
{
    public string Name;
    [TextArea(3, 10)]
    public string CommandDescription;
}
