using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarController : MonoBehaviour
{

    public GameObject thisStar;
    public GameObject thisConnector;
    public GameObject thisV;
    public GameObject previousStar;
    public GameObject previousConnector;

    //public GameObject activeConnector;
    public float offsetConstant;



    //Vector3 mousePos3d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    // Start is called before the first frame update
    void Start()
    {
        offsetConstant = 0.03836f;

    }

    // Update is called once per frame
    void Update()
    {


        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {

                if (hit.collider.tag == "connector")
                {
                    Debug.Log("hit " + hit.transform.name + ": " + hit.collider.transform.name);

                    Renderer mr = hit.collider.transform.GetComponent<Renderer>();

                    //thisStar = hit.transform.gameObject;
                    thisConnector = mr.gameObject;
                    thisStar = thisConnector.transform.parent.gameObject;
                    thisV = thisConnector.transform.Find("vector").gameObject;

                    // on click....
                    if (thisStar == previousStar)
                    {
                        // same connector?
                        if (thisConnector == previousConnector)
                        {
                            thisConnector.GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f, 1f);

                            allClear();
                        }
                        else
                        {
                            previousConnector.GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f, 1f);
                            thisConnector.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 1f);

                            previousStar = thisStar;
                            previousConnector = thisConnector;

                        }

                    }
                    else
                    {
                        // previous star == null?
                        if (previousStar == null)
                        {
                            thisConnector.GetComponent<MeshRenderer>().material.color = new Color(1f, 0f, 0f, 1f);

                            previousStar = thisStar;
                            previousConnector = thisConnector;
                            //activeStar = thisStar;
                            //activeConnector = thisConnector;
                        }
                        else
                        {
                            // connection...
                            previousConnector.GetComponent<MeshRenderer>().material.color = new Color(0f, 1f, 0f, 1f);
                            Debug.Log("connecting...");
                            //Debug.Log(thisV.transform.localPosition);

                            //previousStar.GetComponent<Rigidbody>().useGravity = false;
                            previousStar.transform.parent = thisConnector.transform;

                            previousStar.transform.localPosition = new Vector3(0, 0, 0);
                            //Vector3 connectorOffset = new Vector3(0, 0, 0);


                            //previousStar.transform.localPosition = thisV.transform.localPosition;
                            //previousStar.transform.localPosition = thisV.transform.localPosition;
                            previousStar.transform.localRotation = Quaternion.identity;


                            allClear();
                        }

                    }

                }

            }
        }

    }

    void allClear()
    {
        thisConnector = null;
        thisStar = null;
        previousConnector = null;
        previousStar = null;
        //activeConnector = null;
        //activeStar = null;
    }
}
