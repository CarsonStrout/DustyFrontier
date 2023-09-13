using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public GameObject gun;
    public GameObject[] patrolPoints; // an array of patrol points for the enemy to move to

    public ParticleSystem gunParticles;

    public float speed = 2f; // the speed at which the enemy moves
    private float waitTime;
    public float startWaitTime;
    public float shootingRange = 5f; // the range at which the enemy shoots at the player
    public float visionConeAngle = 90f; // the vision angle of the enemy to spot the player
    public GameObject projectile; // the prefab for the enemy's bullets
    public Transform spawnPos;

    private int currentPoint = 0; // the current patrol point the enemy is moving towards
    private Transform playerTransform; // a reference to the player's transform
    private Vector3 localScale; // the enemy's local scale
    private Vector3 gunLocalScale; // to flip the gun when moving

    public float timeBetweenFiring; // the time elapsed since the last shot
    private float timer;

    // Weapon Parameters
    public int maxAmmo = 6;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isFacingRight = true;
    public bool canFire;

    void Start()
    {
        playerTransform = GameObject.FindWithTag("Player").transform;
        waitTime = startWaitTime;

        currentAmmo = maxAmmo;
        canFire = true;
    }

    void Update()
    {
        if (currentAmmo < 1)
        {
            Reload();
            return;
        }
        // get the direction from the enemy to the player
        Vector2 direction = playerTransform.position - transform.position;

        // check if the player is within the shooting range and in the vision cone
        if (Vector2.Distance(transform.position, playerTransform.position) <= shootingRange &&
            Vector2.Angle(transform.right * localScale.x, direction) <= visionConeAngle / 2)
        {
            Shoot();
            return;
        }

        Patrol();
    }

    private void Shoot()
    {
        Vector3 rotation = playerTransform.position - transform.position; // aims towards the player character
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        gun.transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                currentAmmo--;
                timer = 0;
            }
        }
        else
        {
            canFire = false;
            GameObject bullet = Instantiate(projectile, spawnPos.position, transform.rotation);
            CinemachineShake.Instance.ShakeCamera(4, .1f);
            gunParticles.Play();
        }
    }

    private void Reload()
    {
        gun.transform.Rotate(0, 0, 360 * 6f * Time.deltaTime); // spin animation for reload
        timer += Time.deltaTime;
        if (timer > reloadTime)
        {
            currentAmmo = 6;
            timer = 0;
            canFire = true;
        }
    }

    void Patrol()
    {
        if (isFacingRight)
            gun.transform.rotation = Quaternion.Euler(0, 0, -45);
        else
            gun.transform.rotation = Quaternion.Euler(0, 0, -135);

        transform.position = Vector2.MoveTowards(transform.position, patrolPoints[currentPoint].transform.position, speed * Time.deltaTime);

        // patrols between two selected points, with a pause time in between
        if (Vector2.Distance(transform.position, patrolPoints[currentPoint].transform.position) < 0.2f)
        {
            if (waitTime <= 0)
            {
                currentPoint = (currentPoint + 1) % patrolPoints.Length;
                waitTime = startWaitTime;
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }

        localScale = transform.localScale;
        gunLocalScale = gun.transform.localScale;
        if (currentPoint == 0)
        {
            if (!isFacingRight)
                gunLocalScale *= -1f;
            isFacingRight = true;
            localScale.x = Mathf.Abs(localScale.x);
        }
        else
        {
            if (isFacingRight)
                gunLocalScale *= -1f;

            isFacingRight = false;
            localScale.x = -Mathf.Abs(localScale.x);
        }
        transform.localScale = localScale;
        gun.transform.localScale = gunLocalScale;

    }
}