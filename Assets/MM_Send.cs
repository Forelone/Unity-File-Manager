using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MM_Send : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(transform.root.gameObject);
        InputField IF = GetComponent<InputField>();
        IF.onSubmit.AddListener(LoadScene);
    }

    void LoadScene(string Path) => StartCoroutine(LoadSceneHandle(Path));

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
