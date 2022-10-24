using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParentData : MonoBehaviour
{
    public string parentType;
    // Start is called before the first frame update
    void Awake()
    {
        // initialize parentYype from this object's name
        parentType = this.name;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
