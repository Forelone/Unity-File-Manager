using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(PlayerHands))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerUI : MonoBehaviour
{
    [SerializeField] Camera Eyes;
    [SerializeField] PlayerHands PH;
    [SerializeField] PlayerInput PI;
    [SerializeField] RectTransform Crosshair;
    [SerializeField] Image InfoImage, DropImage;
    [SerializeField] Sprite[] Images; //0 is Pickup, 1 is Drop, 2 is use
    [SerializeField] Slider InteractMeter;
    [SerializeField] Text Text;
    [SerializeField] float CrosshairSpeed;

    Vector2 ScreenCenter;
    void Start()
    {
        ScreenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    void Update()
    {
        Vector2 TargetPos = ScreenCenter;
        Transform T = (PH.HeadRayDidHit) ? PH.HeadRayHit.transform : transform;
        bool IsItem = T.TryGetComponent(out Item I);
        bool IsUseable = T.TryGetComponent(out Useable U);

        if (!PH.IsEquipped && PH.HeadRayDidHit && IsUseable)
            TargetPos = Eyes.WorldToScreenPoint(PH.HeadRayHit.transform.position);

        Crosshair.anchoredPosition = Vector2.Lerp(Crosshair.anchoredPosition, TargetPos, Time.deltaTime * CrosshairSpeed);

        if (!PH.IsEquipped && PH.RStat == 1)
        {
            InteractMeter.maxValue = 1;
            InteractMeter.value = PH.TLeft;
        }
        else if (PH.HeadRayDidHit && IsUseable)
        {
            InteractMeter.maxValue = U.MV;
            InteractMeter.value = U.V;
        }
        else
        {
            InteractMeter.maxValue = 1f;
            InteractMeter.value = 0;
        }

        DropImage.gameObject.SetActive(PH.IsEquipped);

        if (PH.HeadRayDidHit)
        {
            if (IsItem)
            {
                InfoImage.sprite = Images[0];
                InfoImage.color = Color.white;
            }
            else if (IsUseable)
            {
                InfoImage.sprite = Images[2];
                InfoImage.color = Color.white;
            }
        }
        else
        {
            InfoImage.sprite = null;
            InfoImage.color = new Color(0, 0, 0, 0);
        }
    }
}
