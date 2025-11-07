using UnityEngine;


[System.Serializable]
public struct Reward
{
    public string _rewardName;
    public Sprite _rewardIcon;
}

[CreateAssetMenu(menuName = "ScritpableObjects/Data/Reward Data Table")]
public class RewardData : ScriptableObject
{
   public Reward[] _rewards;
}
