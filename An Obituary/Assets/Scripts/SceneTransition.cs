using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public string sceneToLoad;
    public int spawnIndex;
    public bool isEnteringRight;
    public float exitDirection;
    public SceneTransitionData sceneTransitionData;

    protected PlayerController controller;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            controller = other.GetComponent<PlayerController>();
            if (!controller.ignoreSceneTransition)
            {
                if (isEnteringRight)
                {
                    controller.StartForceMovement(1);
                }
                else
                {
                    controller.StartForceMovement(-1);
                }
                TimerManager.Instance.SetTimer(0.4f, TransitionScene);
            }
        }
    }

    public void TransitionScene()
    {
        sceneTransitionData.spawnIndex = spawnIndex;
        sceneTransitionData.direction = exitDirection;
        SceneManager.LoadScene(sceneToLoad);
    }
}
