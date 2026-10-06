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
        EventBus.Subscribe(GameEventType.BATTLE_END, () => menuButton.interactable = false);
        // 플레이어의 턴이 시작됐을 때 게임을 재시작하면 5장이 아닌 10장을 드로우하는 현상을 방지하기 위한 코드
        EventBus.Subscribe(GameEventType.TURN_START, () => menu.gameObject.SetActive(false));

        menuButton.onClick.AddListener(OnClickOptionButton);
        restartButton.onClick.AddListener(OnClickRestartButton);
        continueButton.onClick.AddListener(OnClickContinueButton);
#if UNITY_EDITOR
        exitButton.onClick.AddListener(() => UnityEditor.EditorApplication.isPlaying = false);
#else
        exitButton.onClick.AddListener(() => Application.Quit());
#endif
    }

    private void OnClickOptionButton()
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
