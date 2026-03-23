using UnityEngine;

public class GroundProtocol : MonoBehaviour
{
    PathProtocol PP;

    void Awake() => PP = GetComponentInParent<PathProtocol>();

    public void AddTag(string Tag,string[] Vars)
    {
        PP.AddTag(Tag,Vars);
    }

    public Color GetBGColor() => PP.GetBGColor();
    public void SetBGColor(Color C) => PP.SetBGColor(C);
    public void Save() => PP.Save(false);
}
