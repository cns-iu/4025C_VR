using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;
using UnityEngine.InputSystem.HID;

// V3 2022-10-11

public class ConController : MonoBehaviour
{
    public Material savedMaterial;

    //public List<GameObject> conList;
    public GameObject library;
    public GameObject manifest;

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
    const int bLibrary = 8;
    const int bInitIgnore = 16;

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
                conStatusSet(c, bSelected + bShow + bLibrary, matSelected);    // set connector status
                sysStateSet(sysSelection);                          // set system status
                conFilter(manifest);    // evaluate connector status - consoliodate?
                conFilter(library);
                break;

            case sysSelection + bShow + bSelected:  // 516
                //Debug.Log("-bSelected+sysSelection = " + (bSelected + sysSelection));
                
                conStatusSet(c, bDefault, matDefault);
                sysStateSet(sysDefault);
                conListReset(manifest);
                conListReset(library);
                conFilter(manifest);
                conFilter(library);
                break;

            case sysSelection:  // 512
                //Debug.Log("-sysSelection = " + sysSelection);     

                GameObject p = Instantiate(conListSelected(library).transform.parent.gameObject);
                GameObject o = conListSelected(library).transform.parent.gameObject;               // o is original parent
               
                //conFilter(manifest);
                GameObject sourceConnector = pGetSelectedNode(p);           // connector on new parent
                //Debug.Log("sourceConnector=" + sourceConnector.name);

                conStatusSet(pGetSelectedNode(o), bShow, matDefault);               // reset original source node "model"
                conStatusSet(sourceConnector, bShow + bConnected, matConnected);    // clone source node
                conStatusSet(c, bConnected, matBug);                                // clicked node
    
                GameObject cTaxi = new GameObject();

                Transform manifestT = p.transform.parent;          // save parent (manifest)
                cTaxi.transform.position = sourceConnector.transform.position;
                cTaxi.transform.rotation = sourceConnector.transform.rotation;
                p.transform.parent = cTaxi.transform;    // move complete object into taxi
                cTaxi.transform.position = c.transform.position;
                cTaxi.transform.rotation = c.transform.rotation;
                p.transform.parent = manifestT;     // put object back into manifest

                Destroy(cTaxi);
    
                connections.Add(sourceConnector, c);            // add new connection to connections dictionary, used for counting only

                // connector source/target exchange
                sourceConnector.GetComponent<ConStatus>().thatConnector = c;  // save other connector to this
                c.GetComponent<ConStatus>().thatConnector = sourceConnector;  // save this to other connector
 
                sysStateSet(sysDefault);
                conListReset(manifest);
                conListReset(library);
                conFilter(manifest);
                conFilter(library);

                break;

            case sysDefault + bShow + bConnected:   // 5
                //Debug.Log("--bConnected+sysDefault = " + (bConnected + sysDefault));
                GameObject p1 = c.transform.parent.gameObject;

                uscTraceUp(p1,pDestroy);

                conStatusReset(c.GetComponent<ConStatus>().thatConnector);  // disconnect happens here
                conDisconnectAll(p1);
        
                pDestroy(p1);                   // destroy THIS object last

                sysStateSet(sysDefault);
                conListReset(manifest);
                conListReset(library);
                conFilter(manifest);
           
                break;

            default:
                Debug.Log("onClicked command fall-through!!");
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

    // returns next down parent object or NULL if end of chain
    GameObject uscNextDown(GameObject p)
    {
        foreach (Transform child in p.transform)
        {
            if (conStatusGet(child.gameObject) == bShow + bConnected)
            {
                return child.gameObject.GetComponent<ConStatus>().thatConnector.transform.parent.gameObject;
            }
        }
        return null;
    }

    //public delegate void useTraceUp(GameObject p, int y);
    
    // this is called from disconnect and when next parent
    public void uscTraceUp(GameObject p, Action<GameObject> myDelegate)
    {
        GameObject c = null;
        foreach (Transform child in p.transform)
        {
            c = uscChild(child.gameObject);     
        }
        myDelegate.Invoke(p);
    }


    // 
    GameObject uscChild (GameObject c)
    {
        GameObject p = c.transform.parent.gameObject;
        switch (conStatusGet(c))
        {
            case bShow + bConnected:    // skip
                //Debug.Log("bShow+bConnected: " + c.name + " parent: " + c.transform.parent.gameObject.name);
                break;
           
            case bConnected:            // connected
                //Debug.Log("bConnected: " + c.name + " parent: " + c.transform.parent.gameObject.name);// 0

                p = uscNextParentUp(c);
           
                uscTraceUp(p,pDestroy);
                break;

            case bShow:                 // we're not cathcing this?
                //Debug.Log("bShow: " + c.name + " parent: " + c.transform.parent.gameObject.name);// 0
                break;

            default:
                //Debug.Log("default: " + c.name + " parent: " + c.transform.parent.gameObject.name);
                break;
        }
        return c;
    }


    // returns next up parent object or NULL if end of chain

