using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FPSLimit : MonoBehaviour
{
    [Header("FPS Limit Settings")]
    [SerializeField]
    int targetFPS = 60; // Desired frame rate
    public event Action<int> OnFPSChange;
    public int FPSTarget
    {
        get { return targetFPS; }
        set
        {
            if (targetFPS != value)
            targetFPS = value;

        }
    }
    public void FPSChange(int NewVal) => OnFPSChange.Invoke(NewVal); 

    void OnEnable() =>
        OnFPSChange += HandleFPS;

    void OnDisable() =>
        OnFPSChange -= HandleFPS;

    void HandleFPS(int FPSVal)
    {
        Application.targetFrameRate = FPSVal;
    }
}
