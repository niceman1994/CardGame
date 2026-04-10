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
    [SerializeField] List<RectTransform> addArrowBody = new List<RectTransform>();

    private float arrowBodySpacing;
    private IHealth arrowTarget;

    private void Start()
    {
        MakeSpareArrowBody();
    }

    private void MakeSpareArrowBody()
    {
        for (int i = 0; i < 40; i++)
        {
            RectTransform arrowBody = Instantiate(arrowBodyPrefab, transform);
            arrowBody.name = $"arrowBody_{i}";
            arrowBody.gameObject.SetActive(false);
            addArrowBody.Add(arrowBody);
        }
        arrowBodySpacing = 30.0f;
    }

    public void SetArrowPos()
    {
        arrowHeadImage.transform.localPosition = Vector3.zero;
        addArrowBody.ForEach(x => x.gameObject.SetActive(false));
    }

    public void DrawArrow(Vector3 start, Vector3 end)
    {
        gameObject.SetActive(true);
        
        Vector3 dir = end - start;
        float distance = dir.magnitude;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90;

        if (distance >= 15.0f)
        {
            int addArrowCount = (int)(distance / arrowBodySpacing);

            for (int i = 0; i < addArrowBody.Count; i++)
            {
                if (i <= addArrowCount - 1)
                {
                    addArrowBody[i].gameObject.SetActive(true);
                    addArrowBody[i].transform.position = start + dir.normalized * (arrowBodySpacing * i);
                    addArrowBody[i].transform.rotation = Quaternion.Euler(0, 0, angle);
                }
                else
                    addArrowBody[i].gameObject.SetActive(false);
            }

            if (addArrowCount <= addArrowBody.Count)
                arrowHeadImage.transform.position = end;
        }
        // 방향에 맞게 회전
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public bool CheckValidTarget()
    {
        // UI 레이캐스트로 마우스 위치의 오브젝트 감지
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;

        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        bool isValidTarget = false;

        foreach (var result in results)
        {
            // 몬스터 하위에 몬스터 크기인 RaycastImage 를 만들고 감지를 확인하기 때문에 GetComponentInParent 를 사용함
            var target = result.gameObject.GetComponentInParent<IHealth>();

            if (target != null)
            {
                arrowTarget = target;
                isValidTarget = true;
                break;
            }
        }

        return isValidTarget;
    }

    public IHealth GetCardArrowTarget()
    {
        return arrowTarget;
    }
}
