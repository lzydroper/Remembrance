using SKCell;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : SKMonoSingleton<InputManager>
{
    [SerializeField] private PlayerInputController inputController;
    // 移动事件，返回方向
    public static UnityAction<Vector2Int> OnP1Move;
    public static UnityAction OnP1Confirm;
    public static UnityAction OnP1Rotate;
    public static UnityAction<Vector2Int> OnP2Move;
    public static UnityAction OnP2Confirm;
    public static UnityAction OnP2Rotate;
    private void OnEnable()
    {
        // 输入绑定
        // --- 玩家1 输入绑定 ---
        inputController.onMoveUp1 += OnP1MoveUp;
        inputController.onMoveDown1 += OnP1MoveDown;
        inputController.onMoveLeft1 += OnP1MoveLeft;
        inputController.onMoveRight1 += OnP1MoveRight;
        inputController.onConfirm1 += OnP1ConfirmInput;
        inputController.onRotate1 += OnP1RotateInput;

        // --- 玩家2 输入绑定 ---
        inputController.onMoveUp2 += OnP2MoveUp;
        inputController.onMoveDown2 += OnP2MoveDown;
        inputController.onMoveLeft2 += OnP2MoveLeft;
        inputController.onMoveRight2 += OnP2MoveRight;
        inputController.onConfirm2 += OnP2ConfirmInput;
        inputController.onRotate2 += OnP2RotateInput;
    }

    #region 公共函数

    public void SwitchInput(int playerID)
    {
        DisableAllInputs();
        EnableInput(playerID);
    }
    
    public void EnableInput(int playerID)
    {
        if (playerID == 0)
        {
            inputController.EnableGameplay1Input();
        }
        else if (playerID == 1)
        {
            inputController.EnableGameplay2Input();
        }
    }

    public void DisableInput(int playerID)
    {
        if (playerID == 0)
        {
            inputController.DisableGameplay1Input();
        }
        else if (playerID == 1)
        {
            inputController.DisableGameplay2Input();
        }
    }
    
    [ContextMenu("DisableAllInput")]
    public void DisableAllInputs()
    {
        inputController.DisableAllInputs();
    }

    [ContextMenu("EnableAllInput")]
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

    #region 私有功能函数

    // 玩家1移动输入处理函数
    private void OnP1MoveUp()
    {
        OnP1Move?.Invoke(Vector2Int.up);
    }

    private void OnP1MoveDown()
    {
        OnP1Move?.Invoke(Vector2Int.down);
    }

    private void OnP1MoveLeft()
    {
        OnP1Move?.Invoke(Vector2Int.left);
    }

    private void OnP1MoveRight()
    {
        OnP1Move?.Invoke(Vector2Int.right);
    }

    // 玩家1确认输入处理函数
    private void OnP1ConfirmInput()
    {
        OnP1Confirm?.Invoke();
    }

    // 玩家1旋转输入处理函数
    private void OnP1RotateInput()
    {
        OnP1Rotate?.Invoke();
    }

    // 玩家2移动输入处理函数
    private void OnP2MoveUp()
    {
        OnP2Move?.Invoke(Vector2Int.up);
    }

    private void OnP2MoveDown()
    {
        OnP2Move?.Invoke(Vector2Int.down);
    }

    private void OnP2MoveLeft()
    {
        OnP2Move?.Invoke(Vector2Int.left);
    }

    private void OnP2MoveRight()
    {
        OnP2Move?.Invoke(Vector2Int.right);
    }

    // 玩家2确认输入处理函数
    private void OnP2ConfirmInput()
    {
        OnP2Confirm?.Invoke();
    }

    // 玩家2旋转输入处理函数
    private void OnP2RotateInput()
    {
        OnP2Rotate?.Invoke();
    }

    #endregion
}