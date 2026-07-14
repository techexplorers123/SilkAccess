using SilkAccess.Services;
using UnityEngine;

namespace SilkAccess.Features
{
    internal sealed class PlayerStatusAnnouncer
    {
        public void HandleHotkeys()
        {
            if (Input.GetKeyDown(KeyCode.H))
            {
                AnnounceHealth();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                AnnounceResources();
            }
        }

        private static void AnnounceHealth()
        {
            NvdaSpeech.Speak($"Health: {PlayerData.instance.health}");
        }

        private static void AnnounceResources()
        {
            NvdaSpeech.Speak($"Silk: {PlayerData.instance.silk}. Beads: {PlayerData.instance.geo}.");
        }
    }
}
