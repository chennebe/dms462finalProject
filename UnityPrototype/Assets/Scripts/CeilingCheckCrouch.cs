using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeilingCheckCrouch : MonoBehaviour {

    GameObject p;
    Player player;
    BoxCollider2D ceilingCollider;
    int triggerOn = 0;

	// Use this for initialization
	void Start () {
        p = GameObject.Find("Player");
        player = p.GetComponent<Player>();
        ceilingCollider = GetComponent<BoxCollider2D>();
	}

    void FixedUpdate()
    {
        if (player.crouching)   // If the player is crouching, activate the boxCollider2D trigger.
        {
            ceilingCollider.enabled = true;
        }
        else // Else, turn off the trigger.
        {
            ceilingCollider.enabled = false;
        }

        if (triggerOn == 0)
        {
            player.crouchCanStand = true;
        }
        else
        {
            player.crouchCanStand = false;
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag != "Passable" && other.gameObject.layer == 9) // If the object within the trigger is not passable, and is part of the level...
        {
            triggerOn = 1;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        triggerOn = 0;
    }
}
