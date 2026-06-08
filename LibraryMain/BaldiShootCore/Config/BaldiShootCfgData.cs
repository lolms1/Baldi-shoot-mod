[System.Serializable]
public class BaldiShootCfgData
{
    public float ShootCooldown = 13f;
    public float LaserTimer = 0.6f;
    public float BulletTimer = 0.6f;
    public float BulletInterval = 0.8f;
    public float CleanupTimer = 2f;
    public float BulletSpeed = 200f;
    public float Coefficient = 3.5f;
    public float CooldownCoefficient = 1.2f;
    public float MultiplierLogBase = 2f;
    public float CooldownMultiplierLogBase = 2f;
    public float StarterAnger = 1.5f;
    public float CooldownStarterAnger = 1.5f;
    public int BulletAmount = 3;
    public int BulletCapacity = 15;
    public bool Capacity = false;
    public bool PiercingBullet = false;
    public bool IgnoreStuns = false;
    public bool DebugBaldisAnger = false;
}