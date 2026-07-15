using HarmonyLib;
using SilkAccess.Services;

namespace SilkAccess.Features
{
#pragma warning disable Harmony003 // DialogueLine is a value-type event payload; it is only read here.
    /// <summary>Uses game events instead of polling UI state, so announcements are complete and timely.</summary>
    internal static class GameEventNarrator
    {
        private static int healthBeforeChange;

        [HarmonyPatch(typeof(NPCControlBase), "NewLineStarted")]
        private static class DialogueLineStartedPatch
        {
            private static void Postfix(DialogueBox.DialogueLine line)
            {
                string text = line.Text;
                bool isPlayerChoice = line.IsPlayer;
                if (string.IsNullOrWhiteSpace(text))
                {
                    return;
                }

                NvdaSpeech.Speak(isPlayerChoice ? $"Choice: {text}" : text);
            }
        }

        [HarmonyPatch(typeof(PlayerData), "AddHealth")]
        private static class HealthGainedPatch
        {
            private static void Prefix(PlayerData __instance) => healthBeforeChange = __instance.health;

            private static void Postfix(PlayerData __instance)
            {
                int gained = __instance.health - healthBeforeChange;
                if (gained > 0)
                {
                    NvdaSpeech.Speak($"Gained {gained} health", interrupt: false);
                }
            }
        }

        [HarmonyPatch(typeof(PlayerData), "TakeHealth")]
        private static class HealthLostPatch
        {
            private static void Prefix(PlayerData __instance) => healthBeforeChange = __instance.health;

            private static void Postfix(PlayerData __instance)
            {
                int lost = healthBeforeChange - __instance.health;
                if (lost > 0)
                {
                    NvdaSpeech.Speak($"Lost {lost} health");
                }
            }
        }
    }
#pragma warning restore Harmony003
}
