using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookScript : MonoBehaviour
{
    public List<string> Texts = new List<string>();
    [SerializeField] TextMesh TM0, TM1;
    [SerializeField] Animation Animation;
    [SerializeField] int CurrentPage = -1; //-1 is closed. 0 is first page. Last count of Texts is... well.. the last index.
    void Awake()
    {
        ChangeText(string.Empty,string.Empty);
    }

    public void IncreasePage(int Value = 1)
    {
        int LastPage = Texts.Count - 1;
        if (LastPage == -1) return;

        int DesiredPage = CurrentPage + Value> LastPage ? -1 : CurrentPage + Value;

        switch (DesiredPage)
        {
            case -1:
                Animation.Play("BookClose"); //Cleanest code ever! I love myself! You should too! YOU SHOULD LOVE YOURSELF, NOW!
            break;

            case 0:
                Animation.Play("BookOpen");
            break;

            default:
                Animation.Play("BookFlip");
            break;
        }

        CurrentPage = DesiredPage;

        if (CurrentPage != -1)
        {
            string DisplayText0 = Texts[CurrentPage], DisplayText1;
            DisplayText1 = CurrentPage + Value > LastPage ? string.Empty : Texts[CurrentPage + Value];
            ChangeText(DisplayText0,DisplayText1);
        }
        else
            ChangeText(string.Empty,string.Empty);
    }

    void ChangeText(string DispText0,string DispText1)
    {
        TM0.text = DispText0;
        TM1.text = DispText1;
    }
}
