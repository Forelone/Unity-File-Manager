using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicalSlider : MonoBehaviour
{
    [SerializeField] float MinValue,MaxValue,Value,Difficulty = 100;
    public event Action OnSliderChange;
    private float SliderValue
    { 
        get { return Value; }
        set
        {
            if (Value != value)
            {
                Value = value;
                SliderValueChange();
            }
        }
    }
    public void SliderValueChange() => OnSliderChange?.Invoke();

    [SerializeField] Vector3 RenderMoveAxis;
    [SerializeField] float MoveDist = 1f;
    [SerializeField] bool RenderObject = true;
    Transform RenderTransform;

    /*
        Bu nasıl çalışır?
        Oyuncu bu collider'a raycast attığı zaman PhysicalSlider component'i var mı diye kontrol eder?
        Var ise bu slideri sağ veya sola çekerek veya döndürerek 'Value' değerini kontrol eder.
        Eğer RenderObject aktif ise obje hareket ediyormuş gibi görünür.
    */

    void Start()
    {
        if (RenderObject)
        {
            MeshFilter MF = GetComponent<MeshFilter>();
            Renderer R = GetComponent<Renderer>();

            Mesh mesh = GetComponent<MeshFilter>().mesh;
            Material[] mats = GetComponent<MeshRenderer>().materials;

            RenderTransform = new GameObject("Render").transform;
            RenderTransform.SetPositionAndRotation(transform.position,transform.rotation);
            RenderTransform.SetParent(transform);
            RenderTransform.AddComponent<MeshFilter>().mesh = mesh;
            RenderTransform.AddComponent<MeshRenderer>().materials = mats;
            Destroy(MF);
            Destroy(R);
        }
    }

    void OnEnable() => OnSliderChange += OnValueChange;

    void OnDisable() => OnSliderChange -= OnValueChange;

    void OnValueChange()
    {
        Vector3 TargetPos = transform.localPosition + RenderMoveAxis * MoveDist * Value;
        RenderTransform.localPosition = TargetPos;

        if (Value == MinValue) OnMinValueReach?.Invoke();
        if (Value == MaxValue) OnMaxValueReach?.Invoke();
    }

    public event Action OnMaxValueReach;

    public event Action OnMinValueReach;

    public void StartHolding()
    {
        Vector3 MouseHoldStartPos = Input.mousePosition;
        StartCoroutine(HoldHandle(MouseHoldStartPos));
    }

    bool AlreadyHolding = false;

    IEnumerator HoldHandle(Vector3 MouseStartPos)
    {
        if (!AlreadyHolding)
        {
            AlreadyHolding = true;
            float PositiveX = MouseStartPos.x + Difficulty;
            float NegativeX = MouseStartPos.x - Difficulty;

            while (Input.GetAxisRaw("Interact") == 1)
            {
                float MouseCurrentX = Input.mousePosition.x;
                SliderValue = Mathf.InverseLerp(NegativeX,PositiveX,MouseCurrentX);
                //OnSliderChange.Invoke();
                yield return new WaitForFixedUpdate();
            }
            AlreadyHolding = false;
        }
    }
}
