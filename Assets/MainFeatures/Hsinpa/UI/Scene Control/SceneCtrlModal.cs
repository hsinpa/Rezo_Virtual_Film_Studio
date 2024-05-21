using Hsinpa.View;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using Hsinpa.Addressable;
using System.IO;
using Hsinpa.Utility;
using UnityEngine.UI;
using System.Threading.Tasks;
using Hsinpa.Static;
using UnityEngine.Rendering; // .Universal for urp
using UnityEngine.Rendering.Universal;
using UnityEngine.EventSystems;
using Funly.SkyStudio;
using Hsinpa.Event;
using Hsinpa.GameScene;
using System;

namespace Hsinpa.UI
{
    public class SceneCtrlModal : Modal, IPointerClickHandler
    {
        [Header("Scene Selector")]
        [SerializeField]
        private TextMeshProUGUI scene_selector_prefab;

        [SerializeField]
        private Transform scene_selector_container;

        [Header("Other configuration")]
        [SerializeField]
        private Toggle day_night_toggle;

        [SerializeField]
        private Slider fog_intensity_slider;

        [SerializeField]
        private Slider contrast_slider;

        [SerializeField]
        private Slider saturate_slider;

        [SerializeField]
        private Button color_adjustment;

        [SerializeField]
        private FlexibleColorPicker FColorPicker;

        [Header("Loading")]
        [SerializeField]
        private TextMeshProUGUI loading_text;

        [SerializeField]
        private RectTransform loading_background;

        [Header("Bottom")]
        [SerializeField]
        private Button loadBtn;

        private AddressableSceneManagement addressableSceneManagement;
        private SimpleJSON.JSONNode sceneListJson;
        private string cache_key;
        private List<TextMeshProUGUI> scene_selectors = new List<TextMeshProUGUI>();

        private List<string> scene_list = new List<string>();
        public List<string> Scene_List => scene_list;

        private Volume _volume;
        private TimeOfDayController _timeOfDayController;
        private LightSwitchListener[] _lightSwitchListeners;
        private bool _load_scene_flag = false;

        private float night_value = 0.8f, day_value = 0.45f;

        protected override void Start() {
            addressableSceneManagement = new AddressableSceneManagement();
            string add_path = Path.Combine(Application.streamingAssetsPath, Hsinpa.Static.StaticFlag.Path.AddressableList);

            if (File.Exists(add_path)) {
                string json_raw = File.ReadAllText(add_path);
                sceneListJson = SimpleJSON.JSON.Parse(json_raw);
            }

            UtilityFunc.SetSimpleBtnEvent(loadBtn, OnSceneLoadClick);
            fog_intensity_slider.onValueChanged.AddListener(OnFogSliderChange);
            saturate_slider.onValueChanged.AddListener(OnSaturateToggleChange);
            contrast_slider.onValueChanged.AddListener(OnContrastToggleChange);

            day_night_toggle.onValueChanged.AddListener(OnDayNightToggleChange);
            FColorPicker.onColorChange.AddListener(OnColorToggleChange);
            color_adjustment.onClick.AddListener(() => {
                FColorPicker.gameObject.SetActive(!FColorPicker.gameObject.activeSelf);
            });
        }

        public override void Show(bool isShow)
        {
            base.Show(isShow);

            Setup();
        }

        private void Update()
        {
            loading_background.gameObject.SetActive(_load_scene_flag);

            if (_load_scene_flag && addressableSceneManagement != null)
            {
                loading_text.text = "Loading progress: " + System.Math.Round(addressableSceneManagement.GetCompletePercentage(), 2);
            }
        }

        public void Setup() {
            SetSceneListUI();
            SetOtherConfiguration();
        }

        private void SetSceneListUI() {
            UtilityFunc.ClearChildObject(scene_selector_container);
            scene_list.Clear();
            scene_selectors.Clear();

            var scene_array = sceneListJson["scene"].AsArray;
            int i = 1;

            foreach (var scene_j in scene_array) {
                string add_key = scene_j.ToString().Replace("\"", "");

                string display_name = add_key.Replace("scene@", "");

                var ui_text = UtilityFunc.CreateObjectToParent<TextMeshProUGUI>(scene_selector_container, scene_selector_prefab.gameObject);
                    ui_text.text = $"{i}. {display_name}";
                var ui_button = ui_text.GetComponent<Button>();

                ui_text.color = (add_key == cache_key) ? Color.magenta : Color.white;

                scene_selectors.Add(ui_text);
                scene_list.Add(add_key);

                UtilityFunc.SetSimpleBtnEvent(ui_button, () => {

                    scene_selectors.ForEach(x => {
                        x.color = Color.white;
                    });
                    ui_text.color = Color.magenta;

                    cache_key = add_key;
                });

                i++;
            }
        }

