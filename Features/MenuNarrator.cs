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
                    (selected.GetComponentInParent<InputField>() != null || selected.GetComponentInParent<TMP_InputField>() != null);
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
            MenuOptionHorizontal menuOption = element.GetComponentInParent<MenuOptionHorizontal>();
            if (menuOption != null)
            {
                return DescribeMenuOption(menuOption);
            }

            MenuButton menuButton = element.GetComponentInParent<MenuButton>();
            if (menuButton != null)
            {
                return $"{GetLabel(menuButton.gameObject)}";
            }

            Selectable selectable = element.GetComponentInParent<Selectable>();
            if (selectable == null)
            {
                return GetText(element) ?? element.name;
            }

            string availability = GetAvailability(selectable);

            Toggle toggle = selectable.GetComponent<Toggle>();
            if (toggle != null)
            {
                return $"{GetLabel(toggle.gameObject)} switch {(toggle.isOn ? "On" : "Off")}";
            }

            Slider slider = selectable.GetComponent<Slider>();
            if (slider != null)
            {
                return $"{GetLabel(slider.gameObject)} slider {slider.value:0.##}";
            }

            Dropdown dropdown = selectable.GetComponent<Dropdown>();
            if (dropdown != null)
            {
                return $"{GetLabel(dropdown.gameObject)}. Dropdown {GetSelectedOption(dropdown.options, dropdown.value)}";
            }

            TMP_Dropdown tmpDropdown = selectable.GetComponent<TMP_Dropdown>();
            if (tmpDropdown != null)
            {
                return $"Dropdown: {GetLabel(tmpDropdown.gameObject)}. {GetSelectedOption(tmpDropdown.options, tmpDropdown.value)}";
            }

            InputField inputField = selectable.GetComponent<InputField>();
            if (inputField != null)
            {
                return $"{GetInputLabel(inputField.placeholder, inputField.gameObject)}. editable, has value: {inputField.text}";
            }

            TMP_InputField tmpInputField = selectable.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                return $"{GetInputLabel(tmpInputField.placeholder, tmpInputField.gameObject)}. editable has value: {tmpInputField.text}";
            }

            Button button = selectable.GetComponent<Button>();
            if (button != null)
            {
                return $"{GetLabel(button.gameObject)}";
            }

            return $"{GetLabel(selectable.gameObject)}";
        }

        private static string DescribeMenuOption(MenuOptionHorizontal menuOption)
        {
            string label = GetObjectName(menuOption.gameObject);
            string value = GetText(menuOption.optionText?.gameObject) ?? "No option selected";
            return $"{label}. {value}";
        }

        private static string GetAvailability(Selectable selectable)
        {
            return selectable.IsInteractable() ? "Enabled" : "Disabled";
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
            return GetText(element) ?? GetObjectName(element);
        }

        private static string GetObjectName(GameObject element)
        {
            return element == null ? "Unnamed" : element.name.Replace('_', ' ');
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
