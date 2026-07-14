using System;
using NVAccess.NVDA;

namespace SilkAccess.Services
{
    internal static class NvdaSpeech
    {
        public static void Speak(string message, bool interrupt = true)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            try
            {
                if (NVDA.IsRunning)
                {
                    NVDA.Speak(message, interrupt);
                }
            }
            catch (Exception exception)
            {
                Plugin.Logger?.LogWarning($"Unable to send speech to NVDA: {exception.Message}");
            }
        }
    }
}
