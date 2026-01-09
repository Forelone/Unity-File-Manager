using System;
using UnityEngine;

[RequireComponent(typeof(PlayerInput))]
public class PlayerRotation : MonoBehaviour
{
    [SerializeField] Transform Neck;
    [SerializeField] PlayerInput PInput;




    void FixedUpdate()
    {
        Vector2 DesiredAngle = PInput.DesiredRotation;
        
        Neck.localRotation = Quaternion.Euler(Vector3.right * DesiredAngle.x);
        transform.Rotate(transform.up * DesiredAngle.y);
    }
}
