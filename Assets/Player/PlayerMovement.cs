using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] float WalkSpeed = 1, RunSpeed = 2,JumpPower = 1.5f,DampSpeed = 10,GroundCheckOffset = 1,GroundCheckDist = 0.1f,Gravity = -9.81f;
    bool OnGround;
    [SerializeField] bool DebugMode = true;

    Vector3 Velocity,DesiredVelocity;
    CharacterController CC;
    void Awake()
    {
        CC = GetComponent<CharacterController>();
    }

    void OnEnable()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.OnMovement += HandleMovement;
    }

    void OnDisable()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();
        playerInput.OnMovement -= HandleMovement;        
    }

    void FixedUpdate()
    {
        Vector3 Origin = transform.position - transform.up * GroundCheckOffset, Direction = -transform.up;
        Ray ray = new Ray(Origin,Direction);
        RaycastHit hit;
        OnGround = Physics.Raycast(ray,out hit,GroundCheckDist) && hit.distance < GroundCheckDist / 2;

        Velocity = OnGround ? Vector3.Lerp(Velocity,DesiredVelocity,Time.fixedDeltaTime * DampSpeed) :
                              Vector3.Lerp(Velocity,transform.up * Gravity,Time.fixedDeltaTime / DampSpeed);
        
        CC.Move(Velocity);

        //DON'T TOUCH MY BALLS
        if (DebugMode)
            Debug.DrawRay(Origin,Direction * GroundCheckDist,OnGround ? Color.green : Color.red);
    }

    public void HandleMovement(Vector3 NewMovement)
    {
        float Mag = NewMovement.magnitude;
        NewMovement = NewMovement.normalized;

        DesiredVelocity = transform.forward * NewMovement.z + transform.right * NewMovement.x;
        DesiredVelocity += NewMovement.y > 0 ? transform.up * JumpPower * -Gravity : Vector3.zero;
        DesiredVelocity *= Mag == 2f ? RunSpeed : WalkSpeed; 
    }
}
