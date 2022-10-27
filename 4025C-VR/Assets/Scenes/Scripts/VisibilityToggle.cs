using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

// 2022-10-27

public class VisibilityToggle : MonoBehaviour
{
    public InputActionReference toggleReference = null;
    public int consoleVisibility = 0;
    //public SceneController sceneController;


    private void Awake()
    {
        toggleReference.action.started += Toggle;
        

    }

    private void Start()
    {
        //hide or show HUD
      
        bool isActive = !gameObject.activeSelf;
        gameObject.SetActive(isActive);
    
   
    }


    private void OnDestroy()
    {
        toggleReference.action.started -= Toggle;
    }

    public void Toggle(InputAction.CallbackContext context)
    {
        if (consoleVisibility == 0)
        {
            gameObject.SetActive(true);
            consoleVisibility = 1;
        }
        else
        {
            gameObject.SetActive(false);
            consoleVisibility = 0;
        }
    }


    public void ToggleConsole()
    {
        if (consoleVisibility == 0)
        {
            gameObject.SetActive(false);
        } else
        {
            gameObject.SetActive(true);
        }
    }
}
