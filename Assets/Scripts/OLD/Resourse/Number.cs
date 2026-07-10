using UnityEngine;
using TMPro;
using System.Text;
public class Number : MonoBehaviour
{

    private TextMeshProUGUI myText; 
    private StringBuilder sb = new StringBuilder();
    void Awake()
    {

        myText = GetComponent<TextMeshProUGUI>();
    }

    public void SetNumber(int amount, string prefix = "", string format = "")
    {

        sb.Clear();

        if (!string.IsNullOrEmpty(prefix))
        {
            sb.Append(prefix);
        }

        string amountStr = string.IsNullOrEmpty(format) ? amount.ToString() : amount.ToString(format);
        

        foreach (char c in amountStr)
        {
            sb.Append($"<sprite name=\"{c}\">"); 
        }

        myText.text = sb.ToString();
    }
}

//把数字转换成花样数字的东西 不懂 抄的 别动