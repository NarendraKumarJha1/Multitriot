using System;
using SimpleJSON;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ForgetPassword : MonoBehaviour
{
    [SerializeField]
    private Button createLink;
    [SerializeField]
    private InputField EmailFeild;
    [SerializeField]
    private string Link;

    private void OnEnable()
    {
        createLink.interactable = true;

        createLink.onClick.RemoveAllListeners();
        createLink.onClick.AddListener(() => CreateLink());
    }
    public void CreateLink()
    {
        if (string.IsNullOrEmpty(EmailFeild.text))
        {
            Helper.ShowMessage(UiManager.Instance.logText, GenericStringKeys.EmptyEmail, Color.red, 2f);
            return;
        }


        WWWForm form = new WWWForm();
        form.AddField("email", EmailFeild.text);
        createLink.interactable = false;
        StartCoroutine(ForgetPass(Link, form));
    }

    IEnumerator ForgetPass(string url, WWWForm form)
    {
        UnityWebRequest www = UnityWebRequest.Post(url, form);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            if (ApiManager.instance.showDebugs)
                Debug.Log(string.Format("Response Code : {0}\nData:{1}", www.responseCode, www.downloadHandler.text));
            JsonParser(www.downloadHandler.text);
        }
        else
        {
            if (ApiManager.instance.showDebugs)
                Debug.LogError(string.Format("Response Code : {0}\nError:{1}", www.responseCode, www.error));
            Helper.ShowMessage(UiManager.Instance.logText, "Please check your email.", Color.red, 2f);
            createLink.interactable = true;
        }
    }

    void JsonParser(string json)
    {
        JSONNode node = JSON.Parse(json);

        bool success = bool.Parse(node["success"]);

        if (success)
        {
            Helper.ShowMessage(UiManager.Instance.logText, node["message"], Color.green, 3f);
            GotoLoginPage();
        }
        else
        {
            Helper.ShowMessage(UiManager.Instance.logText, node["message"], Color.red, 2f);
            createLink.interactable = true;
        }
    }

    void GotoLoginPage()
    {
        UiManager.Instance.loginManager.gameObject.SetActive(true);
        UiManager.Instance.PasswordManager.gameObject.SetActive(false);
    }
}

