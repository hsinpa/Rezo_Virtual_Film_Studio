using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Hsinpa.Character {
    public class OuterCameraManager : MonoBehaviour
    {
        [SerializeField, Range(1f, 150)]
        private float translation_sensitivity;

        [SerializeField, Range(1f, 20)]
        private float rotation_sensitivity;

        private Transform mCameraTransform;
        private StarterAssets mStarterAssetsInputs;

        float currentRotationX = 0;
        float currentRotationY = 0;

        Vector2 movementVector;

        private void Start() {
            mCameraTransform = GetComponent<Transform>();
            mStarterAssetsInputs = new StarterAssets();
            mStarterAssetsInputs.Player.Enable();

            mStarterAssetsInputs.Player.Move.performed += OnPlayerMove;
            mStarterAssetsInputs.Player.Move.canceled += OnPlayerMove;

            mStarterAssetsInputs.Player.Look.performed += OnPlayerLook;
        }

        private void Update() {
            PerformMovement();
        }

        private void PerformMovement() {
            Vector3 position = mCameraTransform.position;
            position += ((mCameraTransform.forward * movementVector.y) + (mCameraTransform.right * movementVector.x)) * translation_sensitivity * Time.deltaTime;
            mCameraTransform.position = position;
        }

        #region Action Event
        private void OnPlayerMove(InputAction.CallbackContext callbackContext) {
            movementVector = callbackContext.ReadValue<Vector2>();

            Debug.Log(movementVector);
        }

        private void OnPlayerLook(InputAction.CallbackContext callbackContext) {
            Vector2 vector = callbackContext.ReadValue<Vector2>();
            int maxYAngle = 350;

            currentRotationX += vector.x * rotation_sensitivity;
            currentRotationY += vector.y * rotation_sensitivity;

            currentRotationX = Mathf.Repeat(currentRotationX, 360);
            currentRotationY = Mathf.Clamp(currentRotationY, -maxYAngle, maxYAngle);
            mCameraTransform.rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
        }
        #endregion

    }
}
