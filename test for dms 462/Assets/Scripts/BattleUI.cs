using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleUI : MonoBehaviour {

    public GameObject BattleMenuUI;
    public GameObject InventoryMenuUI;
    public GameObject SpecialMenuUI;

    public void BattleFight()
    {
        Debug.Log("Attacking enemy...");
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
}
