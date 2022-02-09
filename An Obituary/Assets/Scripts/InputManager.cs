using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    //Used for singleton
    public static InputManager Instance { get; private set; }
 
    //Create Keycodes that will be associated with each of our commands.
    //These can be accessed by any other script in our game
    private KeyCode jumpKey;
    private KeyCode leftKey;
    private KeyCode rightKey;
    private KeyCode sprintKey;

    private enum PressType {
        GetDown,
        GetUp,
        Get
    }

    private bool cacheJump;
    private bool cacheLeft;
    private bool cacheRight;
    private bool cacheSprint;
 
    void Awake() // TODO if you change a key while InputManager exists it won't change the key in the InputManager
    {
        //Singleton pattern
        if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
        /*Assign each keycode when the game starts.
         * Loads data from PlayerPrefs so if a user quits the game,
         * their bindings are loaded next time. Default values
         * are assigned to each Keycode via the second parameter
         * of the GetString() function
         */
        leftKey = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("leftKey", "A"));
        rightKey = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("rightKey", "D"));
        jumpKey = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("jumpKey", "Space"));
        sprintKey = (KeyCode) System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("sprintKey", "LeftShift"));
        Debug.Log(sprintKey);
    }

    public int GetHorizontal() {
        bool right = GetRight();
        bool left = GetLeft();
        if(right != left) {
            if (right) {
                return 1;
            } else if (left) {
                return -1;
            }
        }
        return 0;
    }

    /*
     * custom Input for left key based on user keybind choice.
     * type - a negative number for getKeyDown, a positive number for getKeyUp, 0 for getKey.
     */
    public bool GetLeft() {
        return parseKey(leftKey, PressType.Get);
    }

    /*
     * custom Input for right key based on user keybind choice.
     * type - a negative number for getKeyDown, a positive number for getKeyUp, 0 for getKey.
     */
    public bool GetRight() {
        return parseKey(rightKey, PressType.Get);
    }

    /*
     * custom Input for jump key based on user keybind choice.
     * type - a negative number for getKeyDown, a positive number for getKeyUp, 0 for getKey.
     */
    public bool GetJumpDown() {
        return parseKey(jumpKey, PressType.GetDown);
    }

    public bool GetJumpUp() {
        return parseKey(jumpKey, PressType.GetUp);
    }

    /*
     * custom Input for slide key based on user keybind choice.
     * type - a negative number for getKeyDown, a positive number for getKeyUp, 0 for getKey.
     */
    public bool GetSprint() {
        return parseKey(sprintKey, PressType.Get);
    }

    private bool parseKey(KeyCode key, PressType type) {
        switch(type) {
            case PressType.Get:
                return Input.GetKey(key);
            case PressType.GetDown:
                return Input.GetKeyDown(key);
            case PressType.GetUp:
                return Input.GetKeyUp(key);
        }
        return false;
    }
}
