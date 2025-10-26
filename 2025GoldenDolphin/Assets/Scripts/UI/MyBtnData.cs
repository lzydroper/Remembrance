using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MyBtnData<T> : MyBtn
{
    [Header("按钮携带的数据")]
    [SerializeField] private T data;
    public T Data => data;

    // 泛型版本的点击事件
    [System.Serializable]
    public class MyBtnEvent : UnityEvent<T> { }
    public MyBtnEvent onClickWithData = new MyBtnEvent();

    // 允许外部设置数据
    public virtual void SetData(T newData)
    {
        data = newData;
    }

    public override void OnClick()
    {
        if (!IsInteractable) return;
        onClickWithData.Invoke(data);
    }
}