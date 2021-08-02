using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextPopup : MonoBehaviour
{
    public GameObject interactNotifObj;
    public GameObject[] textBoxObjs;

    protected bool interactNotifOn;
    protected int textBoxSlide;
    protected bool inTrigger;

    // Start is called before the first frame update
    void Start()
    {
        inTrigger = false;
        interactNotifOn = false;
        textBoxSlide = -1;
        interactNotifObj.SetActive(interactNotifOn);
        for(int i = 0; i < textBoxObjs.Length; i++)
        {
            textBoxObjs[i].SetActive(false);
        }
    }

    // Update is called once per frame
    public void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            inTrigger = true;
            interactNotifOn = true;
            interactNotifObj.SetActive(interactNotifOn);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if(other.CompareTag("Player") && !other.isTrigger)
        {
            inTrigger = false;
            interactNotifOn = false;
            interactNotifObj.SetActive(interactNotifOn);
            textBoxSlide = -1;
            for(int i = 0; i < textBoxObjs.Length; i++)
            {
                textBoxObjs[i].SetActive(false);
            }
        }
    }

    void Update()
    {
        if (inTrigger)
        {
            if(Input.GetButtonDown("Interact"))
            {
                if (textBoxSlide >= 0)
                {
                    textBoxObjs[textBoxSlide].SetActive(false);
                }
                else
                {
                    interactNotifOn = false;
                    interactNotifObj.SetActive(interactNotifOn);
                    Time.timeScale = 0.0f;
                }
                textBoxSlide += 1;
                if (textBoxSlide == textBoxObjs.Length)
                {
                    interactNotifOn = true;
                    interactNotifObj.SetActive(interactNotifOn);
                    textBoxSlide = -1;
                    Time.timeScale = 1.0f;
                }
                else
                {
                    textBoxObjs[textBoxSlide].SetActive(true);
                }
            }
        }
    }
}
