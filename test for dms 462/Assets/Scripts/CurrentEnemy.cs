using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrentEnemy : MonoBehaviour {

    private new static string name;
    private static int index, ATK, DEF, SPE, HP;
    private static float posX = -10.8f, posY = -0.98f;


    public static string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    public static int Index
    {
        get
        {
            return index;
        }
        set
        {
            index = value;
        }
    }

    public static int Attack
    {
        get
        {
            return ATK;
        }
        set
        {
            ATK = value;
        }
    }

    public static int Defense
    {
        get
        {
            return DEF;
        }
        set
        {
            DEF = value;
        }
    }

    public static int Speed
    {
        get
        {
            return SPE;
        }
        set
        {
            SPE = value;
        }
    }

    public static int Health
    {
        get
        {
            return HP;
        }
        set
        {
            HP = value;
        }
    }

    public static float positionX
    {
        get
        {
            return posX;
        }
        set
        {
            posX = value;
        }
    }

    public static float positionY
    {
        get
        {
            return posY;
        }
        set
        {
            posY = value;
        }
    }
}
