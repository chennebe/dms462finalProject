using System.Collections;
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
    private int poison;
    private bool poisonCheck;

    [SerializeField] private LayerMask groundMask;

    // Use this for initialization
    void Start () {
        PlayerStats.Health = 20;
        poison = 15;
        for (int i = 0; i < EnemiesToDestroy.enemyList.Count; ++i )
        {
            Destroy(GameObject.Find(EnemiesToDestroy.enemyList[i]));
        }
        poisonCheck = false;
        groundCheck = transform.Find("GroundCheck");
        transform.position = new Vector2(PlayerStats.positionX, PlayerStats.positionY);
    }


    // Update is called once per frame
    void Update () {
        if (poisonCheck)
        {
            --poison;
            if(poison <= 0)
            {
                poison = 15;
                --PlayerStats.Health;
            }
        }
        if(PlayerStats.Health <= 0)
        {
            SceneManager.LoadScene(0);
        }
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

            EnemyScript currentE = (EnemyScript)other.gameObject.GetComponent(typeof(EnemyScript));

            CurrentEnemy.Name = currentE.enemy.name;
            CurrentEnemy.Index = currentE.enemy.index;
            CurrentEnemy.Attack = currentE.enemy.ATK;
            CurrentEnemy.Defense = currentE.enemy.DEF;
            CurrentEnemy.Speed = currentE.enemy.SPE;
            CurrentEnemy.Health = currentE.enemy.HP;
            
            SceneManager.LoadScene(1);
        }
        if(other.tag == "spike")
        {
            print("a");
            PlayerStats.Health = 0;
        }
        if(other.tag == "poison")
        {
            poisonCheck = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag == "poison")
        {
            poisonCheck = false;
        }
    }
}
