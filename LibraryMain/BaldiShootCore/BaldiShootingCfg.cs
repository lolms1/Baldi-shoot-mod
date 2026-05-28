using Rewired.Demos;

namespace BaldiShootCore
{
    public static class BaldiShootingCfg
    {
        public static float ShootingCooldown { get; set; } = 14f;
        public static float LaserTimer { get; set; } = 0.5f;
        public static float BulletTimer { get; set; } = 0.5f;
        public static float BulletInterval { get; set; } = 0.4f;
        public static float CleanupTimer { get; set; } = 2f;
        public static float Coefficient { get; set; } = 4.5f;
        public static float CooldownCoefficient { get; set; } = 1.6f;
        public static int BulletAmount { get; set; } = 3;
        public static bool PiercingBullet { get; set; } = false;
        public static bool IgnoreStuns { get; set; } = false;

        public static float GetLaserTimer(float anger)
        {
            return LaserTimer;
        }
    }
}