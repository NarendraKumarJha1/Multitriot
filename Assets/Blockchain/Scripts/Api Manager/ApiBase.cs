using System;
using SimpleJSON;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct FormFields
{
    public string key;
    public string value;
}

[Serializable]
public class ApiStatus
{
    public int status;
    public string msg;
}

public abstract class ApiBase : MonoBehaviour
{
    public List<FormFields> formFields = new List<FormFields>();

    public bool byPassApi = false;

    /*public bool DownloadOnAwake = false;
    public bool DownloadOnStart = false;*/
    public bool DownloadNow = false;

    public const string baseUrl = "http://146.190.109.193:8082";  //"http://45.79.126.10:8082"; //"http://3.120.204.209:5000";
    public string forwordUrl;

    protected string URL { get => string.Format("{0}{1}", baseUrl, forwordUrl); }

    protected virtual void OnValidate() { }

    protected ApiStatus GetApiStatus(string result)
    {
        JSONNode node = JSON.Parse(result);

        ApiStatus apiStatus = new ApiStatus()
        {
            status = node["status"],
            msg = GetStatusMessage(node["status"])
        };

        return apiStatus;
    }

    string GetStatusMessage(int statusCode)
    {
        return statusCode switch
        {
            //success
            101 => GenericStringKeys.UserReg,
            102 => GenericStringKeys.LoginSuccess,
            103 => GenericStringKeys.TxSuccess,
            //Failed
            401 => GenericStringKeys.EmptyData,
            402 => GenericStringKeys.InvalidEmail,
            403 => GenericStringKeys.EmailExist,
            404 => GenericStringKeys.NicknameExist,
            405 => GenericStringKeys.PasswordLen,
            406 => GenericStringKeys.EmptyUsername,
            407 => GenericStringKeys.InvalidReferral,
            408 => GenericStringKeys.SameUserNick,
            409 => GenericStringKeys.NicknameNotMatch,
            410 => GenericStringKeys.PasswordNotMatch,
            _ => GenericStringKeys.SomethingWentWrong,
        };
    }
}