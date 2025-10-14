using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInputController inputController;

    #region InputActionReferences

    

    #endregion
    
    private void OnEnable()
    {
        inputController.onPause += Pause;
    }

    private void OnDisable()
    {
        inputController.onPause -= Pause;
    }

    // Start is called before the first frame update
    void Start()
    {
        inputController.LoadRebinding();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Pause()
    {
        // 
    }
}
