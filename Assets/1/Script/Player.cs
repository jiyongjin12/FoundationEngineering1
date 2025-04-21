using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody rb;
    public float speed;

    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        Vector3 move = new Vector3(x,0,0);

        rb.linearVelocity = move * speed;   
    }


}
