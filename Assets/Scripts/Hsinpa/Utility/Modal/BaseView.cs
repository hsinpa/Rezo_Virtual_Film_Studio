using UnityEngine;

namespace Hsinpa.View
{
    [RequireComponent(typeof(CanvasGroup))]
    public class BaseView : MonoBehaviour
    {
        [SerializeField]
        protected bool enableModalBG;
        public bool EnableModalBG => this.enableModalBG;
        public string bgAddressableKey;

        [SerializeField]
        protected CanvasGroup canvasGroup;

        public virtual void Show(bool isShow) {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = (isShow) ? 1 : 0;
                canvasGroup.blocksRaycasts = isShow;
                canvasGroup.interactable = isShow;
            }

            gameObject.SetActive(isShow);
        }

        public bool isShow => gameObject.activeInHierarchy && canvasGroup.alpha > 0.9f;
    }
}
