using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using System.Text.RegularExpressions;

public class PathProtocol : MonoBehaviour
{
    [SerializeField] string Path;
    [SerializeField] bool Debugg = false;

    [Header("Prefabs")]
    [SerializeField] GameObject GatePrefab;
    [SerializeField] GameObject FailPrefab;
    [SerializeField] Extensions[] OtherPrefabs;
    Dictionary<string,GameObject> ExtensionDictionary;
    public float MinFileScale = 3, MinFolderScale = 2;
    CustomDirectoryInsideSave CurrentSave;
    [SerializeField] Color BackgroundColor;
    Vector3 ColorBG;

    List<ObjectFileInfo> Files;
    List<GATEProtocol> Directories;

    public string GetPath()
    {
        return $"{Path}";
    }

    void Awake() //Debug
    {
        if (Debugg)
        SetupPath(Path);

        ColorBG = new Vector3(BackgroundColor.r,BackgroundColor.g,BackgroundColor.b);
    }

    public void SetupPath(string Path)
    {
        this.Path = Path;

        StartCoroutine(PathHandle());
    }

    public void RequestPathOpen(string PathToRequest, GATEProtocol RequestedBy)
    {
        if (Debugg) print($"opening {PathToRequest}");
        FileProtocol.Instance.OpenFolder(PathToRequest, RequestedBy);
    }

    public void RequestPathClose(string gatePath, GATEProtocol gATEProtocol)
    {
        if (Debugg) print($"closing {gatePath}");
        FileProtocol.Instance.CloseFolder(gatePath, gATEProtocol);
    }

    IEnumerator PathHandle()
    {
        ExtensionDictionary = new Dictionary<string, GameObject>();
        foreach (var ReadyExtPrefab in OtherPrefabs)
        {
            ExtensionDictionary[ReadyExtPrefab.Extension] = ReadyExtPrefab.Prefab;
        }

        Files = new List<ObjectFileInfo>();
        Directories = new List<GATEProtocol>();

        //Step 1 Gather Info
        //Is there a customization file? X
        //How many directories are there? X
        //How many files are there? X

        DirectoryInfo DirectoryInfos = new DirectoryInfo(Path);

        string PathToSave = Path + "/ufmsave.json";
        bool CustomizationFileExists = File.Exists(PathToSave);
        string Data = CustomizationFileExists ? File.ReadAllText(PathToSave) : string.Empty;
        CustomDirectoryInsideSave CustomizationFile = CustomizationFileExists ? JsonUtility.FromJson<CustomDirectoryInsideSave>(Data) : null;
        int DirectoryCount = DirectoryInfos.GetDirectories().Length;
        int FileCount = DirectoryInfos.GetFiles().Length;
        float SetupSize = (DirectoryCount + FileCount) / 2;


        if (Debugg) Debug.Log($"Directory Info:\nCustomization File Exists ? = {CustomizationFileExists}\nDirectories = {DirectoryCount}\nFiles = {FileCount}",gameObject);

        //Step 2 Setup ground
        //customized ? custom size : square
        //customized ? custom position+rotation : generated position + fixed rotation

        Transform Ground = transform.Find("Ground");

        GenerateToGround(FileCount, DirectoryCount, Ground);
        if (CustomizationFileExists)
            ApplyCustomizationToGround(CustomizationFile, Ground);

        //Step 3 Setup directories
        //is directory gate included in save file json?
        //Yes:
        //foreach line in directory.tags
        //string.split directory.tags('\n')
        //switch (tags[0])
        //case X: do thing break;

        Directories =
            GenerateDirectoryPortals(DirectoryInfos, SetupSize);

        if (CustomizationFileExists)
            ApplyCustomizationToDirectories(CustomizationFile, Directories);

        //Step 4 Setup files
        //Same thing as Setting up directories.

        Transform DirFiles = transform.Find("Files");
        Files = GenerateFileObjects(DirectoryInfos, SetupSize, DirFiles);

        if (CustomizationFileExists)
            ApplyFileCustomizationToFiles(CustomizationFile, Files);

        yield return new WaitForEndOfFrame();
    }

