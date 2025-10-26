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

        if (newData == null)
        {
            if (icon) icon.sprite = null;
            if (nameText) nameText.text = "";
            return;
        }

        // 自动更新显示
    }
    
    public override void OnClick()
    {
        if (!IsInteractable) return;
        SetConfirmedState();
        SetParentFocus();
        onClickWithData.Invoke(Data);
    }
}