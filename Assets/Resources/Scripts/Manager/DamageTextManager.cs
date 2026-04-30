using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 생성한 텍스트를 재활용하기 위해 사용하는 클래스
/// </summary>
public class DamageTextManager : Singleton<DamageTextManager>
{
    [SerializeField] int defaultCapity;
    [SerializeField] GameObject damageTextPrefab;
    [SerializeField] Transform damageTextParent;

    private Queue<DamageText> damageTextQueue = new Queue<DamageText>();

    private void Start()
    {
        for (int i = 0; i < defaultCapity; i++)
            damageTextQueue.Enqueue(CreatePoolItem("Damage Text", damageTextParent));
    }

    private DamageText CreatePoolItem(string itemName, Transform parent)
    {
        DamageText damageText = Instantiate(damageTextPrefab, parent).gameObject.GetComponent<DamageText>();
        damageText.name = itemName;
        damageText.gameObject.SetActive(false);
        return damageText;
    }

    public void ReturnDamageTextObject(DamageText obj)
    {
        obj.gameObject.SetActive(false);
        obj.transform.SetParent(damageTextParent);
        damageTextQueue.Enqueue(obj);
    }

    public void ShowDamageText(float num, Transform target)
    {
        if (damageTextQueue.Count > 0)
        {
            var damageObj = damageTextQueue.Dequeue();
            damageObj.transform.SetParent(target);
            damageObj.gameObject.SetActive(true);
            damageObj.TakeDamage(num, target, Color.red);
        }
        else
        {
            var newDamageObj = CreatePoolItem("Damage Text", damageTextParent);
            newDamageObj.transform.SetParent(target);
            newDamageObj.gameObject.SetActive(true);
            newDamageObj.TakeDamage(num, target, Color.red);
            damageTextQueue.Enqueue(newDamageObj);
        }
    }
}