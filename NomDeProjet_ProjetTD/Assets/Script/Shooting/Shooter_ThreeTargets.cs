    using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Shooter_ThreeTargets : MonoBehaviour
{
    [System.Serializable]
    public class Cannon
    {
        public Transform ShootingPoint;
        public GameObject ShootingHead;
        public Vector3 ModelForwardOffset = new Vector3(90f, 0f, 0f);

        [HideInInspector]
        public GameObject Target;
    }

    [Header("Cannons (Left, Middle, Right)")]
    [SerializeField] private Cannon[] Cannons;

    [Header("Tower Specs")]
    [SerializeField] private float _shootingDistance;
    [SerializeField] private float _shootingCooldown;
    [SerializeField] private float _damageAmount;

    [Header("FX")]
    [SerializeField] private GameObject _feedbackFXOut;

    [Header("Target Tags")]
    [SerializeField] private string _targetTag1;
    [SerializeField] private string _targetTag2;

    private void Start()
    {
        StartCoroutine(ShootingRoutine());
    }

    private void Update()
    {
        AssignTargets();
        RotateCannons();
    }

    #region tower orientation
    private void AssignTargets()
    {
        // Gather all valid enemies
        var targets = GameObject.FindGameObjectsWithTag(_targetTag1).ToList();
        if (!string.IsNullOrEmpty(_targetTag2))
            targets.AddRange(GameObject.FindGameObjectsWithTag(_targetTag2));

        // No targets, then clear
        if (targets.Count == 0)
        {
            foreach (var cannon in Cannons)
                cannon.Target = null;
            return;
        }

        foreach (var cannon in Cannons)
        {
            // 1 target, then all cannons use the same one
            if (targets.Count == 1)
            {
                cannon.Target = targets[0];
                continue;
            }

            // Multiple targets, then pick the closest to each cannon
            cannon.Target = targets
                .OrderBy(t => Vector3.Distance(cannon.ShootingPoint.position, t.transform.position))
                .FirstOrDefault();
        }
    }

    private void RotateCannons()
    {
        foreach (var cannon in Cannons)
        {
            if (cannon.Target == null || cannon.ShootingHead == null)
                continue;

            Vector3 dir = cannon.Target.transform.position - cannon.ShootingHead.transform.position;

            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion look = Quaternion.LookRotation(dir);
                cannon.ShootingHead.transform.rotation =
                    look * Quaternion.Euler(cannon.ModelForwardOffset);
            }
        }
    }
    #endregion

    private void Shoot(Cannon cannon)
    {
        if (cannon.Target == null || cannon.ShootingPoint == null) return;

        Vector3 forward = cannon.ShootingPoint.forward;

        if (Physics.Raycast(cannon.ShootingPoint.position, forward, out RaycastHit hit, _shootingDistance))
        {
            Debug.DrawRay(cannon.ShootingPoint.position, forward * hit.distance, Color.red, 0.2f);

            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damageAmount);
            }

            // Muzzle FX
            GameObject fx = Instantiate(_feedbackFXOut, cannon.ShootingPoint.position, cannon.ShootingPoint.rotation);
            Destroy(fx, 1f);
        }
    }

    private IEnumerator ShootingRoutine()
    {
        while (true)
        {
            foreach (var cannon in Cannons)
            {
                if (cannon.Target != null)
                    Shoot(cannon);
            }

            yield return new WaitForSeconds(_shootingCooldown);
        }
    }

    #region worker buff
    public void BuffDamage()
    {
        _damageAmount *= 2;
    }

    public void StopBuff()
    {
        _damageAmount /= 2;
    }
    #endregion
}
