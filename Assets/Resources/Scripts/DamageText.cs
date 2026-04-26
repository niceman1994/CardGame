using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DamageText : MonoBehaviour
{
    // 텍스트가 위로 뜨면서 사라지게할 float 변수 3개
    [SerializeField] float moveSpeed;
    [SerializeField] float textFadeOutTime;
    [SerializeField] Vector3 textMoveVector;
    [SerializeField] TextMeshProUGUI damageText;

    // 피격당했을 때 TakeDamage 함수를 호출
    public void TakeDamage(float num, Transform target, Color color)
    {
        transform.position = target.position + textMoveVector;
        damageText = gameObject.GetComponent<TextMeshProUGUI>();

        // 정수만 나오도록 Math.Truncate()를 사용함
        damageText.text = num != 0 ? $"{num}" : "";
        damageText.color = color;
        damageText.name = "Damage Text";
        StartCoroutine(TextPos(() => TextPoolManager.Instance.ReturnDamageTextObject(this)));
    }

    // 피해를 받거나 아이템을 사용하거나 체력을 회복했을 때 텍스트가 위로 올라가게 하는 함수
    private IEnumerator TextPos(Action poolingAction)
    {
        float elapsedTime = 0.0f;

        while (elapsedTime <= textFadeOutTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            transform.Translate(new Vector3(0.0f, moveSpeed * Time.deltaTime, 0.0f));
        }
        poolingAction.Invoke();
    }
}