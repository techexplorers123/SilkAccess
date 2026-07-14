using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using SilkAccess.Features;
using SilkAccess.Services;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SilkAccess
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "org.tech.silkaccess";
        public const string PluginName = "Silk Access";
        public const string PluginVersion = "1.0.0";

        internal static new ManualLogSource Logger;

        private MenuNarrator menuNarrator;
        private PlayerStatusAnnouncer playerStatusAnnouncer;
        private bool playerReady;

        private void Awake()
        {
            Logger = base.Logger;
            menuNarrator = new MenuNarrator();
            playerStatusAnnouncer = new PlayerStatusAnnouncer();

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} loaded.");
            StartCoroutine(WaitForPlayerData());
        }

        private void OnEnable()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnDisable()
        {
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void Update()
        {
            menuNarrator.Update();

            if (playerReady && !menuNarrator.IsTextInputFocused)
            {
                playerStatusAnnouncer.HandleHotkeys();
            }
        }

        private System.Collections.IEnumerator WaitForPlayerData()
        {
            while (PlayerData.instance == null)
            {
                yield return null;
            }

            playerReady = true;
            Logger.LogInfo("PlayerData is ready.");
            AnnounceScene(SceneManager.GetActiveScene().name, "First game scene loaded");
        }

        private void OnSceneChanged(Scene current, Scene next)
        {
            if (playerReady)
            {
                AnnounceScene(next.name, "New game scene loaded");
            }
        }

        private static void AnnounceScene(string sceneName, string logMessage)
        {
            if (string.IsNullOrEmpty(sceneName))
            {
                return;
            }

            Logger.LogInfo($"{logMessage}: {sceneName}");
            NvdaSpeech.Speak($"Zone: {sceneName}");
        }
    }
}
