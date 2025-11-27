using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float moveSpeed = 2f;          
    public Transform targetPoint;          

    private Health health;

    void Start()
    {
        health = GetComponent<Health>();
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        if (targetPoint == null) return;

        Vector3 dir = (targetPoint.position - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;

        // Si l'ennemi atteint sa cible, on peut le détruire ou infliger des dégâts à la base
        float distance = Vector3.Distance(transform.position, targetPoint.position);
        if (distance < 0.1f)
        {
            ReachDestination();
        }
    }

    void ReachDestination()
    {
        // Ici tu peux infliger des dégâts à la base
        Destroy(gameObject);
    }


    public void TakeDamage(float amount)
    {
        if (health != null)
            health.TakeDamage(amount);
    }
}

