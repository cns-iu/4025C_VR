using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {

	void Update ()
	{


		if (Input.GetMouseButtonDown(0))
		{

			Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePos2d = new Vector2 (mousePos.x, mousePos.y);

			RaycastHit2D hit = Physics2D.Raycast (mousePos2d, Vector2.zero);

			if (hit.collider != null)
			{
				Debug.Log(hit.collider.gameObject.name);

			}

		}
	}

}
