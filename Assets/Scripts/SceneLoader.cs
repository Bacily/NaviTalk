using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneLoader : MonoBehaviour
{

    public void sceneLoader(int sceneIndex) { 
        
        SceneManager.LoadScene(sceneIndex);
    
    }



}
