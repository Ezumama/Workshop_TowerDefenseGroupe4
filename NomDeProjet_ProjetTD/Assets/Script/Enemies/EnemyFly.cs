using UnityEngine;

public class EnemyFly : MonoBehaviour
{
    public float speed = 5f;
    private Transform target;

    void Start()
    {
        target = GameObject.Find("Generateur").transform;
        WaveManager.instance.RegisterEnemy();
    }

    void Update()
    {
        if (!target) return;

        transform.position += (target.position - transform.position).normalized
                              * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 1f)
        {
            WaveManager.instance.UnregisterEnemy();
            Destroy(gameObject);
        }
    }
}




