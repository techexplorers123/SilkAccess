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
                return $"Button: {GetLabel(menuButton.gameObject)}. {GetAvailability(menuButton)}";
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
                return $"Toggle: {GetLabel(toggle.gameObject)}. {(toggle.isOn ? "On" : "Off")}. {availability}";
            }

            Slider slider = selectable.GetComponent<Slider>();
            if (slider != null)
            {
                return $"Slider: {GetLabel(slider.gameObject)}. {slider.value:0.##}. {availability}";
            }

            Dropdown dropdown = selectable.GetComponent<Dropdown>();
            if (dropdown != null)
            {
                return $"Dropdown: {GetLabel(dropdown.gameObject)}. {GetSelectedOption(dropdown.options, dropdown.value)}. {availability}";
            }

            TMP_Dropdown tmpDropdown = selectable.GetComponent<TMP_Dropdown>();
            if (tmpDropdown != null)
            {
                return $"Dropdown: {GetLabel(tmpDropdown.gameObject)}. {GetSelectedOption(tmpDropdown.options, tmpDropdown.value)}. {availability}";
            }

            InputField inputField = selectable.GetComponent<InputField>();
            if (inputField != null)
            {
                return $"Input field: {GetInputLabel(inputField.placeholder, inputField.gameObject)}. {inputField.text}. {availability}";
            }

            TMP_InputField tmpInputField = selectable.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                return $"Input field: {GetInputLabel(tmpInputField.placeholder, tmpInputField.gameObject)}. {tmpInputField.text}. {availability}";
            }

            Button button = selectable.GetComponent<Button>();
            if (button != null)
            {
                return $"Button: {GetLabel(button.gameObject)}. {availability}";
            }

            return $"Control: {GetLabel(selectable.gameObject)}. {availability}";
        }

        private static string DescribeMenuOption(MenuOptionHorizontal menuOption)
        {
            string label = GetObjectName(menuOption.gameObject);
            string value = GetText(menuOption.optionText?.gameObject) ?? "No option selected";
            return $"Option: {label}. {value}. {GetAvailability(menuOption)}";
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
