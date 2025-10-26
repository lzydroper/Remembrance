using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NewBagSystem;
using UnityEngine;

/*
 * 选择物品阶段的流程控制
 */
namespace Control
{
    public class SelectItemControl : MonoBehaviour
    {
        // [SerializeField] private InputManager inputManager;
        [SerializeField] private MyBtnNavigation selectUIGroup;
        private int _firstPlayer = -1;
        public int _currentPlayer = -1;
        public BasicItemData[] _selectResult = new BasicItemData[2];
        private bool _firstSelected;
        private bool _secondSelected;
        public IEnumerator Flow()
        {
            // 显示UI
            selectUIGroup.gameObject.SetActive(true);
            selectUIGroup.SelectFirstAvailable();
            // 获取四个随机物品
            List<BasicItemData> randomItems = ItemManager.instance.GetRandomItem(4);
            // 更新选项image
            List<MyItemBtn> btns = selectUIGroup.buttons.Cast<MyItemBtn>().ToList();
            int n = Mathf.Min(btns.Count, randomItems.Count);
            for (int i = 0; i < n; i++)
            {
                btns[i].SetData(randomItems[i]);
            }
            // 订阅按钮按下事件
            foreach (MyItemBtn btn in btns)
            {
                btn.onClickWithData.AddListener(OnItemSelected);
            }
            // 判断先后顺序
            _firstPlayer = (_firstPlayer + 1) % 2;
            _currentPlayer = _firstPlayer;
            // 显示先手动画
            yield return HintMoveAni(_currentPlayer);
            // 等待先手选择
            // 切换玩家控制权
            // GameManager.instance.SetCursorState(_currentPlayer, CursorState.SelectItem);
            InputManager.instance.SwitchInput(_currentPlayer);
            _firstSelected = false;
            yield return new WaitUntil(() => _firstSelected);
            // GameManager.instance.SetCursorState(_currentPlayer, CursorState.FinishedSelect);
            // 显示后手动画
            _currentPlayer = (_firstPlayer + 1) % 2;
            yield return HintMoveAni(_currentPlayer);
            // 等待后手选择
            // 切换玩家控制权
            // GameManager.instance.SetCursorState(_currentPlayer, CursorState.SelectItem);
            InputManager.instance.SwitchInput(_currentPlayer);
            _secondSelected = false;
            yield return new WaitUntil(() => _secondSelected);
            // GameManager.instance.SetCursorState(_currentPlayer, CursorState.FinishedSelect);
            // 清空订阅事件
            foreach (MyItemBtn btn in btns)
            {
                btn.onClickWithData.RemoveListener(OnItemSelected);
            }
            // 关闭UI
            selectUIGroup.gameObject.SetActive(false);
            yield return null;
        }

        private IEnumerator HintMoveAni(int playerID)
        {
            // TODO:跳一段文字，文字内容为“开始选择！”
            yield return null;
        }

        private void OnItemSelected(BasicItemData item)
        {
            _selectResult[_currentPlayer] = item;
            // 告知GameManager选择结果
            GameManager.instance.SetSelectItem(_currentPlayer, item);
            if (_currentPlayer == _firstPlayer)
            {
                _firstSelected = true;
            }
            else
            {
                _secondSelected = true;
            }
        }

        [ContextMenu("StartFlow")]
        private void StartFlow()
        {
            StartCoroutine(Flow());
        }
    }
}