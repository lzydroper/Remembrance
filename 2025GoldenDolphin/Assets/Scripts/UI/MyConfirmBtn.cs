using DG.Tweening;
using NewBagSystem;
using UnityEngine;
using UnityEngine.UI;

public class MyConfirmBtn : MyBtnData<ConfirmData>
{
    [Header("UI 元素")]
    [SerializeField] private Image icon;
    [SerializeField] private Text nameText;

    public override void SetData(ConfirmData newData)
    {
        base.SetData(newData);

        if (newData == null)
        {
            if (icon) icon.sprite = null;
            if (nameText) nameText.text = "";
            return;
        }

        // 自动更新显示
    }
    
    public override void OnSelect()
    {
        icon.transform.DOScale(Vector3.one * 1.5f, 0.5f);
    }

    public override void OnDeselect()
    {
        icon.transform.DOScale(Vector3.one, 0.5f);
    }
    
    public override void OnClick()
    {
        if (!IsInteractable) return;
        SetConfirmedState();
        OnDeselect();
        icon.color = Color.gray;
        // SetParentFocus();
        onClickWithData.Invoke(Data);
    }

    public override void ResetState()
    {
        base.ResetState();
        icon.color = Color.white;
    }
}

[System.Serializable]
public class ConfirmData
{
    public int playerID;
    public ConfirmType confirmType;
}
[System.Serializable]
public enum ConfirmType
{
    None,
    Repair,
    Wait,
}