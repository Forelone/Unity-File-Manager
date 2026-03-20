using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArms : MonoBehaviour
{
    [SerializeField] Transform LeftArm,RightArm,Head;
    PlayerHands PHands;
    CharacterController CC;
    Camera Eye;

    void Start()
    {
        PHands = GetComponent<PlayerHands>();
        CC = GetComponent<CharacterController>();
        Eye = GetComponentInChildren<Camera>();
    }

    void FixedUpdate()
    {
        float Vel = CC.velocity.magnitude;
        Transform MainArm = PHands.Lefty ? LeftArm : RightArm;
        Transform OtherArm = !PHands.Lefty ? LeftArm : RightArm;
        Vector3 Direction = -transform.up;
        Vector3 ODirection = -transform.up;
        if (PHands.IsHandFull)
        {
            Ray CamRay = Eye.ScreenPointToRay(Input.mousePosition);
            Direction = PHands.Inspecting && !PHands.IsOHandFull ? CamRay.direction : Head.forward;
        }
        else
        {
            if (Vel > 1)
            Direction = -CC.velocity.normalized;
            else
            Direction = -Vector3.up; 
        }

        
        if (PHands.IsOHandFull)
        {
            Vector3 MousePos = Input.mousePosition;
            Ray CamRay = PHands.Eye_.ScreenPointToRay(MousePos);
            ODirection = CamRay.direction;
        }
        else
        {
            if (Vel > 1)
            ODirection = -CC.velocity.normalized;
            else
            ODirection = -Vector3.up;   
        }

        Vector3 Lerp = Vector3.Lerp(MainArm.forward,Direction,0.1f);
        MainArm.forward = Lerp;

        Vector3 Lerp2 = Vector3.Lerp(OtherArm.forward,ODirection,0.1f);
        OtherArm.forward = Lerp2;
    }

    /*
        FixedUpdate()
            -Oyuncunun eli boş mu?
                Evet:
                    -Oyuncu hareket ediyor mu?
                        Evet:
                            Oyuncunun kolu velocity'nin tersine doğru baksın.
                        Hayır:
                            Oyuncunun kolu yere baksın.
                Hayır:
                    Oyuncunun kolu kafanın rotasyonunda olsun
    */
}
