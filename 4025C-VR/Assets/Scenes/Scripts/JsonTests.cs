using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 2022-11-6

public class JsonTests : MonoBehaviour
{
    public List<string> parents;

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
    }

    public void LoadFromJson(string a_Json)
    {
        parents.Clear();
        JsonUtility.FromJsonOverwrite(a_Json, this);
    }

    /*
    public void Start()
    {
        playerName = "Peter Dummkopf";
        lives = 111;
        health = 12.8f;

        string wtf = SaveToString();
        Debug.Log(wtf);
    }
    */

    

    // Given:
    // playerName = "Dr Charles"
    // lives = 3
    // health = 0.8f
    // SaveToString returns:
    // {"playerName":"Dr Charles","lives":3,"health":0.8}
}