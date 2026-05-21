namespace DotNet_Lab01_Core;


public abstract class EducationUnit : IProgressable, IExecutable, IShowable
{
    private static int _nextId = 1;
    private int _difficulty;

    // ===== BASIC INFO =====

    public int Id { get; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; protected set; }
    public DateTime? EndedAt { get; private set; }
    public DateTime Deadline { get; set; }
    public bool IsActive { get; set; }
    public UnitStatus Status { get; private set; }
    public int Progress { get; private set; } // 0 - 100
    public List<string> Tags { get; set; } = new();
    
    public int Difficulty
    {
        get => _difficulty;
        set { if (value > 0 && value <= 100) _difficulty = value; }
    }

    // ===== CONSTRUCTOR =====

    protected EducationUnit(string title, string description, DateTime deadline)
    {
        Id = _nextId++;

        Title = title;
        Description = description;
        Deadline = deadline;

        CreatedAt = DateTime.Now;
        IsActive = true;
        Status = UnitStatus.NotStarted;
        Progress = 0;
    }

    public void Rename(string name){
        if(name != null){
            Title = name;
        }
    }

    public void ChangeDescription(string description){
        if(description != null){
            Description = description;
        }
    }

    public void ChangeDeadline(DateTime deadline){
        if(deadline > CreatedAt){
            Deadline = deadline;
        }
    }

    public void AddTag(string tag)
    {
        if (!Tags.Contains(tag))
            Tags.Add(tag);
    }

    public void AddTags(IEnumerable<string> tags)
    {
        foreach (var tag in tags)
        {
            AddTag(tag);
        }
    }

    public void AddTags(params string[] tags)
    {
        AddTags((IEnumerable<string>)tags);
    }

    public void RemoveTag(string tag)
    {
        Tags.Remove(tag);
    }

    // ===== VIRTUAL METHODS =====

    public virtual void ShowInfo()
    {
        Console.WriteLine(
            $"ID: {Id}\n" +
            $"Title: {Title}\n" +
            $"Description: {Description}\n" +
            $"Status: {Status}\n" +
            $"Progress: {Progress}%\n" +
            $"Difficulty: {Difficulty}\n" +
            $"Tags: {(Tags.Any() ? string.Join(", ", Tags) : "None")}\n" +
            $"Created at: {CreatedAt}\n" +
            $"Deadline: {Deadline}\n" +
            $"Active: {(IsActive ? "Yes" : "No")}"
        );
    }

    public virtual void Activate()
    {
        IsActive = true;
        Console.WriteLine($"{Title} activated.");
    }

    public virtual void Deactivate()
    {
        IsActive = false;
        Console.WriteLine($"{Title} deactivated.");
    }

    // ===== NEW: PROGRESS CONTROL =====

    public virtual void UpdateProgress(int value)
    {
        if (value < 0) value = 0;
        if (value > 100) value = 100;

        Progress = value;

        if (Progress == 0)
            Status = UnitStatus.NotStarted;
        else if (Progress < 100)
            Status = UnitStatus.InProgress;
        else
            Status = UnitStatus.Completed;
    }

    public virtual void MarkCompleted()
    {
        Progress = 100;
        Status = UnitStatus.Completed;

        Console.WriteLine($"{Title} marked as completed.");
    }

    // ===== ABSTRACT METHODS =====

    public abstract void Start();

    public abstract void Complete();

    // ===== OVERRIDE =====

    public override string ToString()
    {
        return
            $"[{Id}] {Title} | " +
            $"Status: {Status} | " +
            $"Progress: {Progress}% | " +
            $"Created: {CreatedAt:d} | " +
            $"Active: {(IsActive ? "Yes" : "No")}";
    }
}

public enum UnitStatus
{
    NotStarted,
    InProgress,
    Completed,
    Paused,
    Archived
}
