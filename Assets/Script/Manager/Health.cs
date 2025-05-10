using UnityEngine;

public class Health : MonoBehaviour
{
    public float HP;
    public float currentHP;

    private void Start()
    {
        currentHP = HP;
    }

    public void TakeDamage(float amount) //TakeDamage_Unit이랑 TakeDamage_Tower로 데미지 받을때 효과 나눌수있음
    {
        currentHP -= amount;
        if (currentHP <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Death");
        Destroy(gameObject);
    }


}
