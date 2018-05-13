using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour {

    public BaseEnemy enemy;

    void DestroyEnemy()
    {
        Destroy(gameObject);
    }
}
