using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerSwitcher : MonoBehaviour
{

	public GameObject StarUS2;
	public GameObject StarUS3;
	public GameObject StarUS4;
	public GameObject StarUS5;
	public GameObject StarUS6;
	public GameObject StarUS7;
	public GameObject StarUS8;
	public GameObject StarUS9;
	public GameObject StarUS10;

	public GameObject StarVS2short;
	public GameObject StarVS2;
	public GameObject StarVS2long;
	public GameObject StarVS3;
	public GameObject StarVS4;
	public GameObject StarVS5;
	public GameObject StarVS6;
	public GameObject StarVS7;
	public GameObject StarVS8;
	public GameObject StarVS9;
	public GameObject StarVS10;


	GameObject requestedStar;
	Rigidbody2D movingStar;
	AudioSource starTone;


//--mode flags
// use static to access from other scripts
	public bool firstStarMode;		// first star gets fixed position
	public bool starCreationMode; 	// this is true during new star deployment
	public static bool starMoveMode; 			// this is true during star movement
	public static bool mouseDown;					// right after star havingbeem dropped
	public static bool collisionStopper;	// used by ConnectorController to prevent parent collider

// macro operations flags
	public static bool autoFlag = true;		// bypassing any snap connections!!
	public static int autoMode;

	public bool showInstructions;
	GameObject instructionsPanel;

	GameObject newStar;
	public GameObject pMarker;
	GameObject newPMarker;

	//public GameObject [] starArray;

	public Vector2 entryPosition;
	public int starType;

	void Start()
	{
		firstStarMode = true;
		starCreationMode = false;
		starMoveMode =false;
		mouseDown = false;
		showInstructions = true;
		instructionsPanel = GameObject.Find("Instructions");
		collisionStopper = false;
	}


	void Update ()
	{

		//print("PlayerSwitcher Update");

		// get mouse position for this update loop
		Vector3 mousePos3d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePos2d = new Vector2 (mousePos3d.x, mousePos3d.y);

		//A click over star to move
		if (Input.GetMouseButtonDown(0) && !starCreationMode && !starMoveMode && !mouseDown)
		{
			//print("update A, click over star to move");

			RaycastHit2D hit = Physics2D.Raycast(mousePos2d, Vector2.zero);

			// only detect star click NOT connectors
			if ((hit.collider != null) && (hit.collider.tag == "objectStar"))
			{
				starMoveMode = true;
				mouseDown = true;

				movingStar = hit.rigidbody;
				movingStar.isKinematic = false;
			}
		}


		//B  move the current star
		if (!Input.GetMouseButton(0) && !starCreationMode && starMoveMode && !mouseDown)
		{
			//print("B: move current star");

			// remove this star from any hierarchy if it is a child
			if (movingStar.transform.parent != null)
			{
				movingStar.transform.parent = null;
			}

			movingStar.MovePosition(mousePos2d);
		}


		//C drop current star
		if (Input.GetMouseButtonDown(0) && !starCreationMode && starMoveMode && !mouseDown)
		{
			//print("C: star drop request for" + movingStar);
			movingStar.isKinematic = true;
			starMoveMode = false;

			movingStar.velocity = Vector3.zero;
			movingStar.transform.rotation = Quaternion.identity;
			movingStar.angularVelocity = 0;
		}


		//D  mouse button up
		if (Input.GetMouseButtonUp(0) && mouseDown)
		{
			//print("D: mouse button up");
			mouseDown = false;
		}


		//E  new star is attached to mouse pointer until next click
		if (!Input.GetMouseButton(0) && starCreationMode)
		{
			// print("moving new star " + newStar);
			//newStar.GetComponent<Rigidbody2D>().MovePosition(mousePos2d);
		}


		//F  exit star creation mode
		if (Input.GetMouseButtonDown(0) && starCreationMode)
		{
			//print("F: exit star creation mode");
			newStar.GetComponent<Rigidbody2D>().isKinematic = true;
			starCreationMode = false;
		}


		//G star deletion stuff
		if (Input.GetMouseButton(1))
		{
			//print("G: star deletion stuff");
			RaycastHit2D hit = Physics2D.Raycast (mousePos2d, Vector2.zero);

			if (hit.collider != null)


			{
				// firstStarMode = true; // only if last star was deleted
				hit.collider.gameObject.SendMessage("destroyStar");
				this.GetComponent<AudioSource>().Play();

				//GameObject dummyObj = GameObject.FindWithTag("objectStar");
				if (GameObject.FindWithTag("objectStar") == null)
				{
					print("last star deleted");
				}
			}
		}

		//H toggle instructions flag via SPACE
		if (Input.GetKeyDown(KeyCode.Space))
		{
				if (showInstructions)
					showInstructions = false;
				else
					showInstructions = true;
		}

		//H1 toggle instructions text according to flag
		if (showInstructions)
					instructionsPanel.SetActive(true);
		else
					instructionsPanel.SetActive(false);

		//I star creation via keypress - uniform stars
		if (Input.GetKeyDown("2"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				CreateStar(StarVS2);
			else
				CreateStar(StarUS2);
		}

		if (Input.GetKeyDown("q") && Input.GetKey(KeyCode.LeftShift))
			CreateStar(StarVS2short);


		if (Input.GetKeyDown("w") && Input.GetKey(KeyCode.LeftShift))
			CreateStar(StarVS2long);


		if (Input.GetKeyDown("3"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				CreateStar(StarVS3);
			else
				CreateStar(StarUS3);
		}

		if (Input.GetKeyDown("4"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				CreateStar(StarVS4);
			else
				CreateStar(StarUS4);
		}

		if (Input.GetKeyDown("5"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				CreateStar(StarVS5);
			else
				CreateStar(StarUS5);
		}

		if (Input.GetKeyDown("6"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				CreateStar(StarVS6);
			else
				CreateStar(StarUS6);
		}

		if (Input.GetKeyDown("7"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				CreateStar(StarVS7);
			else
				CreateStar(StarUS7);
		}

		if (Input.GetKeyDown("8"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				CreateStar(StarVS8);
			else
				CreateStar(StarUS8);
		}

		if (Input.GetKeyDown("9"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				CreateStar(StarVS9);
			else
				CreateStar(StarUS9);
		}

		if (Input.GetKeyDown("0"))
		{
			if (Input.GetKey(KeyCode.LeftShift))
				CreateStar(StarVS10);
			else
				CreateStar(StarUS10);
		}
	}


	//---these are being called from Canvas
	//---create new star
	public void CreateUs2 ()
	{
		CreateStar(StarUS2);
	}

	public void CreateUs3 ()
	{
		CreateStar(StarUS3);
	}

	public void CreateUs4 ()
	{
		CreateStar(StarUS4);
	}

	public void CreateUs5 ()
	{
		CreateStar(StarUS5);
	}

	public void CreateUs6 ()
	{
		CreateStar(StarUS6);
	}

	public void CreateUs7 ()
	{
		CreateStar(StarUS7);
	}

	public void CreateUs8 ()
	{
		CreateStar(StarUS8);
	}

	public void CreateUs9 ()
	{
		CreateStar(StarUS9);
	}

	public void CreateUs10 ()
	{
		CreateStar(StarUS10);
	}

	public void CreateInfo ()
	{
		print("info requested");
	}


	//--enter star creation mode
	public void CreateStar(GameObject requestedStar)
	{

		if (!firstStarMode)
		{
			// get mouse position
			Vector3 mousePos3d = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2d = new Vector2 (mousePos3d.x, mousePos3d.y);

			//newStar = Instantiate(requestedStar, new Vector2(-16,7), Quaternion.identity);
			newStar = Instantiate(requestedStar, mousePos2d, Quaternion.identity);
			starCreationMode = true;
			newStar.GetComponent<Rigidbody2D>().MovePosition(mousePos2d);

		}
		else
		{
			// insert first star in center and set flags to fix it in position
			newStar = Instantiate(requestedStar, new Vector2(0,0), Quaternion.identity);

			firstStarMode = false;		// this is in this script!
			newStar.GetComponent<Rigidbody2D>().isKinematic = true;				// flags on the newly created star
			// newStar.GetComponent<StarController>().fixedStarFlag = true;
			newPMarker = Instantiate(pMarker, new Vector2(0,0), Quaternion.identity);
			newPMarker.transform.parent = newStar.transform;
		}
		starTone = newStar.GetComponent<AudioSource>();
		starTone.Play();

	}


}
