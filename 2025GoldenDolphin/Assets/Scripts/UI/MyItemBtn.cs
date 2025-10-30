using System.Collections.Generic;
using DG.Tweening;
using NewBagSystem;
using UnityEngine;
using UnityEngine.UI;

public class MyItemBtn : MyBtnData<BasicItemData>
{
    [Header("UI 元素")]
    [SerializeField] private Image icon;
    // [SerializeField] private Text hintRecipe;       // 提示玩家当前item的可能候选合成配方
    // private List<Recipe> _recipeCandidate;

    public override void SetData(BasicItemData newData)
    {
        base.SetData(newData);
        if (!newData)
        {
            if (icon) icon.sprite = null;
            return;
        }

        // 自动更新显示
        icon.sprite = newData.gridSprite;
        // hintRecipe.gameObject.SetActive(false);
        // _recipeCandidate = ItemManager.instance.GetPossibleRecipes(newData.id);
    }

    public override void ResetState()
    {
        isInteractable = true;
        icon.color = Color.white;
        // hintRecipe.text = "";
        // hintRecipe.gameObject.SetActive(false);
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
        // hintRecipe.text = "";
        // hintRecipe.gameObject.SetActive(false);
    }
}