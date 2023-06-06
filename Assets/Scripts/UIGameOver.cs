using System;
using UnityEngine;

public class UIGameOver : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverScreen;

    private void Awake()
    {
        _gameOverScreen.SetActive(false);
    }

    private void OnEnable()
    {
        PlayerController.GameOverEvent += GameOver;
    }

    private void OnDisable()
    {
        PlayerController.GameOverEvent -= GameOver;
    }

    private void GameOver()
    {
        _gameOverScreen.SetActive(true);
    }
}