using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NewBagSystem;
using UnityEngine;

namespace Control
{
    public class PlaceItemControl : MonoBehaviour
    {
        [SerializeField] private GameObject[] willRepairHint;
        [SerializeField] private GameObject[] willForceRepairHint;
        
        [SerializeField] private MyBtnNavigation[] navs;
        [SerializeField] private BagController[] bags;
        private List<ConfirmType> choices = new() { ConfirmType.None, ConfirmType.None };
        private bool _bothConfirm;
        public IEnumerator Flow()
        {
            InputManager.instance.EnableAllInputs();
            _bothConfirm = false;
            for (int i = 0; i < 2; i++)
            {
                choices[i] = ConfirmType.None;
                willRepairHint[i].SetActive(false);
                willForceRepairHint[i].transform.DOScale(0f, 0f);
            }
            
            // 订阅事件
            foreach (var nav in navs)
            {
                nav.gameObject.SetActive(true);
                nav.UnsubscribeInputEvents();   // 此时不允许输入（我的代码好乱啊，感觉没人能看懂了Orz）
                // nav.gameObject.SetActive(true);
                List<MyConfirmBtn> btns = nav.buttons.Cast<MyConfirmBtn>().ToList();
                foreach (var btn in btns)
                {
                    btn.onClickWithData.AddListener(OnConfirm);     // 最开始不想着用static action，现在被折磨了
                }
            }
            BagController.OnPlace += OnPlace;
            BagController.OnCannotPlace += OnCannotPlace;
            // 开启整个BagController，触发OnEnable订阅输入事件
            foreach (var bag in bags)
            {
                bag.enabled = true;
            }
            // 根据GameManager的选择结果，让BagController生成物品        =>      在OnEnable中自动完成
            // 订阅两个navigation的btn onclick事件，在回调中记录选项选择情况和选择结果
            // 订阅BagController的static action回调确认放置，在回调中开启对应的navigation
            // 等待双方选择完毕
            yield return new WaitUntil(() => _bothConfirm);
            // 取消各类订阅事件
            foreach (var nav in navs)
            {
                nav.gameObject.SetActive(false);
                List<MyConfirmBtn> btns = nav.buttons.Cast<MyConfirmBtn>().ToList();
                foreach (var btn in btns)
                {
                    btn.onClickWithData.RemoveListener(OnConfirm);     // 最开始不想着用static action，现在被折磨了
                }
            }
            BagController.OnPlace -= OnPlace;
            BagController.OnCannotPlace -= OnCannotPlace;
            // 关闭BagController
            foreach (var bag in bags)
            {
                bag.enabled = false;
            }
            // 执行结果
            Cal();
            // 告知GameManager选择结果
            yield return null;
        }

        private void Cal()
        {
            // 根据是否修复来计算结果
            for (int i = 0; i < 2; i++)
            {
                if (choices[i] == ConfirmType.Repair)
                {
                    // 寻找配方
                    List<string> items = bags[i].GetPlacedItems();
                    Recipe recipe = ItemManager.instance.GetRecipe(items);
                    // 告知
                    GameManager.instance.SetCookResult(i, recipe);
                    // 清空背包
                    bags[i].ClearBag();
                }
            }
        }

        private void OnPlace(int playerID)
        {
            // 放置后应该让玩家选择选项
            navs[playerID].SubscribeInputEvents();
            navs[playerID].SelectFirstAvailable();
        }

        private void OnCannotPlace(int playerID)
        {
            // 无法放置时提醒将要强制放置
            GameObject go = willForceRepairHint[playerID];
            go.SetActive(true);
            go.transform.DOScale(1f, 0.5f).OnComplete(() =>
            {
                go.transform.DOScale(0f, 0.5f);
                willRepairHint[playerID].SetActive(true);
            });
            choices[playerID] = ConfirmType.Repair;
        }

        private void OnConfirm(ConfirmData data)
        {
            navs[data.playerID].UnsubscribeInputEvents();
            choices[data.playerID] = data.confirmType;
            // 若选择修复，展示将要修复的提示信息
            if (data.confirmType == ConfirmType.Repair)
                willRepairHint[data.playerID].SetActive(true);

            if (choices[0] != ConfirmType.None && choices[1] != ConfirmType.None)
                _bothConfirm = true;
        }
        
        [ContextMenu("StartFlow")]
        private void StartFlow()
        {
            StartCoroutine(Flow());
        }
    }
}