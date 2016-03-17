using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class UniverseInterfacer : MonoBehaviour {

    public void LoadScene(int SceneNumber)
    {
        SceneManager.LoadSceneAsync(SceneNumber);
    }
}
