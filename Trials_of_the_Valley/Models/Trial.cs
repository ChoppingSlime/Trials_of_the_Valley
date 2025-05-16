
public enum TrialConditionType
{
    Basic,
    HarvestGeneralCrop, // Harvest any kind of crop
}

public enum TrialTags
{
    FirstYear,    // Only during the first year
    PastFirstYear, // Only after the first year
    SecondYear,    // Only during the second year
    PastSecondYear, // Only after the second year
    Coop,   // Only in coop
    Singleplayer,   // Only in singleplayer
    BusUnlocked,    // Only if bus is unlocked
    ShipUnlocked    // Only if ship is unlocked
}
public enum TrialType
{
    Basic,
    Crafting,
    Location,
    Building,
    ItemDelivery,
    Monster,
    ItemHarvest,
    LostItem,
    SecretLostItem,
    Social
}

public class Trial
{
    /// <summary> Unique string ID for the trial.</summary>
    public string ID { get; set; } = string.Empty;

    /// <summary> Trial type, e.g., Basic, Crafting, Social, etc.</summary>
    public TrialType Type { get; set; } = TrialType.Basic;

    /// <summary> Title of the trial.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary> Description for the trial.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary> Optional objective hint to help the player.</summary>
    public string? ObjectiveHint { get; set; }

    /// <summary> Varyes depending on type.</summary>
    public string? CompletionRequirements { get; set; }

    /// <summary> Optional next quest IDs to unlock after completion, separated by spaces.</summary>
    public string? NextQuestIds { get; set; }

    /// <summary> Money reward for completing the quest.</summary>
    public int MoneyReward { get; set; } = 0;

    /// <summary> Optional reward description, leave blank or very short if no special reward.</summary>
    public string? RewardDescription { get; set; }

    /// <summary> Whether the trial can be cancelled by the player.</summary>
    public bool CanBeCancelled { get; set; } = false;

    /// <summary> Optional reaction text, required for ItemDelivery and some Monster quests.</summary>
    public string? ReactionText { get; set; }

    /// <summary> Optional reaction text, required for ItemDelivery and some Monster quests.</summary>
    public TrialConditionType ConditionType { get; set; }

    public int StartingGoalAmount { get; set; } = 1;

    public int GoalAmountMultiplier { get; set; } = 0;

    public List<string> Tags { get; set; } = new();
}