    GameObject uscNextParentUp(GameObject c)
    {
        GameObject p = null;
        p = c.GetComponent<ConStatus>().thatConnector.transform.parent.gameObject;

        return p;
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
            Debug.Log("pGetSelectedNode: " + child.gameObject.name + " " + child.gameObject.GetComponent<ConStatus>().selected);
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
    // removes connection from respective conList on manifest
    public void pDestroy(GameObject p)
    {
       foreach (Transform child in p.transform)
        {
            p.transform.parent.gameObject.GetComponent<ManifestStatus>().conList.Remove(child.gameObject);
            if (conStatusGet(child.gameObject) == bShow + bConnected) connections.Remove(child.gameObject);   // remove from connections list if necessary     
        }     
        Destroy(p);             // kill parent objec
    }


    // evaluate bool properties on every GO in conList
    public void conFilter(GameObject m)
    {
        List<GameObject> conList = m.GetComponent<ManifestStatus>().conList;
        
        string commandStatus = m.name + ".conList: (" + conList.Count + ") ";      // for console

        foreach (GameObject c in conList)
        {
            int conCommand = makeConCommand(c); // evalute connector & system state

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

                case sysDefault + bLibrary:     // 8
                    conHide(c);
                    break;

                case sysDefault + bShow + bLibrary:     // 9
                    conShow(c);
                    break;

                case sysDefault + bShow + bSelected + bLibrary:    // 3
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

                case sysSelection + bLibrary:   // 520
                    conHide(c);
                    break;

                case sysSelection + bShow + bLibrary:   // 521
                    conHide(c);
                    break;

                case sysSelection + bSelected + bLibrary:  // 522
                    conShow(c);
                    break;

                default:
                    // 
                    break;
            }
        }
        if (nodeMode == false)
        {
            Debug.Log(commandStatus + " sysState " + sysState);
            Debug.Log("--connections: " + connections.Count + "---DONE-----###");
        }
    }


    // set conStatus of c; add required status values
    // usage: conStatusSet(c, bSelected + bShow, matSelected)
    public void conStatusSet(GameObject c, int s, Material m)
    {
        // set all to default
        c.GetComponent<ConStatus>().show = false;
        c.GetComponent<ConStatus>().selected = false;
        c.GetComponent<ConStatus>().connected = false;
        c.GetComponent<ConStatus>().library = false;
        if (m != false) c.GetComponent<MeshRenderer>().material = m;

        // evaluate and update connector status
        if ((s & bShow) == bShow) c.GetComponent<ConStatus>().show = true;
        if ((s & bSelected) == bSelected) c.GetComponent<ConStatus>().selected = true;
        if ((s & bConnected) == bConnected) c.GetComponent<ConStatus>().connected = true;
        if ((s & bLibrary) == bLibrary) c.GetComponent<ConStatus>().library = true;
    }


    // set all objects in list to requested status
    public void conListStatusSet(GameObject manifest, int status, Material material)
    {
        List<GameObject> conList = manifest.GetComponent<ManifestStatus>().conList;

        foreach (GameObject c in conList)
        {
            conStatusSet(c, status, material);
        }
    }


    int conStatusGet(GameObject c)
    {
        int status = bDefault;

        if (c.GetComponent<ConStatus>().show == true) status += bShow;
        if (c.GetComponent<ConStatus>().selected == true) status += bSelected;
        if (c.GetComponent<ConStatus>().connected == true) status += bConnected;
        if (c.GetComponent<ConStatus>().library == true) status += bLibrary;

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
    void conListReset(GameObject m)
    {
        List<GameObject> conList = m.GetComponent<ManifestStatus>().conList;
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
    GameObject conListSelected(GameObject m)
    {
        List<GameObject> conList = m.GetComponent<ManifestStatus>().conList;
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

    // make command token for evaluation in conFilter()
    int makeConCommand(GameObject connector)
    {
        int bCommand = 0;

        // evaluate connector states
        if (connector.GetComponent<ConStatus>().show == true) bCommand += bShow;
        if (connector.GetComponent<ConStatus>().selected == true) bCommand += bSelected;
        if (connector.GetComponent<ConStatus>().connected == true) bCommand += bConnected;
        if (connector.GetComponent<ConStatus>().library == true) bCommand += bLibrary;
        if (connector.GetComponent<ConStatus>().initIgnore == true) bCommand += bInitIgnore;

        bCommand += sysState;   //add sysState
        return bCommand;
    }


    // make command token for onClick
    int makeSysCommand(GameObject connector)
    {
        int cCommand = 0;

        // evaluate command input state
        if (connector.GetComponent<ConStatus>().show == true) cCommand += bShow;
        if (connector.GetComponent<ConStatus>().selected == true) cCommand += bSelected;
        if (connector.GetComponent<ConStatus>().connected == true) cCommand += bConnected;
  
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

        /*
        GameObject go = Instantiate(testNode.transform.parent.gameObject);
        foreach (Transform child in go.transform)
        {
            Debug.Log("child of Instance: " + child.gameObject.name);
            Debug.Log("parent (clone): " + child.parent.gameObject.name);
            if (child.parent.parent == null) Debug.Log("no more parent");
            //Debug.Log("parent.parent: " + child.parent.parent.gameObject.name);
        }
        */

        //Debug.Log("testnode parent: " + testNode.transform.parent.parent.gameObject.name);
        //Debug.Log("[0] in library: " + library.GetComponent<ManifestStatus>().conList[0].name);



    }

    // Update is called once per frame
    void Update()
    {
       

    }
}
