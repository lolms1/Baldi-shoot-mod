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

namespace BaldiShootCore
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
        // Phase 3: Placing bananas
        private bool phase2Complete = false;
        private float cleanupTimer = 0f;

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

            SoundObject shootSound = BasePlugin.assetMan.Get<SoundObject>("BaldiShootSound");
            if (shootSound != null)
            {
                var audmanField = AccessTools.Field(typeof(Baldi), "audMan");
                AudioManager audMan = (AudioManager)audmanField.GetValue(baldi);
                audMan.PlaySingle(shootSound);
            }

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
            phase2Complete = false;
            cleanupTimer = 0f;


        // Load custom sprites from the AssetManager
        aimSprite = BasePlugin.assetMan.Get<Sprite>("BaldiAim");
        shootSprite = BasePlugin.assetMan.Get<Sprite>("BaldiShoot");

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

            if (npc.behaviorStateMachine.CurrentState != this)
            {
                return;
            }

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
            else if (!phase2Complete)
            {
                bulletTimer -= deltaTime;

                if (bulletTimer <= 0f && bulletsFired < 3)
                {
                    FireBullet();
                    bulletsFired++;
                    bulletTimer = 0.4f;
                }

                if (bulletsFired >= 3)
                {
                    phase2Complete = true;
                    cleanupTimer = 2f; 
                }
            }
            else
            {
                cleanupTimer -= deltaTime;

                if (cleanupTimer <= 0f)
                {
                    ProcessAllHits();
                    npc.behaviorStateMachine.ChangeState(previousState);
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
            CreateGameobject.CreateLaserBeam(baldiPos, laserDirection, 1000f, Color.red);
        }

        /// <summary>
        /// Fires a single bullet along the direction of the corresponding laser.
        /// </summary>
        private void FireBullet()
        {
            Vector3 baldiPos = baldi.transform.position + Vector3.up * 2f;
            CreateGameobject.CreateBullet(baldi.ec , baldiPos, bulletDirections[bulletsFired], 20f, new Color(1f, 0.5f, 0f), baldi);
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

        private void ProcessAllHits()
        {
            ITM_NanaPeel bananaPrefab = null;
            foreach (var item in Resources.FindObjectsOfTypeAll<ITM_NanaPeel>())
            {
                if (item.gameObject.scene.name == null)
                {
                    bananaPrefab = item;
                    break;
                }
            }

            Vector3 shootDirection = (target.transform.position - baldi.transform.position).normalized;

            foreach (var kvp in BulletComponent.hitCounts)
            {
                Entity targetEntity = kvp.Key;
                int hits = kvp.Value;

                if (targetEntity == null) continue;

                var freezeMod = new MovementModifier(Vector3.zero, 0f);
                targetEntity.ExternalActivity.moveMods.Add(freezeMod);

                if (BulletComponent.appliedModifiers.ContainsKey(targetEntity))
                {
                    targetEntity.ExternalActivity.moveMods.Remove(BulletComponent.appliedModifiers[targetEntity]);
                }

                float randomAngle = UnityEngine.Random.Range(-45f, 45);
                Vector3 pushDirection = Quaternion.Euler(0f, randomAngle, 0f) * shootDirection;

                float slideSpeed;
                switch (hits)
                {
                    case 1: slideSpeed = 15f; break;
                    case 2: slideSpeed = 60f; break;
                    case 3: slideSpeed = 500f; break;
                    default: slideSpeed = 15f; break;
                }

                if (bananaPrefab != null)
                {
                    ITM_NanaPeel banana = GameObject.Instantiate(bananaPrefab);

                    var speedField = AccessTools.Field(typeof(ITM_NanaPeel), "speed");
                    speedField.SetValue(banana, slideSpeed);

                    var startHeightField = AccessTools.Field(typeof(ITM_NanaPeel), "startHeight");
                    startHeightField.SetValue(banana, 0f);

                    Vector3 spawnPos = targetEntity.transform.position;
                    banana.Spawn(baldi.ec, spawnPos, pushDirection, 0f); 
                }

                Force pushForce = new Force(pushDirection, 20f, -30f);
                targetEntity.AddForce(pushForce);

                baldi.StartCoroutine(RemoveFreezeAfterDelay(targetEntity, freezeMod, 0.3f));
            }

            BulletComponent.hitCounts.Clear();
            BulletComponent.appliedModifiers.Clear();
        }



        private IEnumerator RemoveFreezeAfterDelay(Entity targetEntity, MovementModifier freezeMod, float delay)
        {
            yield return new WaitForSeconds(delay);
            if (targetEntity != null && targetEntity.ExternalActivity != null)
            {
                targetEntity.ExternalActivity.moveMods.Remove(freezeMod);
            }
        }
    }
}
