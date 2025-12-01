using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shooter_MultiTarget : MonoBehaviour
{
    [System.Serializable]
    public class Cannon
    {
        public Transform ShootingPoint;
        public GameObject ShootingHead;
        public Vector3 ModelForwardOffset = new Vector3(90f, 0f, 0f);
        [HideInInspector] public GameObject Target;

        [Tooltip("Check for LEFT cannon, uncheck for RIGHT cannon.")]
        public bool IsLeftSide;
    }

    [Header("Cannons")]
    [SerializeField] private Cannon[] Cannons;

    [Header("Tower Specs")]
    [SerializeField] private float _shootingDistance;
    [SerializeField] private float _shootingCooldown;
    [SerializeField] private float _damageAmount;

    [Header("Feedback FX")]
    [SerializeField] private GameObject _feedbackFXOut;

    [Header("Side Lock Settings")]
    [Tooltip("Prevents cannons from swapping targets when enemies get near the center.")]
    [SerializeField] private float SideDeadZone;

    [Header("Tags")]
    [SerializeField] private string Tag1;
    [SerializeField] private string Tag2;

    private void Start()
    {
        StartCoroutine(ShootingRoutine());
    }

    private void Update()
    {
        AssignTargets();
        RotateCannons();
    }

    private void AssignTargets()
    {
        // Find all possible targets
        var targets = GameObject.FindGameObjectsWithTag(Tag1).ToList();
        if (!string.IsNullOrEmpty(Tag2))
            targets.AddRange(GameObject.FindGameObjectsWithTag(Tag2));

        // No targets, then no target
        if (targets.Count == 0)
        {
            foreach (var cannon in Cannons)
                cannon.Target = null;
            return;
        }

        foreach (var cannon in Cannons)
        {
            // Lose target if too far
            if (cannon.Target != null &&
                Vector3.Distance(transform.position, cannon.Target.transform.position) > _shootingDistance)
            {
                cannon.Target = null;
            }

            // If still has valid target, then keep it
            if (cannon.Target != null) continue;

            // Assign target based on side logic
            cannon.Target = FindTargetForCannon(cannon, targets);
        }
    }

    private GameObject FindTargetForCannon(Cannon cannon, List<GameObject> allTargets)
    {
        // Only one enemy, then both shoot same
        if (allTargets.Count == 1) return allTargets[0];

        // Filter by which side they are on
        var sideTargets = allTargets.Where(t => IsOnCorrectSide(cannon, t.transform.position)).ToList();

        // If no target on side, DON'T swap if very close to center
        if (sideTargets.Count == 0)
            sideTargets = allTargets;

        // Choose closestvtarget
        return sideTargets.OrderBy(
            t => Vector3.Distance(transform.position, t.transform.position)
        ).FirstOrDefault();
    }

    private bool IsOnCorrectSide(Cannon cannon, Vector3 targetPos)
    {
        Vector3 local = transform.InverseTransformPoint(targetPos);

        // Dead zone near the center: keep current target
        if (Mathf.Abs(local.x) < SideDeadZone && cannon.Target != null)
            return true;

        return cannon.IsLeftSide ? local.x < 0f : local.x > 0f;
    }

    private void RotateCannons()
    {
        foreach (var cannon in Cannons)
        {
            if (cannon.Target == null || cannon.ShootingHead == null) continue;

            Vector3 dir = cannon.Target.transform.position - cannon.ShootingHead.transform.position;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion look = Quaternion.LookRotation(dir.normalized);
                cannon.ShootingHead.transform.rotation = look * Quaternion.Euler(cannon.ModelForwardOffset);
            }
        }
    }

    private void Shoot(Cannon cannon)   
    {
        if (cannon.Target == null || cannon.ShootingPoint == null) return;

        RaycastHit hit;
        Vector3 forward = cannon.ShootingPoint.forward;

        if (Physics.Raycast(cannon.ShootingPoint.position, forward, out hit, _shootingDistance))
        {
            Debug.DrawRay(cannon.ShootingPoint.position, forward * hit.distance, Color.red, 0.2f);
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damageAmount);
            }

            // Instantiate FX feedback at the end of canon / On the enemy
            GameObject newMuzzleFlash = Instantiate(_feedbackFXOut, cannon.ShootingPoint.position, cannon.ShootingPoint.rotation);
            //GameObject b = Instantiate(_feedbackFXHitEnemy, hit.point, Quaternion.identity);
            Destroy(newMuzzleFlash, 1);
        }
    }

    private IEnumerator ShootingRoutine()
    {
        while (true)
        {
            foreach (var cannon in Cannons)
            {
                if (cannon.Target != null) Shoot(cannon);
            }
            yield return new WaitForSeconds(_shootingCooldown);
        }
    }
}
