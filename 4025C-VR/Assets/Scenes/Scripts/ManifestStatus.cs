using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2022-12-7

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

        /*
        if ((name == "Library") || (name == "Manifest") || (name == "LoadManifest"))
        {
            Debug.Log("ManifestStatus manifest=" + name);
            connections = new Dictionary<GameObject, GameObject>();
        }
        */

        Debug.Log("ManifestStatus manifest=" + name);
        connections = new Dictionary<GameObject, GameObject>();

        if (isLibrary != true)
        {
            libraryName =  controllerScript.library.GetComponent<ManifestStatus>().libraryName;
            shrinkage = controllerScript.library.GetComponent<ManifestStatus>().shrinkage;
        }




        /*
        if (name != "TestManifest") {
            //connections = new Dictionary<GameObject, GameObject>();
            //parentList.Add(this.transform.GetChild(0).gameObject);
        } else {
            // populate connections list when running from desktop
            Debug.Log("ManifestStatus:Building test connections");
      
            Debug.Log("first pair " + transform.GetChild(1).GetChild(0).gameObject.name + ":" + transform.GetChild(0).GetChild(0).gameObject.name);
            connections.Add(transform.GetChild(1).GetChild(0).gameObject, transform.GetChild(0).GetChild(0).gameObject);

            Debug.Log("first pair " + transform.GetChild(1).GetChild(0).gameObject.name + ":" + transform.GetChild(1).GetChild(1).gameObject.name);
            connections.Add(transform.GetChild(2).GetChild(0).gameObject, transform.GetChild(1).GetChild(1).gameObject);

            Debug.Log("first pair " + transform.GetChild(1).GetChild(0).gameObject.name + ":" + transform.GetChild(1).GetChild(2).gameObject.name);
            connections.Add(transform.GetChild(3).GetChild(0).gameObject, transform.GetChild(1).GetChild(2).gameObject);
            
        }
        */
    }
}
