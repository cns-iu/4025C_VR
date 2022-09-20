using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ButtonPress : MonoBehaviour
{
    public XRController leftHand;
    public InputHelpers.Button button;

    void Update()
    {
        
        bool pressed;
        leftHand.inputDevice.IsPressed(button, out pressed);

        if (pressed)
        {
            Debug.Log("Hello - " + button);
        }
    }
}