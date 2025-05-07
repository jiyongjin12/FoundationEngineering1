using UnityEngine;

public class Test_Anim : MonoBehaviour
{
    public Animator anim;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            anim.SetBool("Attack", true);
            //anim.SetBool("Idle", true);
        }
    }

}
