using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using Random = UnityEngine.Random;

public class PathProtocol : MonoBehaviour
{
    [SerializeField] string Path;
    [SerializeField] long SizeInBytes;
    [SerializeField] int FilesInside;
    [SerializeField] int DirsInside = 1;
    [SerializeField] bool Debugg = false;

    [Header("Prefabs")]
    [SerializeField] GameObject GatePrefab;
    [SerializeField] GameObject FailPrefab;
    [SerializeField] Extensions[] OtherPrefabs;
    public float MinFileScale = 3, MinFolderScale = 2;

    public string GetPath()
    {
        return $"{Path}";
    }

    void Awake() //Debug
    {
        if (Debugg)
        SetupPath(Path);
    }

    public void SetupPath(string Path)
    {
        this.Path = Path;

        StartCoroutine(PathHandle());
    }

    public void RequestPathOpen(string PathToRequest, GATEProtocol RequestedBy)
    {
        print(PathToRequest);
        FileProtocol.Instance.OpenFolder(PathToRequest, RequestedBy);
    }

    IEnumerator PathHandle()
    {
        bool HasCustomizationFile = false; List<string> CustomizationData = new List<string>();
        DirectoryInfo DirInfo = new DirectoryInfo(Path);
        FileInfo[] Files = DirInfo.GetFiles();
        DirectoryInfo[] Directories = DirInfo.GetDirectories();

        gameObject.name = DirInfo.Name;

        long BiggestFileSize = 0;

        foreach (var ThisFile in Files)
        {
            SizeInBytes += ThisFile.Length;
            FilesInside++;

            if (ThisFile.Extension == ".ufmsave")
            {
                HasCustomizationFile = true;
                CustomizationData = File.ReadAllLines(Path + ThisFile.Name).ToList();
            }
            
            if (ThisFile.Length > BiggestFileSize)
                BiggestFileSize = ThisFile.Length;
        }
        foreach (var Directory in Directories)
        {
            DirsInside++;
        }

        int TotalObjects = FilesInside + DirsInside;
        //        print($"{Path} contains {TotalObjects} files with biggest File size being {BiggestFileSize / 1048576}MB");

        //End of gathering directory info

        Color BaseColor = Color.white;
        Vector3 CustomBaseSize = Vector3.zero;
        if (HasCustomizationFile)
        {
            string[] Header = CustomizationData[0].Split("),(");
            var SizeVec = FileProtocol.Instance.StringToVector3(Header[0]);
            var ColorVec = FileProtocol.Instance.StringToVector3(Header[1]);
            BaseColor = new Color(ColorVec.x, ColorVec.y, ColorVec.z);
            GetComponent<Renderer>().material.color = BaseColor;
            CustomBaseSize = SizeVec;
        }

            if (FilesInside == 0) MinFileScale = MinFolderScale;
        int FakeDirsInside = (DirsInside == 0) ? FilesInside : DirsInside; //Act like you have a lot of files so player can walk peacefully.
        int FakeFilesInside = (FilesInside == 0) ? DirsInside : FilesInside; //You get the drill.

        Vector3 StartScale = transform.localScale,
                EndScale = (!HasCustomizationFile) ? Vector3.up * 100
                + (Vector3.right * Mathf.Clamp(FakeFilesInside * MinFileScale, MinFileScale, Mathf.Infinity)
                + Vector3.forward * Mathf.Clamp(FakeDirsInside * MinFolderScale, MinFolderScale, Mathf.Infinity)) : CustomBaseSize;
                
        for (int F = 0; F < 100; F++)
        {
            transform.localScale = Vector3.Lerp(StartScale, EndScale, F / 100f); //Be sure to add 'f' to near static intreger numbers for no accidents.
            yield return new WaitForEndOfFrame();
        }

        //End of animation

        List<Vector3> GatePos = new List<Vector3>();
        List<Quaternion> GateRot = new List<Quaternion>();

        float DoorSizer = (!HasCustomizationFile) ? Mathf.Clamp(FakeDirsInside,1,Mathf.Infinity) * MinFolderScale / 2 : CustomBaseSize.z / 2;
        float FileSizer = (!HasCustomizationFile) ? Mathf.Clamp(FakeFilesInside, 1, Mathf.Infinity) * MinFileScale / 2 : CustomBaseSize.x / 2;
        float UpSizer = (!HasCustomizationFile) ? 50 : CustomBaseSize.y / 2;
        Vector3 LeftStart = transform.position - transform.right * FileSizer - transform.forward * DoorSizer + transform.up * UpSizer,
                LeftEnd = transform.position - transform.right * FileSizer + transform.forward * DoorSizer+ transform.up * UpSizer,
                ForwardStart = LeftEnd,
                ForwardEnd = transform.position + transform.right * FileSizer + transform.forward * DoorSizer+ transform.up * UpSizer,
                RightStart = ForwardEnd,
                RightEnd = transform.position + transform.right * FileSizer - transform.forward * DoorSizer+ transform.up * UpSizer;

        int DirLeft = DirsInside;
        if (DirsInside <= 3)
        {
            for (int i = 0; i < DirsInside; i++)
            {
                float Status = i / (float)DirsInside;
                GatePos.Add(Vector3.Lerp(ForwardStart, ForwardEnd, Status));
                GateRot.Add(Quaternion.Euler(transform.localRotation.eulerAngles + Vector3.up * 0f));
                DirLeft--;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {

            for (int i = 1; i < 4; i++)
            {
                float Status = i / (float)4;
                GatePos.Add(Vector3.Lerp(ForwardStart, ForwardEnd, Status));
                GateRot.Add(Quaternion.Euler(transform.localRotation.eulerAngles + Vector3.up * 0f));
                DirLeft--;
                yield return new WaitForEndOfFrame();
            }

            for (int i = 0, D = DirLeft / 2; i < D; i++)
            {
                float A = i + 1;
                float Status = A / (D + 1);
                GatePos.Add(Vector3.Lerp(LeftStart, LeftEnd, Status));
                GateRot.Add(Quaternion.Euler(transform.localRotation.eulerAngles + Vector3.up * -90f));
                DirLeft--;
                yield return new WaitForEndOfFrame();
            }

            for (int i = 0, D = DirLeft; i < D; i++)
            {
                float A = i + 1;
                float Status = A / (D + 1);
                GatePos.Add(Vector3.Lerp(RightStart, RightEnd, Status));
                GateRot.Add(Quaternion.Euler(transform.localRotation.eulerAngles + Vector3.up * 90f));
                DirLeft--;
                yield return new WaitForEndOfFrame();
            }
        }




        GameObject[] Gates = new GameObject[DirsInside];
        for (int G = 0; G < Gates.Length; G++)
        {
            GameObject GateFab = Instantiate(GatePrefab);
            GateFab.transform.SetPositionAndRotation(GatePos[G], GateRot[G]);
            GateFab.gameObject.name = Directories[G].Name;
            GateFab.transform.SetParent(transform);
            Gates[G] = GateFab;
            yield return new WaitForFixedUpdate();
        }

        //End of Directory Gates

        //Start of File creations
        GameObject FileEntrances = new GameObject("Files");
        FileEntrances.transform.SetParent(transform);

        Vector3 SpawnPos = transform.position + transform.up * 50 + transform.forward * Random.Range(-1,1f) + transform.right * Random.Range(-1,1f);
        GameObject[] FileObjects = new GameObject[Files.Length];
        foreach (var File in Files)
        {
            //print(File.Extension);
            bool MatchedAFileType = false;
            Transform Object = null;
            foreach (var ReadyPrefab in OtherPrefabs)
            {
                if (File.Extension == ReadyPrefab.Extension)
                {
                    Object = Instantiate(ReadyPrefab.Prefab).transform;
                    Object.SetPositionAndRotation(SpawnPos, transform.rotation);
                    MatchedAFileType = true;
                }
            }

            if (!MatchedAFileType)
            {
                Object = Instantiate(FailPrefab).transform;
                Object.SetPositionAndRotation(SpawnPos, transform.rotation);
            }

            var FoundYa = CustomizationData.Find(X => X.Split(" , ")[0] == File.Name);
            if (FoundYa != null)
            {
                FileProtocol Fap = FileProtocol.Instance;
                string[] Values = FoundYa.Split(" , ");
                Vector3 Position = Fap.StringToVector3(Values[1]);
                Quaternion Rotation = Quaternion.Euler(Fap.StringToVector3(Values[2]));
                Object.SetPositionAndRotation(Position, Rotation);
                Object.GetComponent<Renderer>().material.color = Fap.StringToColor3(Values[3]);
            }

            Object.AddComponent<Item>();
            Object.gameObject.name = File.Name;
            Object.GetComponent<Rigidbody>();
            Object.SetParent(FileEntrances.transform);
        }
    }

    public void RequestPathClose(string gatePath, GATEProtocol gATEProtocol)
    {
        //Why am i requesting file protocol to clean up? Chain of command.
        FileProtocol.Instance.CloseFolder(gatePath, gATEProtocol);
    }

    public void KillYourSelf()
    {
        StartCoroutine(AttemptToSelfCare());
    }

    IEnumerator AttemptToSelfCare()
    {
        List<string> SaveStrings = new List<string>();
        Color Col = GetComponent<Renderer>().material.color;
        Vector3 VecCol = new Vector3(Col.r, Col.g, Col.b);
        SaveStrings.Add($"{transform.localScale.ToString()},{VecCol.ToString()}");
        Transform FileTransform = transform.Find("Files");
        for (int i = FileTransform.childCount; i > 0; i--)
        {
            Transform Obj = FileTransform.GetChild(i - 1);

            Color color = Obj.GetComponent<Renderer>().material.color;
            Vector3 col = new Vector3(color.r, color.g, color.b);
            Vector3 pos = Obj.position;
            Vector3 rot = Obj.eulerAngles;
            string LifeString = $"{Obj.gameObject.name} , {pos.ToString()} , {rot.ToString()} , {col.ToString()}";
            //Why not just read like buffers? 
            // coz i like strings. 
            // they're readable by humans
            // And also performance heavy but who cares?
            // in a world where you have to subscribe to everything.
            SaveStrings.Add(LifeString);
            yield return new WaitForFixedUpdate();
        }

        string SavePath = Path + "save.ufmsave";
        if (File.Exists(SavePath))
        {
            print($"Saved to {SavePath}");
        }
        else
        {
            print($"File Already exists. Overwriting it.");
        }
        try {File.WriteAllLines(SavePath, SaveStrings);}
        catch (System.Exception)
        {
            Debug.LogError("An Error Occurred during saving, you prolly need admin access for this one bro.", gameObject);
        }
        StartCoroutine(ChildKillingFunction(transform));        
    }
    
    IEnumerator ChildKillingFunction(Transform T) //Top 10 jokes that went too far
    {
            Vector3 RND = Vector3.up * Random.Range(-1, 1f) + Vector3.forward * Random.Range(-1, 1f) + Vector3.right * Random.Range(-1, 1f);
            Vector3 RND2 = Vector3.up * Random.Range(-1, 1f) + Vector3.forward * Random.Range(-1, 1f) + Vector3.right * Random.Range(-1, 1f);

        float DestroyTimer = Random.Range(5, 10f);
        Collider CL;
        Rigidbody RG;
        for (int i = T.childCount; i > 0; i--)
        {
            Transform Object = T.GetChild(i - 1);

            Object.SetParent(null);

            CL = Object.TryGetComponent(out CL) ? CL : Object.AddComponent<MeshCollider>();
            if (Object.TryGetComponent(out MeshCollider MCL))
            {
                Destroy(MCL);
                CL = Object.AddComponent<BoxCollider>();
            }
            RG = Object.TryGetComponent(out RG) ? RG : Object.AddComponent<Rigidbody>();

            if (Object.TryGetComponent(out GATEProtocol Gate)) Gate.GateClose();
            if (Object.TryGetComponent(out PathProtocol Path)) Path.RequestPathClose(Path.GetPath(),null);

            RND = Vector3.up * Random.Range(-1, 1f) + Vector3.forward * Random.Range(-1, 1f) + Vector3.right * Random.Range(-1, 1f);
            RND2 = Vector3.up * Random.Range(-1, 1f) + Vector3.forward * Random.Range(-1, 1f) + Vector3.right * Random.Range(-1, 1f);
            DestroyTimer = Random.Range(5, 10f);
            RG.AddForce(RND, ForceMode.VelocityChange);
            RG.AddTorque(RND2, ForceMode.VelocityChange);
            if (Object.childCount > 1) StartCoroutine(ChildKillingFunction(Object));
            Destroy(Object.gameObject, DestroyTimer);
            yield return new WaitForFixedUpdate();
        }
        transform.SetParent(null); //Yes
        CL = transform.TryGetComponent(out CL) ? CL : transform.AddComponent<MeshCollider>();
            if (CL.GetType() == typeof(MeshCollider))
            {
                MeshCollider MCL = CL as MeshCollider;
                MCL.convex = true;
            }
        RG = transform.TryGetComponent(out RG) ? RG : transform.AddComponent<Rigidbody>();
        RG.drag = 1f;
        RG.angularDrag = 1f;
            RND = Vector3.up * Random.Range(-1, 1f) + Vector3.forward * Random.Range(-1, 1f) + Vector3.right * Random.Range(-1, 1f);
            RND2 = Vector3.up * Random.Range(-1, 1f) + Vector3.forward * Random.Range(-1, 1f) + Vector3.right * Random.Range(-1, 1f);
            DestroyTimer = Random.Range(5, 10f);
            RG.AddForce(RND, ForceMode.VelocityChange);
            RG.AddTorque(RND2, ForceMode.VelocityChange);

        DestroyTimer = Random.Range(5, 10f);
        Destroy(gameObject, DestroyTimer);
    }

    [Serializable]
    public class Extensions
    {
        public string Extension;
        public GameObject Prefab;
    }
}
