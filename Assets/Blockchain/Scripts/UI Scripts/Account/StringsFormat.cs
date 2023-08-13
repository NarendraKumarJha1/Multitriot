using UnityEngine;
using UnityEngine.UI;

public static class StringsFormat
{
    public static void FormatAddress(this Text text, string address, bool isLogin)
    {
        if (text == null) return;

        string _value = "";
        if (isLogin)
            try
            {
                string _s = address.Substring(0, 5);
                string _e = address.Substring(address.Length - 4, 4);
                _value = _s + "..." + _e;
            }
            catch
            {
                _value = address;
            }
        else
            _value = GenericStringKeys.WalletAddress;
        text.text = _value;
    }

    public static void FormatBalance(this Text text, string balance, bool isLogin = true)
    {
        if (text == null) return;

        string _value = "";
        if (isLogin)
            try
            {
                double _bal = double.Parse(balance) / 1E18;
                _value = string.Format("{0:#,###0} SHOE", _bal);
            }
            catch
            {
                _value = balance;
            }
        else
            _value = GenericStringKeys.NoBalance;

        text.text = _value;
    }
}