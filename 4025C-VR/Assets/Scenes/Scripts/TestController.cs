using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestController : MonoBehaviour
{
    public GameObject thisObject;
    //public Transform child;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void OnTriggerEnter(Collider otherObject)
    {
        Debug.Log(thisObject.name + " colliding with " + otherObject.name);

        // other needs to become child of this
        if (otherObject.gameObject.layer != LayerMask.NameToLayer("Stationary"))
        {
            //thisObject.transform.SetParent(otherObject.gameObject.transform);
            //Debug.Log(thisObject.name + " is now child of " + otherObject.name);
            //otherObject.gameObject.transform.SetParent(thisObject.transform);
            thisObject.transform.SetParent(otherObject.gameObject.transform);

            Debug.Log(otherObject.name + " is now child of " + thisObject.name);
        }

    }
}
