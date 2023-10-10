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

        [SerializeField]
        private Transform innerCameraTransform;

        private Transform mCameraTransform;
        private InputAssets.PlayerActions mPlayerAction;
        float currentRotationX = 0;
        float currentRotationY = 0;

        Vector2 movementVector;

        private bool is_ready;

        public void Setup(InputAssets.PlayerActions playerAction) {
            mPlayerAction = playerAction;

            mCameraTransform = GetComponent<Transform>();

            mPlayerAction.Move.performed += OnPlayerMove;
            mPlayerAction.Move.canceled += OnPlayerMove;
            mPlayerAction.Look.performed += OnPlayerLook;

            is_ready = true;
        }

        public void Teleport(Vector3 position, Quaternion rotation) {
            mCameraTransform.position = PositionTransform(position);

            var coming_euler = rotation.eulerAngles;
            var ignore_z_euler = Quaternion.Euler(coming_euler.x, coming_euler.y, 0);

            mCameraTransform.rotation = ignore_z_euler;
        }

        private void Update() {
            PerformMovement();
        }

        private void PerformMovement() { 
            if (!is_ready) return;

            Vector3 position = mCameraTransform.position;
            position += ((mCameraTransform.forward * movementVector.y) + (mCameraTransform.right * movementVector.x)) * translation_sensitivity * Time.deltaTime;
            mCameraTransform.position = position;
        }

        private Vector3 PositionTransform(Vector3 teleport_position) {
            Vector3 diff = teleport_position - this.innerCameraTransform.position;

            return this.innerCameraTransform.position + diff;
        }

        #region Action Event
        private void OnPlayerMove(InputAction.CallbackContext callbackContext) {
            movementVector = callbackContext.ReadValue<Vector2>();
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
