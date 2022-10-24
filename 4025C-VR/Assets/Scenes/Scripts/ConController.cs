using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;
using UnityEngine.InputSystem.HID;

// V3 2022-10-20

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
        int sysCommand = MakeSysCommand(c);

        // in node debugging mode no command evaluation; only node properties
        if (nodeMode == true)
        {
            Debug.Log("node status: " + ConStatusGet(c) + " system status: " + sysState + " " + c.transform.name);
            return;
        }
        if (nodeMode == false) Debug.Log("cCommand = " + sysCommand);

        // normal command evaluation
        switch (sysCommand)
        {
            case sysDefault + bShow:    // 1
                //Debug.Log("-sysDefault = " + sysDefault);       
                ConStatusSet(c, bSelected + bShow, matSelected);    // set connector status
                SysStateSet(sysSelection);                          // set system status
                ConFilter(manifest);    // evaluate connector status - consoliodate?
                ConFilter(library);
                break;

            case sysSelection + bShow + bSelected:  // 516
                //Debug.Log("-bSelected+sysSelection = " + (bSelected + sysSelection));
                
                ConStatusSet(c, bDefault, matDefault);
                SysStateSet(sysDefault);
                ConListReset(manifest);
                ConListReset(library);
                ConFilter(manifest);
                ConFilter(library);
                break;

            case sysSelection:  // 512
                //Debug.Log("-sysSelection = " + sysSelection);     

                GameObject p = Instantiate(ConListSelected(library).transform.parent.gameObject);
                p.transform.parent = manifest.transform;                                // clone goes into global manifest
                GameObject o = ConListSelected(library).transform.parent.gameObject;               // o is original parent
               
                GameObject sourceConnector = PGetSelectedNode(p);           // connector on new parent

                ConStatusSet(PGetSelectedNode(o), bShow, matDefault);               // reset original source node "model"
                ConStatusSet(sourceConnector, bShow + bConnected, matConnected);    // clone source node
                ConStatusSet(c, bConnected, matBug);                                // clicked node
    
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
 
                SysStateSet(sysDefault);
                ConListReset(manifest);
                ConListReset(library);
                ConFilter(manifest);
                ConFilter(library);

                break;

            case sysDefault + bShow + bConnected:   // 5
                //Debug.Log("--bConnected+sysDefault = " + (bConnected + sysDefault));
                GameObject p1 = c.transform.parent.gameObject;

                UscTraceUp(p1,PDestroy);

                ConStatusReset(c.GetComponent<ConStatus>().thatConnector);  // disconnect happens here
                ConDisconnectAll(p1);
        
                PDestroy(p1);                   // destroy THIS object last

                SysStateSet(sysDefault);
                ConListReset(manifest);
                ConListReset(library);
                ConFilter(manifest);
           
                break;

            default:
                Debug.Log("onClicked command fall-through!!");
                break;

        }
    }

    
    // this is called from disconnect and when next parent
    public void UscTraceUp(GameObject p, Action<GameObject> myDelegate)
    {
        GameObject c = null;
        foreach (Transform child in p.transform)
        {
            c = UscChild(child.gameObject);     
        }
        myDelegate.Invoke(p);
    }


    // 
    GameObject UscChild (GameObject c)
    {
        GameObject p = c.transform.parent.gameObject;
        switch (ConStatusGet(c))
        {
            case bShow + bConnected:    // skip           
                break;
           
            case bConnected:            // connected
                p = UscNextParentUp(c);
                UscTraceUp(p,PDestroy);
                break;

            case bShow:                 // we're not cathcing this?
                break;

            default:
                break;
        }
        return c;
    }


    // returns next up parent object or NULL if end of chain
    GameObject UscNextParentUp(GameObject c)
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
            int s = ConStatusGet(c);

            // evaluate and update connector status
            c.GetComponent<MeshRenderer>().material = matDefault;
            if ((s & bSelected) == bSelected) c.GetComponent<MeshRenderer>().material = matSelected;
            if ((s & bConnected) == bConnected) c.GetComponent<MeshRenderer>().material = matConnected;
        }
    }


    GameObject PGetConnectedNode(GameObject p)
    {
        foreach (Transform child in p.transform)
        {
            if (ConStatusGet(child.gameObject) == bConnected) return child.gameObject;
        }
        return null;
    }


    // return selected node in parent
    GameObject PGetSelectedNode(GameObject p)
    {
        foreach (Transform child in p.transform)
        {
            Debug.Log("PGetSelectedNode parent: "
                + p.name + ", "
                + child.gameObject.name + ", "
                + child.gameObject.GetComponent<ConStatus>().selected);
            if (child.gameObject.GetComponent<ConStatus>().selected == true) return child.gameObject;
        }     
        return null;
    }


    // PReset - reset all nodes (children) on parent object
    void PReset(GameObject p)
    {
        foreach (Transform child in p.transform)
        {
            ConStatusReset(child.gameObject);
        }
    }


    // destroys parent GameObject and all attached/contained nodes
    // removes connection from respective conList on manifest
    public void PDestroy(GameObject p)
    {
       foreach (Transform child in p.transform)
        {
            p.transform.parent.gameObject.GetComponent<ManifestStatus>().conList.Remove(child.gameObject);
            if (ConStatusGet(child.gameObject) == bShow + bConnected) connections.Remove(child.gameObject);   // remove from connections list if necessary     
        }     
        Destroy(p);             // kill parent objec
    }


    // evaluate bool properties on every GO in conList
    public void ConFilter(GameObject m)
    {
        List<GameObject> conList = m.GetComponent<ManifestStatus>().conList;
        
        string commandStatus = m.name + ".conList: (" + conList.Count + ") ";      // for console

        foreach (GameObject c in conList)
        {
            int conCommand = MakeConCommand(c); // evalute connector & system state

            string str = " " + (conCommand & 127).ToString(); // for console
            commandStatus += str;                   // for console

            switch (conCommand)
            {
                case sysDefault + bDefault:     // 0
                    ConHide(c);
                    break;

                case sysDefault + bShow:        // 1
                    ConShow(c);
                    break;

                case sysDefault + bSelected:    // 2
                    ConShow(c);
                    break;

                case sysDefault + bShow + bSelected:    // 3
                    ConShow(c);
                    break;

                case sysDefault + bConnected:   // 4
                    ConHide(c);
                    break;

                case sysDefault + bConnected + bShow:   // 5
                    ConShow(c);
                    break;

                case sysDefault + bLibrary:     // 8
                    ConHide(c);
                    break;

                case sysDefault + bShow + bLibrary:     // 9
                    ConShow(c);
                    break;

                case sysDefault + bShow + bSelected + bLibrary:    // 3
                    ConShow(c);
                    break;

                case sysSelection + bDefault:   // 512
                    ConShow(c);
                    break;

                case sysSelection + bShow:      // 513
                    ConHide(c);
                    break;

                case sysSelection + bSelected:  // 514
                    ConShow(c);
                    break;

                case sysSelection + bShow + bConnected: // 517
                    ConHide(c);
                    break;

                case sysSelection + bLibrary:   // 520
                    ConHide(c);
                    break;

                case sysSelection + bShow + bLibrary:   // 521
                    ConHide(c);
                    break;

                case sysSelection + bSelected + bLibrary:  // 522
                    ConShow(c);
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
    // usage: ConStatusSet(c, bSelected + bShow, matSelected)
    public void ConStatusSet(GameObject c, int s, Material m)
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


    // set all objects in list to requested status
    public void ConListInitIgnoreStatusSet(GameObject m)
    {
        List<GameObject> conList = m.GetComponent<ManifestStatus>().conList;

        foreach (GameObject c in conList)
        {
            c.GetComponent<ConStatus>().initIgnore = true;
        }
    }


    int ConStatusGet(GameObject c)
    {
        int status = bDefault;

        if (c.GetComponent<ConStatus>().show == true) status += bShow;
        if (c.GetComponent<ConStatus>().selected == true) status += bSelected;
        if (c.GetComponent<ConStatus>().connected == true) status += bConnected;
        if (c.GetComponent<ConStatus>().library == true) status += bLibrary;

        return status;
    }


    void ConStatusReset(GameObject c)
    {
        // set all to default
        c.GetComponent<ConStatus>().show = false;
        c.GetComponent<ConStatus>().selected = false;
        c.GetComponent<ConStatus>().connected = false;
        c.GetComponent<MeshRenderer>().material = matDefault;
        //c.GetComponent<ConStatus>().thatConnector = null;
        //c.GetComponent<ConStatus>().thisConnector = null;
    }



    void ConHide(GameObject c)
    {
        c.GetComponent<Renderer>().enabled = false;
        c.GetComponent<Collider>().enabled = false;
    }


    void ConShow(GameObject c)
    {
        c.GetComponent<Renderer>().enabled = true;
        c.GetComponent<Collider>().enabled = true;
    }


    // reset connector relationships according to tags
    void ConListReset(GameObject m)
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
    GameObject ConListSelected(GameObject m)
    {
        List<GameObject> conList = m.GetComponent<ManifestStatus>().conList;
        GameObject connector = null;

        foreach (GameObject c in conList)
        {
            if (c.GetComponent<ConStatus>().selected == true) connector = c; 
        }
        return connector;
    }


    void ConDisconnectAll(GameObject p)
    {

        foreach (Transform child in p.transform )
        {
            if (child.gameObject.GetComponent<ConStatus>().thatConnector != null)
            {
                ConStatusReset(child.gameObject.GetComponent<ConStatus>().thatConnector);
            }
            ConStatusReset(child.gameObject);
        }
    }

    // make command token for evaluation in ConFilter()
    int MakeConCommand(GameObject connector)
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
    int MakeSysCommand(GameObject connector)
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
    void SysStateSet(int state)
    {
        sysState = state;
    }


    // Start is called before the first frame update
    void Start()
    {
        connections = new Dictionary<GameObject, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
       

    }
}
