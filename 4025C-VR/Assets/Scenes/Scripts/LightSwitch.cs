using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class LightSwitch : MonoBehaviour
{
    //private XRController controller = null;
    //private GameObject leftHandController = null;
    GameObject lightAssembly;
    Light spotLight;
    AudioSource soundClip;
   

   private void Start()
     {
        //controller = leftHandController.GetComponent<XRController>();

        lightAssembly = GameObject.Find("SpotLightHanger");
        spotLight = lightAssembly.GetComponent<Light>();

        soundClip = GetComponent<AudioSource>();

     }

    /*


     private void Update()
     {

         CommonInput();

     }

     private void CommonInput()
     {
         //Grip press

        if (controller.inputDevice.TryGetFeatureValue(CommonUsages.gripButton, out bool grip))
             spotLight.enabled = !spotLight.enabled;


     }
    */

    public void toggleLightSwitch()
    {
        //controller = leftHandController.GetComponent<XRController>();
       

        spotLight.enabled = !spotLight.enabled;

        soundClip.Play();
    }

}


