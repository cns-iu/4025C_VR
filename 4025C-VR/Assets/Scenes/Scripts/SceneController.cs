using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2022-10-9


public class SceneController : MonoBehaviour
{
    public GameObject xrPosition;
    public GameObject positionAA;
    public GameObject positionMA;

    public void jumpToAA(GameObject c)
    {
        Debug.Log("jump to AA");
        //xrPosition.transform.position = new Vector3(90f, 0, 50f);
        xrPosition.transform.position = positionAA.transform.position;

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
