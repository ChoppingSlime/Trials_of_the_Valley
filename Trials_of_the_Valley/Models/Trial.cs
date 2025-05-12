class Trial
{
    public string ID;
    public string Name;
    public string Description;
    public string Type;
    public int GoalAmount;
    public int DurationDays;

    // Internal use
    public bool IsComplete;
    public int Progress;
    public DateTime StartDate;
}

// capire come suddividere vari tipi di quest, che richiedono oggetti come input o altro.