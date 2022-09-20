using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MacroMaker : MonoBehaviour
{
    /*
    GameObject newStar;
    Vector2 entryPosition;
    Vector2 insertPosition;
    */

    /*
    public KeyCode activator;
    public List<macroEntry> macroList; // = new List<MacroEntry>();


    void Update()
    {
        if (Input.GetKeyDown(activator))
            MacroCreator.MacroExecuteFirst(macroList);

        if (PlayerSwitcher.autoFlag && ((PlayerSwitcher.autoMode == 1) || (PlayerSwitcher.autoMode == 3)))
            MacroCreator.MacroExecuteSecond(macroList);

        if (PlayerSwitcher.autoFlag && PlayerSwitcher.autoMode == 2)
            MacroCreator.MacroExecuteConnect(macroList);

        if ((MacroCreator.listPointer == macroList.Count) && PlayerSwitcher.autoFlag == true)
            MacroCreator.MacroExecuteEnd();
    }



  /*
  int listPointer;
  macroEntry currentEntry;
  */

  /*
  // Start is called before the first frame update
  void Start()
  {
      listPointer = 0;
      PlayerSwitcher.autoFlag = false;
      PlayerSwitcher.autoMode = 0;  //OFF
  }
  */


  /*
  // Update is called once per frame
  void Update()
  {

    // insert first listmember
    if (Input.GetKeyDown(activator))
    {
      currentEntry = macroList[listPointer];
      entryPosition = new Vector2 (currentEntry.posX, currentEntry.posY);
      newStar = Instantiate(currentEntry.type, entryPosition, Quaternion.identity);

      PlayerSwitcher.autoFlag = true;
      PlayerSwitcher.autoMode = 1;    //INSERT
      listPointer++;
    }

    // INSERT move, insert following list members
    if (PlayerSwitcher.autoFlag && ((PlayerSwitcher.autoMode == 1) || (PlayerSwitcher.autoMode == 3)))
    {
      currentEntry = macroList[listPointer];            // get entry
      entryPosition = new Vector2 (currentEntry.posX, currentEntry.posY);   // save requested position
      insertPosition = new Vector2 (8f,6f);                                 // this dummy position
      newStar = Instantiate(currentEntry.type, insertPosition, Quaternion.identity); // create star at dummy pos

      PlayerSwitcher.autoMode = 2;    //MOVE
      listPointer++;
    }

    // MOVE mode, move current star to final position
    if (PlayerSwitcher.autoFlag && PlayerSwitcher.autoMode == 2)
    {
      newStar.transform.position = entryPosition;
      PlayerSwitcher.autoMode = 3;  //CONNECT
      // if stars won't coolide then ConnectorController won't trigger and reset autoMode!
    }



    // check if we have reached the EOL
    if ((listPointer == macroList.Count) && PlayerSwitcher.autoFlag == true)
    {
      PlayerSwitcher.autoFlag = false;
      PlayerSwitcher.autoMode = 0;    // OFF
      listPointer = 0;
    }


      foreach (macroEntry entry in macroList)
        {

          entryPosition = new Vector2 (entry.posX, entry.posY);
          newStar = Instantiate(entry.type, entryPosition, Quaternion.identity);

        }
        */


        /*
        foreach (listEntry entry in myList)
        {
          entryPosition = new Vector2 (entry.posX, entry.posY);
          newStar = Instantiate(entry.type, entryPosition, Quaternion.identity);

        }
        */





}
