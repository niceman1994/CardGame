using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DamageText : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float textFadeOutTime;
    [SerializeField] Vector3 textMoveVector;
    [SerializeField] TextMeshProUGUI damageText;

    // 피격당했을 때 TakeDamage 함수를 호출
    public void TakeDamage(float num, Transform target, Color color)
    {
        transform.position = target.position + textMoveVector;
        damageText = gameObject.GetComponent<TextMeshProUGUI>();

        // 대미지가 0이면 나오지 않도록 함
        damageText.text = num != 0 ? $"{num}" : "";
        damageText.color = color;
        damageText.name = "Damage Text";
        StartCoroutine(MoveDamageTextPos());
    }

    private IEnumerator MoveDamageTextPos()
    {
        float elapsedTime = 0.0f;

        while (elapsedTime <= textFadeOutTime)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            transform.Translate(new Vector3(0.0f, moveSpeed * Time.deltaTime, 0.0f));
        }
        DamageTextManager.Instance.ReturnDamageTextObject(this);
    }
}