using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

//Attach this script to a GameObject. Make sure the GameObject has a BoxCollider component (Unity Cubes have these automatically as long as they weren’t removed).
//Create three Sliders ( Create>UI>Slider). These are for manipulating the x, y, and z values of the size.
//Click on the GameObject and attach each of the Sliders to the fields in the Inspector.
//In Play Mode, click the GameObject and enable Gizmos to visualize the BoxCollider.

using UnityEngine.UI;

public class Example : MonoBehaviour
{
    //Make sure there is a BoxCollider component attached to your GameObject
    BoxCollider m_Collider;
    public float m_ScaleX, m_ScaleY, m_ScaleZ;
    public Slider m_SliderX, m_SliderY, m_SliderZ;


    void Start()
    {
        //Fetch the Collider from the GameObject
        m_Collider = GetComponent<BoxCollider>();
        //These are the starting sizes for the Collider component
        m_ScaleX = 1.0f;
        m_ScaleY = 1.0f;
        m_ScaleZ = 1.0f;

        //Set all the sliders max values to 20 so the size values don't go above 20
        m_SliderX.maxValue = 20;
        m_SliderY.maxValue = 20;
        m_SliderZ.maxValue = 20;

        //Set all the sliders min values to 1 so the size don't go below 1
        m_SliderX.minValue = 1;
        m_SliderY.minValue = 1;
        m_SliderZ.minValue = 1;
    }

    void Update()
    {
     
    }

    /*
    void FitToChildren(GameObject p)
    {

        if (!(p.GetComponent<Collider>() is BoxCollider))
        {

            bool hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int i = 0; i < p.transform.childCount; ++i)
            {
                Renderer childRenderer = p.transform.GetChild(i).GetComponent<Renderer>();
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

            BoxCollider collider = (BoxCollider)p.GetComponent<Collider>();
            collider.center = bounds.center - p.transform.position;
            collider.size = bounds.size;
        }
        
    }
    */
    

    /*
    [MenuItem("My Tools/Collider/Fit to Children")]
    static void FitToChildren()
    {
        foreach (GameObject rootGameObject in Selection.gameObjects)
        {
            if (!(rootGameObject.collider is BoxCollider))
                continue;

            bool hasBounds = false;
            Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);

            for (int i = 0; i < rootGameObject.transform.childCount; ++i)
            {
                Renderer childRenderer = rootGameObject.transform.GetChild(i).renderer;
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

            BoxCollider collider = (BoxCollider)rootGameObject.collider;
            collider.center = bounds.center - rootGameObject.transform.position;
            collider.size = bounds.size;
        }
    */
}
