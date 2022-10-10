using System.Collections;
using System.Collections.Generic;
//using System.Numerics;
using UnityEngine;

// 2022-10-9


public class SceneController : MonoBehaviour
{
    public GameObject xrPosition;
    public GameObject positionAA;
    public GameObject positionMA;
    public GameObject transportTarget;

    public ConController controllerScript;  //access to ConnectorController
    

    public void jumpToAA(GameObject c)
    {
        Debug.Log("jumping to AA");
        xrPosition.transform.position = positionAA.transform.position;
    }

    public void jumpToMA(GameObject c)
    {
        Debug.Log("jumping to MA");
        xrPosition.transform.position = positionMA.transform.position;
        GameObject myObject = controllerScript.manifest;
        GameObject myCopy = Instantiate(myObject);
        myCopy.transform.position = transportTarget.transform.position;

        Vector3 objectScale = myCopy.transform.localScale;
        myCopy.transform.localScale = new Vector3(objectScale.x / 10, objectScale.y / 10, objectScale.z / 10);
    }


    public void jumpHover(GameObject c)
    {
        Debug.Log("jumpHover");
    }


    public void jumpHoverExited(GameObject c)
    {
        Debug.Log("jumpHoverExited");
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
