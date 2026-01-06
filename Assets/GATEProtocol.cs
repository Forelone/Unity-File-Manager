using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GATEProtocol : MonoBehaviour
{
    PathProtocol Peepee;
    string GatePath = "";
    void Start()
    {
        Peepee = GetComponentInParent<PathProtocol>();
        GatePath = Peepee.GetPath() + $"{gameObject.transform.parent.name}/";
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
