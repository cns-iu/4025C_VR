using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;

public class NewBehaviourScript : MonoBehaviour
{

    //Declare a LindeRenderer to store the component attached to the GameObject
    [SerializeField] LineRenderer rend;

    Vector3[] points;
   
    //Start is called before the first frame update
    void Start()
    {
        //get the LineRenderer attached to the gameobject.
        rend = gameObject.GetComponent<LineRenderer>();

        //initialize the LineRenderer
        points = new Vector3[2];

        //set the start point of the linerenderer to the positiion of the gameobject
        points[0] = Vector3.zero;

        //set the end point 20 units away from the GO on the Z axis (pointing forward)
        points[1] = transform.position + new Vector3(0, 0, 20);

        //finally set the positions array on the LineRenderer to our new values
        rend.SetPositions(points);
        rend.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
