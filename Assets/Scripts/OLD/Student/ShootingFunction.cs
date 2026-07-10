using UnityEngine;

public class ShootingFunction
{
    public void Shooting(CharacterStat Attacker, CharacterStat Target)
    {
        if (Attacker == null || Target == null) return;

        if (Target.isInvincible)
        {
            Target.currentHealth -= 0;
        }
        else
        {
            Target.currentHealth -= Attacker.damage;
        }
        Attacker.currentAmmo -= 1;
    }


    public void BurstShooting(AI Attacker1, CharacterStat Attacker2, CharacterStat Target)
    {
        if (Attacker1 == null || Attacker2 == null || Target == null) return;

        if (Target.isInvincible)
        {
            Target.currentHealth -= 0;
        }
        else
        {
            Target.currentHealth -= Attacker2.damage;
        }
        Attacker2.currentAmmo -= 1;
        Attacker1.burstCount -= 1;
    }
}
