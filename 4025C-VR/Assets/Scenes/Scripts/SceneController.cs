using System.Collections;
using System.Collections.Generic;
using OculusSampleFramework;
//using System.Numerics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

// 2022-10-13


public class SceneController : MonoBehaviour
{
    public GameObject xrPosition;
    public GameObject positionAA;
    public GameObject positionMA;
    public GameObject positionTestArea;
    public GameObject transportTarget;

    public ConController controllerScript;  //access to ConnectorController
    public ManifestStatus manStatus;

    public List<GameObject> manifestList;        // all manifests

    public GameObject testManifest;

    const int bDefault = 0;
    const int bShow = 1;
    const int bSelected = 2;
    const int bConnected = 4;
    const int bLibrary = 8;
    const int bInitIgnore = 16;

    public void jumpToAA(GameObject c)
    {
        xrPosition.transform.position = positionAA.transform.position;
    }

    public void jumpToTestArea(GameObject c)
    {
        xrPosition.transform.position = positionTestArea.transform.position;
    }

    public void jumpToMA(GameObject c)
    {
        xrPosition.transform.position = positionMA.transform.position;
        GameObject manifestOriginal = controllerScript.manifest;
        controllerScript.ConListInitIgnoreStatusSet(manifestOriginal); // prevents instantiated nodes from being added to conList
        GameObject manifestCopy = Instantiate(manifestOriginal);
        manifestList.Add(manifestCopy);         // store in global manifest list

        //Debug.Log("manifestList count " + manifestList.Count);

        // clear original manifest and add default node
        manifestOriginal.GetComponent<ManifestStatus>().conList.Clear();
       
        foreach (Transform child in manifestOriginal.transform)
        {
            GameObject croot = child.GetChild(0).gameObject;
            if (croot.name == "croot")
            {
                manifestOriginal.GetComponent<ManifestStatus>().conList.Add(croot);
                controllerScript.ConStatusSet(croot, 0, controllerScript.matDefault);   // reset new croot
            }
            else
            {
                controllerScript.PDestroy(child.gameObject);
            }
        }

        // package copy into 0.1 scale XRInteractable
        // move to MA and scale
        manifestCopy.transform.position = transportTarget.transform.position;
        Vector3 objectScale = manifestCopy.transform.localScale;
        manifestCopy.transform.localScale = new Vector3(objectScale.x / 10, objectScale.y / 10, objectScale.z / 10);

        // todo package into BoxCollider
        //BoxCollider bc = manifestCopy.AddComponent<BoxCollider>() as BoxCollider;
        FitToChildren(manifestCopy);
        
        //manifestCopy.AddComponent<XR>

    }


    void FitToChildren(GameObject m)
    {
        BoxCollider bc = m.AddComponent<BoxCollider>() as BoxCollider;
        if (m.GetComponent<Collider>() is BoxCollider)
        {

            bool hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int i = 0; i < m.transform.childCount; ++i)
            {
                Renderer childRenderer = m.transform.GetChild(i).GetComponent<Renderer>();
                if (childRenderer != null)
                {
                    if (hasBounds)
                    {
                        bounds.Encapsulate(childRenderer.bounds);
                    }
                    else
                    {
                        bounds = childRenderer.bounds;
                        hasBounds = true;
                    }
                }
            }

            BoxCollider collider = (BoxCollider)m.GetComponent<Collider>();
            collider.center = bounds.center - m.transform.position;
            collider.size = bounds.size;
            Rigidbody rb = m.AddComponent<Rigidbody>() as Rigidbody;
            Debug.Log("bounds " + bounds.size);
            m.AddComponent<XRGrabInteractable>();
        }
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
        //GroupCollider(testManifest);
        FitToChildren(testManifest);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
