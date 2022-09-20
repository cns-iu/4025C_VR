using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //controllerScript.makeConList(thisConnector);
        controllerScript.conList.Add(thisConnector);
        //controllerScript.origins.Add(thisConnector, thisConnector.transform.position);

        // set to default visibility
        if (thisConnector.tag == "cPassive")
        {
            //controllerScript.makePassiveList(thisConnector);
            thisConnector.GetComponent<Renderer>().enabled = false;
            thisConnector.GetComponent<Collider>().enabled = false;
            show = false;
        }
        else
        {
            //controllerScript.makeActiveList(thisConnector);
            thisConnector.GetComponent<Renderer>().enabled = true;
            thisConnector.GetComponent<Collider>().enabled = true;
            show = true;
        }

        // initialize any connector properties
       
        selected = false;
        connected = false;

    }

}
