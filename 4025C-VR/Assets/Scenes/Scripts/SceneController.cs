using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using static JsonTests;

// 2022-11-16

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
    public GameObject loadManifest;     // loading objects deposited to this manifest
    GameObject saveManifest;
    //GameObject tempManifest;            // to store manifest when loading

    /*
    const int bDefault = 0;
    const int bShow = 1;
    const int bSelected = 2;
    const int bConnected = 4;
    const int bLibrary = 8;
    const int bInitIgnore = 16;
    */

    public AudioSource beamingSound;
    public AudioSource enterAssembly;
    public AudioSource enterMain;
    public AudioSource enter51;

    // for preference saving tests
    public int testInt = 0;     // counts number of prefs save/load from last reset
    public VisibilityToggle visibilityToggle;
    public DMMMover dmmMover;
    public string fileName = "SaveState01.txt";


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
        beamingSound.Play();
        enterAssembly.Play();
    }

    public void jumpToTest(GameObject c)
    {
        xrPosition.transform.position = jumpTargetTest.transform.position;
        jumpTargetAssembly.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetMain.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetTest.GetComponent<TransportSwitch>().thisTarget.SetActive(false);
        beamingSound.Play();
        enter51.Play();
    }

    public void jumpToMain(GameObject c)
    {
        xrPosition.transform.position = jumpTargetMain.transform.position;
        jumpTargetAssembly.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetMain.GetComponent<TransportSwitch>().thisTarget.SetActive(false);
        jumpTargetTest.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        beamingSound.Play();
        enterMain.Play();

        if (controllerScript.manifest != null) AssemblyPackage(controllerScript.manifest);
        /*
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
        */
    }


    void AssemblyPackage(GameObject m)
    {
        GameObject manifestCopy = new GameObject();

        if (m.GetComponent<ManifestStatus>().conList.Count != 1)
        {
            controllerScript.ConListInitIgnoreStatusSet(m); // prevents instantiated nodes from being added to conList
            manifestCopy = Instantiate(m);
            manifestList.Add(manifestCopy);         // store in global manifest list

            m.GetComponent<ManifestStatus>().conList.Clear();

            foreach (Transform child in m.transform)
            {
                GameObject croot = child.GetChild(0).gameObject;
                if (croot.name == "croot")
                {
                    m.GetComponent<ManifestStatus>().conList.Add(croot);    //add to conList
                    controllerScript.ConStatusSet(croot, 0, controllerScript.matDefault);   // reset new croot
                    croot.GetComponent<ConStatus>().initIgnore = false;
                }
                else
                {
                    controllerScript.PDestroy(child.gameObject);
                }
            }      
        }

        FitToChildren(manifestCopy);
        Rigidbody rb = manifestCopy.AddComponent<Rigidbody>() as Rigidbody;

        // de-activate all nodes; can this be reversible?
        /*foreach (GameObject c in m.GetComponent<ManifestStatus>().conList)
        {
            c.SetActive(false);
        }*/

        manifestCopy.AddComponent<XRGrabInteractable>();

        manifestCopy.transform.position = transportTarget.transform.position;
        Vector3 objectScale = manifestCopy.transform.localScale;
        manifestCopy.transform.localScale = new Vector3(objectScale.x / 10, objectScale.y / 10, objectScale.z / 10);
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



    public void SaveData()
    {
        saveManifest = controllerScript.manifest;   // for testing between VR and computer
        // switch manifest when running on desktop computer
        if (SystemInfo.deviceType == DeviceType.Desktop) saveManifest = controllerScript.testManifest;
        
        // encode parent list
        foreach (GameObject g in saveManifest.GetComponent<ManifestStatus>().parentList)
        {
            GetComponent<JsonTests>().parents.Add(g.GetComponent<ParentData>().parentType);
        }     

        // encode connections dictionary
        foreach (KeyValuePair<GameObject, GameObject> entry in saveManifest.GetComponent<ManifestStatus>().connections)
        {
            // key = from, value = to
            GetComponent<JsonTests>().connectionsList.Add(ConnectionEncode(entry.Key, entry.Value));
        }

        string wtf = GetComponent<JsonTests>().SaveToString();
        WriteToFile(fileName, wtf);
        Debug.Log("saving data to file: " + fileName + "->" + wtf);
    }



    // return from child, from parent, to child, to parent IDs
    // parameter is a connector node
    public JsonTests.connectionEntry ConnectionEncode(GameObject from, GameObject to)
    {    
        int fromChild = from.transform.GetSiblingIndex();
        int fromParent = IndexInParentList(from.transform.parent.gameObject);
        int toChild = to.transform.GetSiblingIndex();
        int toParent = IndexInParentList(to.transform.parent.gameObject);

        connectionEntry listEntry = new connectionEntry(fromChild, fromParent, toChild, toParent);
  
        return listEntry;
    }



    // return index of this gameobject in parent list
    public int IndexInParentList(GameObject g)
    {
        int parentID = -1;

        for (int i = 0; i < saveManifest.GetComponent<ManifestStatus>().parentList.Count; i++)
        {
            if (saveManifest.GetComponent<ManifestStatus>().parentList[i] == g)
            {
                parentID = i;
                break;
            }
        }
        return parentID;
    }



    public void LoadData()
    {
        LoadFromFile(fileName, out var wtf);
        Debug.Log("json loaded: " + wtf);

        GetComponent<JsonTests>().LoadFromJson(wtf);

        foreach (string typeID in GetComponent<JsonTests>().parents)
        {     
            if (typeID != "root")    // skip root - it's already in LoadManifest
            {      
                GameObject p = LibraryItemCloner(typeID, loadManifest);     // clone item from library by typeID string
                p.transform.parent = loadManifest.transform;
                loadManifest.GetComponent<ManifestStatus>().parentList.Add(p);
            }
            else
            {
                // add root to parentList
                loadManifest.GetComponent<ManifestStatus>().parentList.Add(loadManifest.transform.GetChild(0).gameObject);
            }
        }

        foreach (connectionEntry listEntry in GetComponent<JsonTests>().connectionsList)
        {         
            GameObject p = loadManifest.GetComponent<ManifestStatus>().parentList[listEntry.fromParent];    // fromParent (source parent)
            GameObject fromC = p.transform.GetChild(listEntry.fromChild).gameObject;
            GameObject d = loadManifest.GetComponent<ManifestStatus>().parentList[listEntry.toParent];
            GameObject toC = d.transform.GetChild(listEntry.toChild).gameObject;

            GameObject cTaxi = new GameObject();
            Transform manifestT = p.transform.parent;          // save parent (manifest) parent to move
            cTaxi.transform.position = fromC.transform.position;
            cTaxi.transform.rotation = fromC.transform.rotation;
            p.transform.parent = cTaxi.transform;    // move complete object into taxi
            cTaxi.transform.position = toC.transform.position;
            cTaxi.transform.rotation = toC.transform.rotation;
            p.transform.parent = manifestT;     // put object back into manifest
            Destroy(cTaxi);
   
            loadManifest.GetComponent<ManifestStatus>().connections.Add(fromC, toC);    // build connections dict.
        }

        AssemblyPackage(loadManifest);

        /*
        if (loadManifest.GetComponent<ManifestStatus>().conList.Count != 1)
        {
            controllerScript.ConListInitIgnoreStatusSet(loadManifest); // prevents instantiated nodes from being added to conList
            GameObject manifestCopy = Instantiate(loadManifest);
            manifestList.Add(manifestCopy);         // store in global manifest list

            loadManifest.GetComponent<ManifestStatus>().conList.Clear();

            foreach (Transform child in loadManifest.transform)
            {
                GameObject croot = child.GetChild(0).gameObject;
                if (croot.name == "croot")
                {
                    loadManifest.GetComponent<ManifestStatus>().conList.Add(croot);    //add to conList
                    controllerScript.ConStatusSet(croot, 0, controllerScript.matDefault);   // reset new croot
                    croot.GetComponent<ConStatus>().initIgnore = false;
                }
                else
                {
                    controllerScript.PDestroy(child.gameObject);
                }
            }   
            if (manifestCopy != null) AssemblyPackage(manifestCopy);
        }
        */
    }


    GameObject LibraryItemCloner(string s, GameObject m)
    {
        GameObject op = FindInLibrary(s);

        GameObject tempManifest = controllerScript.manifest;
        controllerScript.manifest = loadManifest;
        GameObject p = Instantiate(op);
        //loadManifest = controllerScript.manifest;
        controllerScript.manifest = tempManifest;
        return p;
    }


    GameObject FindInLibrary(string pType)
    {
        GameObject p = null;

        foreach (Transform t in controllerScript.library.transform)
        {
            if (t.gameObject.GetComponent<ParentData>().parentType == pType)
            {
                p = t.gameObject;
                break;
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


    public void OnKeyboard(InputValue v)
    {
        //float c = v.GetType
        Debug.Log("keyboard button: " + v.Get<float>());
    }


    // Start is called before the first frame update
    void Start()
    {

        LoadPrefs();    // only seems to work from Start()

        // go to main area first
        jumpTargetAssembly.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        jumpTargetMain.GetComponent<TransportSwitch>().thisTarget.SetActive(false);
        jumpTargetTest.GetComponent<TransportSwitch>().thisTarget.SetActive(true);
        GameObject.Find("ToAssembly").GetComponent<AudioSource>().Play();
    }


    // what a friggin crutch!
    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard.sKey.wasPressedThisFrame) SaveData();
        if (keyboard.lKey.wasPressedThisFrame) LoadData();
        if (keyboard.tKey.wasPressedThisFrame) SomeTest();

    }

    // press "T" in Desktop mode
    void SomeTest()
    {
        Debug.Log("SystemInfo: " + SystemInfo.deviceType);

    }

}
