using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideShow : MonoBehaviour
{

    public GameObject element;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
       // toggleActive();
    }

    // Update is called once per frame
    public void toggleActive()
    {
     
        if (element.activeSelf)
        {
            element.SetActive(false);
        }
        else
        {
            element.SetActive(true);

        }
        //element.SetActive(true);
    }
}
