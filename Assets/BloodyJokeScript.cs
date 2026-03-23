using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BloodyJokeScript : MonoBehaviour
{
    public string[] Jokes;

    void Awake()
    {
        int MyLifeIsSuffering = Random.Range(0,Jokes.Length - 1);
        string SoIDecidedTo = Jokes[MyLifeIsSuffering];
        TextMesh EnjoyFromIt = GetComponent<TextMesh>();

        EnjoyFromIt.text = SoIDecidedTo;

        if (Random.Range(0,0.5f) > 0.5f)
        {
            int IfYouCantEnjoyLife = Random.Range(0,EnjoyFromIt.text.Length);
            int MakeItFunSoYouCan = IfYouCantEnjoyLife;
            while (MakeItFunSoYouCan > 0)
            {
                int EnjoyItWhileAlive = Random.Range(0,EnjoyFromIt.text.Length);

                if (Random.Range(0,0.5f) > 0.5f)
                { EnjoyFromIt.text.Remove(EnjoyItWhileAlive,1); }
                else
                { EnjoyFromIt.text += EnjoyFromIt.text[EnjoyItWhileAlive].ToString(); }
                MakeItFunSoYouCan--;
            }
        }
    }
}
