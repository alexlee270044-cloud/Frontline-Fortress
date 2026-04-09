using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Camera cam; // 카메라 변수
    public GameObject bulletPrefab;  // 총알 프리팹
    public Transform firePoint;  // 총알 발사 지점

    [Header("총 Settings")]
    public float bulletSpeed = 45f;     // 총알 속도
    public float bulletDamage = 5f;     // 🔹 데미지 변수 추가
    public float fireRate = 0.1f;       // 발사 간격 (초)
    private float nextFireTime = 0f;    // 다음 발사 가능 시간

    private void Start()
    {
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (firePoint == null)
        {
            firePoint = cam.transform;
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            Shoot();
        }
    }

    void Shoot()
    {
        if (firePoint != null && bulletPrefab != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            // 🔹 총알 방향 및 속도 지정
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 fireDirection = cam.transform.forward;
                rb.AddForce(fireDirection * bulletSpeed, ForceMode.Impulse);
            }

            // 🔹 총알에 데미지 설정
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetDamage(bulletDamage);
            }
        }
    }
}