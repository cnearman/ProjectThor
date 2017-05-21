using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Healths : MonoBehaviour {

    public int health;

    public Image pip1;
    public Image pip2;
    public Image pip3;
    public Image pip4;
    public Image pip5;

    // Update is called once per frame
    void Update () {
        if(health == 5)
        {
            pip1.color = Color.red;
            pip2.color = Color.red;
            pip3.color = Color.red;
            pip4.color = Color.red;
            pip5.color = Color.red;

        } else if(health == 4)
        {
            pip1.color = Color.red;
            pip2.color = Color.red;
            pip3.color = Color.red;
            pip4.color = Color.red;
            pip5.color = Color.white;
        }
        else if (health == 3)
        {
            pip1.color = Color.red;
            pip2.color = Color.red;
            pip3.color = Color.red;
            pip4.color = Color.white;
            pip5.color = Color.white;
        }
        else if (health == 2)
        {
            pip1.color = Color.red;
            pip2.color = Color.red;
            pip3.color = Color.white;
            pip4.color = Color.white;
            pip5.color = Color.white;
        }
        else if (health == 1)
        {
            pip1.color = Color.red;
            pip2.color = Color.white;
            pip3.color = Color.white;
            pip4.color = Color.white;
            pip5.color = Color.white;
        }
        else if (health <= 0)
        {
            pip1.color = Color.white;
            pip2.color = Color.white;
            pip3.color = Color.white;
            pip4.color = Color.white;
            pip5.color = Color.white;

            SceneManager.LoadScene("FailScreen");
        }
    }
}
