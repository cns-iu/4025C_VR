using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//2022-12-2

public class PaperBasket : MonoBehaviour
{

    public SceneController sceneController;  //access to ConnectorController

    private void OnTriggerEnter(Collider other)
    {
        if (sceneController.manifestList != null)
        { 
            // check if other is on manifestList
            foreach (GameObject m in sceneController.manifestList)
            {
                if (other.gameObject == m)
                {
                    sceneController.manifestList.Remove(m);  
                    Destroy(m);
                    return;     // outa here before next iteration!!
                }
            }
        } 
    }
}
