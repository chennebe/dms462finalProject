using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour {

    public BaseEnemy enemy;

	public void Attack()
    {
        PlayerStats.Health -=  enemy.ATK;
    }
}
