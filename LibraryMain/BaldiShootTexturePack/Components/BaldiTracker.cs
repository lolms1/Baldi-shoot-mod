using UnityEngine;

public class BaldiTrackerComponent : MonoBehaviour
{
    public bool IsActive { get; private set; }

    private PlayerManager player;

    void Awake()
    {
        player = GetComponent<PlayerManager>();
    }

    public void Activate()
    {
        IsActive = true;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}