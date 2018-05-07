﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class player_movement : MonoBehaviour {
    public int playerspeed = 10;
    private bool facingright = false;
    public int playerjumppower = 250;
    private float movex;
    private Transform groundCheck;
    private bool isGrounded;

    [SerializeField] private LayerMask groundMask;

    // Use this for initialization
    void Start () {
        for (int i = 0; i < EnemiesToDestroy.enemyList.Count; ++i )
        {
            Destroy(GameObject.Find(EnemiesToDestroy.enemyList[i]));
        }
        groundCheck = transform.Find("GroundCheck");
        transform.position = new Vector2(PlayerStats.positionX, PlayerStats.positionY);
    }


    // Update is called once per frame
    void Update () {
        Playermove();
        isGrounded = Physics2D.OverlapCapsule(groundCheck.position, new Vector2(1f, 1f), CapsuleDirection2D.Horizontal, 0, groundMask);		
        if (SceneManager.GetActiveScene().name == "level1")
        {
            gameObject.SetActive(true);
        }
        else if (SceneManager.GetActiveScene().name == "RPGScene")
        {
            gameObject.SetActive(false);
        }

	}
    void Playermove(){
        //controls
        movex = Input.GetAxis("Horizontal");
        if (Input.GetKeyDown("space") && isGrounded) { jump(); }

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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy")
        {
            PlayerStats.positionX = this.transform.position.x;
            PlayerStats.positionY = this.transform.position.y;
            EnemiesToDestroy.currentEnemy = other.name;
            EnemiesToDestroy.enemyList.Add(other.name);
            SceneManager.LoadScene(1);
        }
    }
}
