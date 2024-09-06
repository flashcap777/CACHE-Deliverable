using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.XR.CoreUtils;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
{
    [SerializeField] Camera MainCamera;
    [SerializeField] XROrigin Origin;
    [SerializeField] CharacterController Controller;
    [SerializeField] InputAction LeftStick;
    [SerializeField] InputAction RightStick;
    [SerializeField] InputAction StartButton;
    [SerializeField] float MaxHeightIncrease;
    [SerializeField] float MinHeightDecrease;
    [SerializeField] float Speed;
    Vector2 LeftAxis;
    Vector2 RightAxis;
    Vector3 MoveDirection;
    float CurrentHeight;
    private void OnEnable()
    {
        LeftStick.Enable();
        RightStick.Enable();
        StartButton.Enable();
        StartButton.performed += RestartGame;
    }
    private void OnDisable()
    {
        LeftStick.Disable();
        RightStick.Disable();
        StartButton.Disable();
    }

    void RestartGame(InputAction.CallbackContext obj)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Debug Function used for Movement
    void Movement() 
    {
        LeftAxis = LeftStick.ReadValue<Vector2>();
        var input = new Vector2(LeftAxis.x, LeftAxis.y);
        input = Vector2.ClampMagnitude(input, 1);
        Vector3 camF = MainCamera.transform.forward;
        Vector3 camR = MainCamera.transform.right;
        camF.y = 0;
        camR.y = 0;
        camF = camF.normalized;
        camR = camR.normalized;
        MoveDirection = Vector3.zero + (camR * input.x + camF * input.y).normalized;
        Controller.Move(MoveDirection * Speed * Time.deltaTime);
        Debug.Log(LeftAxis);
    }
    void AffectHeight()
    {
        RightAxis = RightStick.ReadValue<Vector2>();
        CurrentHeight += RightAxis.y * Speed * Time.deltaTime;
        CurrentHeight = Mathf.Clamp(CurrentHeight, MinHeightDecrease, MaxHeightIncrease);
        Origin.CameraYOffset = CurrentHeight;
    }
    private void Start()
    {
        CurrentHeight = Origin.CameraYOffset;
        MinHeightDecrease += Origin.CameraYOffset;
        MaxHeightIncrease += Origin.CameraYOffset;
    }
    private void Update()
    {
        AffectHeight();
    }
}
