
using UnityEngine;
using TMPro;
using System;
using Zork;
using Zork.Common;
using IInputService = Zork.Common.IInputService;

public class UnityInputService : MonoBehaviour, IInputService

{
    [SerializeField]
    private TMP_InputField InputField;

    public event EventHandler<string> InputReceived;

    
    public void ProcessInput()
    {
        string inputString = InputField.text;
        if (string.IsNullOrWhiteSpace(inputString) == false)
        {
          InputReceived?.Invoke(this, inputString);
        }
        InputField.text = string.Empty;
    }


}
