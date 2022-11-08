using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using OculusSampleFramework;
using OVRSimpleJSON;
//using System.Numerics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using static JsonTests;

// 2022-11-7


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

    //public GameObject testManifest;
    public GameObject loadNode;             // loading objects deposited to this manifest
    GameObject saveManifest;

    const int bDefault = 0;
    const int bShow = 1;
    const int bSelected = 2;
    const int bConnected = 4;
    const int bLibrary = 8;
    const int bInitIgnore = 16;

    // for preference saving tests
    public int testInt = 0;     // counts number of prefs save/load from last reset
    public VisibilityToggle visibilityToggle;
    public DMMMover dmmMover;
    public string fileName = "SaveState01.txt";

    //public JsonTests jsonTests;

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

        // de-activate all nodes; can this be reversible?
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
        //SaveData();
        // build and packacke test assembly
        //AssemblyPackage(testManifest);
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

        Debug.Log("saving prefs:" + testInt + " console: " +
            visibilityToggle.consoleVisibility + " DMM: " +
            dmmMover.distance);

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
        dmmMover.Layout();

        Debug.Log("loading prefs: " + testInt + " console: " +
            visibilityToggle.consoleVisibility + " DMM: " +
            dmmMover.distance);
    }



    public void OnKeyboard(InputValue v)
    {
        //float c = v.GetType
        Debug.Log("keyboard button: " + v.Get<float>());
    }

    // what a friggin crutch!
    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard.sKey.wasPressedThisFrame) SaveData();
        if (keyboard.lKey.wasPressedThisFrame) LoadData();
        if (keyboard.tKey.wasPressedThisFrame) SomeTest();

    }

    void SomeTest()
    {
        Debug.Log("SystemInfo: " + SystemInfo.deviceType);
         
    }


    public void SaveData()
    {
        saveManifest = controllerScript.manifest;
        //controllerScript.connections;


        // switch manifest when running on desktop computer
        if (SystemInfo.deviceType == DeviceType.Desktop) saveManifest = controllerScript.testManifest;

        // encode parent list
        foreach (GameObject g in saveManifest.GetComponent<ManifestStatus>().parentList)
        {
            GetComponent<JsonTests>().parents.Add(g.GetComponent<ParentData>().parentType);
        }

        //foreach (GameObject g in controllerScript.connections.Keys)
        foreach (KeyValuePair<GameObject, GameObject> entry in controllerScript.connections)
        {
            Debug.Log("entry key: " + entry.Key.name);
            Debug.Log("entry value: " + entry.Value.name);

        }
        /*
        // encode conList
        Debug.Log("conList count: " + saveManifest.GetComponent<ManifestStatus>().conList.Count);
        foreach (GameObject g in saveManifest.GetComponent<ManifestStatus>().conList)
        {
            JsonTests.cListEntry listEntry = new cListEntry();
            //listEntry.parentID = g.GetComponent<ConStatus>().???;
            listEntry.parentID = 0;
            listEntry.conName = g.name;
            listEntry.conThat = -1;
            listEntry.conActive = false;
            GetComponent<JsonTests>().cList.Add(listEntry);
        }*/
  

        string wtf = GetComponent<JsonTests>().SaveToString();
        WriteToFile(fileName, wtf);
        Debug.Log("saving data to file: " + fileName + "->" + wtf);
    }

   

    public void LoadData()
    {
        

        LoadFromFile(fileName, out var wtf);
        Debug.Log("json loaded: " + wtf);

        GetComponent<JsonTests>().LoadFromJson(wtf);
        //Debug.Log("starModel: " + GetComponent<JsonTests>().starModel);

        foreach (string s in GetComponent<JsonTests>().parents)
        {
            GameObject g = FindInLibrary(s);
            Debug.Log("found string: " + g.name);
        }

        /*
        GameObject d = null;
        foreach (Transform t in controllerScript.library.transform)
        {
        
            if (t.gameObject.GetComponent<ParentData>().parentType == GetComponent<JsonTests>().starModel)
            {
                d = t.gameObject;
                Debug.Log("found object in library");   //this is no matching?
            }
            
        }
        */

        /*
        if (d != null)
        {
            GameObject p = Instantiate(d);
      
            p.transform.parent = loadNode.transform;
            p.transform.localPosition = new Vector3(0, 1.5f, 0);
            Debug.Log("instantiated: " + p.name);
        }
        */
    }


    GameObject FindInLibrary(string pType)
    {
        GameObject p = null;

        foreach (Transform t in controllerScript.library.transform)
        {
            if (t.gameObject.GetComponent<ParentData>().parentType == pType)
            {
                p = t.gameObject;
                Debug.Log("found object in library");   //this is no matching?
            }
        }
        return p;
    }

   

    public static bool WriteToFile(string a_FileName, string a_FileContents)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            File.WriteAllText(fullPath, a_FileContents);
            Debug.Log("writing to: " + fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write to {fullPath} with exception {e}");
            return false;
        }
    }

    public static bool LoadFromFile(string a_FileName, out string result)
    {
        var fullPath = Path.Combine(Application.persistentDataPath, a_FileName);

        try
        {
            result = File.ReadAllText(fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read from {fullPath} with exception {e}");
            result = "";
            return false;
        }
    }
}
