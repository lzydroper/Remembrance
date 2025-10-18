using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInputController : ScriptableObject, PlayerControl.IPlayer1Actions, PlayerControl.IPlayer2Actions
{
    private PlayerControl playerControl;
    private PlayerInput playerInput;
    
    #region Events

    // TODO: 添加事件
    public event UnityAction onPause = delegate { };
    public event UnityAction onMoveUp1 = delegate { };
    public event UnityAction onMoveDown1 = delegate { };
    public event UnityAction onMoveLeft1 = delegate { };
    public event UnityAction onMoveRight1 = delegate { };
    public event UnityAction onRotate1 = delegate { };
    public event UnityAction onConfirm1 = delegate { };
    public event UnityAction onMoveUp2 = delegate { };
    public event UnityAction onMoveDown2 = delegate { };
    public event UnityAction onMoveLeft2 = delegate { };
    public event UnityAction onMoveRight2 = delegate { };
    public event UnityAction onRotate2 = delegate { };
    public event UnityAction onConfirm2 = delegate { };

    #endregion

    #region Life-cycle

    private void OnEnable()
    {
        playerControl = new PlayerControl();
        
        // TODO： 初始化回调
        playerControl.Player1.SetCallbacks(this);
        playerControl.Player2.SetCallbacks(this);
    }

    private void OnDisable()
    {
        DisableAllInputs();
    }

    #endregion
    
    #region ChangeMap

    private void DisableAllInputs()
    {
        playerControl.Player1.Disable();
        playerControl.Player2.Disable();
        playerControl.PlayerRebinding.Disable();
    }

    public void EnableGameplay1Input()
    {
        DisableAllInputs();
        playerControl.Player1.Enable();
    }

    public void EnableGameplay2Input()
    {
        DisableAllInputs();
        playerControl.Player2.Enable();
    }
    
    private void PlayerControlToRebinding()
    {
        DisableAllInputs();
        playerControl.PlayerRebinding.Enable();
    }
    private void PlayerRebindingToControl()
    {
        DisableAllInputs();
        playerControl.Player1.Enable();
    }

    public void PLayer1AndPlayer2ToControl()
    {
        DisableAllInputs();
        playerControl.Player1.Enable();
        playerControl.Player2.Enable();
    }
    
    #endregion

    #region Rebinding

    /// <summary>
    /// 重新绑定单个键
    /// </summary>
    /// <param name="reference">需要绑定的改绑的键</param>
    /// <param name="text">显示文本</param>
    public void RebindingSingle(InputActionReference reference, Text text)
    {
        Debug.Log("rebinding");
        PlayerControlToRebinding();
        text.text = "Enter...";
        reference.action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation =>
            {
                var p = operation.action.bindings[0].effectivePath;
                var s = InputControlPath.ToHumanReadableString(p,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                text.text = $"{s}";
                Debug.Log("Done.");
                operation.Dispose();
                PlayerRebindingToControl();
            })
            .OnCancel(operation =>
            {
                var p = operation.action.bindings[0].effectivePath;
                var s = InputControlPath.ToHumanReadableString(p,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                text.text = $"{s}";
                Debug.Log("Done.");
                operation.Dispose();
                PlayerRebindingToControl();
            })
            .Start();
    }
    /// <summary>
    /// 重新绑定二维向量之Up
    /// </summary>
    /// <param name="reference"></param>
    /// <param name="text"></param>
    public void RebindingVector2Up(InputActionReference reference, Text text)
    {
        Debug.Log("rebinding");
        PlayerControlToRebinding();
        text.text = "Enter...";
        var vector2 = reference.action.ChangeCompositeBinding("2D Vector");
        var up = vector2.NextCompositeBinding("Up");
        reference.action.PerformInteractiveRebinding()
            .OnMatchWaitForAnother(0.1f)
            .WithControlsExcluding("Mouse")
            .WithCancelingThrough("<Keyboard>/escape")
            .OnComplete(operation =>
            {
                var p = reference.action.GetBindingDisplayString(up.bindingIndex);
                var s = InputControlPath.ToHumanReadableString(p,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                text.text = $"{s}";
                Debug.Log("Done.");
                operation.Dispose();
                PlayerRebindingToControl();
            })
            .OnCancel(operation =>
            {
                var p = reference.action.GetBindingDisplayString(up.bindingIndex);
                var s = InputControlPath.ToHumanReadableString(p,
                    InputControlPath.HumanReadableStringOptions.OmitDevice);
                text.text = $"{s}";
                Debug.Log("Done.");
                operation.Dispose();
                PlayerRebindingToControl();
            })
            .Start();
    }
    
    /// <summary>
    /// 存储键位
    /// 使用最简单的PLayerPrefs
    /// </summary>
    public void SaveRebinding()
    {
        string referenceData = playerInput.actions.SaveBindingOverridesAsJson();
        Debug.Log(referenceData);
        PlayerPrefs.SetString("binding", referenceData);
    }
    
    /// <summary>
    /// 读取键位
    /// 文本的显示请在调用后进行修改
    /// </summary>
    public void LoadRebinding()
    {
        var bindingData = PlayerPrefs.GetString("binding");
        if (bindingData == "")
        {
            Debug.Log("There is no binding data.");
        }
        else
        {
            playerInput.actions.LoadBindingOverridesFromJson(bindingData);
            Debug.Log("Binding done.");
        }
    }

    #endregion

    #region Functions

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onPause.Invoke();
        }
    }

    public void OnMoveUp2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMoveUp2.Invoke();
        }
    }

    public void OnMoveDown2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMoveDown2.Invoke();
        }
    }

    public void OnMoveLeft2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMoveLeft2.Invoke();
        }
    }

    public void OnMoveRight2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMoveRight2.Invoke();
        }
    }

    public void OnRotate2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onRotate2.Invoke();   
        }
    }

    public void OnConfirm2(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onConfirm2.Invoke();
        }
    }

    public void OnMoveUp1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {   
            onMoveUp1.Invoke();
        }
    }

    public void OnMoveDown1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMoveDown1.Invoke();   
        }
    }

    public void OnMoveLeft1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMoveLeft1.Invoke();
        }
    }

    public void OnMoveRight1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMoveRight1.Invoke();
        }
    }

    public void OnRotate1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onRotate1.Invoke();
        }
    }

    public void OnConfirm1(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onConfirm1.Invoke();
        }
    }

    #endregion
}
