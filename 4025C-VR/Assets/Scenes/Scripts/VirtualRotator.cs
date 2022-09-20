using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// rotates the object based on slider in put

public class VirtualRotator : MonoBehaviour
{
    public GameObject rotator;
    public Slider slider;
    float xAngle, yAngle, zAngle;

    public void rotate()
    {
        yAngle = slider.value;
        rotator.transform.rotation = Quaternion.AngleAxis(yAngle, Vector3.up);
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
