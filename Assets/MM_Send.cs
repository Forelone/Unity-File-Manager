using System.Collections;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MM_Send : MonoBehaviour
{
    InputField IF;
    Text T;

    void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);
        IF = GetComponent<InputField>();
        IF.onSubmit.AddListener(LoadScene);
    }

    public void LoadLastPath()
    {
        string Path = PlayerPrefs.GetString("LastPath");
        LoadScene(Path);
    }

    void LoadScene(string Path) 
    {
        if (Directory.Exists(Path))
        {
            PlayerPrefs.SetString("LastPath",Path);
            StartCoroutine(LoadSceneHandle(Path));
        }
        else
        {
            IF.text = "<color=red>This path does not exist!</color>";
        }
    }

    IEnumerator LoadSceneHandle(string Path)
    {
        if (Path[Path.Length - 1] != '/') Path += '/';

        SceneManager.LoadScene("World");

        while (SceneManager.GetActiveScene().name != "World" && !SceneManager.GetActiveScene().isLoaded) yield return new WaitForFixedUpdate();

        GameObject FM = null;
        while (FM == null)
        {
            FM = GameObject.FindGameObjectWithTag("GameController");
            if (FM == null) {Debug.LogError("FUCK!");}
            yield return new WaitForFixedUpdate();
        }

        FileProtocol FP = FM.GetComponent<FileProtocol>();
        FP.StartPath = Path;
        FP.enabled = true;
        Destroy(transform.root.gameObject);
    }
}   
