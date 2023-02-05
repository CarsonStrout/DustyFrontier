using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRevolver : MonoBehaviour
{
    private Vector3 mousePos;
    public Camera cam;
    public Transform playerPos;

    public GameObject projectile;
    public Transform spawnPos;

    public float bulletSpeed = 10f; // the speed at which the bullet moves
    public float timeBetweenFiring; // the time elapsed since the last shot
    private float timer;

    public int maxAmmo = 6;
    private int currentAmmo;
    public float reloadTime = 1f;
    private bool isFacingRight = true;

    public bool canFire;


    private void Start()
    {
        currentAmmo = maxAmmo;
        canFire = true;
    }

    private void Update()
    {
        if (mousePos.x < playerPos.position.x && isFacingRight)
            Flip();
        else if (mousePos.x > playerPos.position.x && !isFacingRight)
            Flip();

        if (currentAmmo > 0)
        {
            if (Input.GetMouseButtonDown(0) && canFire)
            {
                Shoot();
            }
        }
        else
        {
            Reload();
            return;
        }

        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 rotation = mousePos - transform.position;
        float rotZ = Mathf.Atan2(rotation.y, rotation.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotZ);

        if (!canFire)
        {
            timer += Time.deltaTime;
            if (timer > timeBetweenFiring)
            {
                canFire = true;
                timer = 0;
            }
        }
    }

    private void Shoot()
    {
        canFire = false;
        currentAmmo--;
        GameObject bullet = Instantiate(projectile, spawnPos.position, transform.rotation);
    }

    private void Reload()
    {
        transform.Rotate(0, 0, 360 * 6f * Time.deltaTime);
        timer += Time.deltaTime;
        if (timer > reloadTime)
        {
            currentAmmo = 6;
            timer = 0;
            canFire = true;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 localScale = transform.localScale;
        localScale *= -1f;
        transform.localScale = localScale;
    }
}
