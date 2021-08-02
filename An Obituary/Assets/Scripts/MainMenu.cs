using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{    
    public void onPressPlay ()
    {
        SceneManager.LoadScene("Scene 0");
    }

    public void onPressQuit ()
    {
        Application.Quit();
    }
}
