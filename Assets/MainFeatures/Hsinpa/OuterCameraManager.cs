using HTC.UnityPlugin.Vive;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using static Hsinpa.Type.GeneralTypeStruct;

namespace Hsinpa.Character {
    public class OuterCameraManager : MonoBehaviour
    {
        [SerializeField, Range(1f, 150)]
        private float translation_sensitivity;

        [SerializeField, Range(1f, 50)]
        private float inner_camera_translation_sensitivity;

        [SerializeField, Range(1f, 20)]
        private float rotation_sensitivity;

        [SerializeField]
        private Transform innerCameraTransform;

        [SerializeField]
        private VivePoseTracker vivePoseTracker;


        private float fov_min_range_config = 30;
        private float fov_max_range_config = 150;

        private Transform mCameraTransform;
        private Camera[] mCamera;

        private CameraTransform[] mCameraTransformConfigs;

        private InputAssets.PlayerActions mPlayerAction;
        float currentRotationX = 0;
        float currentRotationY = 0;

        Vector2 movementVector;

        private bool is_ready;
        private float record_time;
        private bool can_manual_look_flag = false;
        private Vector3 outer_cache_position = new Vector3(0, 0, 0);

        public void Setup(InputAssets.PlayerActions playerAction) {
            mPlayerAction = playerAction;

            mCameraTransform = GetComponent<Transform>();
            mCamera = mCameraTransform.GetComponentsInChildren<Camera>();
            mCameraTransformConfigs = new CameraTransform[mCamera.Length];
            
            for (int i = 0; i < mCamera.Length; i++) {
                mCameraTransformConfigs[i] = new CameraTransform { position = mCamera[i].transform.localPosition,
                                                                    rotation = mCamera[i].transform.localRotation.eulerAngles };
            }

            mPlayerAction.Move.performed += OnPlayerMove;
            mPlayerAction.Move.canceled += OnPlayerMove;
            mPlayerAction.Look.performed += OnPlayerLook;
            mPlayerAction.LookTrigger.performed += OnLookTrigger;
            mPlayerAction.LookTrigger.canceled += OnLookCancel;

            mPlayerAction.Zoom.performed += OnCameraZoom;

            is_ready = true;
        }

        public void Teleport(Vector3 position, Quaternion rotation) {
            mCameraTransform.position = PositionTransform(position);

            var coming_euler = rotation.eulerAngles;
            var ignore_z_euler = Quaternion.Euler(coming_euler.x, coming_euler.y, 0);

            outer_cache_position = mCameraTransform.position;
            mCameraTransform.rotation = ignore_z_euler;
        }

        private void Update() {
            PerformMovement();
            //PerformInnerMovement();
        }

        public void UpdateCameraConfig(Type.GeneralTypeStruct.ConfigFileStruct globalConfigFileStruct) {
            if (mCamera.Length > 0) {
                mCamera[0].transform.localPosition = new Vector3 (
                    mCameraTransformConfigs[0].position.x + globalConfigFileStruct.Project_1_Pos_X,
                    mCameraTransformConfigs[0].position.y + globalConfigFileStruct.Project_1_Pos_Y,
                    mCameraTransformConfigs[0].position.z + globalConfigFileStruct.Project_1_Pos_Z
                );

                mCamera[0].transform.localRotation = Quaternion.Euler( new Vector3(
                    mCameraTransformConfigs[0].rotation.x + globalConfigFileStruct.Project_1_Rot_X,
                    mCameraTransformConfigs[0].rotation.y + globalConfigFileStruct.Project_1_Rot_Y,
                    mCameraTransformConfigs[0].rotation.z + globalConfigFileStruct.Project_1_Rot_Z
                    ));
            }

            if (mCamera.Length > 1) {
                mCamera[1].transform.localPosition = new Vector3(
                    mCameraTransformConfigs[1].position.x + globalConfigFileStruct.Project_2_Pos_X,
                    mCameraTransformConfigs[1].position.y + globalConfigFileStruct.Project_2_Pos_Y,
                    mCameraTransformConfigs[1].position.z + globalConfigFileStruct.Project_2_Pos_Z
                );

                mCamera[1].transform.localRotation = Quaternion.Euler(new Vector3(
                    mCameraTransformConfigs[1].rotation.x + globalConfigFileStruct.Project_2_Rot_X,
                    mCameraTransformConfigs[1].rotation.y + globalConfigFileStruct.Project_2_Rot_Y,
                    mCameraTransformConfigs[1].rotation.z + globalConfigFileStruct.Project_2_Rot_Z
                    ));
            }
        }

        private void PerformMovement() { 
            if (!is_ready) return;

            if (movementVector.magnitude < 0.01f) return;

            Vector3 position = mCameraTransform.position;
            position += ((innerCameraTransform.forward * movementVector.y) + (innerCameraTransform.right * movementVector.x)) * translation_sensitivity * Time.deltaTime;
            mCameraTransform.position = position;
            outer_cache_position = mCameraTransform.position;
        }

        private void PerformInnerMovement() {
            if (!is_ready) return;

            var local_position = innerCameraTransform.localPosition * inner_camera_translation_sensitivity;
                local_position.y = innerCameraTransform.localPosition.y;

            var offset = Matrix4x4.Rotate(innerCameraTransform.rotation).MultiplyPoint3x4(local_position);

            mCameraTransform.position = outer_cache_position + (local_position);
        }

        private Vector3 PositionTransform(Vector3 teleport_position) {
            Vector3 diff = teleport_position - this.innerCameraTransform.position;

            return this.innerCameraTransform.position + diff;
        }

        #region Action Event
        private void OnCameraZoom(InputAction.CallbackContext callbackContext) {
            Vector2 camera_zoom_delta = callbackContext.ReadValue<Vector2>();

            foreach (var camera in mCamera) {
                if (!camera.orthographic)
                    camera.fieldOfView = Mathf.Clamp(camera.fieldOfView + (camera_zoom_delta.y * Time.deltaTime), fov_min_range_config, fov_max_range_config);
            }
        }

        private void OnPlayerMove(InputAction.CallbackContext callbackContext) {
            movementVector = callbackContext.ReadValue<Vector2>();
        }

        private void OnPlayerLook(InputAction.CallbackContext callbackContext) {
            if (!can_manual_look_flag) return;
            Vector2 vector = callbackContext.ReadValue<Vector2>();
            int maxYAngle = 350;

            currentRotationX += vector.x * rotation_sensitivity;
            currentRotationY += vector.y * rotation_sensitivity;

            currentRotationX = Mathf.Repeat(currentRotationX, 360);
            currentRotationY = Mathf.Clamp(currentRotationY, -maxYAngle, maxYAngle);
            mCameraTransform.rotation = Quaternion.Euler(currentRotationY, currentRotationX, 0);
        }

        private void OnLookTrigger(InputAction.CallbackContext callbackContext) {
            can_manual_look_flag = true;
        }

        private void OnLookCancel(InputAction.CallbackContext callbackContext) {
            can_manual_look_flag = false;
        }

        #endregion

    }
}
