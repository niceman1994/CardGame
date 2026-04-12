using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ExtraDrawCardData", menuName = "CardScriptable/CreateExtraDrawCardData")]
public class ExtraDrawCardData : CardData
{
    public int addDrawCount;

    public override void Execute(IHealth target)
    {
        GameEvents.OnExtraCardDraw?.Invoke(addDrawCount);
    }
}
