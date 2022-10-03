using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;
using UnityEngine.InputSystem.HID;

// V2 2022-10-3

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
    public bool nodeMode = false;   // node debugging mode

    // connector state codes (using lower 8 bits)
    const int bDefault = 0;
    const int bShow = 1;
    const int bSelected = 2;
    const int bConnected = 4;

    // system state codes (using upper 8 bits)
    const int sysDefault = 0;
    const int sysBuild = 256;
    const int sysSelection = 512;

    private Dictionary<GameObject,GameObject> connections;


    public void connectorClicked(GameObject c)
    {
        // sysCommand is bSelected + bConnected + sysState
        int sysCommand = makeSysCommand(c);

        // in node debugging mode no command evaluation; only node properties
        if (nodeMode == true)
        {
            Debug.Log("node status: " + conStatusGet(c) + " system status: " + sysState + " " + c.transform.name);
            return;
        }
        if (nodeMode == false) Debug.Log("cCommand = " + sysCommand);

        // normal command evaluation
        switch (sysCommand)
        {
            case sysDefault + bShow:    // 1
                //Debug.Log("-sysDefault = " + sysDefault);       
                conStatusSet(c, bSelected + bShow, matSelected);    // set connector status
                sysStateSet(sysSelection);                          // set system status
                conUpdate();                                       // evaluate connector status

                break;

            case sysSelection + bShow + bSelected:  // 516
                //Debug.Log("-bSelected+sysSelection = " + (bSelected + sysSelection));
                
                conStatusSet(c, bDefault, matDefault);
                sysStateSet(sysDefault);
                conListReset();
                conUpdate();

                break;

            case sysSelection:  // 512
                //Debug.Log("-sysSelection = " + sysSelection);     

                // these GOs are parent objects (stars)
                GameObject o = Instantiate(conListSelected().transform.parent.gameObject);  // parent clone is used as original
                pReset(o);

                GameObject p = conListSelected().transform.parent.gameObject;               // used as new parent (star)   
                GameObject sourceConnector = pGetSelectedNode(p);                             // use selected source connector as pivot point
          
                // we have to reset original & clone stuff
                conStatusSet(conListSelected(), bShow, matDefault);             // reset original connector "model"
                conStatusSet(sourceConnector, bShow + bConnected, matConnected);
                conStatusSet(c, bShow + bConnected, matBug);
                   
                GameObject cTaxi = new GameObject();
                cTaxi.transform.position = sourceConnector.transform.position;
                cTaxi.transform.rotation = sourceConnector.transform.rotation;
                p.transform.parent = cTaxi.transform;    // move complete object into taxi
                cTaxi.transform.position = c.transform.position;
                cTaxi.transform.rotation = c.transform.rotation;
                p.transform.parent  = null;
                Destroy(cTaxi);
    
                connections.Add(sourceConnector, c);            // add new connection to connections dictionary, used for counting only

                //p.GetComponent<pStatus>().upStreanChain.Add(p); // add self to UCS
                
                // trace downstream and add to each USC

                // connector source/target exchange
                sourceConnector.GetComponent<ConStatus>().thatConnector = c;  // save other connector to this
                c.GetComponent<ConStatus>().thatConnector = sourceConnector;  // save this to other connector
 
                sysStateSet(sysDefault);
                conListReset();
                conUpdate();

                break;

            case sysDefault + bShow + bConnected:   // 5
                //Debug.Log("--bConnected+sysDefault = " + (bConnected + sysDefault));
                GameObject p1 = c.transform.parent.gameObject;
        
                //GameObject pFirst = uscTraceDown(p1);
                // walk up chain and pDestroy() every p
                GameObject pLast = uscTraceUp(p1);

                //if (pFirst == true) Debug.Log("pFirst: " + pFirst.name);
                //if (pLast == true) Debug.Log("pLast: " + pLast.name);

                conStatusReset(c.GetComponent<ConStatus>().thatConnector);  // disconnect happens here
                conDisconnectAll(p1);
        
                pDestroy(p1);                   // destroy THIS objects last

                sysStateSet(sysDefault);
                conListReset();
                conUpdate();
           
                break;

        }
    }


    GameObject uscTraceDown(GameObject p)
    {
        while (uscNextDown(p) == true)
        {
            p = uscNextDown(p);//
        }
        return p;
    }


    GameObject uscTraceUp(GameObject p)
    {
        while (uscNextUp(p) == true)
        {
            p = uscNextUp(p);
            pDestroy(p);
        }
        return p;
    }


    // returns next down parent object or NULL if end of chain
    GameObject uscNextDown(GameObject p)
    {
        foreach (Transform child in p.transform)
        {
            if (conStatusGet(child.gameObject) == bShow + bConnected)
            {
                return child.gameObject.GetComponent<ConStatus>().thatConnector.transform.parent.gameObject;
                //return p;
            }
        }
        return null;
    }


    // returns next up parent object or NULL if end of chain
    GameObject uscNextUp(GameObject p)
    {
        foreach (Transform child in p.transform)
        {
            if (conStatusGet(child.gameObject) == bConnected)
            {
                return child.gameObject.GetComponent<ConStatus>().thatConnector.transform.parent.gameObject;
            }
        }
        return null;
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
                    conHide(c);
                    break;

                case sysDefault + bShow:        // 1
                    conShow(c);
                    break;

                case sysDefault + bSelected:    // 2
                    conShow(c);
                    break;

                case sysDefault + bShow + bSelected:    // 3
                    conShow(c);
                    break;
               
                case sysDefault + bConnected:   // 4
                    conHide(c);
                    break;

                case sysDefault + bConnected + bShow:   // 5
                    conShow(c);
                    break;
            
                case sysSelection + bDefault:   // 512
                    conShow(c);
                    break;

                case sysSelection + bShow:      // 513
                    conHide(c);
                    break;

                case sysSelection + bSelected:  // 514
                    conShow(c);
                    break;

                case sysSelection + bShow + bConnected: // 517
                    conHide(c);
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


    GameObject pGetConnectedNode(GameObject p)
    {
        foreach (Transform child in p.transform)
        {
            if (conStatusGet(child.gameObject) == bConnected) return child.gameObject;
        }
        return null;
    }


    // return selected node in parent
    GameObject pGetSelectedNode(GameObject p)
    {
        foreach (Transform child in p.transform)
        {
            if (child.gameObject.GetComponent<ConStatus>().selected == true) return child.gameObject;
        }     
        return null;
    }


    // pReset - reset all nodes (children) on parent object
    void pReset(GameObject p)
    {
        foreach (Transform child in p.transform)
        {
            conStatusReset(child.gameObject);
        }
    }


    // destroys parent GameObject and all attached/contained nodes
    void pDestroy(GameObject p)
    {
       foreach (Transform child in p.transform)
        {
           conList.Remove(child.gameObject);
        }     
        Destroy(p);             // kill parent object
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


    void conStatusReset(GameObject c)
    {
        // set all to default
        c.GetComponent<ConStatus>().show = false;
        c.GetComponent<ConStatus>().selected = false;
        c.GetComponent<ConStatus>().connected = false;
        c.GetComponent<MeshRenderer>().material = matDefault;
        //c.GetComponent<ConStatus>().thatConnector = null;
        //c.GetComponent<ConStatus>().thisConnector = null;
    }



    void conHide(GameObject c)
    {
        c.GetComponent<Renderer>().enabled = false;
        c.GetComponent<Collider>().enabled = false;
    }


    void conShow(GameObject c)
    {
        c.GetComponent<Renderer>().enabled = true;
        c.GetComponent<Collider>().enabled = true;
    }


    // reset connector relationships according to tags
    void conListReset()
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


    void conDisconnectAll(GameObject p)
    {

        foreach (Transform child in p.transform )
        {
            if (child.gameObject.GetComponent<ConStatus>().thatConnector != null)
            {
                conStatusReset(child.gameObject.GetComponent<ConStatus>().thatConnector);
            }
            conStatusReset(child.gameObject);
        }


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
