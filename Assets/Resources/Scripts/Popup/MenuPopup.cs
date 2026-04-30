using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPopup : MonoBehaviour
{
    [SerializeField] Button menuButton;
    [SerializeField] GameObject menu;
    [SerializeField] Button restartButton;
    [SerializeField] Button continueButton;
    [SerializeField] Button exitButton;

    private void Awake()
    {
        GameEvents.OnBattleStart += () => menuButton.interactable = true;
        GameEvents.OnBattleEnd += () => menuButton.interactable = false;

        menuButton.onClick.AddListener(OnClickOptionOpenButton);
        restartButton.onClick.AddListener(OnClickRestartButton);
        continueButton.onClick.AddListener(OnClickContinueButton);
#if UNITY_EDITOR
        exitButton.onClick.AddListener(() => UnityEditor.EditorApplication.isPlaying = false);
#else
        exitButton.onClick.AddListener(() => Application.Quit());
#endif
    }

    private void OnClickOptionOpenButton()
    {
        menuButton.interactable = false;
        menu.gameObject.SetActive(true);
        GameEvents.OnOpenPopup?.Invoke();
    }

    private void OnClickRestartButton()
    {
        menu.gameObject.SetActive(false);
        GameEvents.OnGameRestart?.Invoke();
    }

    private void OnClickContinueButton()
    {
        menu.gameObject.SetActive(false);
        GameEvents.OnClosePopup?.Invoke();
        menuButton.interactable = true;
    }
}
