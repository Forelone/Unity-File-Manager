using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public event Action<Vector3> OnMovement;
    private Vector3 Movement;
    public Vector3 DesiredMovement 
    { 
        get { return Movement; }
        set
        {
            if (Movement != value)
            Movement = value;
            MovementChange(Movement);
        } 
    }
    void MovementChange(Vector3 NewMove) => OnMovement?.Invoke(NewMove);

    public event Action<Vector2> OnMouseMovement;
    private Vector2 Rotation;
    public Vector2 DesiredRotation 
    { 
        get { return Rotation; }
        set
        {
            if (Rotation != value)
            Rotation = value;
            RotationChange(Rotation);
        } 
    }
    void RotationChange(Vector2 NewRota) => OnMouseMovement?.Invoke(NewRota);
    
    public event Action OnPrimaryClick;
    private bool M1 = false;
    public bool PrimaryHandUse 
    { 
        get { return M1; }
        set 
        { 
            if (M1 != value) 
            M1 = value; 
            PrimaryClick(); 
        }    
    } 
    void PrimaryClick() => OnPrimaryClick?.Invoke();
    
    public event Action OnSecondaryClick;    
    private bool M2 = false;
    public bool SecondaryHandUse 
    { 
        get { return M2; }
        set { if (M2 != value) M2 = value; SecondaryClick(); } 
    } 
    void SecondaryClick() => OnSecondaryClick?.Invoke();
    
    public event Action OnInteractClick;
    private bool M3 = false;
    public bool InteractiveUse 
    { 
        get { return M3; } 
        set { if (M3 != value) M3 = value; MiddleClick(); }
    } 
    void MiddleClick() => OnInteractClick?.Invoke();

    public event Action OnRefuseClick;
    private bool M4 = false;
    public bool Refuse
    {
        get { return M4; }
        set { if (M4 != value) M4 = value; RefuseClick(); }
    }
    void RefuseClick() => OnRefuseClick?.Invoke();

    public event Action OnInspectClick;
    private bool M5 = false;
    public bool Inspect
    {
        get { return M5; }
        set { if (M5 != value) M5 = value; InspectClick(); }
    }
    void InspectClick() => OnInspectClick?.Invoke();

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public bool IsPlayerLefty { get {return PlayerPrefs.GetInt("Lefty") > 0; }}
    Vector2 PastRotation = Vector2.zero;
    Vector3 PastMovement = Vector3.zero;
    void FixedUpdate()
    {
        float PressShift = Input.GetAxisRaw("Sprint");
        float h = Input.GetAxisRaw("Horizontal"), v = Input.GetAxisRaw("Vertical"), u = Input.GetAxisRaw("Jump") == 1 ? 1 : 0;
        Movement = new Vector3(h,u,v);
        Movement *= PressShift == 1 ? 2 : 1;
        if (Movement != PastMovement) OnMovement.Invoke(Movement); 
        PastMovement = Movement;
    }

    void Update() //Much, Much better.
    {
        float X = Input.GetAxis("Mouse X"), Y = Input.GetAxis("Mouse Y");
        Rotation = new Vector2(X,Y);
        if (Rotation != PastRotation) OnMouseMovement.Invoke(Rotation);
        PastRotation = Rotation;
        
        M1 = Input.GetAxisRaw("Fire1") == 1;
        if (PrimaryHandUse) OnPrimaryClick.Invoke();

        M2 = Input.GetAxisRaw("Fire2") == 1;
        if (SecondaryHandUse) OnSecondaryClick.Invoke();

        M3 = Input.GetAxisRaw("Interact") == 1;
        if (InteractiveUse) OnInteractClick.Invoke();
        
        M4 = Input.GetAxisRaw("Drop") == 1;
        if (Refuse) OnRefuseClick.Invoke();
    
        M5 = Input.GetAxisRaw("Inspect") == 1;
        if (Inspect) OnInspectClick.Invoke();

        Cursor.lockState = Inspect ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = Inspect;
    }


}
