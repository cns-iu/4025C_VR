using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2022-11-27
// saves a list of manifest names

public class JsonManifests : MonoBehaviour
{
    public List<string> manifests;

    public string SaveToString() {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json) {  
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }
}