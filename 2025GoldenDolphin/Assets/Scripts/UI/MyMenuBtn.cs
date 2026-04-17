using UnityEngine;

public class MyMenuBtn : MyBtn
{
    [SerializeField] private GameObject handPointer;
    
    public override void OnSelect()
    {
        handPointer.SetActive(true);
    }

    public override void OnDeselect()
    {
        handPointer.SetActive(false);
    }
}