using Hsinpa.Character;
using Hsinpa.Ctrl;
using Hsinpa.Entity;
using Hsinpa.Static;
using Hsinpa.UI;
using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Hsinpa.Main
{
    public class GameLogicManager : MonoBehaviour
    {
        [SerializeField]
        private RootSceneCtrl rootSceneCtrl;

        [SerializeField]
        private OuterCameraManager outerCameraManager;

        [SerializeField]
        private Canvas[] maskCanvas;

        [SerializeField]
        private MeshRenderer[] calibration_camMeshes;

        private InputAssets mIputActions;
        private InterestPointManager mInterestPointManager;

        private int uiRefCount = 0;
        private int maskCanvasIndex = 0;
        private bool projector_ui_enable_flag = false;
        private float record_time;

        void Start()
        {
            maskCanvasIndex = maskCanvas.Length;
            mIputActions = new InputAssets();
            mIputActions.Player.Enable();
            outerCameraManager.Setup(mIputActions.Player);

            mIputActions.UI.Enable();
            mIputActions.UI.Mask_UI.started += OnUIMaskEvent;
            mIputActions.UI.Scene_UI.started += OnUISceneEvent;
            mIputActions.UI.Calibration_UI.started += OnCalibrationUIEvent;

            mIputActions.Plugin_Projector.Enable();
            mIputActions.Plugin_Projector.Enable_Canvas.started += OnUIProjectionEvent;

            RegisterInput();

            Type.GeneralTypeStruct.GlobalConfigFileStruct = Type.GeneralTypeStruct.SyncData();

            UpdateExternalConfig(Type.GeneralTypeStruct.GlobalConfigFileStruct);

            rootSceneCtrl.LoadDefaultScene();
        }

        #region Input Callback
        private void RegisterInput() {
            rootSceneCtrl.SceneActiveEvent += OnSceneChange;
            mIputActions.Player.Teleport.started += OnTeleportEvent;
        }

        private void OnSceneChange() {
            mInterestPointManager = GameObject.FindObjectOfType<InterestPointManager>();
            mInterestPointManager.SetUp();

            var target_point = mInterestPointManager.GetTrasformPoint(0);
            if (target_point != null) outerCameraManager.Teleport(target_point.position, target_point.rotation);

            rootSceneCtrl.ReloadSceneConfig();
        }

        private void OnTeleportEvent(InputAction.CallbackContext callbackContext) {
            if (int.TryParse(callbackContext.control.name, out int index ))
            {
                var target_point = mInterestPointManager.GetTrasformPoint(index);
                if (target_point != null) outerCameraManager.Teleport(target_point.position, target_point.rotation);
            }
        }

        private void OnUISceneEvent(InputAction.CallbackContext callbackContext)
        {
            var sceneUIModal = Modals.instance.GetModal<SceneCtrlModal>();

            if (sceneUIModal.isShow)
                Modals.instance.Close<SceneCtrlModal>();
            else
                Modals.instance.OpenModal<SceneCtrlModal>();

            PlayerInputEnable(!sceneUIModal.isShow);
        }

        private void OnUIMaskEvent(InputAction.CallbackContext callbackContext)
        {
            foreach (var mask_canvas in maskCanvas) mask_canvas.gameObject.SetActive(false);

            maskCanvasIndex = (maskCanvasIndex + 1) % (maskCanvas.Length + 1);

            foreach (var mask_canvas in maskCanvas) mask_canvas.transform.parent.GetChild(1).GetComponent<GraphicRaycaster>().enabled = maskCanvasIndex != maskCanvas.Length;

            if (maskCanvasIndex == maskCanvas.Length) {
                PlayerInputEnable(true);
                Debug.Log("maskCanvasIndex " + maskCanvasIndex);


                return;
            }

            maskCanvas[maskCanvasIndex].gameObject.SetActive(true);


            if (uiRefCount == 0)
                PlayerInputEnable(false);
        }

        private void OnCalibrationUIEvent(InputAction.CallbackContext callbackContext) {
            if (calibration_camMeshes == null) return;

            foreach (MeshRenderer mesh in calibration_camMeshes) {
                if (mesh.material == null) continue;

                string calibration_strength = "_CalibrateTexStr";
                float c_str = mesh.material.GetFloat(calibration_strength);
                mesh.material.SetFloat(calibration_strength, 1 - c_str);
            }
        }

        private void OnUIProjectionEvent(InputAction.CallbackContext callbackContext)
        {
            projector_ui_enable_flag = !projector_ui_enable_flag;

            if (projector_ui_enable_flag) {
                mIputActions.Player.Disable();
                mIputActions.UI.Disable();
            } else {
                mIputActions.Player.Enable();
                mIputActions.UI.Enable();
            }
        }

        private void PlayerInputEnable(bool is_enable)
        {
            if (is_enable)
            {
                uiRefCount = Mathf.Clamp(uiRefCount - 1, 0, uiRefCount);

                if (uiRefCount == 0)
                    mIputActions.Player.Enable();
            }
            else
            {
                uiRefCount++;
                mIputActions.Player.Disable();
            }
        }
        #endregion

        private void Update() {
            if (Type.GeneralTypeStruct.GlobalConfigFileStruct.SyncData && Time.time > record_time) {
                Type.GeneralTypeStruct.GlobalConfigFileStruct = Type.GeneralTypeStruct.SyncData();

                UpdateExternalConfig(Type.GeneralTypeStruct.GlobalConfigFileStruct);

                record_time = Time.time + 1;
            }
        }

        private void UpdateExternalConfig(Type.GeneralTypeStruct.ConfigFileStruct generalTypeStruct) {
            calibration_camMeshes[0].material.SetFloat("_Rotation", generalTypeStruct.Project_1_Cam_Rot);
            calibration_camMeshes[1].material.SetFloat("_Rotation", generalTypeStruct.Project_2_Cam_Rot);

            if (outerCameraManager != null)
                outerCameraManager.UpdateCameraConfig(generalTypeStruct);
        }

        private void OnDestroy()
        {
            rootSceneCtrl.SceneActiveEvent -= OnSceneChange;
        }
    }
}