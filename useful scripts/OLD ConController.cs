using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConController : MonoBehaviour
{

    public GameObject thisStar;
    public GameObject thisConnector;
    public GameObject previousStar;
    public GameObject previousConnector;
    public Material savedMaterial;

    public List<GameObject> conList;

    public Material matHover;   // blue?
    public Material matSelected;    // red
    public Material matConnected;   // dark blue

    // command codes
    const int bDefault = 0;
    const int bShow = 1;
    const int bSelected = 2;
    const int bConnected = 4;

    public void connectorClicked(GameObject connector)
    {
        thisConnector = connector;
        thisStar = thisConnector.transform.parent.gameObject;

        GetComponent<AudioSource>().Play();

        // on click
        if (thisStar == previousStar)
        {
            // same connector?
            // ==> revert to default material
            if (thisConnector == previousConnector)
            {
                thisConnector.GetComponent<MeshRenderer>().material = thisConnector.GetComponent<ConStatus>().defaultMaterial;
                thisConnector.GetComponent<ConStatus>().selected = false;

                allClear();
                conUpdate();
     
            } else
            {
                // different connector?
                // ==> reset previous, this goes selected
                previousConnector.GetComponent<MeshRenderer>().material = thisConnector.GetComponent<ConStatus>().defaultMaterial;
                thisConnector.GetComponent<MeshRenderer>().material = matSelected;
                thisConnector.GetComponent<ConStatus>().selected = true;

                conUpdate();

                previousStar = thisStar;
                previousConnector = thisConnector;
            }

        } else
        {
            // clicked on different or new star
            if (previousStar == null)
            {
                thisConnector.GetComponent<MeshRenderer>().material = matSelected;
                thisConnector.GetComponent<ConStatus>().selected = true;

                conUpdate();

                previousStar = thisStar;
                previousConnector = thisConnector;
            } else
            {
                // connecting.....
                previousConnector.GetComponent<MeshRenderer>().material = thisConnector.GetComponent<ConStatus>().defaultMaterial;

                // connection action
                previousStar.transform.parent = thisConnector.transform;

                if (previousStar.GetComponent<Rigidbody>())
                {
                    previousStar.GetComponent<Rigidbody>().isKinematic = true;
                }

                previousStar.transform.localPosition = new Vector3(0, 0, 0);
                previousStar.transform.localRotation = Quaternion.identity;

                // mark as connected
                previousConnector.GetComponent<ConStatus>().connected = true;
                previousConnector.GetComponent<MeshRenderer>().material = matConnected;
                thisConnector.GetComponent<ConStatus>().connected = true;
                thisConnector.GetComponent<MeshRenderer>().material = matConnected;
                thisConnector.GetComponent<ConStatus>().show = false;

                allClear();
                conUpdate();       
            }
        }
    }


    public void connectorHover(GameObject connector)
    { 
        savedMaterial = connector.GetComponent<MeshRenderer>().material;
        connector.GetComponent<MeshRenderer>().material = matHover;
    }


    public void connectorHoverExited(GameObject connector)
    {
        /*
        if (connector.GetComponent<ConStatus>().selected == false)
        {
            connector.GetComponent<MeshRenderer>().material = savedMaterial;
        }*/
        connector.GetComponent<MeshRenderer>().material = savedMaterial;
    }


    // evaluate bool properties on every GO in conList
    public void conUpdate()
    {
        // if object selected flip conList (except selected)
        GameObject c = conListFlipShow();
        string commandStatus = "status: ";
 
        foreach (GameObject connector in conList)
        {
            int bCommand = makebCommand(connector);
            //Debug.Log("bcommand: " + bCommand);
            //string bCommandStr = KeyValuePair.to.(bcommand);

            string str = "," + bCommand.ToString();
            //string str2 = bCommand.ToString();
            //commandStatus += str1;
            commandStatus += str;

            //sb.AppendLine(i.ToString());

            switch (bCommand)
            {
                case bDefault:
                    connector.GetComponent<Renderer>().enabled = false;
                    connector.GetComponent<Collider>().enabled = false;
                    break;    

                case bShow:
                    connector.GetComponent<Renderer>().enabled = true;
                    connector.GetComponent<Collider>().enabled = true;
                    break;

                case bSelected:
                    connector.GetComponent<Renderer>().enabled = true;
                    connector.GetComponent<Collider>().enabled = true;
                    break;

                case bConnected:
                    connector.GetComponent<Renderer>().enabled = false;
                    connector.GetComponent<Collider>().enabled = false;
                    break;

                case bShow + bSelected:
                    connector.GetComponent<Renderer>().enabled = true;
                    connector.GetComponent<Collider>().enabled = true;
                    break;

                default:
                    // 
                    break;
            }

        }
        Debug.Log(commandStatus);
        Debug.Log("--------------");
    }


    // does conList has selected object?
    // returns: null if false, GameObject when true
    GameObject conListSelected()
    {
        GameObject connector;
        connector = null;

        foreach (GameObject c in conList)
        {
            if (c.GetComponent<ConStatus>().selected == true) connector = c;
        }

        return connector;
    }


    // reverse show/hide status of each element in conList
    // except selected
    GameObject conListFlipShow()
    {
        GameObject connector = conListSelected();

        //only run if there is a selection
        if (connector != null)
        {
            foreach (GameObject c in conList)
            {
                if (c.GetComponent<ConStatus>().show == true)
                {
                    c.GetComponent<ConStatus>().show = false;
                } else
                {
                    c.GetComponent<ConStatus>().show = true;
                }

                if (c.GetComponent<ConStatus>().selected == true) c.GetComponent<ConStatus>().show = true;
            }
        }
        return connector;
    }


    int makebCommand(GameObject connector)
    {
        // create binary command code by addin\
        int bCommand = 0;

        if (connector.GetComponent<ConStatus>().show == true)
        {
            bCommand += bShow;
        }
        if (connector.GetComponent<ConStatus>().selected == true)
        {
            bCommand += bSelected;
        }
        if (connector.GetComponent<ConStatus>().connected == true)
        {
            bCommand += bConnected;
        }
        return bCommand;
    }


    // reset connector relationships
    void allClear()
    {
        thisConnector = null;
        thisStar = null;
        previousConnector = null;
        previousStar = null;

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


    // make three list of connectors
    public void makeConList(GameObject connector)
    {
        conList.Add(connector);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
