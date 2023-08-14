using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerProfileWindow : MonoBehaviour
{

    public GSF_MainMenu _mainMenu;
    public Button[] avatarSelectBtns;
    public Image previewAvatarFrame;
    public InputField playerNameIF;
    public Text messageText;


    private void OnEnable()
    {
        UpdatePreview();
    }
    void Start()
    {
        SetupAvatarSelectButtons();
        
    }

    void UpdatePreview()
    {
        if (PlayerPrefs.HasKey("SELECTED_AVATAR"))
            previewAvatarFrame.sprite = _mainMenu.avaterImages[PlayerPrefs.GetInt("SELECTED_AVATAR")];

        //playerNameIF.interactable = false;
        playerNameIF.text = UserDatabase.Instance.localUserData.nickname; ;
    }

    void SetupAvatarSelectButtons()
    {
        int index = 0;
        foreach (Button b in avatarSelectBtns)
        {
            int tempIndex = index;
            b.onClick.AddListener(() =>
            {
                previewAvatarFrame.sprite = _mainMenu.avaterImages[tempIndex];
                PlayerPrefs.SetInt("SELECTED_AVATAR", tempIndex);
                PlayerPrefs.Save();
            });
            Helper.FindChildByName(b.gameObject, "Frame").GetComponent<Image>().sprite = _mainMenu.avaterImages[index];
            index++;
        }
    }

    public void Save()
    {
        if (playerNameIF.text.Trim().Length >= 3)
        {
            Helper.ShowMessage(messageText, "Profile Saved Successfully!", Color.yellow, 1.5f);
        }
        else
            Helper.ShowMessage(messageText, "Please enter atleast 3 characters in the Name Field!", Color.red, 1.5f);

        Debug.LogError("username changed to : " + playerNameIF.text);
        string name = playerNameIF.text.ToLower();
        ApiManager.instance.editProfile.Download(name, PlayerPrefs.GetInt("SELECTED_AVATAR"));
        UserDatabase.Instance.localUserData.nickname = playerNameIF.text;
        UserDatabase.Instance.localUserData.avtarId = PlayerPrefs.GetInt("SELECTED_AVATAR");
       
        _mainMenu.UpdateProfile();
    }
}