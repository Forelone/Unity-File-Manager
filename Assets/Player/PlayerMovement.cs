using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float KMH = 1, AccelerationMul;
    [SerializeField] PlayerInput PInput;
    [SerializeField] Rigidbody RG;

    [SerializeField] float JumpMul = 2,JumpCooldown = 1f;
    float Cooldown = 0;
    bool TrackingJump = false;

    void FixedUpdate()
    {
        Vector3 PMove = PInput.DesiredMovement * KMH;
        float YAxis = RG.velocity.y;
        Vector3 O = Vector3.Lerp(RG.velocity, PMove + Vector3.up * YAxis, Time.fixedDeltaTime * AccelerationMul);
        RG.velocity = O;

        if (PInput.Jump && !TrackingJump) StartCoroutine(JumpHandle());
    }

    int JumpFrame = 4, CurFrame; //Player has to leave jump button within 4 frame. why you ask? for hands implementation i say.
    IEnumerator JumpHandle()
    {
        if (!TrackingJump && Cooldown <= 0f)
        {
            TrackingJump = true;

            for (CurFrame = JumpFrame; CurFrame > 0; CurFrame--)
            {
                if (!PInput.Jump)
                {
                    RG.AddForce(transform.up * KMH * JumpMul, ForceMode.VelocityChange);
                    StartCoroutine(CooldownHandle());
                    break;
                }
                yield return new WaitForEndOfFrame();
            }
            TrackingJump = false;
        }
    }

    IEnumerator CooldownHandle()
    {
        Cooldown = JumpCooldown;
        while (Cooldown > 0)
        {
            Cooldown -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }
}
