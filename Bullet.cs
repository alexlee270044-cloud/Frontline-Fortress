using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private bool hasHit = false;

    public void SetDamage(float value)
    {
        damage = value;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        hasHit = true;

        // 적에게 데미지 적용
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // 바닥에 맞았을 때 사라지기
        if (other.CompareTag("Ground"))
        {
            Debug.Log("총알이 바닥에 맞음");
        }

        Destroy(gameObject);
    }
}