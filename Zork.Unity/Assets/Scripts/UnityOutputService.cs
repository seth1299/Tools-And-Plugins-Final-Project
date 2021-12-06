
using UnityEngine;
using Zork;
using System;
using Newtonsoft.Json;
using TMPro;
public class UnityOutputService : MonoBehaviour, IOutputService

{
    public void Clear()
    {
        throw new System.NotImplementedException();
    }

    public void WriteLine(object value)
    {
        OutputText.text = ($"{value.ToString()}\n");
    }

    public void Write(object value)
    {
        OutputText.text = ($"{value.ToString()}");
    }

    [SerializeField]
    private TextMeshProUGUI OutputText;

}