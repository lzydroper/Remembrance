using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MyBtnNavigation : MonoBehaviour
{
    // 为scroll view增加的额外屎山
    [SerializeField] private ScrollRect contentScrollRect; 
    // 导航系统根据这个来判断事件订阅情况
    [SerializeField] private bool setForPlayer1 = true;
    [SerializeField] private bool setForPlayer2 = true;
    [SerializeField] private bool startForSelect = false;       // 是否在启用时，立即选择一个可选项
    // --- 私有变量 ---
    public List<MyBtn> buttons = new();
    private int _selectedIndex = -1;
    private bool _isInitialized = false;

    // 当前选中的按钮索引
    public int SelectedIndex
    {
        get => _selectedIndex;
        private set
        {
            // 如果旧索引有效，取消选中
            if (_selectedIndex >= 0 && _selectedIndex < buttons.Count)
            {
                buttons[_selectedIndex].OnDeselect();
            }

            _selectedIndex = value;
            
            // 如果新索引有效，选中它
            if (_selectedIndex >= 0 && _selectedIndex < buttons.Count)
            {
                var targetBtn = buttons[_selectedIndex];
                targetBtn.OnSelect();
                
                // scroll view 特判
                if (contentScrollRect)
                {
                    if (targetBtn.transform.IsChildOf(contentScrollRect.content))
                    {
                        AutoScrollTo(targetBtn.GetComponent<RectTransform>());
                    }
                }
            }
        }
    }

    private void AutoScrollTo(RectTransform target)
    {
        float targetY = -target.localPosition.y;
        
        contentScrollRect.content.DOLocalMoveY(
            targetY - (contentScrollRect.viewport.rect.height / 2),
            0.3f);
    }

    public void CancelSelect()
    {
        SelectedIndex = -1;
    }

    [ContextMenu("SelectFirstAvailable")]
    public void SelectFirstAvailable()
    {
        StartCoroutine(SelectFirstAvailableButtonAfterFrame());
    }

    public void Initialize()
    {
        // if (_isInitialized)
        //     return;
        // _isInitialized = true;
        buttons.Clear();
        MyBtn[] allButtons = GetComponentsInChildren<MyBtn>(true);
        foreach (var btn in allButtons)
        {
            buttons.Add(btn);
            btn.Initialize(this);
        }
        // foreach (Transform child in transform)
        // {
        //     if (child.TryGetComponent<MyBtn>(out var btn))
        //     {
        //         buttons.Add(btn);
        //         btn.Initialize(this);
        //     }
        // }
    }
    
    public void RefreshButtonList()
    {
        Initialize();
        // 刷新后，重新定位到一个默认按钮，防止焦点丢失
        SelectFirstAvailable();
    }

    void OnEnable()
    {
        Initialize();
        ResetAllButtons();
        SubscribeInputEvents();
        
        if (startForSelect)
            SelectFirstAvailable();
    }

    void OnDisable()
    {
        UnsubscribeInputEvents();
        SelectedIndex = -1;
    }

    private IEnumerator SelectFirstAvailableButtonAfterFrame()
    {
        yield return null; // 等待一帧
        // 查找第一个可用的按钮并选中
        int firstAvailable = FindNextAvailableButton(-1);
        yield return null; // 等待setdata
        yield return null; // 等待setdata
        yield return null; // 等待setdata
        SelectedIndex = firstAvailable;
    }

    public void SubscribeInputEvents()
    {
        if (setForPlayer1)
        {
            InputManager.OnP1Confirm += Submit;
            InputManager.OnP1Move += Navigate;
        }

        if (setForPlayer2)
        {
            InputManager.OnP2Confirm += Submit;
            InputManager.OnP2Move += Navigate;
        }
    }

    public void UnsubscribeInputEvents()
    {
        if (setForPlayer1)
        {
            InputManager.OnP1Confirm -= Submit;
            InputManager.OnP1Move -= Navigate;
        }

        if (setForPlayer2)
        {
            InputManager.OnP2Confirm -= Submit;
            InputManager.OnP2Move -= Navigate;
        }
    }
    
    // 提交（点击）当前选中的按钮
    private void Submit()
    {
        if (_selectedIndex != -1)
        {
            buttons[_selectedIndex].OnClick();
        }
    }

    // 核心导航逻辑
    private void Navigate(Vector2Int direction)
    {
        if (_selectedIndex == -1 || buttons.Count <= 1) return;

        MyBtn currentButton = buttons[_selectedIndex];
        Vector3 currentPos = currentButton.transform.position;
        
        int bestTargetIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < buttons.Count; i++)
        {
            if (i == _selectedIndex) continue;

            // !!! 新增：跳过不可交互的按钮 !!!
            if (!buttons[i].isActiveAndEnabled ||
                !buttons[i].IsInteractable) 
                continue;
            
            MyBtn targetButton = buttons[i];
            Vector3 targetPos = targetButton.transform.position;
            Vector3 toTarget = targetPos - currentPos;
            
            if (Vector2.Dot(direction, toTarget.normalized) < 0.3f)
            {
                continue;
            }

            float distance;
            if (direction == Vector2.up || direction == Vector2.down)
            {
                distance = Mathf.Abs(toTarget.x) * 10f + Mathf.Abs(toTarget.y);
            }
            else
            {
                distance = Mathf.Abs(toTarget.y) * 10f + Mathf.Abs(toTarget.x);
            }
            
            if (distance < minDistance)
            {
                minDistance = distance;
                bestTargetIndex = i;
            }
        }

        if (bestTargetIndex != -1)
        {
            SelectedIndex = bestTargetIndex;
        }
    }
    
    public void OnButtonConfirmed(MyBtn confirmedButton)
    {
        // 查找下一个可用的按钮
        int nextIndex = FindNextAvailableButton(_selectedIndex);
        
        // 将焦点切换到下一个可用的按钮
        SelectedIndex = nextIndex;
    }
    
    private int FindNextAvailableButton(int startIndex)
    {
        if (buttons.Count == 0) return -1;

        // 从 startIndex + 1 开始循环查找
        for (int i = 1; i <= buttons.Count; i++)
        {
            int checkIndex = (startIndex + i) % buttons.Count;
            // 防止按钮处于inactive状态
            if (buttons[checkIndex].isActiveAndEnabled && buttons[checkIndex].IsInteractable)
            {
                return checkIndex; // 找到了，返回索引
            }
        }

        return -1; // 没找到任何可用的按钮
    }
    
    public void ResetAllButtons()
    {
        foreach (var btn in buttons)
        {
            btn.ResetState();
        }
    }
}