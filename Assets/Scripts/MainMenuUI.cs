using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button exitGameButton;
    [SerializeField] private Button readMeButton;
    [SerializeField] private GameObject readMePanel;
    [SerializeField] private Button closeReadMeButton;


    private void Awake()
    {
        readMePanel.SetActive(false);
        
        startGameButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });

        readMeButton.onClick.AddListener(() =>
        {
            OpenPanel();
        });

        closeReadMeButton.onClick.AddListener(() =>
        {
            ClosePanel();
        });
        
        exitGameButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });
    }

    private void OpenPanel()
    {
        readMePanel.SetActive(true);
    }

    private void ClosePanel()
    {
        readMePanel.SetActive(false);
    }
}
