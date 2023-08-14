using UnityEngine;
using UnityEngine.UI;

public class RacerInfoPrefab : MonoBehaviour
{
    public Text posText;
    public Text nameText;

    public void Init(string pos, string name)
    {
        posText.text = pos;
        nameText.text = name;
    }
}