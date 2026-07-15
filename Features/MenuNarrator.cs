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

            Toggle toggle = selectable.GetComponent<Toggle>();
            if (toggle != null)
            {
                return WithState($"{GetLabel(toggle.gameObject)} switch {(toggle.isOn ? "On" : "Off")}", selectable);
            }

            Slider slider = selectable.GetComponent<Slider>();
            if (slider != null)
            {
                return WithState($"{GetLabel(slider.gameObject)} slider {slider.value:0.##}", selectable);
            }

            Dropdown dropdown = selectable.GetComponent<Dropdown>();
            if (dropdown != null)
            {
                return WithState($"{GetLabel(dropdown.gameObject)}. Dropdown {GetSelectedOption(dropdown.options, dropdown.value)}", selectable);
            }

            TMP_Dropdown tmpDropdown = selectable.GetComponent<TMP_Dropdown>();
            if (tmpDropdown != null)
            {
                return WithState($"Dropdown: {GetLabel(tmpDropdown.gameObject)}. {GetSelectedOption(tmpDropdown.options, tmpDropdown.value)}", selectable);
            }

            InputField inputField = selectable.GetComponent<InputField>();
            if (inputField != null)
            {
                return WithState($"{GetInputLabel(inputField.placeholder, inputField.gameObject)}. editable, has value: {inputField.text}", selectable);
            }

            TMP_InputField tmpInputField = selectable.GetComponent<TMP_InputField>();
            if (tmpInputField != null)
            {
                return WithState($"{GetInputLabel(tmpInputField.placeholder, tmpInputField.gameObject)}. editable, has value: {tmpInputField.text}", selectable);
            }

            Button button = selectable.GetComponent<Button>();
            if (button != null)
            {
                return WithState(GetLabel(button.gameObject), selectable);
            }

            return WithState(GetLabel(selectable.gameObject), selectable);
        }

        private static string DescribeMenuOption(MenuOptionHorizontal menuOption)
        {
            string label = GetObjectName(menuOption.gameObject);
            string value = GetText(menuOption.optionText?.gameObject) ?? "No option selected";
            return $"{label}. {value}";
        }

        private static string WithState(string announcement, Selectable selectable)
        {
            string state = selectable.IsInteractable() ? string.Empty : ". Disabled";
            return $"{announcement}{state}{GetPosition(selectable)}";
        }

        private static string GetPosition(Selectable selectable)
        {
            Transform parent = selectable.transform.parent;
            if (parent == null)
            {
                return string.Empty;
            }

            var siblings = parent.GetComponentsInChildren<Selectable>(false);
            int position = 0;
            int count = 0;
            foreach (Selectable sibling in siblings)
            {
                if (!sibling.gameObject.activeInHierarchy || !sibling.IsInteractable())
                {
                    continue;
                }

                count++;
                if (sibling == selectable)
                {
                    position = count;
                }
            }

            return count > 1 && position > 0 ? $". {position} of {count}" : string.Empty;
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

            foreach (Text text in element.GetComponentsInChildren<Text>(true))
            {
                if (text.gameObject.activeInHierarchy && !string.IsNullOrWhiteSpace(text.text))
                {
                    return text.text.Trim();
                }
            }

            foreach (TMP_Text tmpText in element.GetComponentsInChildren<TMP_Text>(true))
            {
                if (tmpText.gameObject.activeInHierarchy && !string.IsNullOrWhiteSpace(tmpText.text))
                {
                    return tmpText.text.Trim();
                }
            }

            return null;
        }
    }
}
