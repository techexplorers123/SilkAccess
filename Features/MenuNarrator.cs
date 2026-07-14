using SilkAccess.Services;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace SilkAccess.Features
{
    internal sealed class MenuNarrator
    {
        private GameObject lastSelected;
        private string lastAnnouncement;

        public bool IsTextInputFocused
        {
            get
            {
                GameObject selected = EventSystem.current?.currentSelectedGameObject;
                return selected != null &&
                    (selected.GetComponent<InputField>() != null || selected.GetComponent<TMP_InputField>() != null);
            }
        }

        public void Update()
        {
            GameObject selected = EventSystem.current?.currentSelectedGameObject;
            if (selected == null)
            {
                lastSelected = null;
                lastAnnouncement = null;
                return;
            }

            string announcement = Describe(selected);
            if (selected != lastSelected || announcement != lastAnnouncement)
            {
                lastSelected = selected;
                lastAnnouncement = announcement;

                if (!string.IsNullOrEmpty(announcement))
                {
                    NvdaSpeech.Speak(announcement);
                }
            }
        }

        private static string Describe(GameObject element)
        {
            Toggle toggle = element.GetComponent<Toggle>();
            if (toggle != null)
            {
                return $"Toggle: {GetLabel(toggle.gameObject)}. {(toggle.isOn ? "On" : "Off")}";
            }

            Slider slider = element.GetComponent<Slider>();
            if (slider != null)
            {
                return $"Slider: {GetLabel(slider.gameObject)}. {slider.value:0.##}";
            }

            Dropdown dropdown = element.GetComponent<Dropdown>();
            if (dropdown != null)
            {
                return $"Dropdown: {GetLabel(dropdown.gameObject)}. {GetSelectedOption(dropdown.options, dropdown.value)}";
            }

            TMP_Dropdown tmpDropdown = element.GetComponent<TMP_Dropdown>();
            if (tmpDropdown != null)
            {
                return $"Dropdown: {GetLabel(tmpDropdown.gameObject)}. {GetSelectedOption(tmpDropdown.options, tmpDropdown.value)}";
            }

            InputField inputField = element.GetComponent<InputField>();
            if (inputField != null)
            {
                return $"Input field: {GetInputLabel(inputField.placeholder, inputField.gameObject)}. {inputField.text}";
            }

            TMP_InputField tmpInputField = element.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                return $"Input field: {GetInputLabel(tmpInputField.placeholder, tmpInputField.gameObject)}. {tmpInputField.text}";
            }

            Button button = element.GetComponent<Button>();
            if (button != null)
            {
                return $"Button: {GetLabel(button.gameObject)}";
            }

            return GetText(element) ?? element.name;
        }

        private static string GetSelectedOption<T>(System.Collections.Generic.IList<T> options, int index) where T : Dropdown.OptionData
        {
            return index >= 0 && index < options.Count && !string.IsNullOrEmpty(options[index].text)
                ? options[index].text
                : "No option selected";
        }

        private static string GetSelectedOption(System.Collections.Generic.IList<TMP_Dropdown.OptionData> options, int index)
        {
            return index >= 0 && index < options.Count && !string.IsNullOrEmpty(options[index].text)
                ? options[index].text
                : "No option selected";
        }

        private static string GetInputLabel(Graphic placeholder, GameObject fallback)
        {
            return GetText(placeholder?.gameObject) ?? GetLabel(fallback);
        }

        private static string GetLabel(GameObject element)
        {
            return GetText(element) ?? element.name;
        }

        private static string GetText(GameObject element)
        {
            if (element == null)
            {
                return null;
            }

            Text text = element.GetComponentInChildren<Text>(true);
            if (text != null && !string.IsNullOrWhiteSpace(text.text))
            {
                return text.text;
            }

            TMP_Text tmpText = element.GetComponentInChildren<TMP_Text>(true);
            return tmpText != null && !string.IsNullOrWhiteSpace(tmpText.text) ? tmpText.text : null;
        }
    }
}
