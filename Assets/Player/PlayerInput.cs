using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] Vector3 Movement;
    public Vector3 DesiredMovement { get { return Movement; } }

    [SerializeField] Vector2 Rotation;
    public Vector2 DesiredRotation { get { return Rotation; } }
    [SerializeField] float VerticalMul = 1, HorizontalMul = 1;

    [SerializeField] float MinXRot = -80, MaxXRot = 80;

    [SerializeField] bool M1 = false, M2 = false, M3 = false, J = false;
    public bool PrimaryHandUse { get { return M1; } }
    public bool SecondaryHandUse { get { return M2; } }
    public bool InteractiveUse { get { return M3; } }
    public bool Jump { get { return J; } }
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    float VerRot = 0;

    void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal"), v = Input.GetAxis("Vertical");
        Movement = transform.forward * v + transform.right * h;

        float X = Input.GetAxis("Mouse X") * HorizontalMul * Time.deltaTime, Y = Input.GetAxis("Mouse Y") * VerticalMul * Time.deltaTime;

        VerRot -= Y;
        VerRot = Mathf.Clamp(VerRot, MinXRot, MaxXRot);        
        
        Rotation = Vector2.up * X + Vector2.right * VerRot;

        M1 = Input.GetAxisRaw("Fire1") == 1;
        M2 = Input.GetAxisRaw("Fire2") == 1;
        M3 = Input.GetAxisRaw("Fire3") == 1;
        J = Input.GetAxisRaw("Jump") == 1;
    }
}
