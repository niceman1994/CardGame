using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManaController : MonoBehaviour
{
    [SerializeField] Text currentManaText;
    [SerializeField] Text maxManaText;

    private int restoreMana;
    private int currentMana;
    private int maxMana;

    // Deck의 Start에서 CardDraw함수가 실행되기 때문에 이벤트 등록을 포함하는 InitMana 함수는 Awake에서 함
    private void Awake()
    {
        GameEvents.OnBattleStart += InitMana;
        GameEvents.OnManaRestore += ManaRestore;
        GameEvents.OnManaBoost += AddMana;
        GameEvents.OnBattleEnd += () => currentMana = 0;
    }

    private void InitMana()
    {
        currentMana = 0;
        restoreMana = 5;
        maxMana = 12;
        currentManaText.text = $"{currentMana}";
        maxManaText.text = $"{maxMana}";
    }

    private void ManaRestore()
    {
        if (currentMana + restoreMana <= maxMana)
            currentMana += restoreMana;
        else
            currentMana = maxMana;

        currentManaText.text = $"{currentMana}";

        if (restoreMana < maxMana)
            restoreMana++;
    }

    public bool TrySpendMana(int cardCost)
    {
        if (currentMana < cardCost)
            return false;

        currentMana -= cardCost;
        currentManaText.text = $"{currentMana}";

        return true;
    }

    private void AddMana(int addMana)
    {
        currentMana += addMana;
        currentManaText.text = $"{currentMana}";
    }
}
