using BepInEx;
using Invector.vCharacterController;
using Invector.vMelee;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VAProxyCheatMenu
{
    internal class cheat : MonoBehaviour
    {
        private static readonly KeyCode TriggerKey = KeyCode.F7;

        private static readonly KeyCode TeleportKey = KeyCode.B;

        private bool active = false;

        private bool dontDie = false;

        private bool teleportOnB = false;

        private bool domeDisabled = false;

        private string walkSpeed = "2";
        private string sprintSpeed = "16";
        private string runningSpeed = "8";
        private string jumpHeight = "10";
        private string jumpMultiplier = "1";
        private string jumpTimer = "0.3";
        private string rollSpeed = "60";
        private bool autoParry = false;
        private string damageMultiplier = "1";
        private string airSpeed = "10";
        private string extraGravity = "-40";

        //private const float noGravity = 9.82f;
        private const string default_walkSpeed = "2";
        private const string default_sprintSpeed = "16";
        private const string default_runningSpeed = "8";
        private const string default_jumpHeight = "10";
        private const string default_jumpMultiplier = "1";
        private const string default_jumpTimer = "0.3";
        private const string default_rollSpeed = "60";
        private const string default_airSpeed = "10";
        private const string default_extraGravity = "-40";

        private class SenDataHelper
        {
            public GameObject raw;
            public Inventory inv;
            public Gimmicks gimmicks;
            public vThirdPersonController thirdPC;
            public vThirdPersonMotor.vMovementSpeed movement;
            public vMeleeManager melee;
            public Transform transform;

            public SenDataHelper(GameObject Sen)
            {
                raw = Sen;
                inv = Sen.GetComponent<Inventory>();
                gimmicks = Sen.GetComponent<Gimmicks>();
                thirdPC = Sen.GetComponent<vThirdPersonController>();
                movement = thirdPC.freeSpeed;
                melee = Sen.GetComponent<vMeleeManager>();
                transform = Sen.transform;
            }

            public bool Verify()
            {
                return raw != null && inv != null && gimmicks != null && thirdPC != null && movement != null && melee != null && transform != null;
            }
        }

        private void Awake()
        {
            SceneManager.activeSceneChanged += (Scene old, Scene newS) =>
            {
                currentScene = newS.name;
            };
        }

        private SenDataHelper SenData;

        private string currentScene = "Menu";

        public Rect windowRect = new Rect(20, 20, 200, 500);
        public void OnGUI()
        {
            if (active)
                windowRect = GUILayout.Window(0, windowRect, DrawWindow, "VAP cheats");
        }

        private void Update()
        {
            Dome dome = GameObject.FindObjectOfType<Dome>();
            if (dome != null && domeDisabled) dome.enabled = false;
            if (!(SenData is null) && SenData.Verify())
            {
                if (dontDie)
                {
                    SenData.thirdPC.AddHealth(9999);
                    SenData.thirdPC.Immune(true);
                }
                float.TryParse(runningSpeed, out SenData.movement.runningSpeed);
                float.TryParse(sprintSpeed, out SenData.movement.sprintSpeed);
                float.TryParse(walkSpeed, out SenData.movement.walkSpeed);
                float.TryParse(jumpHeight, out SenData.thirdPC.jumpHeight);
                if (float.TryParse(jumpMultiplier, out float jmp))
                    SenData.thirdPC.SetJumpMultiplier(jmp);
                float.TryParse(jumpTimer, out SenData.thirdPC.jumpTimer);
                float.TryParse(rollSpeed, out SenData.thirdPC.rollSpeed);
                float.TryParse(airSpeed, out SenData.thirdPC.airSpeed);
                float.TryParse(extraGravity, out SenData.thirdPC.extraGravity);
                if (autoParry)
                {
                    SenData.thirdPC.parry = true;
                }
                if (!(SenData.melee.CurrentActiveAttackWeapon is null) && float.TryParse(damageMultiplier, out float dmg)) SenData.melee.CurrentActiveAttackWeapon.damage.damageValue = (int)System.Math.Round(dmg);
            }
            else
            {
                GameObject S105Test = GameObject.Find("S-105");
                if (S105Test)
                {
                    SenData = new SenDataHelper(S105Test);
                }
            }
        }

        private bool TriggerDown = false;
        private bool TeleportDown = false;

        private void LateUpdate()
        {
            if (UnityInput.Current.GetKeyDown(TriggerKey) && !TriggerDown)
            {
                TriggerDown = true;
                active = !active;
                if (active || currentScene == "Menu")
                {
                    Plugin.Log.LogInfo("Unlocking cursor.");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
                else
                {
                    Plugin.Log.LogInfo("Locking cursor.");
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
            else if (UnityInput.Current.GetKeyUp(TriggerKey))
            {
                TriggerDown = false;
            }

            if (UnityInput.Current.GetKeyDown(TeleportKey) && teleportOnB && !TeleportDown)
            {
                TeleportDown = true;
                Ray ray = Camera.current.ViewportPointToRay(Camera.current.ScreenToViewportPoint(Input.mousePosition));
                LayerMask ignore = LayerMask.GetMask("Elevator", "Sand", "Water", "Default", "Magma", "Cloud", "Building", "Limb", "BodyPart", "Feet", "Dome");
                if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, ignore))
                {
                    try
                    {
                        Plugin.Log.LogInfo($"Raycast hit at position {hit.point}");
                    }
                    catch
                    {
                    }
                    Plugin.Log.LogInfo("Raycast hit. Warping.");
                    SenData.transform.localPosition = hit.point;

                }
                else
                {
                    Plugin.Log.LogInfo("Could not hit part when raycasting.");
                }
            }
            else if (UnityInput.Current.GetKeyUp(TeleportKey))
            {
                TeleportDown = false;
            }
        }

        private void DrawWindow(int windowId)
        {
            if (GUILayout.Button("Get all weapons."))
            {
                DATA data = GameObject.Find("MAINMENU").GetComponent("DATA") as DATA;
                int i = 0;
                while (true)
                {
                    Plugin.Log.LogInfo($"Trying data.AddWeapon({i++})");
                    try
                    {
                        data.AddWeapon(i);
                    }
                    catch
                    {
                        break;
                    }
                }
                Plugin.Log.LogInfo($"Reached int {i} before encountering an error with data.AddWeapon()");
            }

            dontDie = GUILayout.Toggle(dontDie, "Prevent dying.");
            autoParry = GUILayout.Toggle(autoParry, "Automatically parry.");
            teleportOnB = GUILayout.Toggle(teleportOnB, "Teleport to cursor when pressing B.");
            domeDisabled = GUILayout.Toggle(domeDisabled, "Disable dome.");

            GUILayout.Label($"Walkspeed: {walkSpeed}");
            walkSpeed = GUILayout.TextArea(walkSpeed);
            GUILayout.Label($"Run speed: {runningSpeed}");
            runningSpeed = GUILayout.TextArea(runningSpeed);
            GUILayout.Label($"Sprint speed: {sprintSpeed}");
            sprintSpeed = GUILayout.TextArea(sprintSpeed);
            GUILayout.Label($"Jump height: {jumpHeight}");
            jumpHeight = GUILayout.TextArea(jumpHeight);
            GUILayout.Label($"Jump height multiplier: {jumpMultiplier}");
            jumpMultiplier = GUILayout.TextArea(jumpMultiplier);
            GUILayout.Label($"Time jump force is applied for: {jumpTimer}");
            jumpTimer = GUILayout.TextArea(jumpTimer);
            GUILayout.Label($"Roll speed: {rollSpeed}");
            rollSpeed = GUILayout.TextArea(rollSpeed);
            GUILayout.Label($"Damage multiplier: {damageMultiplier}");
            damageMultiplier = GUILayout.TextArea(damageMultiplier);
            GUILayout.Label($"Air speed: {airSpeed}");
            airSpeed = GUILayout.TextArea(airSpeed);
            GUILayout.Label($"Extra gravity: {extraGravity}");
            extraGravity = GUILayout.TextArea(extraGravity);

            if (GUILayout.Button("Reset above options."))
            {
                walkSpeed = default_walkSpeed;
                runningSpeed = default_runningSpeed;
                sprintSpeed = default_sprintSpeed;
                jumpHeight = default_jumpHeight;
                jumpMultiplier = default_jumpMultiplier;
                jumpTimer = default_jumpTimer;
                rollSpeed = default_rollSpeed;
                airSpeed = default_airSpeed;
                extraGravity = default_extraGravity;
                damageMultiplier = "1";
            }

            GUI.DragWindow(new Rect(0, 0, 10000, 10000));
        }
    }
}
