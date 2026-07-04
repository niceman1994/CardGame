using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CardArrow : MonoBehaviour
{
    [SerializeField] Image arrowHeadImage;
    [SerializeField] RectTransform arrowBodyPrefab;

    private List<RectTransform> arrowBodys = new List<RectTransform>();
    private float arrowBodySpacing;

    private void Start()
    {
        MakeSpareArrowBody();
    }

    private void MakeSpareArrowBody()
    {
        for (int i = 0; i < 60; i++)
        {
            RectTransform arrowBody = Instantiate(arrowBodyPrefab, transform);
            arrowBody.name = $"arrowBody_{i}";
            arrowBody.gameObject.SetActive(false);
            arrowBodys.Add(arrowBody);
        }
    }

    public void SetArrowPos()
    {
        arrowHeadImage.transform.localPosition = Vector3.zero;
        arrowBodys.ForEach(x => x.gameObject.SetActive(false));
    }

    public void DrawArrow(Vector3 start, Vector3 end)
    {
        // 해상도에 따라 화살표 몸통 이미지의 간격을 조절하기 위한 변수
        arrowBodySpacing = arrowBodyPrefab.rect.height * 2.0f * (Screen.height / 1080.0f);
        gameObject.SetActive(true);
        
        Vector3 dir = end - start;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;
        
        if (distance > arrowBodySpacing)
        {
            int addArrowCount = (int)(distance / arrowBodySpacing);
            
            for (int i = 0; i < arrowBodys.Count; i++)
            {
                if (i <= addArrowCount - 1)
                {
                    arrowBodys[i].gameObject.SetActive(true);
                    arrowBodys[i].transform.position = start + dir.normalized * (arrowBodySpacing * i);
                    arrowBodys[i].transform.rotation = Quaternion.Euler(0, 0, angle);
                }
                else
                    arrowBodys[i].gameObject.SetActive(false);
            }

            if (addArrowCount <= arrowBodys.Count)
                arrowHeadImage.transform.position = end;
        }
        // 방향에 맞게 회전
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
