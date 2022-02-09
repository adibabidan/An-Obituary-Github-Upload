using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TextScreen : MonoBehaviour
{
    public GameObject[] textBoxObjs;
    public SceneTransitionData sceneTransitionData;
    public string scene;

    protected int slide;

    // Start is called before the first frame update
    void Start()
    {
        slide = 0;
        textBoxObjs[slide].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            textBoxObjs[slide].SetActive(false);
            slide += 1;
            if (slide == textBoxObjs.Length)
            {
                sceneTransitionData.spawnIndex = 0;
                sceneTransitionData.direction = 0;
                SceneManager.LoadScene(scene);
            }
            else
            {
                textBoxObjs[slide].SetActive(true);
            }
        }
    }
}
