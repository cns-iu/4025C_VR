using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// All this does at the moment is provide the public struct

[System.Serializable]
public struct macroEntry  // this struct is available globally
{
    public GameObject type;
    public float posX;
    public float posY;
}


[System.Serializable]   // available globally, for test
public struct nacroEntry
{
    public GameObject starType;
    public int c1child;
    public int c1connector;
    public int c2child;
    public int c2connector;
    public int c3child;
    public int c3connector;
    public int c4child;
    public int c4connector;
}


public class MacroCreator : MonoBehaviour
{
    static GameObject newStar;
    static GameObject currentStar;
    static Vector2 entryPosition;
    static Vector2 insertPosition;

    public static int listPointer;
    static macroEntry currentMacroEntry;
    static nacroEntry currentNacroEntry;
    static nacroEntry currentChild;

    public static List<macroEntry> macroListCurrent;
    public static List<nacroEntry> nacroListCurrent;

    public static int nacroStateFlag = 0;	//INIT (enable NacroExecuteZero)
    public static bool nacroAutoFlag = false;
    string childVconnector;

    void Update()
    {
    //---NACRO execution
        if (nacroStateFlag == 1)
            NacroExecuteFirst();

        if (nacroStateFlag == 2)
            NacroExecuteBranches();

    //---------MAcro execution
    /*
        if (PlayerSwitcher.autoFlag && ((PlayerSwitcher.autoMode == 1) || (PlayerSwitcher.autoMode == 3)))
            MacroExecuteSecond();

        if (PlayerSwitcher.autoFlag && PlayerSwitcher.autoMode == 2)
            MacroExecuteConnect();

        if(listPointer != 0)
        {
            if ((MacroCreator.listPointer == macroListCurrent.Count) && PlayerSwitcher.autoFlag == true)
                MacroExecuteEnd();
        }
        */
    }


    // start of list, executed once!
    public static void NacroExecuteZero()
    {
        listPointer = 0;

        currentNacroEntry = nacroListCurrent[listPointer++];
        entryPosition = new Vector2 (0f,0f);
        currentStar = Instantiate(currentNacroEntry.starType, entryPosition, Quaternion.identity);

        nacroStateFlag = 2;  // BRANCHES enable NacroExecuteParent
        nacroAutoFlag = true;
    }


    // next element of list, executed when listPointer is increased
    // this will note instantiate a new star!
    public void NacroExecuteFirst()
    {
        if (listPointer == nacroListCurrent.Count)
        {
            nacroStateFlag = 0;
            listPointer = 0;
        }
        else
        {
            currentNacroEntry = nacroListCurrent[listPointer++];  // update current entry
            nacroStateFlag = 2;  // BRANCHES enable NacroExecuteParent
        }
    }



    // executed on nacroStateFlag=2
    public void NacroExecuteBranches()
    {
        if (currentNacroEntry.c1child != 0)
        {
            ExecuteChild(currentNacroEntry.c1child, currentNacroEntry.c1connector, "v1");
            currentNacroEntry.c1child = 0;
        }

        if (currentNacroEntry.c2child != 0)
        {
            ExecuteChild(currentNacroEntry.c2child, currentNacroEntry.c2connector,"v2");
            currentNacroEntry.c2child = 0;
        }

        if (currentNacroEntry.c3child != 0)
        {
            ExecuteChild(currentNacroEntry.c3child, currentNacroEntry.c3connector, "v3");
            currentNacroEntry.c3child = 0;
        }

        if (currentNacroEntry.c4child != 0)
        {
            ExecuteChild(currentNacroEntry.c4child, currentNacroEntry.c4connector, "v4");
            currentNacroEntry.c4child = 0;
        }
        nacroStateFlag = 1;     // FIRST enable NacroExecuteFirst)
    }


    public void NacroExecuteConnect()
    {
    }





    // executed when nacroStateFlag = 2
    public void ExecuteChild(int childID, int childConnector, string vConnector)
    {
        Transform parentVtransform;
        Transform childVtransform;


        parentVtransform = currentStar.transform.Find(vConnector).transform;
        print("Connector: "+vConnector+" "+parentVtransform.position.x+", "+parentVtransform.position.y);

        // here we have X/Y of connector from parent connectors

        if (childConnector == 1)
            childVconnector = "v1";
        if (childConnector == 2)
            childVconnector = "v2";
        if (childConnector == 3)
            childVconnector = "v3";
        if (childConnector == 4)
            childVconnector = "v4";

        currentChild = nacroListCurrent[childID];
        childVtransform = currentChild.starType.transform.Find(childVconnector).transform;
        print ("childVconnector :"+childVconnector+" "+childVtransform.localPosition.x+", "+childVtransform.localPosition.y);

        entryPosition = new Vector2 (
                    parentVtransform.position.x - childVtransform.localPosition.x,
                    parentVtransform.position.y - childVtransform.localPosition.y);
        /*
        entryPosition = new Vector2 (
                    childVtransform.localPosition.x + parentVtransform.localPosition.x,
                    childVtransform.localPosition.y + parentVtransform.localPosition.y);
                    */

        //entryPosition = new Vector2 (parentTransform.position.x, parentTransform.position.y);
        newStar = Instantiate(currentChild.starType, entryPosition, Quaternion.identity);

    }



    //------this is the MACRO section----------
    public static void MacroExecuteFirst()
    {
        listPointer = 0;
        PlayerSwitcher.autoFlag = false;
        PlayerSwitcher.autoMode = 0;  //OFF

        currentMacroEntry = macroListCurrent[listPointer];
        entryPosition = new Vector2 (currentMacroEntry.posX, currentMacroEntry.posY);
        newStar = Instantiate(currentMacroEntry.type, entryPosition, Quaternion.identity);

        PlayerSwitcher.autoFlag = true;
        PlayerSwitcher.autoMode = 1;    //INSERT
        listPointer++;
    }

    public void MacroExecuteSecond()
    {
        // INSERT move, insert following list members
        currentMacroEntry = macroListCurrent[listPointer];            // get entry
        entryPosition = new Vector2 (currentMacroEntry.posX, currentMacroEntry.posY);   // save requested position
        insertPosition = new Vector2 (8f,6f);                                 // this dummy position
        newStar = Instantiate(currentMacroEntry.type, insertPosition, Quaternion.identity); // create star at dummy pos

        PlayerSwitcher.autoMode = 2;    //MOVE
        listPointer++;
    }

    public void MacroExecuteConnect()
    {
        newStar.transform.position = entryPosition;
        PlayerSwitcher.autoMode = 3;  //CONNECT
    }

    public void MacroExecuteEnd()
    {
            PlayerSwitcher.autoFlag = false;
            PlayerSwitcher.autoMode = 0;    // OFF
            listPointer = 0;
    }

}