        private void SetOtherConfiguration() {
            // Fog intensity
            float fog_intensity_normalize = PlayerPrefs.GetFloat(StaticFlag.PlayerPref.FogIntensityPref, 0);
            fog_intensity_slider.SetValueWithoutNotify(fog_intensity_normalize);
            float intensity = (Mathf.Lerp(StaticFlag.Config.FOG_INTENSITY_MIN, StaticFlag.Config.FOG_INTENSITY_MAX, fog_intensity_normalize));
            RenderSettings.fogDensity = intensity;

            //Fog color
            float color_r = PlayerPrefs.GetFloat(StaticFlag.PlayerPref.Color_RTogglePref, 1);
            float color_g = PlayerPrefs.GetFloat(StaticFlag.PlayerPref.Color_GTogglePref, 1);
            float color_b = PlayerPrefs.GetFloat(StaticFlag.PlayerPref.Color_BTogglePref, 1);
            Color fogColor = new Color(color_r, color_g, color_b);
            RenderSettings.fogColor = fogColor;

            //colorAdjust.colorFilter.value = new Color(color_r, color_g, color_b);
            //color_adjustment.targetGraphic.color = colorAdjust.colorFilter.value;
            FColorPicker.SetColor(fogColor);
            FColorPicker.ChangeMode(FlexibleColorPicker.MainPickingMode.HS);

            // Post processing Contrast & Saturate
            _volume = GameObject.FindFirstObjectByType<Volume>();
            if (_volume != null && _volume.profile.TryGet(out ColorAdjustments colorAdjust)) {

                contrast_slider.interactable = true;
                saturate_slider.interactable = true;

                //Contrast
                float contrastValue = PlayerPrefs.GetFloat(StaticFlag.PlayerPref.ContrastTogglePref, 0.5f);
                contrast_slider.SetValueWithoutNotify(contrastValue);
                colorAdjust.contrast.value = RescaleVariable(contrastValue, scale: 100);

                //Saturation
                float saturateValue = PlayerPrefs.GetFloat(StaticFlag.PlayerPref.SaturateTogglePref, 0.5f);
                saturate_slider.SetValueWithoutNotify(saturateValue);
                colorAdjust.saturation.value = RescaleVariable(saturateValue, scale: 100);
            } else
            {
                contrast_slider.interactable = false;
                saturate_slider.interactable = false;
            }

            // Day Night Toggle
            _timeOfDayController = GameObject.FindFirstObjectByType<TimeOfDayController>();
            _lightSwitchListeners = GameObject.FindObjectsOfType<LightSwitchListener>(includeInactive: true);
            if (_timeOfDayController != null) {
                if (!day_night_toggle.gameObject.activeInHierarchy)
                    day_night_toggle.gameObject.SetActive(true);

                int day_night_switch = PlayerPrefs.GetInt(StaticFlag.PlayerPref.DayNightTogglePref, 1);
                _timeOfDayController.skyTime = Mathf.Lerp(night_value, day_value, day_night_switch);
                day_night_toggle.SetIsOnWithoutNotify(day_night_switch == 1);

                OnDayNightToggleChange(day_night_switch == 1);
            } else
            {
                day_night_toggle.gameObject.SetActive(false);
            }
        }

        public async Task<bool> LoadScene(string addressable_key) {
            cache_key = addressable_key;

            Debug.Log("addressable_key " + addressable_key);

            bool load_sucess = await addressableSceneManagement.LoadAddScene(addressable_key);

            PlayerPrefs.SetString(StaticFlag.PlayerPref.LoadScenePref, addressable_key);
            PlayerPrefs.Save();

            //Skybox and Fog
            SetOtherConfiguration();

            return load_sucess;
        }

        #region UI Event
        private async void OnSceneLoadClick() {
            if (string.IsNullOrEmpty(cache_key)) return;

            this._load_scene_flag = true;
            bool result =  await LoadScene(cache_key);
            this._load_scene_flag = false;
        }

        private void OnFogSliderChange(float p_value) {
            float intensity = (Mathf.Lerp(StaticFlag.Config.FOG_INTENSITY_MIN, StaticFlag.Config.FOG_INTENSITY_MAX, p_value));
            RenderSettings.fogDensity = intensity;

            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.FogIntensityPref, p_value);
            PlayerPrefs.Save();
        }

        private void OnDayNightToggleChange(bool p_value) {
            int day_night_switch = p_value ? 1 : 0;

            PlayerPrefs.SetInt(StaticFlag.PlayerPref.DayNightTogglePref, day_night_switch);

            if (_timeOfDayController != null) {
                _timeOfDayController.skyTime = Mathf.Lerp(night_value, day_value, day_night_switch);

                SimpleEventSystem.Send(MessageEventFlag.HsinpaEvent.UIEVent.DayLightSwitchEvent, p_value);
            }

            if (_lightSwitchListeners != null) { 
                foreach (var lightListener in _lightSwitchListeners) {
                    lightListener.SetVisibility(p_value);
                }
            }

            PlayerPrefs.Save();
        }

        private void OnContrastToggleChange(float p_value) {
            if (_volume != null && _volume.profile.TryGet(out ColorAdjustments colorAdjust)) {
                colorAdjust.contrast.value = RescaleVariable(p_value, scale: 100);
            }

            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.ContrastTogglePref, p_value);
            PlayerPrefs.Save();
        }

        private void OnSaturateToggleChange(float p_value) {
            if (_volume != null && _volume.profile.TryGet(out ColorAdjustments colorAdjust)) {
                colorAdjust.saturation.value = RescaleVariable(p_value, scale: 100); ;
            }

            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.SaturateTogglePref, p_value);
            PlayerPrefs.Save();
        }

        private void OnColorToggleChange(Color color) {
            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.Color_RTogglePref, color.r);
            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.Color_GTogglePref, color.g);
            PlayerPrefs.SetFloat(StaticFlag.PlayerPref.Color_BTogglePref, color.b);

            //if (_volume != null && _volume.profile.TryGet(out ColorAdjustments colorAdjust)) {
            //    colorAdjust.colorFilter.value = color;
            //    color_adjustment.targetGraphic.color = color;
            //}
            RenderSettings.fogColor = color;

            PlayerPrefs.Save();
        }

        private float RescaleVariable(float value, float scale) {
            return ((value * 2) - 1) * scale;
        }

        public void OnPointerClick(PointerEventData eventData) {
            FColorPicker.gameObject.SetActive(false);
        }
        #endregion
    }
}
