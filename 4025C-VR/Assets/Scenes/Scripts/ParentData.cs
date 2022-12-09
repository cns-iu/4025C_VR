using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2022-12-8

public class ParentData : MonoBehaviour
{
    public string parentType;
    // Start is called before the first frame update
    void Awake()
    {
        // initialize parentYype from this object's name if in library
        if (this.transform.parent != null)
        {
            //if (this.transform.parent.gameObject.name == "Library") parentType = this.name;
            if (this.transform.parent.gameObject.GetComponent<ManifestStatus>().isLibrary == true) parentType = this.name;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
