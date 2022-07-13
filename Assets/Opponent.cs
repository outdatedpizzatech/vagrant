using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Opponent : MonoBehaviour
{
    public int hitPoints;

    public void ReceiveDamage(int damage)
    {
        hitPoints -= damage;
    }
}