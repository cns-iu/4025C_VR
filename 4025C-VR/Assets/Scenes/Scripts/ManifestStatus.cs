using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2022-11-25

public class ManifestStatus : MonoBehaviour
{
    public List<GameObject> conList;
    public List<GameObject> parentList;
    public Dictionary<GameObject, GameObject> connections;

    // Start is called before the first frame update
    void Awake() {
        if (name != "TestManifest") {
            connections = new Dictionary<GameObject, GameObject>();
            //parentList.Add(this.transform.GetChild(0).gameObject);
        } else {
            // populate connections list when running from desktop
       
            Debug.Log("Building test connections");
            // C0 of P1
            connections.Add(transform.GetChild(1).GetChild(0).gameObject, transform.GetChild(0).GetChild(0).gameObject);
            Debug.Log("first pair " + transform.GetChild(1).GetChild(0).gameObject.name + ":" + transform.GetChild(0).GetChild(0).gameObject.name);

            connections.Add(transform.GetChild(2).GetChild(0).gameObject, transform.GetChild(1).GetChild(1).gameObject);
            Debug.Log("first pair " + transform.GetChild(1).GetChild(0).gameObject.name + ":" + transform.GetChild(1).GetChild(1).gameObject.name);

            connections.Add(transform.GetChild(3).GetChild(0).gameObject, transform.GetChild(1).GetChild(2).gameObject);
            Debug.Log("first pair " + transform.GetChild(1).GetChild(0).gameObject.name + ":" + transform.GetChild(1).GetChild(2).gameObject.name);
        }
    }

}
