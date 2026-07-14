using SilkAccess.Services;
using TMPro;
using UnityEngine;

namespace SilkAccess.Features
{
    internal sealed class DialogueNarrator
    {
        private const float StableTextDelay = 0.5f;

        private string observedText;
        private string lastSpokenText;
        private float lastTextChangeTime;

        public void Update()
        {
            DialogueBox dialogueBox = Object.FindFirstObjectByType<DialogueBox>();
            if (dialogueBox == null || !dialogueBox.isActiveAndEnabled)
            {
                Reset();
                return;
            }

            TMP_Text textComponent = dialogueBox.GetComponentInChildren<TMP_Text>(true);
            string dialogueText = textComponent?.text?.Trim();
            if (string.IsNullOrEmpty(dialogueText))
            {
                Reset();
                return;
            }

            if (dialogueText != observedText)
            {
                observedText = dialogueText;
                lastTextChangeTime = Time.unscaledTime;
                return;
            }

            if (dialogueText != lastSpokenText && Time.unscaledTime - lastTextChangeTime >= StableTextDelay)
            {
                lastSpokenText = dialogueText;
                NvdaSpeech.Speak(dialogueText);
            }
        }

        private void Reset()
        {
            observedText = null;
            lastSpokenText = null;
        }
    }
}
