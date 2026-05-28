using UnityEngine;

public class ITM_BaldiTracker : Item
{
    [SerializeField]
    private int value = 500;

    [SerializeField]
    private bool multiply = true;

    public override bool Use(PlayerManager pm)
    {
        var tracker = pm.gameObject.GetComponent<BaldiTrackerComponent>();
        if (tracker == null)
        {
            tracker = pm.gameObject.AddComponent<BaldiTrackerComponent>();
        }
        if (!tracker.IsActive)
        {
            Singleton<CoreGameManager>.Instance.AddPoints(this.value, pm.playerNumber, true, true, this.multiply);
            tracker.Activate();
        }
        else
        {
            Singleton<CoreGameManager>.Instance.AddPoints(0, pm.playerNumber, true, true, this.multiply);
        }

        return true;
    }
}