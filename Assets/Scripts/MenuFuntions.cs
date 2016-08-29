using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuFuntions : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ChangeScene(int level)
    {
        if(level == 1)
        {
            SceneManager.LoadScene("test_move");
        } else if (level == 2)
        {
            SceneManager.LoadScene("test_scene");
        } else if (level == 3)
        {
            SceneManager.LoadScene("basicTouch");
        }
    }
}
