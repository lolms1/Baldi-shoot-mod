using BepInEx;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Components;
using MTM101BaldAPI.Components.Animation;
using MTM101BaldAPI.ObjectCreation;
using MTM101BaldAPI.Registers;
using System.Collections;
using UnityEngine;

namespace BaldiTestMod
{
    /// <summary>
    /// Custom Baldi state where he stops moving, aims at the player,
    /// fires 3 lasers and 3 bullets in sequence, then returns to chasing.
    /// 
    /// Architecture: Inherits from Baldi_SubState (same as Baldi_Praise).
    /// While this state is active, the original Baldi_Chase.Update() logic
    /// (slapping, pathfinding) is completely paused.
    /// </summary>
    public class Baldi_ShootState : Baldi_SubState
    {
        // Core state duration — counts down every frame; when it hits zero, Baldi returns to previous behavior
        private float time;
        // The player Baldi is aiming at (grabbed from ec.Players[0] on Enter)
        private PlayerManager target;

        // Phase 1: Laser counters
        private int lasersFired = 0;
        private float laserTimer = 0f;
        private bool phase1Complete = false;

        // Phase 2: Bullet counters
        private float bulletTimer = 0f;
        private int bulletsFired = 0;

        // Stores the direction of each laser so its corresponding bullet follows the same path
        private Vector3[] bulletDirections = new Vector3[3];

        // Visual state: we disable the Animator for the entire state and manually control the sprite
        private SpriteRenderer aimRenderer;
        private Sprite aimSprite;
        private Sprite shootSprite;

        public Baldi_ShootState(NPC npc, Baldi baldi, NpcState previousState, float time)
            : base(npc, baldi, previousState)
        {
            this.time = time;
        }

        public override void Enter()
        {
            base.Enter();

            // Grab the primary player as the target
            target = baldi.ec.Players[0];

            // Freeze Baldi in place — override both speed and maxSpeed so Navigator doesn't drift
            npc.Navigator.SetSpeed(0f);
            npc.Navigator.maxSpeed = 0f;

            // Reset all timers and counters
            lasersFired = 0;
            laserTimer = 0f;
            phase1Complete = false;
            bulletsFired = 0;
            bulletTimer = 0f;

            // Load custom sprites from the AssetManager
            aimSprite = BasePlugin.assetMan.Get<Sprite>("placeholder2");
            shootSprite = BasePlugin.assetMan.Get<Sprite>("placeholder4");

            // Find the VISIBLE SpriteRenderer (accounting for SpriteOverlay if active)
            var spriteOverlay = baldi.GetComponentInChildren<SpriteOverlay>();
            if (spriteOverlay != null)
            {
                // SpriteOverlay hides the original renderer and creates a child "FakeRenderer"
                var fakeRendererTransform = spriteOverlay.transform.Find("FakeRenderer");
                aimRenderer = fakeRendererTransform.GetComponent<SpriteRenderer>();
            }
            else
            {
                // No SpriteOverlay — use the original renderer directly
                aimRenderer = baldi.GetComponentInChildren<SpriteRenderer>();
            }

            // Disable the Animator so it doesn't overwrite our custom sprite,
            // then set the aiming sprite for the entire state duration
            if (aimRenderer != null)
            {
                // Access the private "animator" field on Baldi via Harmony's AccessTools
                var animatorField = AccessTools.Field(typeof(Baldi), "animator");
                Animator animator = (Animator)animatorField.GetValue(baldi);

                animator.enabled = false;
                aimRenderer.sprite = aimSprite;
            }
        }

        public override void Update()
        {
            base.Update();

            // DeltaTime scaled by the NPC's speed (so fast-forward/slowdown effects apply)
            // I'll improve it later
            float deltaTime = Time.deltaTime * npc.TimeScale;

            if (!phase1Complete)
            {
                // PHASE 1: Fire 3 lasers with 0.2s interval
                laserTimer -= deltaTime;

                if (laserTimer <= 0f && lasersFired < 3)
                {
                    FireLaser();
                    lasersFired++;
                    laserTimer = 0.2f; // Reset interval timer
                }

                if (lasersFired >= 3)
                {
                    phase1Complete = true;
                    bulletTimer = 0.1f; // Small pause between last laser and first bullet
                }
            }
            else
            {
                // PHASE 2: Fire 3 bullets with 0.4s interval 
                bulletTimer -= deltaTime;

                if (bulletTimer <= 0f && bulletsFired < 3)
                {
                    FireBullet();
                    bulletsFired++;
                    bulletTimer = 0.4f; // Reset interval timer
                }
            }

            // Main state timer when expired, return to previous behavior
            time -= deltaTime;
            if (time <= 0f)
            {
                npc.behaviorStateMachine.ChangeState(previousState);
            }
        }

        public override void Exit()
        {
            base.Exit();

            // Re-enable the Animator so Baldi resumes normal animations
            var animatorField = AccessTools.Field(typeof(Baldi), "animator");
            Animator animator = (Animator)animatorField.GetValue(baldi);
            animator.enabled = true;

            // Restore Baldi's original speed
            npc.Navigator.SetSpeed(baldi.baseSpeed);
            npc.Navigator.maxSpeed = baldi.baseSpeed;
        }

        /// <summary>
        /// Fires a single laser beam with random spread.
        /// Saves the direction so the corresponding bullet uses the same path.
        /// Plays a one-frame "shoot" animation overlay.
        /// </summary>
        private void FireLaser()
        {
            // Start position slightly above Baldi (eye level)
            Vector3 baldiPos = baldi.transform.position + Vector3.up * 2f;
            Vector3 playerPos = target.transform.position;

            // Base direction toward player
            Vector3 baseDirection = (playerPos - baldiPos).normalized;

            // Add random spread: ±0.5 degrees on both axes
            float randomAngleX = UnityEngine.Random.Range(-0.5f, 0.5f);
            float randomAngleY = UnityEngine.Random.Range(-0.5f, 0.5f);
            Vector3 laserDirection = Quaternion.Euler(randomAngleX, randomAngleY, 0f) * baseDirection;

            // Save this laser's direction for its bullet
            bulletDirections[lasersFired] = laserDirection;

            // Create the visual laser beam (pure visual, no collision)
            CreateGameobject.CreateLaserBeam(baldiPos, laserDirection, 15f, Color.red);
        }

        /// <summary>
        /// Fires a single bullet along the direction of the corresponding laser.
        /// </summary>
        private void FireBullet()
        {
            Vector3 baldiPos = baldi.transform.position + Vector3.up * 2f;
            CreateGameobject.CreateBullet(baldiPos, bulletDirections[bulletsFired], 20f, new Color(1f, 0.5f, 0f));
            baldi.StartCoroutine(PlayShootFrame());
        }

        /// <summary>
        /// Coroutine that swaps to the shoot sprite for 0.1 seconds, then restores the aim sprite.
        /// This gives a visual "kick" effect when Baldi fires a bullet.
        /// </summary>
        private IEnumerator PlayShootFrame()
        {
            if (aimRenderer == null || shootSprite == null) yield break;

            aimRenderer.sprite = shootSprite;
            yield return new WaitForSeconds(0.1f);
            aimRenderer.sprite = aimSprite;
        }
    }
}
