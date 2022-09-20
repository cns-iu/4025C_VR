using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandLiner : MonoBehaviour
{

    public GameObject buttonPressed;
    public Text commandText;
    public string command;
 

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void layerInfo()
    {
        commandText.text = command;           


    }

    public void columnInfo()
    {
        commandText.text = command;

    }

    public void rowInfo()
    {
        commandText.text = command;
        //commandLine.GetComponent<Text>().text = "command";

    }
}
