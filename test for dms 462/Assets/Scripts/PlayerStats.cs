using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStats
{

    private static int ATK, DEF, SPE, HP;  

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
}
