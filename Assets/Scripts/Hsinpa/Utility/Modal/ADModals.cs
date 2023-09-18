using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using Hsinpa.Utility;
using UnityEngine.AddressableAssets;
using System.Threading;

namespace Hsinpa.View
{
    /// <summary>
    /// To Load a ADModal GameObject please first set up in ADModalAsset
    /// </summary>
    public class ADModals : MonoBehaviour
    {
        [SerializeField]
        Image background;
        Sprite defaultBG;
        Color defaultBGColor;

        ExternalAddressableTool externalAddrTool;

        Dictionary<string, Modal> modals = new Dictionary<string, Modal>();

        private static ADModals _instance;

        public static ADModals instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<ADModals>();

                    if (_instance != null)
                        _instance.SetUp();
                }
                return _instance;
            }
        }

        // <string, Modal>
        private OrderedDictionary openModals = new OrderedDictionary();
        private Modal currentModals;

        public int modalCount => openModals.Count;

        CancellationTokenSource cancelSrc;

        private void Awake()
        {

            if (_instance == null)
            {
                _instance = this;
                DontDestroyOnLoad(this.transform.parent.gameObject);
            }
            else
                Destroy(this.transform.parent.gameObject);

            externalAddrTool = new ExternalAddressableTool();
            defaultBG = background.sprite;
            defaultBGColor = background.color;
        }

        public void SetUp()
        {
            modals.Clear();
            externalAddrTool.Dispose();
            //externalAddr.Demo();
        }

        public Modal GetModal(string id)
        {
            Modal modal = null;
            modals.TryGetValue(id, out modal);

            if (modal == null)
            {
                Modal addressableAsset = externalAddrTool.LoadGameObject<Modal>(id);

                var targetModal = Utility.UtilityFunc.CreateObjectToParent<Modal>(transform, addressableAsset.gameObject);

                if (targetModal != null)
                {
                    modals.Add(id, targetModal);
                    return targetModal;
                }
                else
                {
                    Debug.LogError("Modals, No such modal : " + id);
                    return null;
                }
            }

            return modal;
        }

        public Modal OpenModal(string id, bool anim = false)
        {
            var targetModal = GetModal(id);

            if (targetModal == null)
            {
                return null;
            }

            if (anim)
                targetModal.ShowTransition(true);
            else
                targetModal.Show(true);
            ;
            if (!openModals.Contains(id))
            {
                openModals.Add(id, targetModal);
            }

            currentModals = targetModal;

            background.enabled = (currentModals.EnableModalBG);

            cancelSrc?.Cancel();
            cancelSrc = new CancellationTokenSource();
            UpdateBG(cancelSrc.Token);


            return targetModal;
        }

        UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle<Sprite> bgHandle;
        private async void UpdateBG(CancellationToken token = default)
        {
            if (currentModals.EnableModalBG && !string.IsNullOrEmpty(currentModals.bgAddressableKey))
            {
                bgHandle = Addressables.LoadAssetAsync<Sprite>(currentModals.bgAddressableKey);
                await bgHandle.Task;
                token.ThrowIfCancellationRequested();
                background.sprite = bgHandle.Result;
                background.color = Color.white;
            }
            else if (background.sprite != defaultBG)
            {
                background.sprite = defaultBG;
                background.color = defaultBGColor;
            }
        }

        public void CloseModal(string id)
        {
            if (openModals.Count > 0)
            {
                if (openModals.Contains(id))
                {
                    Modal modal = openModals[id] as Modal;
                    modal.Show(false);
                    openModals.Remove(id);
                }
            }

            currentModals = (openModals.Count > 0) ? openModals[openModals.Count - 1] as Modal : null;
            background.enabled = (currentModals != null && currentModals.EnableModalBG);
            if (!background.enabled && bgHandle.IsValid()) Addressables.Release(bgHandle);
        }

        public void CloseAll()
        {
            if (modals == null) return;

            foreach (var modal in modals)
            {
                if (modal.Value.isShow)
                    modal.Value.Show(false);
            }

            background.enabled = false;
            if(bgHandle.IsValid()) Addressables.Release(bgHandle);
            openModals.Clear();
        }

        public void Dispose()
        {
            CloseAll();
            externalAddrTool.Dispose();
        }

        private void OnDestroy()
        {
            Dispose();
        }
    }
}