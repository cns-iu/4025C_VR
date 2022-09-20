using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NacroUni : MonoBehaviour
{

    public KeyCode activator;
    public List<nacroEntry> nacroList;

    void Update()
    {
        if (Input.GetKeyDown(activator))
        {
            MacroCreator.nacroListCurrent = nacroList;
            MacroCreator.NacroExecuteZero();
        }

    }

}
