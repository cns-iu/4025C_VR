using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CubePusher : MonoBehaviour
{
    public Text layerText;
    public Text columnText;
    public Text rowText;
    string cubeAddress;
    GameObject theCube;
    float yDisplacement;

    // pushes cube out
    public void pushCube()
    {
        //synthesize address name
        cubeAddress = layerText.text + "_" + columnText.text + "_" + rowText.text;
        theCube = GameObject.Find(cubeAddress);

        if (layerText.text == "Top")
        {
            yDisplacement = 0.02f;
        } else
        {
            yDisplacement = -0.02f;
        }

        if (theCube.tag == "Untagged")
        {
            theCube.transform.Translate(0.0f, yDisplacement, 0.0f);
            theCube.tag = "Selected";
        }
    }

    // pulls cube back in
    public void pullCube()
    {
        //synthesize address name
        cubeAddress = layerText.text + "_" + columnText.text + "_" + rowText.text;
        theCube = GameObject.Find(cubeAddress);

        if (layerText.text == "Top")
        {
            yDisplacement = -0.02f;
        } else
        {
            yDisplacement = 0.02f;
        }

        if (theCube.tag == "Selected")
        {
            theCube.transform.Translate(0.0f, yDisplacement, 0.0f);
            theCube.tag = "Untagged";
        }
    

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
