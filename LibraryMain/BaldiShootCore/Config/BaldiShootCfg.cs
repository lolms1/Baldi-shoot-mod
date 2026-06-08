namespace BaldiShootCore
{
    public static class BaldiShootCfg
    {
        private static BaldiShootCfgData _defaults;

        public static float ShootCooldown { get; set; }
        public static float LaserTimer { get; set; }
        public static float BulletTimer { get; set; }
        public static float BulletInterval { get; set; }
        public static float CleanupTimer { get; set; }
        public static float BulletSpeed { get; set; }
        public static float Coefficient { get; set; }
        public static float CooldownCoefficient { get; set; }
        public static float MultiplierLogBase { get; set; }
        public static float CooldownMultiplierLogBase { get; set; }
        public static float StarterAnger { get; set; }
        public static float CooldownStarterAnger { get; set; }
        public static int BulletAmount { get; set; }
        public static int BulletCapacity { get; set; }
        public static bool Capacity { get; set; }
        public static bool PiercingBullet { get; set; }
        public static bool IgnoreStuns { get; set; }
        public static bool DebugBaldisAnger { get; set; }

        public static void SetDefaults(BaldiShootCfgData defaults)
        {
            _defaults = defaults;
        }

        public static void ResetToDefaults()
        {
            if (_defaults == null) return;
            ShootCooldown = _defaults.ShootCooldown;
            LaserTimer = _defaults.LaserTimer;
            BulletTimer = _defaults.BulletTimer;
            BulletInterval = _defaults.BulletInterval;
            CleanupTimer = _defaults.CleanupTimer;
            BulletSpeed = _defaults.BulletSpeed;
            Coefficient = _defaults.Coefficient;
            CooldownCoefficient = _defaults.CooldownCoefficient;
            MultiplierLogBase = _defaults.MultiplierLogBase;
            CooldownMultiplierLogBase = _defaults.MultiplierLogBase;
            StarterAnger = _defaults.StarterAnger;
            CooldownStarterAnger = _defaults.CooldownStarterAnger;
            BulletAmount = _defaults.BulletAmount;
            BulletCapacity = _defaults.BulletCapacity;
            Capacity = _defaults.Capacity;
            PiercingBullet = _defaults.PiercingBullet;
            IgnoreStuns = _defaults.IgnoreStuns;
            DebugBaldisAnger = _defaults.DebugBaldisAnger;
        }
    }
}