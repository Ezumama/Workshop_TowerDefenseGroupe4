using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class Shooter : MonoBehaviour
{
    [Header("TowerSpecifications")]
    [SerializeField] private Transform _shootingPoint;
    [SerializeField] private float _shootingDistance;
    [SerializeField] private GameObject _feedbackFXOut;
    //[SerializeField] private ParticleSystem _feedbackFXHitEnemy;
    [SerializeField] private GameObject _towerShootingHead;

    // Making sure the tower canon is facing forward for LookAt
    [Header("Face forward correction")]
    [SerializeField] private Vector3 _modelForwardOffset = new Vector3(90f, 0f, 0f);

    [Header("Tower Specs")]
    [SerializeField] private float _damageAmount;
    [SerializeField] private float _shootingCooldown;

    [Header("Tag(s) for targets")]
    [SerializeField] private string _tag1;
    // Leave empty if GroundTower only
    [SerializeField] private string _tag2;

    private GameObject _target;

    void Start()
    {
        StartCoroutine(DoShooting());
    }

    private void Update()
    {
        FindTarget();

        Debug.DrawRay(_shootingPoint.position, _shootingPoint.forward * 2f, Color.green);

        // LookAt
        if (_target != null)
        {
            Vector3 dir = _target.transform.position - _towerShootingHead.transform.position;

            if (dir.sqrMagnitude > 0.0001f)
            {
                Quaternion look = Quaternion.LookRotation(dir.normalized);
                _towerShootingHead.transform.rotation = look * Quaternion.Euler(_modelForwardOffset);
            }
        }
    }

    // Find bots with given tag(s)
    private void FindTarget()
    {
        GameObject closestBot = null;
        float closestDistance = Mathf.Infinity;

        // Collect all objects with given tag(s)
        var bots = GameObject.FindGameObjectsWithTag(_tag1);

        if (!string.IsNullOrEmpty(_tag2))
            bots = bots.Concat(GameObject.FindGameObjectsWithTag(_tag2)).ToArray();

        foreach (GameObject bot in bots)
        {
            float distance = Vector3.Distance(transform.position, bot.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBot = bot;
            }
        }

        _target = closestBot;
    }


    void Shooting()
    {
        if (_shootingPoint == null || _towerShootingHead == null) return;

        // Making sure the shooting point follows the orientation of the cannon
        RaycastHit hit;
        Vector3 forward = _shootingPoint.transform.forward;

        if (Physics.Raycast(_shootingPoint.position, forward, out hit, _shootingDistance))
        {
            Debug.DrawRay(_shootingPoint.position, forward * hit.distance, Color.red, 0.2f);
            
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(_damageAmount);       
            }

            // Instantiate FX feedback at the end of canon / On the enemy
            GameObject newMuzzleFlash = Instantiate(_feedbackFXOut, _shootingPoint.position, _shootingPoint.rotation);
            //GameObject b = Instantiate(_feedbackFXHitEnemy, hit.point, Quaternion.identity);
            Destroy(newMuzzleFlash, 1);
            //Destroy(b, 1);
        }
    }

    IEnumerator DoShooting()
    {
        while (true)
        {
            if (_target != null)
            {
                Shooting();
            }
        

            yield return new WaitForSeconds(_shootingCooldown);
        }
    }
}
