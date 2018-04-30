using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleUI : MonoBehaviour {

    public GameObject BattleMenuUI;
    public GameObject InventoryMenuUI;
    public GameObject SpecialMenuUI;
    public GameObject enemyObj;
    EnemyStateMachine eSM;

    void Start()
    {
        enemyObj = GameObject.Find("Enemy");
        eSM = (EnemyStateMachine)enemyObj.GetComponent(typeof(EnemyStateMachine));

        PlayerStats.Health = 100;
        PlayerStats.Attack = 10;
        PlayerStats.Defense = 10;
        PlayerStats.Speed = 10;
    }

    public enum TurnState
    {
        PLAYERTURN,
        ENEMYTURN,
        VICTORY,
        GAMEOVER
    }

    public TurnState currentState;

    public void BattleFight()
    {
        Debug.Log("Attacking enemy...");
        // Affect enemy HP based on player's attack and enemy defense.
        eSM.enemy.HP -= PlayerStats.Attack;
        currentState = TurnState.ENEMYTURN;
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
        SceneManager.LoadScene(0);
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
        eSM.Attack();
        currentState = TurnState.PLAYERTURN;
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

        switch (currentState)
        {
            case (TurnState.PLAYERTURN):
                BattleMenuUI.SetActive(true);
                Debug.Log("Player's turn");
                break;
            case (TurnState.ENEMYTURN):
                BattleMenuUI.SetActive(false);
                Debug.Log("Enemy's turn");
                EnemyAction();
                break;
            case (TurnState.VICTORY):
                Debug.Log("Battle won, returning to overworld...");
                SceneManager.LoadScene(0);
                break;
            case (TurnState.GAMEOVER):
                Debug.Log("Game Over.");
                break;
        }

    }
}
