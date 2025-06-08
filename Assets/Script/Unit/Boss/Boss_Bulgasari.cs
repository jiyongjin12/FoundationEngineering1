using UnityEngine;

public class Boss_Bulgasari : MonoBehaviour
{
    public Unit Bulgasari;

    public GameObject Body;

    public float GrowUPSize;
    public float GrowUPRange;
    public int MaxGrowUP;

    private int currentGrowCount = 0;

    void Update()
    {
        // Bulgasari가 적을 처치했을 때
        if (currentGrowCount < MaxGrowUP && Bulgasari.KillingUnit)
        {
            Grow();
            Bulgasari.KillingUnit = false;
        }
    }

    private void Grow()
    {
        // 1) 스케일 증가
        Body.transform.localScale += Vector3.one * GrowUPSize;

        // 2) Bulgasari의 AttackRange 증가
        Bulgasari.FinalRange += GrowUPRange;

        currentGrowCount++;
    }
}
