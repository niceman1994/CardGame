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
        EventBus.Subscribe(GameEventType.BATTLE_START, () => menuButton.interactable = true);
        EventBus.Subscribe(GameEventType.BATTLE_END, () => menuButton.interactable = false);

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
        EventBus.Publish(GameEventType.OPENPOPUP);
    }

    private void OnClickRestartButton()
    {
        menu.gameObject.SetActive(false);
        EventBus.Publish(GameEventType.RESTART);
    }

    private void OnClickContinueButton()
    {
        menu.gameObject.SetActive(false);
        EventBus.Publish(GameEventType.CLOSEPOPUP);
        menuButton.interactable = true;
    }
}