    private List<ObjectFileInfo> GenerateFileObjects(DirectoryInfo Directories, float SetupSize, Transform DirFiles)
    {
        List<ObjectFileInfo> FileTransforms = new List<ObjectFileInfo>();
        foreach (var InfoOfFiles in Directories.GetFiles())
        {
            string ExtensionOfFile = InfoOfFiles.Extension;
            float X = Random.Range(-SetupSize, SetupSize),
                  Y = Random.Range(-SetupSize, SetupSize),
                  Z = Random.Range(-SetupSize, SetupSize);
            Vector3 SetRndPos = transform.position + new Vector3(X, 0, Z);
            Quaternion SetRndRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, Y, 0));

            if (Debugg && ExtensionDictionary == null) Debug.LogError("Dictionary not found!");
            Transform FileObject = Instantiate(ExtensionDictionary.ContainsKey(ExtensionOfFile) ? ExtensionDictionary[ExtensionOfFile] : FailPrefab, DirFiles).transform;

            FileObject.gameObject.name = InfoOfFiles.Name;
            FileObject.position = SetRndPos;
            FileObject.rotation = SetRndRot;
            var F = FileObject.GetOrAddComponent<ObjectFileInfo>();
            F.Setup(InfoOfFiles.FullName);
            FileTransforms.Add(F);
        }

        return FileTransforms;
    }

    public void SaveAndDestroySelf()
    {
        CustomDirectoryInsideSave CDI_Save = CurrentSave != null ? CurrentSave : new CustomDirectoryInsideSave(); //If a tool has manipulated the ground. 
        // this will be created already.

        List<CustomDirectorySave> CDS = new List<CustomDirectorySave>();
        foreach (var Directory in Directories)
        {
            if (Directory.CDS != null) CDS.Add(Directory.CDS); 
        }
        CDI_Save.CustomDirectorySaves = CDS.ToArray();

        List<CustomFileSave> CFS = new List<CustomFileSave>();
        foreach (var File in Files)
        {
            if (File.CFS != null) CFS.Add(File.CFS); 
        }
        CDI_Save.CustomFileSaves = CFS.ToArray();

        string HOLYMOLYBIGASSLARGEDATA = JsonUtility.ToJson(CDI_Save);
        string SavePath = $"{Path}ufmsave.json";
        try 
        {File.WriteAllText(SavePath,HOLYMOLYBIGASSLARGEDATA);}
        catch (System.Exception Err) 
        {Debug.LogError(Err);/* Deez nuts */}

        Destroy(gameObject);
    }

    public void SetBGColor(Color applyColor)
    {
        BackgroundColor = applyColor;
                ColorBG = new Vector3(BackgroundColor.r,BackgroundColor.g,BackgroundColor.b);
    }

    public Color GetBGColor() { return BackgroundColor; }

    public void ApplyCustomizationToGround(CustomDirectoryInsideSave CDIS,Transform ApplyTo)
    {
        CurrentSave = CurrentSave ?? CDIS;
        foreach (var Tag in CDIS.GroundTags)
            {
                string[] Vars = Tag.Split();
                switch (Vars[0])
                {
                    case "Position":
                        Vector3 SetPos = new Vector3(float.Parse(Vars[1]),float.Parse(Vars[2]),float.Parse(Vars[3]));
                        ApplyTo.position = SetPos;
                    break;

                    case "Rotation":
                        Quaternion SetRot = Quaternion.Euler(new Vector3(float.Parse(Vars[1]),float.Parse(Vars[2]),float.Parse(Vars[3])));
                        ApplyTo.rotation = SetRot;
                    break;
                
                    case "Size":
                        Vector3 SetSize = new Vector3(float.Parse(Vars[1]),float.Parse(Vars[2]),float.Parse(Vars[3]));
                        ApplyTo.localScale = SetSize;
                    break;

                    case "Color":
                        Color SetColor = new Color(float.Parse(Vars[1]),float.Parse(Vars[2]),float.Parse(Vars[3]),float.Parse(Vars[4]));
                        ApplyTo.GetComponent<Renderer>().material.color = SetColor;
                    break;

                    case "BGColor":
                        Color SetColor_ = new Color(float.Parse(Vars[1]),float.Parse(Vars[2]),float.Parse(Vars[3]),float.Parse(Vars[4]));
                        SetBGColor(SetColor_);
                    break;
                }
            }
    }

    private static void ApplyFileCustomizationToFiles(CustomDirectoryInsideSave CustomizationFile, List<ObjectFileInfo> FileTransforms)
    {
        foreach (var FileSave in CustomizationFile.CustomFileSaves)
        {
            Transform FoundT = FileTransforms.Find(x => x.transform.name == FileSave.Name).transform;
            if (FoundT == null) continue;

            foreach (var Tag in FileSave.Tags)
            {
                string[] Vars = Tag.Split();
                switch (Vars[0])
                {
                    case "Position":
                        Vector3 SetPos = new Vector3(float.Parse(Vars[1]), float.Parse(Vars[2]), float.Parse(Vars[3]));
                        FoundT.position = SetPos;
                        break;

                    case "Rotation":
                        Quaternion SetRot = Quaternion.Euler(new Vector3(float.Parse(Vars[1]), float.Parse(Vars[2]), float.Parse(Vars[3])));
                        FoundT.rotation = SetRot;
                        break;

                    case "Size":
                        Vector3 SetSize = new Vector3(float.Parse(Vars[1]), float.Parse(Vars[2]), float.Parse(Vars[3]));
                        FoundT.localScale = SetSize;
                        break;

                    case "Color":
                        Color SetColor = new Color(float.Parse(Vars[1]), float.Parse(Vars[2]), float.Parse(Vars[3]), float.Parse(Vars[4]));
                        FoundT.GetComponent<Renderer>().material.color = SetColor;
                        break;
                    case "Model":
                        Vars[0] = string.Empty;
                        string Path = string.Join(" ",Vars);
                        Path = Path.Trim().Replace("'","");

                        Mesh mesh = FoundT.AddComponent<ModelFile>().GetMeshFromPath(Path);

                        if (FoundT.TryGetComponent(out BoxCollider B)) {Destroy(B); FoundT.AddComponent<MeshCollider>().convex = true;}
                        if (FoundT.TryGetComponent(out MeshFilter MF)) MF.mesh = mesh;
                        if (FoundT.TryGetComponent(out MeshCollider MC)) MC.sharedMesh = mesh;
                        break;
                    case "IMG:Open":
                        var IH = FoundT.GetComponent<ImageHandler>(); //It's impossible to not have this. if you encounter NullReferenceException, you manipulated the files. go fix them.
                        bool E = Vars[1] == "True";
                        IH.ImageEnabled = E;
                        break;
                    case "AF:Play":
                        var AH = FoundT.GetComponent<AudioHandler>();
                        bool P = Vars[1] == "True";
                        AH.enabled = P;
                        break;
                    case "Texture":
                        Vars[0] = string.Empty;
                        string IPath = string.Join(" ",Vars);
                        IPath = IPath.Trim().Replace("'","");

                        byte[] TxtData = File.ReadAllBytes(IPath);
                        Texture2D Txt = new Texture2D(2,2);
                        Txt.LoadImage(TxtData);
                        Txt.Apply();
                        Material ApplyMaterial = new Material(Shader.Find("Unlit/Texture"));
                        ApplyMaterial.mainTexture = Txt;

                        FoundT.GetComponent<Renderer>().material = ApplyMaterial;
                        break;
                }
            }
            FoundT.GetComponent<ObjectFileInfo>().LoadTags(FileSave.Tags.ToArray());
        }
    }

    void ApplyCustomizationToDirectories(CustomDirectoryInsideSave CustomizationFile, List<GATEProtocol> DirectoryTransforms)
    {
        foreach (var Save in CustomizationFile.CustomDirectorySaves)
        {
            Transform FoundT = DirectoryTransforms.Find(x => x.transform.name == Save.Name).transform;
            if (FoundT == null) continue;

            foreach (var Tag in Save.Tags)
            {
                string[] Vars = Tag.Split();
                switch (Vars[0])
                {
                    case "Position":
                        Vector3 SetPos = new Vector3(float.Parse(Vars[1]), float.Parse(Vars[2]), float.Parse(Vars[3]));
                        FoundT.position = SetPos;
                    break;

                    case "Rotation":
                        Quaternion SetRot = Quaternion.Euler(new Vector3(float.Parse(Vars[1]), float.Parse(Vars[2]), float.Parse(Vars[3])));
                        FoundT.rotation = SetRot;
                    break;

                    case "Size":
                        Vector3 SetSize = new Vector3(float.Parse(Vars[1]), float.Parse(Vars[2]), float.Parse(Vars[3]));
                        FoundT.localScale = SetSize;
                    break;

                    case "Color":
                        Color SetColor = new Color(float.Parse(Vars[1]), float.Parse(Vars[2]), float.Parse(Vars[3]), float.Parse(Vars[4]));
                        FoundT.GetComponent<GATEProtocol>().ApplyCustomColor(SetColor);
                    break;
                }
            }
            FoundT.GetComponent<GATEProtocol>().LoadTags(Save.Tags.ToArray());
        }
    }

    List<GATEProtocol> GenerateDirectoryPortals(DirectoryInfo Directories, float SetupSize)
    {
        Transform DirTransform = transform.Find("Directories");
        List<GATEProtocol> DirectoryTransforms = new List<GATEProtocol>();
        foreach (var InfoOfDirectory in Directories.GetDirectories())
        {
            Random.InitState(InfoOfDirectory.FullName.GetHashCode());
            Transform Portal = Instantiate(GatePrefab, DirTransform).transform;
            float X = Random.Range(-SetupSize, SetupSize),
                  Y = Random.Range(-SetupSize, SetupSize),
                  Z = Random.Range(-SetupSize, SetupSize);
            Vector3 SetRndPos = transform.position + new Vector3(X, 0, Z);
            Quaternion SetRndRot = Quaternion.Euler(transform.eulerAngles + new Vector3(0, Y, 0));

            Portal.gameObject.name = InfoOfDirectory.Name;
            Portal.position = SetRndPos;
            Portal.rotation = SetRndRot;
            DirectoryTransforms.Add(Portal.GetComponent<GATEProtocol>());
        }

        return DirectoryTransforms;
    }

    public void GenerateToGround(int FileCount,int DirCount, Transform ApplyTo)
    {
            int GroundLength = DirCount + FileCount;
            Vector3 GroundSize = Vector3.one * GroundLength;

            ApplyTo.localScale = GroundSize;
            ApplyTo.localPosition = Vector3.down * GroundLength / 2;
    }

    [Serializable]
    public class Extensions
    {
        public string Extension;
        public GameObject Prefab;
    }

    [System.Serializable]
    public class CustomDirectoryInsideSave
    {
        public List<string> GroundTags; //Position X Y Z
                              //Rotation X Y Z
                              //Color <RED 0-255> <GREEN 0-255> <BLUE 0-255>
                              //Material <PATH>

        public CustomDirectorySave[] CustomDirectorySaves;
        public CustomFileSave[] CustomFileSaves; //This only includes Customized files. Others generate random according to seed.
    }

    [System.Serializable]
    public class CustomDirectorySave
    {
        public string Name;
        public List<string> Tags;

        //Same as below.
    }

    [System.Serializable]
    public class CustomFileSave
    {
        public string Name; //DisplayName for convinience.
        public List<string> Tags; //Position X Y Z
                              //Rotation X Y Z
                              //Color <RED 0-255> <GREEN 0-255> <BLUE 0-255>
                              //Material <PATH>
                              //Model <PATH>
    }

    public void AddTag(string Tag,string[] Vars)
    {
        CurrentSave = CurrentSave ?? new CustomDirectoryInsideSave();
        CurrentSave.GroundTags = CurrentSave.GroundTags ?? new List<string>();
        string T = Tag + ' ' + string.Join(' ',Vars);
        
        bool Exists = false;
        for (int i = 0; i < CurrentSave.GroundTags.Count; i++)
        {
            string[] Str = CurrentSave.GroundTags[i].Split(' ');
            if (Tag == Str[0])
            {
                CurrentSave.GroundTags[i] = T;
                Exists = true;
                break;
            } 
            else continue;
        }
        if (!Exists) CurrentSave.GroundTags.Add(T);
    }
}

/*03.03.2026 Note:
    Now that i take a look at this code,
    Its shit xd

    but it can be better.
*/