using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;
using static JsonEntries;
using static JsonManifests;
//using UnityEditor;

// 2022-11-28

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

    public GameObject loadManifest;     // loading objects deposited to this manifest

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


        // here make a copy of pristine LoadManifest
        controllerScript.ConListInitIgnoreStatusSet(controllerScript.manifest);
        GameObject manifestCopy = Instantiate(controllerScript.manifest);
        controllerScript.ConListInitIgnoreStatusClear(controllerScript.manifest);
        controllerScript.ConListInitIgnoreStatusClear(manifestCopy);
        manifestCopy.GetComponent<ManifestStatus>().connections = new Dictionary<GameObject, GameObject>();

        // the package will appear here (target object)
        controllerScript.manifest.transform.position = transportTarget.transform.position;
        if (controllerScript.manifest != null) AssemblyPackage(controllerScript.manifest);

        manifestCopy.name = "Manifest";
        controllerScript.manifest = manifestCopy;
    }

  

    GameObject AssemblyPackage(GameObject m)
    {
        if (m.GetComponent<ManifestStatus>().conList.Count > 1) {

            m.name = "manifest_" + manifestList.Count;
            manifestList.Add(m);         // store in global manifest list

            FitToChildren(m);
            Rigidbody rb = m.AddComponent<Rigidbody>() as Rigidbody;

            m.AddComponent<XRGrabInteractable>();

            //m.transform.position = transportTarget.transform.position;
            Vector3 objectScale = m.transform.localScale;
            m.transform.localScale = new Vector3(objectScale.x / 10, objectScale.y / 10, objectScale.z / 10);
        }       
        return m;
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
        if (manifestList.Count > 0)
        {
            foreach (GameObject m in manifestList)
            {
                SaveManifest(m);
            }
            SaveManifestList(manifestList);
        }
        else
        {
            Debug.Log("Empty manifestList; nothing to save");
        }
    }


    // press "T" on Desktop computer keyboard
    public void SaveDataDesktop()
    {
        if (manifestList.Count > 0)
        {
            foreach (GameObject m in manifestList)
            {
                SaveManifest(m);                
            }
            SaveManifestList(manifestList);
        }
        else
        {
            Debug.Log("Empty manifestList; nothing to save");
        }
    }


    // saves current manifestList
    void SaveManifestList(List<GameObject> mList)
    {
        // build manifestList for saving
        GetComponent<JsonManifests>().manifests.Clear();
        foreach (GameObject g in mList)
        {
            GetComponent<JsonManifests>().manifests.Add(g.name);
        }
        // saves manifestList
        WriteToFile("manifests.txt", GetComponent<JsonManifests>().SaveToString());
    }



    void SaveManifest(GameObject m)
    {
        // save position and rotation
        GetComponent<JsonEntries>().posX = m.transform.position.x;
        GetComponent<JsonEntries>().posY = m.transform.position.y;
        GetComponent<JsonEntries>().posZ = m.transform.position.z;
        GetComponent<JsonEntries>().rotX = m.transform.rotation.x;
        GetComponent<JsonEntries>().rotY = m.transform.rotation.y;
        GetComponent<JsonEntries>().rotZ = m.transform.rotation.z;


        GetComponent<JsonEntries>().parents.Clear();
        foreach (GameObject g in m.GetComponent<ManifestStatus>().parentList)
        {
            GetComponent<JsonEntries>().parents.Add(g.GetComponent<ParentData>().parentType);
        }
        // encode connections dictionary
        GetComponent<JsonEntries>().connectionsList.Clear();
        foreach (KeyValuePair<GameObject, GameObject> entry in m.GetComponent<ManifestStatus>().connections)
        {
            // key = from, value = to
            GetComponent<JsonEntries>().connectionsList.Add(ConnectionEncode(entry.Key, entry.Value));
        }
        string wtf = GetComponent<JsonEntries>().SaveToString();
        WriteToFile(m.name + ".txt", wtf);
        Debug.Log("saving manifest to file: " + m.name + "->" + wtf);
    }


    // return from child, from parent, to child, to parent IDs
    // parameter is a connector node
    public JsonEntries.connectionEntry ConnectionEncode(GameObject from, GameObject to)
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
        GameObject p = g.transform.parent.gameObject;

        for (int i = 0; i < p.GetComponent<ManifestStatus>().parentList.Count; i++)
        {
            if (p.GetComponent<ManifestStatus>().parentList[i] == g)
            {
                parentID = i;
                break;
            }
        }
        return parentID;
    }



    public void LoadData()
    {
        // load and init manifestList temp (system manifestList if built during recreation)
        LoadFromFile("manifests.txt", out var wtg);
        GetComponent<JsonManifests>().LoadFromJson(wtg);

        foreach (string f in GetComponent<JsonManifests>().manifests)
        {
            LoadFromFile(f + ".txt", out var wtf);
            Debug.Log("json loaded: " + wtf);

            GetComponent<JsonEntries>().LoadFromJson(wtf);

            // the manifest will go to this pos/rot
            loadManifest.transform.position = new Vector3(GetComponent<JsonEntries>().posX, GetComponent<JsonEntries>().posY, GetComponent<JsonEntries>().posZ);
            loadManifest.transform.rotation = Quaternion.Euler(GetComponent<JsonEntries>().rotX, GetComponent<JsonEntries>().rotY, GetComponent<JsonEntries>().rotZ);

            // here make a copy of pristine LoadManifest
            controllerScript.ConListInitIgnoreStatusSet(loadManifest);
            GameObject manifestCopy = Instantiate(loadManifest);
            controllerScript.ConListInitIgnoreStatusClear(loadManifest);
            controllerScript.ConListInitIgnoreStatusClear(manifestCopy);
            manifestCopy.GetComponent<ManifestStatus>().connections = new Dictionary<GameObject, GameObject>();

            foreach (string typeID in GetComponent<JsonEntries>().parents)
            {
                if (typeID != "root")    // skip root - it's already in LoadManifest
                {
                    GameObject p = LibraryItemCloner(typeID, loadManifest);     // clone item from library by typeID string
                    p.transform.parent = loadManifest.transform;
                    loadManifest.GetComponent<ManifestStatus>().parentList.Add(p);
                }
                else
                {
                    // no action for the root
                    //loadManifest.GetComponent<ManifestStatus>().parentList.Add(loadManifest.transform.GetChild(0).gameObject);
                }
            }

            foreach (connectionEntry listEntry in GetComponent<JsonEntries>().connectionsList)
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
            manifestCopy.name = "LoadManifest";
            loadManifest = manifestCopy;
        }
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

        // add root parent to loadManifest once only
        loadManifest.GetComponent<ManifestStatus>().parentList.Add(loadManifest.transform.GetChild(0).gameObject);

       
    }


    // what a friggin crutch!
    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard.sKey.wasPressedThisFrame) SaveData();
        if (keyboard.lKey.wasPressedThisFrame) LoadData();
        if (keyboard.tKey.wasPressedThisFrame) SaveDataDesktop();
        if (keyboard.xKey.wasPressedThisFrame) FileSelection();

    }


    //[MenuItem("Example/Overwrite Texture")]
    public void FileSelection()
    {
        Debug.Log("xkey");
        //Debug.Log("parents.count=" + GetComponent<JsonEntries>().parents.Count);


    }



}
