using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour {

    public GameObject BattleMenuUI;
    public GameObject InventoryMenuUI;
    public GameObject SpecialMenuUI;
    public GameObject DialogueMenuUI;
    private GameObject enemyObj;

    public Sprite gndImg, flyImg, bossImg;

    public Text message;
    private string text;

    EnemyStateMachine eSM;
    bool canPassDialogue = false;
    TurnState nextState;

    void Start()
    {
        enemyObj = GameObject.Find("Enemy");
        eSM = (EnemyStateMachine)enemyObj.GetComponent(typeof(EnemyStateMachine));

        if (CurrentEnemy.Index == 0)
        {
            enemyObj.GetComponent<SpriteRenderer>().sprite = gndImg;
        }
        else if (CurrentEnemy.Index == 1)
        {
            enemyObj.GetComponent<SpriteRenderer>().sprite = flyImg;
        }
        else
        {
            enemyObj.GetComponent<SpriteRenderer>().sprite = bossImg;
        }

        eSM.enemy.HP = CurrentEnemy.Health;
        eSM.enemy.ATK = CurrentEnemy.Attack;
        eSM.enemy.DEF = CurrentEnemy.Defense;
        eSM.enemy.SPE = CurrentEnemy.Speed;

        message = DialogueMenuUI.GetComponentInChildren<Text>();

        // This is for testing purposes.
        PlayerStats.Health = 100;
        PlayerStats.Attack = 10;
        PlayerStats.Defense = 10;
        PlayerStats.Speed = 10;
    }

    public enum TurnState
    {
        PLAYERTURN,
        ENEMYTURN,
        PROCESSING,
        VICTORY,
        GAMEOVER
    }

    public TurnState currentState;

    public void BattleFight()
    {
        Debug.Log("Attacking enemy...");
        text = "The Player attacks " + eSM.enemy.name + " for " + PlayerStats.Attack + " damage! (Press space to continue)";
        currentState = TurnState.PROCESSING;
        // Affect enemy HP based on player's attack and enemy defense.
        eSM.enemy.HP -= PlayerStats.Attack;
        nextState = TurnState.ENEMYTURN;
        canPassDialogue = true;
    }

    public void BattleInventory()
    {
        Debug.Log("Loading Inventory Menu...");
        BattleMenuUI.SetActive(false);
        InventoryMenuUI.SetActive(true);
    }
    
    public void BattleSpecial()
    {
        Debug.Log("Loading Special Moves Menu...");
        BattleMenuUI.SetActive(false);
        SpecialMenuUI.SetActive(true);
    }

    public void BattleEscape()
    {
        Debug.Log("Escaping...");
        SceneManager.LoadScene(1);
    }

    public void InventoryBack()
    {
        Debug.Log("Exiting Inventory Menu...");
        InventoryMenuUI.SetActive(false);
        BattleMenuUI.SetActive(true);
    }

    public void SpecialBack()
    {
        Debug.Log("Exiting Special Menu...");
        SpecialMenuUI.SetActive(false);
        BattleMenuUI.SetActive(true);
    }

    void EnemyAction()
    {
        text = "The Player took " + eSM.enemy.ATK + " damage from the enemy! (Press space to continue)";
        currentState = TurnState.PROCESSING;
        eSM.Attack();
        nextState = TurnState.PLAYERTURN;
        canPassDialogue = true;
    }

    void PrintMessage(string text)
    {
        message.text = text;
    }

    void PassDialogue(TurnState tS)
    {
        currentState = tS;
        canPassDialogue = false;
    }

    void Update()
    {
        if (eSM.enemy.HP <= 0)
        {
            currentState = TurnState.VICTORY;
        }
        if (PlayerStats.Health <= 0)
        {
            currentState = TurnState.GAMEOVER;
        }

        if (canPassDialogue && Input.GetKeyDown(KeyCode.Space))
        {
            PassDialogue(nextState);
        }

        switch (currentState)
        {
            case (TurnState.PLAYERTURN):
                DialogueMenuUI.SetActive(false);
                BattleMenuUI.SetActive(true);
                Debug.Log("Player's turn");
                break;
            case (TurnState.ENEMYTURN):
                BattleMenuUI.SetActive(false);
                Debug.Log("Enemy's turn");
                EnemyAction();
                break;
            case (TurnState.PROCESSING):
                BattleMenuUI.SetActive(false);
                DialogueMenuUI.SetActive(true);
                PrintMessage(text);
                break;
            case (TurnState.VICTORY):
                Debug.Log("Battle won, returning to overworld...");
                SceneManager.LoadScene(1);
                break;
            case (TurnState.GAMEOVER):
                Debug.Log("Game Over.");
                break;
        }

    }
}
