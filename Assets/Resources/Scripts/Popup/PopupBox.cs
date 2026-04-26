using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupBox : MonoBehaviour
{
    [SerializeField] protected Button checkButton;
    [SerializeField] protected Button xButton;
    [SerializeField] protected Text questionText;

    public virtual void OpenPopup(string popupText) { }

    protected virtual void OnClickConfirmButton() { }
    protected virtual void OnClickCloseButton() { }
}
