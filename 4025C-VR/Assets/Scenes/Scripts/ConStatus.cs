using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2022-10-13

public class ConStatus : MonoBehaviour
{
    public GameObject thisConnector;
    public GameObject thatConnector;
    public ConController controllerScript;  //access to ConnectorController
    public bool show;
    public bool selected;
    public bool connected;
    public bool library;        //is it a library object?
    public bool initIgnore;     //ignore on conStatus awake
                                            //
    public Material currentMaterial;
    public Material defaultMaterial;


    // Awake is called before the first frame update

    // this setup for each connector node - super important!!!
    private void Awake()
    {
        /*
            Debug.Log("conStatus.awake " + thisConnector.transform.parent.gameObject.name +
            " " +  thisConnector.name +
            " selected = " +
            selected);
        */
        if (thisConnector.transform.parent.parent != null)
        { 
            // these are in manifests (library) grandparent != NULL
            GameObject pp = thisConnector.transform.parent.parent.transform.gameObject;
            pp.GetComponent<ManifestStatus>().conList.Add(thisConnector);

            if (pp.name == "Library")
            {
                // library
                selected = false;
                connected = false;
                library = true;
            }
            else
            {
                // manifest
                selected = false;
                connected = false;
                library = false;
            }
        }
        else
        {
            // here parent is on top-level
            // ignore connectors with set initIgnore status
            if (thisConnector.GetComponent<ConStatus>().initIgnore != true)
            {
                // this catches newly instantiated (in root hierarchy)
                //thisConnector.transform.parent.parent = controllerScript.manifest.transform; //======!!!!!^$&%*^
                controllerScript.manifest.GetComponent<ManifestStatus>().conList.Add(thisConnector);
                //selected = false;
                /*
                Debug.Log("conStatus.top level " + thisConnector.transform.parent.gameObject.name + " " +
                    thisConnector.name + " selected = " + selected);
                */
                connected = false;
                library = false;
            }
        }
      
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
