using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInputController inputController;

    [SerializeField] private InventoryController inventoryController1;
    [SerializeField] private InventoryController inventoryController2;

    #region InputActionReferences

    

    #endregion

    #region Life-cycle

    private void OnEnable()
    {
        inputController.onPause += Pause;
        inputController.onMoveUp1 += MoveUp1;
        inputController.onMoveDown1 += MoveDown1;
        inputController.onMoveLeft1 += MoveLeft1;
        inputController.onMoveRight1 += MoveRight1;
        inputController.onRotate1 += Rotate1;
        inputController.onConfirm1 += Confirm1;
        inputController.onMoveUp2 += MoveUp2;
        inputController.onMoveDown2 += MoveDown2;
        inputController.onMoveLeft2 += MoveLeft2;
        inputController.onMoveRight2 += MoveRight2;
        inputController.onRotate2 += Rotate2;
        inputController.onConfirm2 += Confirm2;
    }

    private void OnDisable()
    {
        inputController.onPause -= Pause;
        inputController.onMoveUp1 -= MoveUp1;
        inputController.onMoveDown1 -= MoveDown1;
        inputController.onMoveLeft1 -= MoveLeft1;
        inputController.onMoveRight1 -= MoveRight1;
        inputController.onRotate1 -= Rotate1;
        inputController.onConfirm1 -= Confirm1;
        inputController.onMoveUp2 -= MoveUp2;
        inputController.onMoveDown2 -= MoveDown2;
        inputController.onMoveLeft2 -= MoveLeft2;
        inputController.onMoveRight2 -= MoveRight2;
        inputController.onRotate2 -= Rotate2;
        inputController.onConfirm2 -= Confirm2;
    }

    // Start is called before the first frame update
    void Start()
    {
        // inputController.LoadRebinding();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #endregion

    #region Functions

    void MoveUp1()
    {
        
    }

    void MoveDown1()
    {
        
    }

    void MoveLeft1()
    {
        
    }

    void MoveRight1()
    {
        
    }

    void Rotate1()
    {
        
    }

    void Confirm1()
    {
        
    }
    
    void MoveUp2()
    {
        
    }

    void MoveDown2()
    {
        
    }

    void MoveLeft2()
    {
        
    }

    void MoveRight2()
    {
        
    }

    void Rotate2()
    {
        
    }

    void Confirm2()
    {
        
    }
    
    void Pause()
    {
        // 
    }

    #endregion
}
