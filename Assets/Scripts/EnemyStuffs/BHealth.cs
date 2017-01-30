using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class BHealth : MonoBehaviour {

    public int health;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            //Destroy(gameObject);
            SceneManager.LoadScene("WinScreen");
        }
    }
}
