using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitSpawnButtonUI : MonoBehaviour
{
    public int UseKeyCode; // 키보드 연결 값 ( 1 2 3 ...)

    public Image UnitPicture; // 캐릭터 모습

    public Image UnitSpawnCoolDownSlide; // 쿨다운 슬라이더
    public TMP_Text UnitSpawnCoolDownText; // 쿨다운 텍스트
}
