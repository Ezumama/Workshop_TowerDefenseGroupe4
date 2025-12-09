using UnityEngine;

[RequireComponent(typeof(EnemyNav))]
public class EnemyHealer : MonoBehaviour
{
    [Header("Healing Settings")]
    public float healRadius = 5f;      // rayon du cylindre (XZ)
    public float healHeight = 3f;      // hauteur du cylindre (Y)
    public float healAmount = 10f;
    public float healInterval = 2f;

    private float healTimer = 0f;

    // --- Tags attendus pour la guérison ---
    // Assurez-vous que ces tags existent dans votre projet Unity (Edit -> Project Settings -> Tags and Layers)
    private const string EnemyAirTag = "EnemyAir";
    private const string EnemyGroundTag = "EnemyGround";


    void Update()
    {
        healTimer += Time.deltaTime;

        if (healTimer >= healInterval)
        {
            HealNearbyEnemies();
            healTimer = 0f;
        }
    }

    void HealNearbyEnemies()
    {
        // On récupère LARGE (Sphere) puis on filtre
        Collider[] hits = Physics.OverlapSphere(transform.position, healRadius);

        foreach (var hit in hits)
        {
            // Vérification 1 : Est-ce que l'objet a un composant Health ?
            Health enemy = hit.GetComponent<Health>();

            // Si l'objet n'a pas de Health ou si c'est l'ennemi soigneur lui-même, on passe au suivant.
            if (enemy == null || enemy.gameObject == this.gameObject)
                continue;

            // 🔥 CORRECTION : Vérification du tag
            // L'ennemi doit être marqué comme "EnemyAir" OU "EnemyGround"
            string targetTag = hit.gameObject.tag;
            if (targetTag != EnemyAirTag && targetTag != EnemyGroundTag)
                continue;

            // 1) Vérification horizontale (cercle)
            Vector3 flatSelf = new Vector3(transform.position.x, 0, transform.position.z);
            Vector3 flatOther = new Vector3(hit.transform.position.x, 0, hit.transform.position.z);

            float horizontalDist = Vector3.Distance(flatSelf, flatOther);

            if (horizontalDist > healRadius)
                continue;

            // 2) Vérification verticale (hauteur du cylindre)
            // Correction : Utilisation de la hauteur complète pour le calcul du centre
            float verticalDist = Mathf.Abs(hit.transform.position.y - transform.position.y);

            // Vérifie si la distance verticale est inférieure à la moitié de la hauteur (distance du centre)
            if (verticalDist > healHeight * 0.5f)
                continue;

            // → L’ennemi est DANS LE CYLINDRE et a le bon tag → HEAL
            enemy.Heal(healAmount);
        }
    }

    // Le code des Gizmos (OnDrawGizmosSelected et DrawCircle) n'a pas été modifié.
    private void OnDrawGizmosSelected()
    {
        // Gizmo du cylindre (approximation)
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);

        // Dessine un disque en haut et en bas
        DrawCircle(transform.position + Vector3.up * (healHeight * 0.5f), healRadius);
        DrawCircle(transform.position - Vector3.up * (healHeight * 0.5f), healRadius);

        // Lignes de liaison
        Gizmos.DrawLine(transform.position + new Vector3(healRadius, healHeight * 0.5f, 0),
                        transform.position + new Vector3(healRadius, -healHeight * 0.5f, 0));

        Gizmos.DrawLine(transform.position + new Vector3(-healRadius, healHeight * 0.5f, 0),
                        transform.position + new Vector3(-healRadius, -healHeight * 0.5f, 0));
    }

    void DrawCircle(Vector3 center, float radius)
    {
        int segments = 40;
        float angle = 0f;

        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0) * radius, 0, Mathf.Sin(0) * radius);

        for (int i = 1; i <= segments; i++)
        {
            angle += 2 * Mathf.PI / segments;

            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);

            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }
}


