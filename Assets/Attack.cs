using UnityEngine;

class Attack
{
    public int Damage;
    public bool IsCritical;

    public Attack()
    {
        Damage = Random.Range(4, 9);

        if (Random.Range(0f, 1f) > 0.1f)
        {
            IsCritical = true;
            Damage *= 3;
        }
    }
}

