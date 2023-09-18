using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using DG.Tweening;

namespace Hsinpa.View
{
    public class Modal : BaseView
    {
        [SerializeField]
        protected Button CloseBtn;

        /// <summary>
        /// Don't call this function, except from Modal.cs, Unless you are author
        /// </summary>
        /// <param name="isShow"></param>
        public override void Show(bool isShow)
        {
            base.Show(isShow);
        }

        public void ShowTransition(bool isShow)
        {
            if (canvasGroup == null) return;

            canvasGroup.blocksRaycasts = isShow;
            canvasGroup.interactable = isShow;

            int alpha = (isShow) ? 1 : 0;
            //canvasGroup.DOFade(alpha, 0.2f);
            //.onComplete = () => {
            //    gameObject.SetActive(isShow);
            //};
            gameObject.SetActive(isShow);
        }

        protected virtual void Start()
        {
            SetCloseBtnEvent();
        }

        private void SetCloseBtnEvent()
        {
            if (CloseBtn != null)
            {
                CloseBtn.onClick.RemoveAllListeners();
                CloseBtn.onClick.AddListener(() => {
                    Modals.instance.Close();
                });
            }
        }
    }
}