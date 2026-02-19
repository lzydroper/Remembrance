using Photon.Pun;
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
        
        // 默认启用输入
        EnableAllInputs();
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
    // private void OnP1MoveUp()
    // {
    //     OnP1Move?.Invoke(Vector2Int.up);
    // }
    //
    // private void OnP1MoveDown()
    // {
    //     OnP1Move?.Invoke(Vector2Int.down);
    // }
    //
    // private void OnP1MoveLeft()
    // {
    //     OnP1Move?.Invoke(Vector2Int.left);
    // }
    //
    // private void OnP1MoveRight()
    // {
    //     OnP1Move?.Invoke(Vector2Int.right);
    // }
    private void OnP1MoveUp()    { OnP1MoveDir(Vector2Int.up); }
    private void OnP1MoveDown()  { OnP1MoveDir(Vector2Int.down); }
    private void OnP1MoveLeft()  { OnP1MoveDir(Vector2Int.left); }
    private void OnP1MoveRight() { OnP1MoveDir(Vector2Int.right); }
    
    private void OnP1MoveDir(Vector2Int dir)
    {
        // 单机模式直接操作
        if (!GameManager.instance.GetIsMultiPlaying())
        {
            OnP1Move?.Invoke(dir);
        }
        // 联机模式判断是否有权限操作，具体为若是主机，则只能操作P1相关动作，若是客机，则只能操作P2相关动作
        else if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC(nameof(RpcP1Move), RpcTarget.All, dir);
        }
    }

    // 玩家1确认输入处理函数
    private void OnP1ConfirmInput()
    {
        // 单机模式直接操作
        if (!GameManager.instance.GetIsMultiPlaying())
        {
            OnP1Confirm?.Invoke();
        }
        // 联机模式判断是否有权限操作，具体为若是主机，则只能操作P1相关动作，若是客机，则只能操作P2相关动作
        else if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC(nameof(RpcP1Confirm), RpcTarget.All);
        }
    }

    // 玩家1旋转输入处理函数
    private void OnP1RotateInput()
    {
        // 单机模式直接操作
        if (!GameManager.instance.GetIsMultiPlaying())
        {
            OnP1Rotate?.Invoke();
        }
        // 联机模式判断是否有权限操作，具体为若是主机，则只能操作P1相关动作，若是客机，则只能操作P2相关动作
        else if (PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC(nameof(RpcP1Rotate), RpcTarget.All);
        }
    }

    // 玩家2移动输入处理函数
    // private void OnP2MoveUp()
    // {
    //     OnP2Move?.Invoke(Vector2Int.up);
    // }
    //
    // private void OnP2MoveDown()
    // {
    //     OnP2Move?.Invoke(Vector2Int.down);
    // }
    //
    // private void OnP2MoveLeft()
    // {
    //     OnP2Move?.Invoke(Vector2Int.left);
    // }
    //
    // private void OnP2MoveRight()
    // {
    //     OnP2Move?.Invoke(Vector2Int.right);
    // }
    private void OnP2MoveUp()    { OnP2MoveDir(Vector2Int.up); }
    private void OnP2MoveDown()  { OnP2MoveDir(Vector2Int.down); }
    private void OnP2MoveLeft()  { OnP2MoveDir(Vector2Int.left); }
    private void OnP2MoveRight() { OnP2MoveDir(Vector2Int.right); }
    
    private void OnP2MoveDir(Vector2Int dir)
    {
        // 单机模式直接操作
        if (!GameManager.instance.GetIsMultiPlaying())
        {
            OnP2Move?.Invoke(dir);
        }
        // 联机模式判断是否有权限操作，具体为若是主机，则只能操作P1相关动作，若是客机，则只能操作P2相关动作
        else if (!PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC(nameof(RpcP2Move), RpcTarget.All, dir);
        }
    }

    // 玩家2确认输入处理函数
    private void OnP2ConfirmInput()
    {
        // 单机模式直接操作
        if (!GameManager.instance.GetIsMultiPlaying())
        {
            OnP2Confirm?.Invoke();
        }
        // 联机模式判断是否有权限操作，具体为若是主机，则只能操作P1相关动作，若是客机，则只能操作P2相关动作
        else if (!PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC(nameof(RpcP2Confirm), RpcTarget.All);
        }
    }

    // 玩家2旋转输入处理函数
    private void OnP2RotateInput()
    {
        // 单机模式直接操作
        if (!GameManager.instance.GetIsMultiPlaying())
        {
            OnP2Rotate?.Invoke();
        }
        // 联机模式判断是否有权限操作，具体为若是主机，则只能操作P1相关动作，若是客机，则只能操作P2相关动作
        else if (!PhotonNetwork.IsMasterClient)
        {
            GameManager.instance.photonView.RPC(nameof(RpcP2Rotate), RpcTarget.All);
        }
    }

    #endregion

    #region 联机输入RPC处理

    [PunRPC]
    private void RpcP1Move(Vector2Int dir)
    {
        OnP1Move?.Invoke(dir); // 触发事件，UI听到后会移动光标
    }

    [PunRPC]
    private void RpcP1Confirm()
    {
        OnP1Confirm?.Invoke();
    }

    [PunRPC]
    private void RpcP1Rotate()
    {
        OnP1Rotate?.Invoke();
    }

    [PunRPC]
    private void RpcP2Move(Vector2Int dir)
    {
        OnP2Move?.Invoke(dir);
    }

    [PunRPC]
    private void RpcP2Confirm()
    {
        OnP2Confirm?.Invoke();
    }

    [PunRPC]
    private void RpcP2Rotate()
    {
        OnP2Rotate?.Invoke();
    }

    #endregion
}