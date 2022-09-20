using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;

public class ConController : MonoBehaviour
{
    public Material savedMaterial;

    public List<GameObject> conList;

    public Material matDefault;
    public Material matHover;   // blue?
    public Material matSelected;    // red
    public Material matConnected;   // dark blue
    public Material matBug;         //debugging

    public int sysState = 0;        // system state = default
    public bool nodeMode = false;

    // connector state codes
    const int bDefault = 0;
    const int bShow = 1;
    const int bSelected = 2;
    const int bConnected = 4;

    // system state codes
    const int sysDefault = 0;
    const int sysSelection = 128;

    private Dictionary<GameObject,GameObject> connections;


    public void connectorClicked(GameObject c)
    {
        // sysCommand is bSelected + bConnected + sysState
        int sysCommand = makeSysCommand(c);

        if (nodeMode == true)
        {
            Debug.Log("node status: " + conStatusGet(c) + ", " + c.GetComponent<MeshRenderer>().material);
            return;
        }
        if (nodeMode == false) Debug.Log("cCommand = " + sysCommand);
 
        switch (sysCommand)
        {
            case sysDefault + bShow:    // 1
                //Debug.Log("-sysDefault = " + sysDefault);       
                conStatusSet(c, bSelected + bShow, matSelected);    // set connector status
                sysStateSet(sysSelection);                          // set system status
                conUpdate();                                       // evaluate connector status

                break;

            case sysSelection + bShow + bSelected:  // 131
                //Debug.Log("-bSelected+sysSelection = " + (bSelected + sysSelection));
                
                conStatusSet(c, bDefault, matDefault);
                sysStateSet(sysDefault);
                conReset();
                conUpdate();

                break;

            case sysSelection:  // 128
                //Debug.Log("-sysSelection = " + sysSelection);     
                GameObject sourceConnector = Instantiate(conListSelected());    // clone source connector

                conStatusSet(conListSelected(), bShow, matDefault);             // reset original connector "model"
                conStatusSet(sourceConnector, bShow + bConnected, matConnected);
                conStatusSet(c, bShow + bConnected, matBug);

                // make child of clicked and update location
                sourceConnector.transform.parent = c.transform;
                if (sourceConnector.GetComponent<Rigidbody>())
                {
                    sourceConnector.GetComponent<Rigidbody>().isKinematic = true;
                }
                sourceConnector.transform.localPosition = new Vector3(0, 0, 0);   // 0,0,0 for real
                sourceConnector.transform.localRotation = Quaternion.identity;

                connections.Add(sourceConnector, c);     // add new connection to connections dictionary, used for counting)

                // connector source/target exchange
                sourceConnector.GetComponent<ConStatus>().thatConnector = c;  // save other connector to this
                c.GetComponent<ConStatus>().thatConnector = sourceConnector;  // save this to other connector
 
                sysStateSet(sysDefault);
                conReset();
                conUpdate();

                break;

            case sysDefault + bShow + bConnected:   // 5
                //Debug.Log("--bConnected+sysDefault = " + (bConnected + sysDefault));

                GameObject destConnector = c.GetComponent<ConStatus>().thatConnector;
                conStatusSet(destConnector, bDefault, matDefault);

                connections.Remove(c);  // remove from connections list
                conList.Remove(c);      // remove from connectores list
                Destroy(c);             // kill object

                sysStateSet(sysDefault);
                conReset();
                conUpdate();
           
                break;

        }
    }


    public void connectorHover(GameObject c)
    { 
        c.GetComponent<MeshRenderer>().material = matHover;
    }


    public void connectorHoverExited(GameObject c)
    {
        if (c != null)  // make sure it;s a valid object
        {
            int s = conStatusGet(c);
            Debug.Log("onHoverExit: " + s);

            // evaluate and update connector status
            c.GetComponent<MeshRenderer>().material = matDefault;
            if ((s & bSelected) == bSelected) c.GetComponent<MeshRenderer>().material = matSelected;
            if ((s & bConnected) == bConnected) c.GetComponent<MeshRenderer>().material = matConnected;
        }
    }


