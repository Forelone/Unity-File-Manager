using System.Collections;
using System.Collections.Generic;
using Dummiesman;
using Unity.VisualScripting;
using UnityEngine;

public class ModelFile : MonoBehaviour
{
    IEnumerator Start()
    {
        ObjectFileInfo OFI = null;
        yield return new WaitUntil(() => TryGetComponent(out OFI));

        GameObject GObj = new OBJLoader().Load(OFI.Path);
        Transform ModelGObj = GObj.transform.GetChild(0);
        ModelGObj.SetPositionAndRotation(transform.position,transform.rotation);
        ModelGObj.SetParent(transform.parent);
        ModelGObj.name = transform.name;
        ModelGObj.AddComponent<Rigidbody>();
        ModelGObj.AddComponent<MeshCollider>().convex = true;
        ModelGObj.GetComponent<Renderer>().material = new Material(Shader.Find("Unlit/Color"));
        ModelGObj.AddComponent<ObjectFileInfo>().Setup(OFI);
        ModelGObj.AddComponent<Item>();

        Destroy(gameObject);
        Destroy(GObj);
    }
}
