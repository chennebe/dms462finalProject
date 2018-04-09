using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player_movement : MonoBehaviour {
    public int playerspeed = 10;
    private bool facingright = false;
    public int playerjumppower = 250;
    private float movex;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Playermove();
		
	}
    void Playermove(){
        //controls
        movex = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown("space")) { jump(); }

            //direction
        if (movex < 0.0f && facingright == false){
            Flipplayer();
        }
        else if (movex > 0.0f && facingright == true){
            Flipplayer();
        }
        


        //physics 
        gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(movex * playerspeed, gameObject.GetComponent<Rigidbody2D>().velocity.y);

    }
    void jump(){
        GetComponent<Rigidbody2D>().AddForce (Vector2.up * playerjumppower);
    }
    void Flipplayer(){
        facingright = !facingright;
        Vector2 localscale = gameObject.transform.localScale;
        localscale.x *= -1;
        transform.localScale = localscale;

    }
}
