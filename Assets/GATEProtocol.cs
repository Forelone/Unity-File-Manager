using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GATEProtocol : MonoBehaviour
{
    PathProtocol Peepee;
    [SerializeField] TextMesh TM;
    string GatePath = "";
    void Start()
    {
        Peepee = GetComponentInParent<PathProtocol>();
        string Path = gameObject.transform.parent.name;
        GatePath = Peepee.GetPath() + $"{Path}/";
        TM.text = Path;
    }

    public PathProtocol GivePeepee() //*WHEEZE*
    { return Peepee; }

    public void GateOpen()
    {
        Peepee.RequestPathOpen(GatePath, this);
    }
    
    public void GateClose()
    {
        Peepee.RequestPathClose(GatePath, this);
    }
}
