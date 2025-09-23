using System.Collections;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

namespace SilkAccess
{
    [BepInPlugin("org.tech.silkaccess", "silk access", "1.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;
        private bool playerReady = false;
        private GameObject lastSelected;

        private void Awake()
        {
            Logger = base.Logger;
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded!");
            StartCoroutine(WaitForPlayerData());
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += SceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= SceneChanged;
        }

        private void SceneChanged(Scene current, Scene next)
        {
            if (playerReady)
            {
                string sceneName = next.name;
                if (!string.IsNullOrEmpty(sceneName))
                {
                    Logger.LogInfo($"New game scene loaded: {sceneName}");
                    if (NVAccess.NVDA.NVDA.IsRunning)
                    {
                        NVAccess.NVDA.NVDA.Speak($"Zone: {sceneName}");
                    }
                }
            }
        }

        private IEnumerator WaitForPlayerData()
        {
            while (PlayerData.instance == null)
                yield return null;

            playerReady = true;
            Logger.LogInfo("PlayerData is ready!");
            Logger.LogInfo($"Current health = {PlayerData.instance.health}");

            // Announce the first game scene
            string sceneName = SceneManager.GetActiveScene().name;
            if (!string.IsNullOrEmpty(sceneName))
            {
                Logger.LogInfo($"First game scene loaded: {sceneName}");
                if (NVAccess.NVDA.NVDA.IsRunning)
                {
                    NVAccess.NVDA.NVDA.Speak($"Zone: {sceneName}");
                }
            }
        }

        private void Update()
        {
            // Menu narration should work everywhere
            GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
            if (currentSelected != lastSelected)
            {
                lastSelected = currentSelected;
                if (currentSelected != null)
                {
                    string textToSpeak = GetTextFromUI(currentSelected);
                    if (!string.IsNullOrEmpty(textToSpeak))
                    {
                        if (NVAccess.NVDA.NVDA.IsRunning)
                        {
                            NVAccess.NVDA.NVDA.Speak(textToSpeak);
                        }
                    }
                }
            }

            if (!playerReady) return;

            // In-game features
            if (Input.GetKeyDown(KeyCode.H))
            {
                string msg = $"Current health is {PlayerData.instance.health}";
                if (NVAccess.NVDA.NVDA.IsRunning)
                {
                    NVAccess.NVDA.NVDA.Speak(msg);
                }
            }
        }

        private string GetTextFromUI(GameObject uiElement)
        {
            if (uiElement == null) return null;

            // Handle Toggle switches
            Toggle toggle = uiElement.GetComponent<Toggle>();
            if (toggle != null)
            {
                string label = "";
                Text labelText = toggle.GetComponentInChildren<Text>();
                if (labelText != null)
                {
                    label = labelText.text;
                }
                else
                {
                    TextMeshProUGUI labelTmpro = toggle.GetComponentInChildren<TextMeshProUGUI>();
                    if (labelTmpro != null)
                    {
                        label = labelTmpro.text;
                    }
                }
                return $"Toggle: {label} {(toggle.isOn ? "On" : "Off")}";
            }

            // Handle Sliders
            Slider slider = uiElement.GetComponent<Slider>();
            if (slider != null)
            {
                string label = "";
                Text labelText = slider.GetComponentInChildren<Text>();
                if (labelText != null)
                {
                    label = labelText.text;
                }
                else
                {
                    TextMeshProUGUI labelTmpro = slider.GetComponentInChildren<TextMeshProUGUI>();
                    if (labelTmpro != null)
                    {
                        label = labelTmpro.text;
                    }
                }
                return $"Slider: {label} {slider.value}";
            }

            // Handle Dropdowns
            Dropdown dropdown = uiElement.GetComponent<Dropdown>();
            if (dropdown != null)
            {
                string label = "";
                Text labelText = dropdown.GetComponentInChildren<Text>();
                if (labelText != null)
                {
                    label = labelText.text;
                }
                else
                {
                    TextMeshProUGUI labelTmpro = dropdown.GetComponentInChildren<TextMeshProUGUI>();
                    if (labelTmpro != null)
                    {
                        label = labelTmpro.text;
                    }
                }
                return $"Dropdown: {label} {dropdown.options[dropdown.value].text}";
            }

            // Handle Input Fields
            InputField inputField = uiElement.GetComponent<InputField>();
            if (inputField != null)
            {
                string label = "";
                if (inputField.placeholder != null)
                {
                    label = inputField.placeholder.GetComponent<Text>()?.text;
                }

                if (string.IsNullOrEmpty(label))
                {
                    Text labelText = inputField.GetComponentInChildren<Text>();
                    if (labelText != null)
                    {
                        label = labelText.text;
                    }
                    else
                    {
                        TextMeshProUGUI labelTmpro = inputField.GetComponentInChildren<TextMeshProUGUI>();
                        if (labelTmpro != null)
                        {
                            label = labelTmpro.text;
                        }
                    }
                }
                return $"Input Field: {label} {inputField.text}";
            }


            // Fallback for other UI elements (like buttons)
            Button button = uiElement.GetComponent<Button>();
            if (button != null)
            {
                string label = "";
                Text labelText = button.GetComponentInChildren<Text>();
                if (labelText != null)
                {
                    label = labelText.text;
                }
                else
                {
                    TextMeshProUGUI labelTmpro = button.GetComponentInChildren<TextMeshProUGUI>();
                    if (labelTmpro != null)
                    {
                        label = labelTmpro.text;
                    }
                }
                return $"Button: {label}";
            }

            // Fallback for plain text elements
            Text textComponent = uiElement.GetComponent<Text>();
            if (textComponent != null)
            {
                return textComponent.text;
            }

            TextMeshProUGUI tmproComponent = uiElement.GetComponent<TextMeshProUGUI>();
            if (tmproComponent != null)
            {
                return tmproComponent.text;
            }

            return null;
        }
    }
}
