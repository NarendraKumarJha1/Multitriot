using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Message : MonoBehaviour
{
    public TextMeshProUGUI _message;

    private void Start()
    {
        _message = GetComponent<TextMeshProUGUI>();
    }

}
