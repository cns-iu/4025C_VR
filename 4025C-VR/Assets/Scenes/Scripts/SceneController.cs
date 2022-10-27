using System.Collections;
using System.Collections.Generic;
using OculusSampleFramework;
//using System.Numerics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// 2022-10-27


public class SceneController : MonoBehaviour
{
    public GameObject xrPosition;
    public GameObject jumpTargetAssembly;
    public GameObject jumpTargetMain;
    public GameObject jumpTargetTest;
    public GameObject transportTarget;

    public ConController controllerScript;  //access to ConnectorController
    public ManifestStatus manStatus;

    public List<GameObject> manifestList;        // all manifests

    public GameObject testManifest;

    const int bDefault = 0;
    const int bShow = 1;
    const int bSelected = 2;
    const int bConnected = 4;
    const int bLibrary = 8;
    const int bInitIgnore = 16;

    // for preference saving tests
    public int testInt = 0;
    public VisibilityToggle visibilityToggle;
    public DMMMover dmmMover;



    public void soundTrigger(GameObject c)
    {
        c.GetComponent<AudioSource>().Play();
        Debug.Log("beam sound should be here");
    }

    public void jumpToAssembly(GameObject c)
    {     
        xrPosition.transform.position = jumpTargetAssembly.transform.position;
        jumpTargetAssembly.GetComponent<TransportSwitch>().thisTarget.SetActive(false);
        jumpTargetMain.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetTest.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetAssembly.GetComponent<AudioSource>().Play();

    }

    public void jumpToTest(GameObject c)
    {
        xrPosition.transform.position = jumpTargetTest.transform.position;
        jumpTargetAssembly.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetMain.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetTest.GetComponent<TransportSwitch>().thisTarget.SetActive(false);
        jumpTargetTest.GetComponent<AudioSource>().Play();
    }

    public void jumpToMain(GameObject c)
    {
        xrPosition.transform.position = jumpTargetMain.transform.position;
        jumpTargetAssembly.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetMain.GetComponent<TransportSwitch>().thisTarget.SetActive(false);
        jumpTargetTest.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetMain.GetComponent<AudioSource>().Play();


        if (controllerScript.manifest.GetComponent<ManifestStatus>().conList.Count != 1)
        {
            //GameObject manifestOriginal = controllerScript.manifest;
            controllerScript.ConListInitIgnoreStatusSet(controllerScript.manifest); // prevents instantiated nodes from being added to conList
            GameObject manifestCopy = Instantiate(controllerScript.manifest);
            manifestList.Add(manifestCopy);         // store in global manifest list

            // clear original manifest and add default node
            controllerScript.manifest.GetComponent<ManifestStatus>().conList.Clear();

            foreach (Transform child in controllerScript.manifest.transform)
            {
                GameObject croot = child.GetChild(0).gameObject;
                if (croot.name == "croot")
                {
                    controllerScript.manifest.GetComponent<ManifestStatus>().conList.Add(croot);    //add to conList
                    controllerScript.ConStatusSet(croot, 0, controllerScript.matDefault);   // reset new croot
                    croot.GetComponent<ConStatus>().initIgnore = false;
                }
                else
                {
                    controllerScript.PDestroy(child.gameObject);
                }
            }
            // check if user made something
            if (manifestCopy != null) AssemblyPackage(manifestCopy);
        }
    }


    void AssemblyPackage(GameObject m)
    {
        FitToChildren(m);
        Rigidbody rb = m.AddComponent<Rigidbody>() as Rigidbody;

        // de-activate all nodes
        foreach (GameObject c in m.GetComponent<ManifestStatus>().conList)
        {
            c.SetActive(false);
        }

        m.AddComponent<XRGrabInteractable>();

        m.transform.position = transportTarget.transform.position;
        Vector3 objectScale = m.transform.localScale;
        m.transform.localScale = new Vector3(objectScale.x / 10, objectScale.y / 10, objectScale.z / 10);

    }


    // add boxcollider and size it to enclose all children
    void FitToChildren(GameObject m)
    {
        BoxCollider bc = m.AddComponent<BoxCollider>() as BoxCollider;
        if (m.GetComponent<Collider>() is BoxCollider)
        {

            bool hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int i = 0; i < m.transform.childCount; ++i)
            {
                Renderer childRenderer = m.transform.GetChild(i).GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    if (hasBounds)
                    {
                        bounds.Encapsulate(childRenderer.bounds);
                    }
                    else
                    {
                        bounds = childRenderer.bounds;
                        hasBounds = true;
                    }
                }
            }

            BoxCollider collider = (BoxCollider)m.GetComponent<Collider>();
            collider.center = bounds.center - m.transform.position;
            collider.size = bounds.size;
            //Debug.Log("bounds " + bounds.size);
        }
    }



    public void jumpHover(GameObject c)
    {
        //Debug.Log("jumpHover");
    }


    public void jumpHoverExited(GameObject c)
    {
        //Debug.Log("jumpHoverExited");
    }


    // Start is called before the first frame update
    void Start()
    {

        LoadPrefs();    // only seems to work from Start()
        // build and packacke test assembly
        AssemblyPackage(testManifest);
        jumpTargetAssembly.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetMain.GetComponent<TransportSwitch>().thisTarget.SetActive(false);
        jumpTargetTest.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        GameObject.Find("ToAssembly").GetComponent<AudioSource>().Play();
    }


    void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            SavePrefs();
        }
    }

    /*
    not working on Oculus
    private void OnApplicationQuit()
    {
        SavePrefs();
    }
    */


    public void SavePrefs()
    {
        testInt ++;

        Debug.Log("saving prefs:" + testInt + " console: " + visibilityToggle.consoleVisibility);

        PlayerPrefs.SetInt("TestInt", testInt);
        PlayerPrefs.SetInt("consoleVisibility", visibilityToggle.consoleVisibility);
        PlayerPrefs.SetFloat("distance", dmmMover.distance);
        PlayerPrefs.Save();
    }


    public void LoadPrefs()
    {
        
        testInt = PlayerPrefs.GetInt("TestInt", 0);
        visibilityToggle.consoleVisibility = PlayerPrefs.GetInt("consoleVisibility", 0);
        dmmMover.distance = PlayerPrefs.GetFloat("distance", 1f);

        visibilityToggle.ToggleConsole();

        Debug.Log("loading prefs: " + testInt + " console: " + visibilityToggle.consoleVisibility);

    }
}
