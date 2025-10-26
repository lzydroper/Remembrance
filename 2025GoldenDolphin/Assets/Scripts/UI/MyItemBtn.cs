using DG.Tweening;
using NewBagSystem;
using UnityEngine;
using UnityEngine.UI;

public class MyItemBtn : MyBtnData<BasicItemData>
{
    [Header("UI 元素")]
    [SerializeField] private Image icon;
    [SerializeField] private Text nameText;

    public override void SetData(BasicItemData newData)
    {
        base.SetData(newData);
        if (!newData)
        {
            if (icon) icon.sprite = null;
            if (nameText) nameText.text = "";
            return;
        }

        // 自动更新显示
        icon.sprite = newData.gridSprite;
    }

    public override void ResetState()
    {
        isInteractable = true;
        icon.color = Color.white;
    }

    public override void OnSelect()
    {
        icon.sprite = Data.gridHSprite;
        icon.transform.DOScale(Vector3.one * 1.5f, 0.5f);
    }

    public override void OnDeselect()
    {
        icon.sprite = Data.gridSprite;
        icon.transform.DOScale(Vector3.one, 0.5f);
    }

    public override void OnClick()
    {
        // Debug.Log("======");
        if (!isInteractable) return;
        SetConfirmedState();
        SetParentFocus();
        onClickWithData.Invoke(Data);
        icon.color = Color.gray;
    }
}