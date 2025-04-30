using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    public float Speed;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        Vector3 inputDir = new Vector3(x,0,0);

        rb.linearVelocity = inputDir * Speed; // ¹¹Áö ¹º°¡ ¹Ù²å´Âµð?
    }

    
}
