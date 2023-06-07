using System;
using TMPro;
using UnityEngine;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lenghtTMP;

    private void OnEnable()
    {
        PlayerLength.ChangedLengthEvent += ChangedLengthText;
    }

    private void OnDisable()
    {
        PlayerLength.ChangedLengthEvent -= ChangedLengthText;
    }

    private void ChangedLengthText(ushort value)
    {
        _lenghtTMP.SetText($"Length {value}");
    }
}