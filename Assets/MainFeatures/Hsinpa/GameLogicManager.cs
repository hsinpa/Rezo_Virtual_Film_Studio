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

        private InputAssets mIputActions;
        private InterestPointManager mInterestPointManager;

        private int uiRefCount = 0;

        private int maskCanvasIndex = 0;

        void Start()
        {
            maskCanvasIndex = maskCanvas.Length;
            mIputActions = new InputAssets();
            mIputActions.Player.Enable();
            outerCameraManager.Setup(mIputActions.Player);

            mIputActions.UI.Enable();
            mIputActions.UI.Mask_UI.started += OnUIMaskEvent;
            mIputActions.UI.Projection_UI.started += OnUIProjectionEvent;
            mIputActions.UI.Scene_UI.started += OnUISceneEvent;

            RegisterInput();
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


            if (maskCanvasIndex == maskCanvas.Length) {
                PlayerInputEnable(true);
                Debug.Log("maskCanvasIndex " + maskCanvasIndex);

                return;
            }

            maskCanvas[maskCanvasIndex].gameObject.SetActive(true);

            if (uiRefCount == 0)
                PlayerInputEnable(false);
        }

        private void OnUIProjectionEvent(InputAction.CallbackContext callbackContext)
        {

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

        private void OnDestroy()
        {
            rootSceneCtrl.SceneActiveEvent -= OnSceneChange;
        }
    }
}