using System.Collections;
using System.Collections.Generic;
using Dummiesman;
using Unity.VisualScripting;
using UnityEngine;

public class ModelFile : MonoBehaviour
{
    string CustomPath = "";
    Coroutine DefaultRoutine;
    void Start()
    {
        DefaultRoutine = StartCoroutine(CreateObject(""));
    }

    public string GetPath()
    {
        return CustomPath;
    }

    IEnumerator CreateObject(string Path)
    {
        bool IsCustomPath = CustomPath != "";
        ObjectFileInfo OFI = null;
        yield return new WaitUntil(() => IsCustomPath || TryGetComponent(out OFI));

        GameObject GObj = new OBJLoader().Load(IsCustomPath ? CustomPath : OFI.Path);
        Transform ModelT = GObj.transform.GetChild(0);

        Mesh mesh = ModelT.GetComponent<MeshFilter>().sharedMesh;

        transform.GetOrAddComponent<MeshFilter>().sharedMesh = mesh;
        if (transform.TryGetComponent(out Collider Col)) Destroy(Col);
        transform.AddComponent<BoxCollider>();
        Destroy(ModelT.gameObject);
        Destroy(GObj);
    }

    public void OverrideModel(string PathToModel)
    {
        if (DefaultRoutine != null) StopCoroutine(DefaultRoutine);
        CustomPath = PathToModel;
        DefaultRoutine = StartCoroutine(CreateObject(PathToModel));
    }
}
