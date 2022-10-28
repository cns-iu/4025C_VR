using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonTests : MonoBehaviour
{
    
    public string starModel;
    public int starConnectors;
    public int starID;

    public string SaveToString()
    {
        return JsonUtility.ToJson(this);
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