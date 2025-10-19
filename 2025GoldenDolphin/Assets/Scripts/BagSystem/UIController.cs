using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;

namespace BagSystem
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] public GameObject BtnUIPanel;
        private int _selectedIndex = -1; // -1 表示没有选中任何按钮
        public bool thinkBtn { get; private set; }= false;
        public bool cookBtn { get; private set; } = false;

        public void ResetBtn()
        {
            thinkBtn = false;
            cookBtn = false;
        }

        // // // --- 按钮动作接口 (使用事件) ---
        public event Action OnBtnClicked = delegate { };
        // public event Action OnFixButtonClicked;
        // public event Action OnWaitButtonClicked;
        
        #region Public API (供 PlayerCursorController 调用)

        /// <summary>
        /// 当光标进入UI区域时调用
        /// </summary>
        public void OnCursorEnter()
        {
            Debug.Log("[UIController] 光标进入UI区域");
            // 进入时不需要立即高亮，因为 OnCursorMove 会紧接着被调用
        }

        /// <summary>
        /// 当光标离开UI区域时调用
        /// </summary>
        public void OnCursorExit()
        {
            Debug.Log("[UIController] 光标离开UI区域");
            _selectedIndex = -1; // 重置选中索引
            ResetAllButtonVisuals();
        }

        /// <summary>
        /// 当光标在UI区域内移动时调用
        /// </summary>
        /// <param name="localUIPos">光标在UI区域内的本地坐标</param>
        public void OnCursorMove(Vector2Int localUIPos)
        {
            // 假设你的UI区域宽度为2，本地坐标的x值 0 代表左按钮，1 代表右按钮
            int newIndex = localUIPos.x;

            // 如果选择没有变化，则不执行任何操作
            if (newIndex == _selectedIndex) return;

            _selectedIndex = newIndex;
            UpdateVisuals();
        }

        /// <summary>
        /// 当在UI区域按下确认键时调用
        /// </summary>
        /// <param name="localUIPos">光标在UI区域内的本地坐标</param>
        public void OnConfirmAction(Vector2Int localUIPos)
        {
            // if (_selectedIndex < 0 || _selectedIndex >= buttonImages.Length) return;

            Debug.Log($"[UIController] 确认了按钮: {_selectedIndex}");

            // 确认效果
            ConfirmVisuals(_selectedIndex);

            // 根据选中的按钮触发对应的事件
            switch (_selectedIndex)
            {
                case 0: // 第一个按钮：“修复”
                    cookBtn = true;
                    // OnFixButtonClicked?.Invoke();
                    break;
                case 1: // 第二个按钮：“再等等”
                    thinkBtn = true;
                    // OnWaitButtonClicked?.Invoke();
                    break;
            }
            
            // 回调告知
            OnBtnClicked?.Invoke();
        }

        #endregion
        
        #region 视觉效果处理

        /// <summary>
        /// 根据当前选中的索引更新所有按钮的视觉效果
        /// </summary>
        private void UpdateVisuals()
        {
        }

        /// <summary>
        /// 将所有按钮重置为正常状态
        /// </summary>
        private void ResetAllButtonVisuals()
        {
        }

        /// <summary>
        /// 播放按钮按下的视觉反馈动画
        /// </summary>
        private void ConfirmVisuals(int index)
        {
        }

        #endregion
    }
}