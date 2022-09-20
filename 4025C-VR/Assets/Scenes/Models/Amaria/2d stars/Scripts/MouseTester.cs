using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MouseTester : MonoBehaviour
{


//--mode flags
	// public bool firstStarMode = false;			// first star gets fixed position
	// public bool starCreationMode; // this is true during new star deployment
	public bool starMoveMode; 		// this is true during star movement
	public bool mouseDown;
	// public bool starDroppedMode = true;	// right after star havingbeem dropped


	void Start()
	{
		starMoveMode = false;
		mouseDown = false;
	}


	void Update ()
	{

		//A ----star movement stuff
		// clicked over star to move?
		if (Input.GetMouseButtonDown(0) && !starMoveMode && !mouseDown)
		{
			print("A clicked to move star (raycast) picked up for moving");
			starMoveMode = true;
			mouseDown = true;

		}

		//B move the current star
		if (!Input.GetMouseButton(0) && starMoveMode && !mouseDown)
		{
			print("B move current star");
		}

		//C drop currently moving star
		if (Input.GetMouseButtonDown(0) && starMoveMode && !mouseDown)
		{
			print("C drop current star");
			starMoveMode = false;

		}

		//D mouse button up
		if (Input.GetMouseButtonUp(0) && mouseDown)
		{
			print("D mouse button up");
			mouseDown = false;
		}

//------star creation stuff
		//E1 exit star creation mode, click(0)
		// if (Input.GetMouseButtonDown(0) && starCreationMode)
		// {
		// 	print("E1 exit star creation mode");
		//
		// }
		//
		// //E2 new star is attached to mouse pointer until next click
		// if(!firstStarMode && starCreationMode)
		// {
		// 	print("E2 moving new star");
		//
		// }
		//
		//F --star deletion stuff
		if (Input.GetMouseButton(1))
		{
			print("F destroy star clicked on");
		}

	}



}
