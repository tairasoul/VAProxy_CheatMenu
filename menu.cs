using BepInEx;
using BepInEx.Logging;
using UnityEngine;


namespace VAProxyCheatMenu
{
    [BepInPlugin("tairasoul.vaproxy.cheatmenu", "VACheats", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log;
        private static bool init = false;

        private void Awake()
        {
            Log = Logger;
            Log.LogInfo("VACheats awake.");
        }

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            Init();
        }

        private void Init()
        {
            if (!init)
            {
                Log.LogInfo("Initializing CheatMenu.");
                init = true;
                GameObject cheat = new GameObject("CheatMenu");
                DontDestroyOnLoad(cheat);
                cheat.AddComponent<cheat>();
                Log.LogInfo("CheatMenu initialized. Have fun!");
            }
        }
    }
}
