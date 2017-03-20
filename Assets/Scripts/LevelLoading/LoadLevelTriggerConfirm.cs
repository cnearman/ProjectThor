using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LoadLevelTriggerConfirm : MonoBehaviour {

    public string sceneToLoad;

    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("enderting");
            //other.gameObject.GetComponent<Assets.Scripts.Player>().PromptLevel(sceneToLoad);
        }
    }
}
