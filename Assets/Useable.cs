using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Useable : MonoBehaviour
{
    [SerializeField] UnityEvent Use;
    [SerializeField] UnityEvent AlternateUse;
    [SerializeField] float RequiredUseDelay = 0, RequiredAlternativeUseDelay = 0, DelayAfterUse = 0, AlternativeDelayAfterUse = 0;
    float UseFloat, MaxUseFloat;
    bool CanUse = true, CanAlternateUse = true, IsTrying, IsAlternativeTrying;
    bool UseFrameCheck = false, UseAltFrameCheck = false;

    public float V { get { return UseFloat; } }
    public float MV { get { return MaxUseFloat; } }

    [SerializeField]
    bool Debug = true;

    public void Use_()
    {
        if (CanUse)
        {
            if (RequiredUseDelay == 0)
            {
                Use.Invoke();
                if (DelayAfterUse > 0) StartCoroutine(Use_DelayHandle());
            }
            else
            {
                if (!IsTrying)
                    StartCoroutine(UseHoldHandle());
                else
                    UseFrameCheck = true;
            }
        }
    }

    public void Use_Alternative()
    {
        if (CanAlternateUse)
        {
            if (RequiredUseDelay == 0)
            {
                AlternateUse.Invoke();
                if (AlternativeDelayAfterUse > 0) StartCoroutine(Use_AlternativeDelayHandle());
            }
            else
            {
                if (!IsAlternativeTrying)
                    StartCoroutine(UseAltHoldHandle());
                else
                    UseAltFrameCheck = true;
            }
        }
    }

    IEnumerator Use_AlternativeDelayHandle()
    {
        UseFloat = 0f; MaxUseFloat = AlternativeDelayAfterUse;

        CanAlternateUse = false;
        while (UseFloat < MaxUseFloat)
        {
            if (Debug)
            print("Alternative Delay:" + UseFloat + "/" + MaxUseFloat);
            UseFloat += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        CanAlternateUse = true;
    }

    IEnumerator Use_DelayHandle()
    {
        UseFloat = 0f; MaxUseFloat = DelayAfterUse;

        CanUse = false;
        while (UseFloat < MaxUseFloat)
        {
            if (Debug)
            print("Primary Delay:" + UseFloat + "/" + MaxUseFloat);
            UseFloat += 0.01f;
            yield return new WaitForSeconds(0.01f);
        }
        CanUse = true;
    }

    IEnumerator UseHoldHandle()
    {
        IsTrying = true;
        UseFrameCheck = true;
        UseFloat = 0; MaxUseFloat = RequiredUseDelay;

        while (UseFloat < MaxUseFloat)
        {
            if (Debug) print("Primary: " + UseFloat + "/" + RequiredUseDelay);

            if (UseFrameCheck) { UseFloat += Time.deltaTime; UseFrameCheck = false; }
            else UseFloat -= Time.deltaTime;

            if (UseFloat > MaxUseFloat)
            {
                if (Debug) print("Use Success");
                Use.Invoke();
                if (DelayAfterUse > 0) StartCoroutine(Use_DelayHandle());
                break;
            }
            else if (UseFloat <= 0)
            {
                if (Debug) print("Use Fail");
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime * 2);
        }
        UseFloat = 0;
        IsTrying = false;
    }

    IEnumerator UseAltHoldHandle()
    {
        IsAlternativeTrying = true;
        UseAltFrameCheck = true;
        UseFloat = 0; MaxUseFloat = RequiredAlternativeUseDelay;

        while (UseFloat < MaxUseFloat)
        {
            if (Debug) print("Alternative: " + UseFloat + "/" + RequiredAlternativeUseDelay);

            if (UseAltFrameCheck) { UseFloat += Time.deltaTime; UseAltFrameCheck = false; }
            else UseFloat -= Time.deltaTime;

            if (UseFloat > MaxUseFloat)
            {
                if (Debug) print("Alternate Success");
                AlternateUse.Invoke();
                if (AlternativeDelayAfterUse > 0) StartCoroutine(Use_AlternativeDelayHandle());
                break;
            }
            else if (UseFloat <= 0)
            {
                if (Debug) print("Alternate Fail");
                break;
            }
            yield return new WaitForSeconds(Time.deltaTime * 2);
        }
        UseFloat = 0;
        IsAlternativeTrying = false;
    }
}
