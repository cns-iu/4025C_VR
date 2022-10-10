using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

// 2022-10-10


public class SceneController : MonoBehaviour
{
    public GameObject xrPosition;
    public GameObject positionAA;
    public GameObject positionMA;
    public GameObject transportTarget;

    public ConController controllerScript;  //access to ConnectorController

    public List<GameObject> manifestList;        // all manifests
    

    public void jumpToAA(GameObject c)
    {
        xrPosition.transform.position = positionAA.transform.position;
    }

    public void jumpToMA(GameObject c)
    {
        // clone assembly  --- conlists are not correct
        // ???? instantiate calls conStatus!!!!!!
        xrPosition.transform.position = positionMA.transform.position;
        GameObject manifestOriginal = controllerScript.manifest;
        GameObject manifestCopy = Instantiate(manifestOriginal);
        manifestList.Add(manifestCopy);

        Debug.Log("manifestList count " + manifestList.Count);

        // delete all children controllerScript.manifest EXCEPT for root
        // set proper conStatus on root
        manifestOriginal.GetComponent<ManifestStatus>().conList.Clear();
        Debug.Log("old BEFORE manifest conList count " + manifestOriginal.GetComponent<ManifestStatus>().conList.Count);
        foreach (Transform child in manifestOriginal.transform)
        {
            GameObject croot = child.GetChild(0).gameObject;
            if (croot.name == "croot")
            {
                manifestOriginal.GetComponent<ManifestStatus>().conList.Add(croot);

                /*
                croot.GetComponent<ConStatus>().show = false;
                croot.GetComponent<ConStatus>().selected = false;
                croot.GetComponent<ConStatus>().connected = false;
                croot.GetComponent<MeshRenderer>().material = m;
                */

                controllerScript.conStatusSet(croot, 0, controllerScript.matDefault);   // reset new croot
            }
            else
            {
                controllerScript.pDestroy(child.gameObject);
            }
        }
        Debug.Log("old AFTER manifest conList count " + manifestOriginal.GetComponent<ManifestStatus>().conList.Count);
        Debug.Log("new manifest conList count " + manifestCopy.GetComponent<ManifestStatus>().conList.Count);

        // move to MA and scale
        manifestCopy.transform.position = transportTarget.transform.position;
        Vector3 objectScale = manifestCopy.transform.localScale;
        manifestCopy.transform.localScale = new Vector3(objectScale.x / 10, objectScale.y / 10, objectScale.z / 10);

        // todo package into BoxCollider
        BoxCollider bc = manifestCopy.AddComponent<BoxCollider>() as BoxCollider;
        //manifestCopy.AddComponent<XR>

    }


    public void jumpHover(GameObject c)
    {
        //Debug.Log("jumpHover");
    }


    public void jumpHoverExited(GameObject c)
    {
        //Debug.Log("jumpHoverExited");
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
