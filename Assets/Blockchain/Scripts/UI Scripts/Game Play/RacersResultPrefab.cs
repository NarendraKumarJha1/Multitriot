using UnityEngine;
using UnityEngine.UI;

public class RacersResultPrefab : MonoBehaviour
{
    public Text position;
    public Text driver;
    public Text vehicleName;
    public Text bestLapTime;
    public Text totalTime;

    public void Init(int pos, string position, string driver, string vehicleName, string bestLapTime, string totalTime)
    {
        int tPos = -65 - (90 * pos);
        this.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, tPos);

        this.position.text = position;
        this.driver.text = driver;
        this.vehicleName.text = vehicleName;
        this.bestLapTime.text = bestLapTime;
        this.totalTime.text = totalTime;
        gameObject.SetActive(true);
    }

    public void EmptyPrefab()
    {
        position.text = string.Empty;
        driver.text = string.Empty;
        vehicleName.text = string.Empty;
        bestLapTime.text = string.Empty;
        totalTime.text = string.Empty;
    }
}