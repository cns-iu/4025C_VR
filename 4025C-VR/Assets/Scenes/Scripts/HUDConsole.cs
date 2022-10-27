using TMPro;
using UnityEngine;

// 2022-10-27

public class HUDConsole : MonoBehaviour {

    public ConController controllerScript;    //access to ConnectorController
    public SceneController sceneController;
    public TextMeshProUGUI buttonModeText;

    private const int MAX_SIZE = 2048;

    public TextMeshProUGUI textField;

    private string fullLog = string.Empty;


    private void Start() {
        Application.logMessageReceived += EventLogRecieved; 
    }


    private void OnDestroy() {
        Application.logMessageReceived -= EventLogRecieved;
    }


    public void ClearLog()
    {
        fullLog = string.Empty;
        textField.text = fullLog;
    }


    public void ModeSwitch()
    {
        if (controllerScript.nodeMode == false)
        {
            controllerScript.nodeMode = true;
            buttonModeText.text = "Nodes";
        }
        else
        {
            controllerScript.nodeMode = false;
            buttonModeText.text = "System";
        }
    }


    public void ResetPrefs()
    {
        Debug.Log("Before reset prefs: " +
            sceneController.testInt);

        sceneController.testInt = 0;

        Debug.Log("After reset prefs: " +
            sceneController.testInt);
    }


    private void EventLogRecieved(string pMessage, string pStackTrace, LogType pType) {
        fullLog = $"[{pType}] {pMessage}\n{fullLog}";
        if (fullLog.Length > MAX_SIZE) {
            fullLog = fullLog.Substring(0, MAX_SIZE);
        }
        textField.text = fullLog;
    }
}
