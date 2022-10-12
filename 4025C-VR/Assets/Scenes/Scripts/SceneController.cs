using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

// 2022-10-11


public class SceneController : MonoBehaviour
{
    public GameObject xrPosition;
    public GameObject positionAA;
    public GameObject positionMA;
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

    public void jumpToMA(GameObject c)
    {
        xrPosition.transform.position = positionMA.transform.position;
        GameObject manifestOriginal = controllerScript.manifest;
        controllerScript.conListStatusSet(manifestOriginal, bInitIgnore, null); // prevents instantiated nodes from being added to conList
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
                controllerScript.conStatusSet(croot, 0, controllerScript.matDefault);   // reset new croot
            }
            else
            {
                controllerScript.pDestroy(child.gameObject);
            }
        }

        // package copy into 0.1 scale XRInteractable
        // move to MA and scale
        manifestCopy.transform.position = transportTarget.transform.position;
        Vector3 objectScale = manifestCopy.transform.localScale;
        manifestCopy.transform.localScale = new Vector3(objectScale.x / 10, objectScale.y / 10, objectScale.z / 10);

        // todo package into BoxCollider
        groupCollider(manifestCopy);
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


    void groupCollider(GameObject m)
    {
        float maxX = 0, maxY = 0, maxZ = 0, minX = 0, minY = 0, minZ = 0;
        List<GameObject> conList = m.GetComponent<ManifestStatus>().conList;
        Debug.Log("groupCollider:");

        foreach (GameObject c in conList)
        {


            Debug.Log(c.name);
            Debug.Log(c.transform.position.x + " " + c.transform.position.y + " " + c.transform.position.z);
            Debug.Log(c.transform.localPosition.x + " " + c.transform.localPosition.y + " " + c.transform.localPosition.z);

            if (c.transform.position.x > maxX) maxX = c.transform.position.x;
            if (c.transform.position.y > maxY) maxY = c.transform.position.y;
            if (c.transform.position.z > maxZ) maxZ = c.transform.position.z;
            if (c.transform.position.x < minX) minX = c.transform.position.x;
            if (c.transform.position.y < minY) minY = c.transform.position.y;
            if (c.transform.position.z < minZ) minZ = c.transform.position.z;

        }
        manStatus.colliderMaxX = maxX;
        manStatus.colliderMaxY = maxY;
        manStatus.colliderMaxZ = maxZ;
        manStatus.colliderMinX = minX;
        manStatus.colliderMinY = minY;
        manStatus.colliderMinZ = minZ;

        BoxCollider bc = m.AddComponent<BoxCollider>() as BoxCollider;
        bc.size = new Vector3(maxX+Mathf.Abs(minX), maxY+Mathf.Abs(minY), maxZ+Mathf.Abs(minZ));

    }



    // Start is called before the first frame update
    void Start()
    {
        groupCollider(testManifest);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
