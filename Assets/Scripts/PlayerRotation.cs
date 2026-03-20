using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerRotation : MonoBehaviour
{
    [SerializeField] Transform Neck;
    void Start()
    {
        PlayerInput playerInput = GetComponent<PlayerInput>();

        playerInput.OnMouseMovement += HandleRotation;
    }

    [SerializeField] float MinY = -80, MaxY = 80;

    float NeckRot = 0f;
    void HandleRotation(Vector2 NewRotation)
    {
        NeckRot = Mathf.Clamp(NeckRot - NewRotation.y,MinY,MaxY);

        Neck.localRotation = Quaternion.AngleAxis(NeckRot, Vector3.right);
        transform.Rotate(transform.up * NewRotation.x);
    }
}
