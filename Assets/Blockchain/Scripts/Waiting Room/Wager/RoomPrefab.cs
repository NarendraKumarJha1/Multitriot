using UnityEngine;
using UnityEngine.UI;

public class RoomPrefab : MonoBehaviour
{
    public Text roomAmount;

    /// <summary>
    /// adding the button functionality to bet 
    /// </summary>
    /// <param name="amt"></param>
    public void Init(int amt)
    {
        roomAmount.text = amt.ToString();

        this.GetComponent<Button>().OnClick(OnClickRoomBtn);
    }

    /// <summary>
    /// its a token button method. how much bet we are placing to enter the race in wager mode
    /// the button text taken as token value 
    /// </summary>
    void OnClickRoomBtn()
    {

        //Debug.LogError(transform.GetChild(0).GetComponent<Text>().text);

        int amt = System.Convert.ToInt32(roomAmount.text.Trim());

        int MyTokens = System.Convert.ToInt32("100000");
        int MyDummyTokens = 0;
        try
        {
            MyDummyTokens = UserDatabase.Instance.localUserData.dummytokens;
        }
        catch { }
        //TURNON Later

        if (PhotonManager.FreeToPlay)
        {
            if (MyDummyTokens >= amt)
            {
                WaitingRoomManager.onClickRoomBtn(this);
            }
            else
            {
                //dont have suffiecient tokens to enter race
                GSF_PlayerSelection.Instance.Selection_UI.floatingWindow.SetActive(true);
            }
        }
        else
        {
            if (MyTokens >= amt)
            {
                WaitingRoomManager.onClickRoomBtn(this);
            }
            else
            {
                //dont have suffiecient tokens to enter race
                GSF_PlayerSelection.Instance.Selection_UI.floatingWindow.SetActive(true);
            }
        }
    }
}