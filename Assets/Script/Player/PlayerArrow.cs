using DamageNumbersPro;
using UnityEngine;

public class PlayerArrow : MonoBehaviour
{
    private float speed;
    private float damage;
    private Vector3 direction;

    public void Initialize(float damage, Vector3 direction, float speed)
    {
        this.damage = damage;
        this.direction = direction.normalized;
        this.speed = speed;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var h = other.GetComponent<Health>();
            if (h != null)
            {
                h.TakeDamage(damage);
                UIManager.Instance.damageNumberPrefab.Spawn(other.transform.position, damage);
            }
            Destroy(gameObject);
        }
    }

    void OnBecameInvisible()
    {
        // 화면 밖으로 나갔을 때 정리
        Destroy(gameObject);
    }
}
