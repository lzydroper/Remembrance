using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Image))]
public class MyBtn : MonoBehaviour
{
    // --- 公开配置 ---
    [Header("状态颜色")]
    public Color normalColor = Color.white;
    public Color selectedColor = new Color(0.9f, 0.9f, 0.9f, 1f);
    public Color confirmedColor = Color.gray; // 新增：确认后的颜色

    [Header("按钮事件")]
    public UnityEvent onClick = new UnityEvent();

    // --- 状态 ---
    private bool _isInteractable = true;
    public bool IsInteractable => _isInteractable;

    // --- 私有引用 ---
    private Image _image;
    private MyBtnNavigation _navigationParent; // 新增：对父导航器的引用

    void Awake()
    {
        _image = GetComponent<Image>();
        // 初始设置为普通颜色
        _image.color = normalColor;
    }

    // 由父导航器在初始化时调用
    public void Initialize(MyBtnNavigation parent)
    {
        _navigationParent = parent;
    }

    // 当按钮被导航系统选中时调用
    public void OnSelect()
    {
        if (!_isInteractable) return; // 如果不可交互，则不能被选中
        if (_image != null)
        {
            _image.color = selectedColor;
        }
    }

    // 当按钮被导航系统取消选中时调用
    public void OnDeselect()
    {
        if (!_isInteractable) return; // 不可交互的按钮保持其确认颜色
        if (_image != null)
        {
            _image.color = normalColor;
        }
    }

    // 当按钮被确认（点击）时调用
    public void OnClick()
    {
        if (!_isInteractable) return; // 如果不可交互，则不执行任何操作

        // 触发在Inspector中配置的事件
        onClick.Invoke();
    }

    // !!! 核心新方法: 将按钮设置为确认状态 !!!
    // 你可以从按钮自己的 onClick 事件中调用此方法
    public void SetConfirmedState()
    {
        if (!_isInteractable) return;

        _isInteractable = false;
        _image.color = confirmedColor;

        // 通知父导航器，它需要更新焦点
        if (_navigationParent != null)
        {
            _navigationParent.OnButtonConfirmed(this);
        }
    }

    // (可选) 用于重置按钮状态的方法
    public void ResetState()
    {
        _isInteractable = true;
        _image.color = normalColor;
    }
}