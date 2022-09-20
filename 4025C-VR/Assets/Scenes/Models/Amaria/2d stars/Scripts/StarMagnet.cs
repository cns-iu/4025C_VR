using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class StarMagnet : MonoBehaviour
{

  Collider2D m_ObjectCollider;


    // Start is called before the first frame update
    void Start()
    {
      //Fetch the GameObject's Collider (make sure they have a Collider component)
        m_ObjectCollider = GetComponent<Collider2D>();
        //Here the GameObject's Collider is not a trigger


        //m_ObjectCollider.isTrigger = false;
        //Output whether the Collider is a trigger type Collider or not
        Debug.Log("Trigger On : " + m_ObjectCollider.isTrigger);
        Debug.Log("this is new StarMagnet");

    }

    // Update is called once per frame
    void Update()

    {

    }

    void OnCollisionEnter2D(Collision2D other)
  	{
  		//StarMagnet otherMagnet = other.gameObject.GetComponent<StarMagnet>();

      Debug.Log("in collision detector " + other.gameObject.name);

      /*
  		if (otherMagnet != null)
  		{
  			//player.ChangeHealth(-1);
        Debug.Log("starmagnet collision detected");
  		}
      */

  	}
}
