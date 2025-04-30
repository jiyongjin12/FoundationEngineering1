using UnityEngine;

public class Unit : MonoBehaviour
{
    public Rigidbody rb;
    public bool Left;

    [SerializeField]
    private float Speed;

    [SerializeField]
    private Vector3 _Move;


    private void Update()
    {
        if (Left == true)
        {
            _Move = new Vector3(-1, 0, 0);

        }
        else
        {
            _Move = new Vector3(1, 0, 0);
        }

        rb.linearVelocity = _Move * Speed;
        
    }
}
