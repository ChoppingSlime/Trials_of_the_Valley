
public enum TrialConditionType
{
    HarvestSpecificCrop,    // Harvest a specific type of crop
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

public class Trial
{
    public string ID { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    /// <summary> How completing the quest is determined.</summary>
    public TrialConditionType ConditionType { get; set; }

    /// <summary> Specific item target id.</summary>
    public string? TargetEntity { get; set; }

    public int StartingGoalAmount { get; set; } = 1;

    /// <summary> Multiplier every year.</summary>
    public int GoalAmountMultiplier { get; set; } = 0; 

    /// <summary> How many days does it last.</summary>
    public int DurationDays { get; set; } = 112; // 1 year default

    /// <summary> Used to filter only available tags.</summary>
    public List<string> Tags { get; set; } = new(); // 

  
}
