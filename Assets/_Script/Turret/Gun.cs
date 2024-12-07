using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Gun : MonoBehaviour
{
    public GameObject shotPrefab;
    public Transform[] gunPoints;
    public float fireRate;

    bool firing;
    float fireTimer;

    int gunPointIndex;

    void Awake()
    {
        if (shotPrefab == null)
        {
            shotPrefab = Resources.Load<GameObject>("Bullet"); // Ensure the Bullet prefab is in the Resources folder
            if (shotPrefab == null)
            {
                Debug.LogError("Bullet prefab not found in Resources/Prefabs!");
            }
        }
    }

    void Update()
    {
        if (firing)
        {
            while (fireTimer >= 1 / fireRate)
            {
                SpawnShot();
                fireTimer -= 1 / fireRate;
            }

            fireTimer += Time.deltaTime;
            firing = false;
        }
        else
        {
            if (fireTimer < 1 / fireRate)
            {
                fireTimer += Time.deltaTime;
            }
            else
            {
                fireTimer = 1 / fireRate;
            }
        }
    }

    void SpawnShot()
    {
        if (gunPoints == null || gunPoints.Length == 0)
        {
            Debug.LogError("GunPoints array is empty! Please assign at least one Transform in the Inspector.");
            return;
        }

        if (gunPointIndex < 0 || gunPointIndex >= gunPoints.Length)
        {
            Debug.LogError($"GunPointIndex out of range! gunPointIndex: {gunPointIndex}, gunPoints.Length: {gunPoints.Length}");
            gunPointIndex = 0; // Reset to prevent further errors
            return;
        }

        var gunPoint = gunPoints[gunPointIndex++];
        Instantiate(shotPrefab, gunPoint.position, gunPoint.rotation);
        gunPointIndex %= gunPoints.Length; // Wrap the index
    }

    public void Fire()
    {
        firing = true;
    }
}