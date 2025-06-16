using TMPro;
using UnityEngine;

public class ShowCoinText : MonoBehaviour 
{
    public TMP_Text coinText;

    private void Update()
    {
        CommaText();
    } 

    private void CommaText()
    {
        string Coin = string.Format("{0:#,###}", MoneyManager.Instance.Coins);
        coinText.text = Coin;
    }
}
