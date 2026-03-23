using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class TextHandler : MonoBehaviour
{
    Text TM;

    void OnEnable()
    {
        TM = GetComponentInChildren<Text>();
        //First we get full path
        PathProtocol PP = GetComponentInParent<PathProtocol>();
        string Name = transform.name;
        string FullPath = PP.GetPath() + Name;
        print(FullPath);
        //Then we check if we can convert it
        string FileData = File.ReadAllText(FullPath);
        int LettersLeft = FileData.Length;
        string Page = "";
        //???
        for (int i = 0,Limit = 49; i < LettersLeft; i++)
        {
            Page += FileData[i].ToString();
            Page = (i != 0 && i % Limit == 0) ? Page + "\n" : Page;
        }
        TM.text = Page;
        //Profit!!!
    }

    void OnDisable()
    {
        TM.text = "";
    }
}
