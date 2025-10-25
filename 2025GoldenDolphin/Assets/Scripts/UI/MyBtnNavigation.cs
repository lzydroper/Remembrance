using System.Collections;
using System.Collections.Generic;
using PhaseSystem;
using UnityEngine;

public class MyBtnNavigation : MonoBehaviour
{
    // 导航系统根据这个来判断事件订阅情况
    [SerializeField] private bool setForPlayer1 = true;
    [SerializeField] private bool setForPlayer2 = true;
    // --- 私有变量 ---
    private List<MyBtn> _buttons = new();
    private int _selectedIndex = -1;

    // 当前选中的按钮索引
    public int SelectedIndex
    {
        get => _selectedIndex;
        private set
        {
            // 如果旧索引有效，取消选中
            if (_selectedIndex >= 0 && _selectedIndex < _buttons.Count)
            {
                _buttons[_selectedIndex].OnDeselect();
            }

            _selectedIndex = value;
            
            // 如果新索引有效，选中它
            if (_selectedIndex >= 0 && _selectedIndex < _buttons.Count)
            {
                _buttons[_selectedIndex].OnSelect();
            }
        }
    }

    private void Start()
    {
        _buttons.Clear();
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeInHierarchy && child.TryGetComponent<MyBtn>(out var btn))
            {
                _buttons.Add(btn);
                btn.Initialize(this);
            }
        }
    }

    void OnEnable()
    {
        // InitBtn();
        SubscribeInputEvents();
        StartCoroutine(SelectFirstAvailableButtonAfterFrame());
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
        SelectedIndex = firstAvailable;
    }

    private void SubscribeInputEvents()
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

    private void UnsubscribeInputEvents()
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
            _buttons[_selectedIndex].OnClick();
        }
    }

    // 核心导航逻辑
    private void Navigate(Vector2 direction)
    {
        if (_selectedIndex == -1 || _buttons.Count <= 1) return;

        MyBtn currentButton = _buttons[_selectedIndex];
        Vector3 currentPos = currentButton.transform.position;
        
        int bestTargetIndex = -1;
        float minDistance = float.MaxValue;

        for (int i = 0; i < _buttons.Count; i++)
        {
            if (i == _selectedIndex) continue;

            // !!! 新增：跳过不可交互的按钮 !!!
            if (!_buttons[i].IsInteractable) continue;
            
            MyBtn targetButton = _buttons[i];
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
        if (_buttons.Count == 0) return -1;

        // 从 startIndex + 1 开始循环查找
        for (int i = 1; i <= _buttons.Count; i++)
        {
            int checkIndex = (startIndex + i) % _buttons.Count;
            if (_buttons[checkIndex].IsInteractable)
            {
                return checkIndex; // 找到了，返回索引
            }
        }

        return -1; // 没找到任何可用的按钮
    }
    
    public void ResetAllButtons()
    {
        foreach (var btn in _buttons)
        {
            btn.ResetState();
        }
        // 重置后，重新选择第一个可用的
        SelectedIndex = FindNextAvailableButton(-1);
    }
}