    // evaluate bool properties on every GO in conList
    public void conUpdate()
    {
        // if object selected flip conList (except selected)
        //GameObject c = conListFlipShow();
        string commandStatus = "conList(" + conList.Count + ") ";      // for console
 
        foreach (GameObject c in conList)
        {
            int conCommand = makeConCommand(c); // evalute connector & system state

            // show commands without sysState!!!

            string str = " " + (conCommand & 127).ToString(); // for console
            commandStatus += str;                   // for console

            switch (conCommand)
            {
                case sysDefault + bDefault:     // 0
                    c.GetComponent<Renderer>().enabled = false;
                    c.GetComponent<Collider>().enabled = false;
                    break;

                case sysDefault + bShow:        // 1
                    c.GetComponent<Renderer>().enabled = true;
                    c.GetComponent<Collider>().enabled = true;
                    break;

                case sysDefault + bSelected:    // 2
                    c.GetComponent<Renderer>().enabled = true;
                    c.GetComponent<Collider>().enabled = true;
                    break;

                case sysDefault + bShow + bSelected:    // 3
                    c.GetComponent<Renderer>().enabled = true;
                    c.GetComponent<Collider>().enabled = true;
                    break;
               
                case sysDefault + bConnected:   // 4
                    c.GetComponent<Renderer>().enabled = false;
                    c.GetComponent<Collider>().enabled = false;
                    break;

                case sysDefault + bConnected + bShow:   // 5
                    c.GetComponent<Renderer>().enabled = true;
                    c.GetComponent<Collider>().enabled = true;
                    break;
            
                case sysSelection + bDefault:   // 128
                    c.GetComponent<Renderer>().enabled = true;
                    c.GetComponent<Collider>().enabled = true;
                    break;

                case sysSelection + bShow:      // 129
                    c.GetComponent<Renderer>().enabled = false;
                    c.GetComponent<Collider>().enabled = false;
                    break;

                case sysSelection + bSelected:  // 130
                    c.GetComponent<Renderer>().enabled = true;
                    c.GetComponent<Collider>().enabled = true;
                    break;

                case sysSelection + bShow + bConnected:
                    c.GetComponent<Renderer>().enabled = false;
                    c.GetComponent<Collider>().enabled = false;
                    break;


                default:
                    // 
                    break;
            }

        }
        if (nodeMode == false)
        {
            Debug.Log(commandStatus + " sysState " + sysState);
            Debug.Log("--connections: " + connections.Count + "---DONE-----");
        }
    }


    void conStatusSet(GameObject c, int s, Material m)
    {
        // set all to default
        c.GetComponent<ConStatus>().show = false;
        c.GetComponent<ConStatus>().selected = false;
        c.GetComponent<ConStatus>().connected = false;
        c.GetComponent<MeshRenderer>().material = m;

        // evaluate and update connector status
        if ((s & bShow) == bShow) c.GetComponent<ConStatus>().show = true;
        if ((s & bSelected) == bSelected) c.GetComponent<ConStatus>().selected = true;
        if ((s & bConnected) == bConnected) c.GetComponent<ConStatus>().connected = true;
    }


    int conStatusGet(GameObject c)
    {
        int status = bDefault;

        if (c.GetComponent<ConStatus>().show == true) status += bShow;
        if (c.GetComponent<ConStatus>().selected == true) status += bSelected;
        if (c.GetComponent<ConStatus>().connected == true) status += bConnected;

        return status;
    }


    // reset connector relationships according to tags
    void conReset()
    {
        // reset show status
        // clear selected status
        foreach (GameObject connector in conList)
        {
            if (connector.tag == "cPassive")
            {
                connector.GetComponent<ConStatus>().show = false;
            }
            else
            {
                connector.GetComponent<ConStatus>().show = true;
            }

            connector.GetComponent<ConStatus>().selected = false;
        }
    }


    // does conList have selected object?
    // returns: null if false, GameObject when true
    GameObject conListSelected()
    {
        GameObject connector = null;

        foreach (GameObject c in conList)
        {
            if (c.GetComponent<ConStatus>().selected == true) connector = c;
        }
        return connector;
    }


    // makes a unqiue command byte by adding binary numbers
    int makeConCommand(GameObject connector)
    {
        int bCommand = 0;

        // evaluate connector states
        if (connector.GetComponent<ConStatus>().show == true) bCommand += bShow;
        if (connector.GetComponent<ConStatus>().selected == true) bCommand += bSelected;
        if (connector.GetComponent<ConStatus>().connected == true) bCommand += bConnected;
   
        bCommand += sysState;   //add sysState
        return bCommand;
    }


    // makes a click-reponse command byte for onClick evaluation
    int makeSysCommand(GameObject connector)
    {
        int cCommand = 0;

        // evaluate command input state
        if (connector.GetComponent<ConStatus>().selected == true) cCommand += bSelected;
        if (connector.GetComponent<ConStatus>().connected == true) cCommand += bConnected;
        if (connector.GetComponent<ConStatus>().show == true) cCommand += bShow;

        cCommand += sysState;   //add sysState
        return cCommand;
    }


    // using function to set system state for future expansion
    void sysStateSet(int state)
    {
        sysState = state;
    }


    // Start is called before the first frame update
    void Start()
    {
        connections = new Dictionary<GameObject, GameObject>();
        //origins = new Dictionary<GameObject, Vector3>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
