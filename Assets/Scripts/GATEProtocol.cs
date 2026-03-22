using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GATEProtocol : MonoBehaviour
{
    PathProtocol.CustomDirectorySave CDS_;
    public PathProtocol.CustomDirectorySave CDS { get { return CDS_; }}
    PathProtocol Peepee;
    [SerializeField] Renderer CustomizableRenderer;
    public bool IsOpen = false;
    public string GatePath = "";
    void Start()
    {
        Peepee = GetComponentInParent<PathProtocol>();
        string Path = gameObject.transform.name;
        GatePath = Peepee.GetPath() + $"{Path}/";
    }

    public PathProtocol GivePeepee() //*WHEEZE*
    { return Peepee; }

    public void GateToggle()
    {
        if (!IsOpen)
            Peepee.RequestPathOpen(GatePath, this);
        else
            Peepee.RequestPathClose(GatePath, this);
        if (TryGetComponent(out Animator Anim))
        {
            Anim.Play(IsOpen ? "DoorClose" : "DoorOpen");
        }
        if (GetComponentInChildren<Animator>())
        {
            GetComponentInChildren<Animator>().Play(IsOpen ? "DoorClose" : "DoorOpen");
        }
        IsOpen = !IsOpen;
    }

    public void GateOpen()
    {
        Peepee.RequestPathOpen(GatePath, this);
    }
    
    public void GateClose()
    {
        Peepee.RequestPathClose(GatePath, this);
    }

    public void AddTag(string Tag,string[] Vars)
    {
        CDS_ = CDS_ == null ? new PathProtocol.CustomDirectorySave() : CDS_;
        CDS_.Tags = CDS_.Tags == null ? new List<string>() : CDS_.Tags;
        CDS_.Name = gameObject.transform.name;
        string T = Tag + ' ' + string.Join(' ',Vars);
        
        bool Exists = false;
        for (int i = 0; i < CDS_.Tags.Count; i++)
        {
            string[] Str = CDS_.Tags[i].Split(' ');
            if (Tag == Str[0])
            {
                CDS_.Tags[i] = T;
                Exists = true;
                break;
            } 
            else continue;
        }
        if (!Exists) CDS_.Tags.Add(T);
    }

    public void LoadTags(string[] Tags_)
    {
        CDS_ = CDS_ == null ? new PathProtocol.CustomDirectorySave() : CDS_;
        CDS_.Tags = CDS_.Tags ?? Tags_.ToList();
        CDS_.Name = gameObject.transform.name;
    }

    public void ApplyCustomColor(Color C)
    {
        if (CustomizableRenderer == null)
        {
            GetComponentInChildren<Renderer>().material.color = C;
            return;
        }

        CustomizableRenderer.material.color = C;
    }
}
