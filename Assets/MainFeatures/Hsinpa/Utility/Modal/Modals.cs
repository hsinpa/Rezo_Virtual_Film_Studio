using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Hsinpa.View
{

    public class Modals : MonoBehaviour
    {
        [SerializeField]
        Image background;

        BaseView[] modals;

        private static Modals _instance;

        public static Modals instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<Modals>();
                    _instance.SetUp();
                }
                return _instance;
            }
        }
        
        private List<Modal> openModals = new List<Modal>();
        private Modal currentModals;

        public int modalCount => openModals.Count; 

        public void SetUp()
        {
            modals = GetComponentsInChildren<Modal>();
        }

        public T GetModal<T>() where T : Modal
        {
            return modals.First(x=> typeof(T) == x.GetType()) as T;
        }

        public T OpenModal<T>(bool anim = false) where T : Modal
        {
            if (modals == null) return null;

            Modal targetModal = null;

            foreach (Modal modal in modals)
            {
                if (typeof(T) == modal.GetType())
                {
                    targetModal = modal;

                    if (anim)
                        targetModal.ShowTransition(true);
                    else
                        targetModal.Show(true);
                }
                //else
                //{
                //    modal.Show(false);
                //}
            }

            int modalIndex = openModals.FindIndex(x => x.GetType() == typeof(T));

            if (modalIndex < 0) {
                openModals.Add(targetModal as T);
            }

            currentModals = targetModal as T;

            background.enabled = (currentModals.EnableModalBG);

            return targetModal as T;
        }

        public void Close() {
            if (currentModals != null)
                currentModals.Show(false);

            if (openModals.Count > 0) {
                openModals.RemoveAt(openModals.Count - 1);
            }

            currentModals = (openModals.Count > 0) ? openModals[openModals.Count - 1] : null;
            background.enabled = (currentModals != null && currentModals.EnableModalBG);
        }

        public void Close<T>() where T : Modal
        {
            var closeIndex = openModals.FindIndex(x => x.GetType() == typeof(T));

            if (closeIndex >= 0) {
                openModals[closeIndex].Show(false);
                openModals.RemoveAt(closeIndex);
            }

            currentModals = (openModals.Count > 0) ? openModals[openModals.Count - 1] : null;
            background.enabled = (currentModals != null && currentModals.EnableModalBG);
        }



        public void CloseAll()
        {
            if (modals == null) return;

            foreach (var modal in modals)
            {

                if (modal.isShow)
                    modal.Show(false);
            }

            background.enabled = false;
            openModals.Clear();
        }
    }
}