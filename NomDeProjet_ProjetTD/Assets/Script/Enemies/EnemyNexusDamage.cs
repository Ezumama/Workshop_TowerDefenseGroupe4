using UnityEngine;

public class EnemyNexusDamage : MonoBehaviour
{
    public float damageToNexus = 1f;
    public float attackCooldown = 1.0f;
    private float lastAttackTime;

    void Start()
    {
        // Initialiser pour permettre au premier coup de passer immédiatement (Time.time est > 0 au Start)
        lastAttackTime = Time.time - attackCooldown;
    }

    public void TryDamageNexus(Health nexusHealth)
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (nexusHealth != null)
            {
                nexusHealth.TakeDamage(damageToNexus);
                // Si vous voulez le déloguer
                Debug.Log($"[Attaque Nexus] {gameObject.name} fait {damageToNexus} dégâts."); 
            }
            lastAttackTime = Time.time;
        }
    }
}