using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    [SerializeField] private PlayerInputController inputController;
    // 移动事件，返回方向
    public static UnityAction<Vector2> OnP1Move;
    public static UnityAction OnP1Confirm;
    public static UnityAction OnP1Rotate;
    public static UnityAction<Vector2> OnP2Move;
    public static UnityAction OnP2Confirm;
    public static UnityAction OnP2Rotate;
    private void OnEnable()
    {
        // 输入绑定
    }

    #region 公共函数

    public void DisableAllInputs()
    {
        inputController.DisableAllInputs();
    }

    public void EnableAllInputs()
    {
        inputController.EnableAllInputs();
    }
    
    public void EnableGameplay1Input()
    {
        inputController.EnableGameplay1Input();
    }

    public void EnableGameplay2Input()
    {
        inputController.EnableGameplay2Input();
    }

    public void DisableGameplay1Input()
    {
        inputController.DisableGameplay1Input();
    }

    public void DisableGameplay2Input()
    {
        inputController.DisableGameplay2Input();
    }

    #endregion
        
    /*
     * 记录：
     * MainMenu时，P1(P2需要吗？)操作控制主界面按钮的光标移动和确认
     * Playing时，
     *      SelectItem控制光标移动选择物品
     */
}