using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacroUni : MonoBehaviour
{

    public KeyCode activator;
    public List<macroEntry> macroList;

    void Update()
    {

        if ((Input.GetKeyDown(activator)) && (MacroCreator.nacroStateFlag == 0))
        {
            MacroCreator.macroListCurrent = macroList;
            MacroCreator.MacroExecuteFirst();
        }

    }

}
