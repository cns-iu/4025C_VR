using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// V3

public class ConStatus : MonoBehaviour
{
    public GameObject thisConnector;
    public GameObject thatConnector;
    public ConController controllerScript;    //access to ConnectorController
    public bool show;
    public bool selected;
    public bool connected;
    public Material currentMaterial;
    public Material defaultMaterial;


    // Start is called before the first frame update

    private void Awake()
    {
        // add to global connector list

        //controllerScript.conList.Add(thisConnector);
        // get parent of parent = manifest

        //Debug.Log("problem is here:");
        //if (thisConnector.transform.parent.parent.transform.gameObject == null) Debug.Log("null parent:");

        if (thisConnector.transform.parent.parent != null)
        {
            GameObject pp = thisConnector.transform.parent.parent.transform.gameObject;

            pp.GetComponent<ManifestStatus>().conList.Add(thisConnector);
            Debug.Log("ConStatus awake() " + pp.name);
            selected = false;
            connected = false;
        }
        else
        {
            Debug.Log("Awake else...." + thisConnector.name + ", " + thisConnector.transform.parent.gameObject.name);
            thisConnector.transform.parent.parent = controllerScript.manifest.transform;
            controllerScript.manifest.GetComponent<ManifestStatus>().conList.Add(thisConnector);
            //selected = false;
            connected = false;
        }
      
   
        
        //controllerScript.conList.Add(thisConnector);
        

        // set to default visibility
        if (thisConnector.tag == "cPassive")
        {
            thisConnector.GetComponent<Renderer>().enabled = false;
            thisConnector.GetComponent<Collider>().enabled = false;
            show = false;
        }
        else
        {
            thisConnector.GetComponent<Renderer>().enabled = true;
            thisConnector.GetComponent<Collider>().enabled = true;
            show = true;
        }

        // initialize any connector properties
       


    }

}
