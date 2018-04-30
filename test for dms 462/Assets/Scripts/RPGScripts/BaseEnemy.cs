using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy{

	public enum EnemyType
    {
        RedCube,
        A
    }

    public string name;

    public int HP;
    public int ATK;
    public int DEF;
    public int SPE;
}
