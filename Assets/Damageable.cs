using UnityEngine;

public class Damageable : MonoBehaviour
{
    public int hitPoints;

    public void ReceiveDamage(int damage)
    {
        hitPoints -= damage;
    }
}