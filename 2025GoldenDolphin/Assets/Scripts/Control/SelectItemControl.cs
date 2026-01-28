using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using NewBagSystem;
using SKCell;
using UnityEngine;

/*
 * 选择物品阶段的流程控制
 */
namespace Control
{
    public class SelectItemControl : MonoBehaviour
    {
        // [SerializeField] private InputManager inputManager;
        // 选择物品后，实体化物体生成的位置
        [SerializeField] private Transform objectGenerateTransform;
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
            // selectUIGroup.SelectFirstAvailable();
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
            selectUIGroup.SelectFirstAvailable();
            InputManager.instance.SwitchInput(_currentPlayer);
            _firstSelected = false;
            yield return new WaitUntil(() => _firstSelected);
            // GameManager.instance.SetCursorState(_currentPlayer, CursorState.FinishedSelect);
            // 显示后手动画
            InputManager.instance.DisableAllInputs();
            _currentPlayer = (_firstPlayer + 1) % 2;
            yield return HintMoveAni(_currentPlayer);
            // 等待后手选择
            // 切换玩家控制权
            // GameManager.instance.SetCursorState(_currentPlayer, CursorState.SelectItem);
            selectUIGroup.SelectFirstAvailable();
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

        [Header("开始选择提示动画相关")] 
        [SerializeField] private GameObject hintPanel;
        [SerializeField] private GameObject arrow;
        [SerializeField] private SKText hintText;
        [SerializeField] private float enterDuration = 1.5f;   // 进入动画时长
        [SerializeField] private float stayDuration = 1.0f;    // 停留时间
        [SerializeField] private float exitDuration = 1.15f;    // 退出动画时长
        private Vector3 _staRotation = new Vector3(0f, 0f, 90f);
        private IEnumerator HintMoveAni(int playerID)
        {
            hintPanel.SetActive(true);
            // arrow.SetActive(true);
            // hintText.gameObject.SetActive(true);
            yield return null;
            
            float targetAngle = playerID == 0 ? 540f : -360f;
            arrow.transform.DOScale(1f, enterDuration).From(0f).SetEase(Ease.OutBack);
            arrow.transform.DORotate(new Vector3(0f, 0f, targetAngle), enterDuration,
                RotateMode.FastBeyond360).From(_staRotation).SetEase(Ease.OutQuad);
            hintText.DOFade(1f, enterDuration).From(0f);
            
            yield return new WaitForSeconds(enterDuration);
            yield return new WaitForSeconds(stayDuration);
            
            // 反向播放
            arrow.transform.DOScale(0f, exitDuration).SetEase(Ease.OutBack);
            arrow.transform.DORotate(new Vector3(0f, 0f, -targetAngle), exitDuration,
                RotateMode.FastBeyond360).SetEase(Ease.InQuad);
            hintText.DOFade(0f, exitDuration);
            
            yield return new WaitForSeconds(exitDuration - 0.5f);
            
            // arrow.SetActive(false);
            // hintText.gameObject.SetActive(false);
            hintPanel.SetActive(false);
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
            // 选择物品后，在指定位置实例化对应3d物体
            GameObject d3Object = Instantiate(item.d3Prefab, objectGenerateTransform);
            d3Object.transform.position = objectGenerateTransform.transform.position;
        }

        [ContextMenu("StartFlow")]
        private void StartFlow()
        {
            StartCoroutine(Flow());
        }

        [ContextMenu("ShowHintMoveAni")]
        private void ShowHintMoveAni()
        {
            StartCoroutine(HintMoveAni(1));
        }
    }
}