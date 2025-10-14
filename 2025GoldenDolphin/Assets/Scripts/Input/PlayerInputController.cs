using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInputController : ScriptableObject, PlayerControl.IGamePlayActions
{
    private PlayerControl playerControl;
    private PlayerInput playerInput;
    
    #region Events

    // TODO: 添加事件
    public event UnityAction onPause = delegate { };

    #endregion

    #region Life-cycle

    private void OnEnable()
    {
        playerControl = new PlayerControl();
        
        // TODO： 初始化回调
        playerControl.GamePlay.SetCallbacks(this);
    }

    private void OnDisable()
    {
        DisableAllInputs();
    }

    #endregion
    
    #region ChangeMap

    private void DisableAllInputs()
    {
        playerControl.GamePlay.Disable();
    }

    public void EnableGameplayInput()
    {
        playerControl.GamePlay.Enable();
    }
    
    private void PlayerControlToRebinding()
    {
        playerControl.GamePlay.Disable();
        playerControl.PlayerRebinding.Enable();
    }
    private void PlayerRebindingToControl()
    {
        playerControl.PlayerRebinding.Disable();
        playerControl.GamePlay.Enable();
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

    #endregion
}
