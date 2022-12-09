using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2022-12-8

public class ManifestStatus : MonoBehaviour
{
    public bool isLibrary;
    public string libraryName;
    public int shrinkage;
    public List<GameObject> conList;
    public List<GameObject> parentList;
    public Dictionary<GameObject, GameObject> connections;  // = new Dictionary<GameObject, GameObject>();
    public ConController controllerScript;  //access to ConnectorController

    // Start is called before the first frame update
    void Awake() {

        Debug.Log("ManifestStatus manifest=" + name);
        connections = new Dictionary<GameObject, GameObject>();

        if (isLibrary != true)
        {
            libraryName =  controllerScript.library.GetComponent<ManifestStatus>().libraryName;
            shrinkage = controllerScript.library.GetComponent<ManifestStatus>().shrinkage;
        }
    }
}
