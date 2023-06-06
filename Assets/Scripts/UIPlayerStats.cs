using System;
using TMPro;
using UnityEngine;

public class UIPlayerStats : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _lenghtTMP;

    private void OnEnable()
    {
        PlayerLenght.ChangedLenghtEvent += ChangedLenghtText;
    }

    private void OnDisable()
    {
        PlayerLenght.ChangedLenghtEvent -= ChangedLenghtText;
    }

    private void ChangedLenghtText(ushort value)
    {
        _lenghtTMP.SetText($"Length {value}");
    }
}