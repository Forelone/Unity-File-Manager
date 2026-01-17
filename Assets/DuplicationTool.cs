using System.IO;
using UnityEngine;

public class DuplicationTool : MonoBehaviour
{
    [SerializeField] float MaxDistance = 5f;
    ObjectFileInfo FileToCopy;
    public void Copy()
    {
        if (Physics.Raycast(transform.position,transform.forward,out RaycastHit hit,MaxDistance) && hit.rigidbody != null && hit.transform.TryGetComponent(out ObjectFileInfo OFI))
        {
            FileToCopy = OFI;
        }
    }

    public void Paste() //Todo:: when a raycasthit hits the openfolder paste the file to openfolder instead of same. (done)
    {
        if (FileToCopy == null) return;
        if (Physics.Raycast(transform.position,transform.forward,out RaycastHit hit,MaxDistance))
        {
            string FileName = FileToCopy.name += Random.Range(0,999999).ToString() + FileToCopy.Extension;
            string TargetPath = FileToCopy.DirPath; //This is directory path
            PathProtocol Peepee;
            Transform Parent = FileToCopy.transform.parent;
            if (hit.transform.TryGetComponent(out Peepee))
            {
                TargetPath = Peepee.GetPath(); //This is directory path. returns directory.
                Parent = hit.transform.Find("Files");
            }
            GameObject Dupe = Instantiate(FileToCopy.gameObject);
            Dupe.name = FileName;
            Dupe.transform.SetPositionAndRotation(hit.point - transform.forward,Dupe.transform.rotation);
            Dupe.transform.SetParent(Parent);
            Destroy(Dupe.GetComponent<ObjectFileInfo>());
            TargetPath += FileName;
            
            File.Copy(FileToCopy.Path,TargetPath); //Oh boy, what could go wrong?
            FileInfo Info = new FileInfo(TargetPath);
            Dupe.AddComponent<ObjectFileInfo>().Setup(Info);

            FileToCopy = null;
        }
    }
}
