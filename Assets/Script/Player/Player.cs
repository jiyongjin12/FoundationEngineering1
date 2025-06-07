using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerData playerData;
    public Health PlayerHp;

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerHp.HP = playerData.MaxHealth;
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        Vector3 inputDir = new Vector3(x,0,0);

        rb.linearVelocity = inputDir * playerData.MoveSpeed;
    }

    
}
