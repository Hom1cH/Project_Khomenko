namespace DotNet_Lab01_Core
{
    public class ParacTask : EducationUnit, ICompute
    {
        private int _credits;
        public string TaskName
        {
            get => Title;
            set { if (value != null) Title = value; }
        }

        public string? TaskDescription
        {
            get => Description;
            set { if (value != null) Description = value; }
        }

        public bool IsCompleted { get; set; }
        public int? CourseId { get; set; }

        public int Credits
        {
            get => _credits;
            set { if (value > 0) _credits = value; }
        }

        public ParacTask() : base(string.Empty, string.Empty, DateTime.Now) { }

        public ParacTask(string taskName, DateTime deadline, int difficulty, int credits, string taskDescription)
            : base(taskName, taskDescription, deadline)
        {
            if (credits > 0) _credits = credits;
            Difficulty = difficulty;
            IsCompleted = false;
            Start();
        }

        public override void Start()
        {
            Activate();
            UpdateProgress(0);
            Console.WriteLine($"Task '{TaskName}' started.");
        }
        public override void Complete()
        {
            MarkCompleted();
        }

        public override void ShowInfo()
        {
            Console.WriteLine("====================================================");
            Console.WriteLine($"Task №{Id}:");
            base.ShowInfo();
            Console.WriteLine(
                $"Course ID: {CourseId?.ToString() ?? "None"}\n" +
                $"Credits: {Credits}\n" +
                $"Workload: {ComputeWorkload()}\n"
            );
        }

        public double ComputeWorkload()
        {
            return Math.Round(
                Difficulty * Credits / (Deadline - CreatedAt).TotalDays
            );
        }

        public override string ToString()
        {
            return $"Task name: {TaskName}\nCourse ID: {CourseId?.ToString() ?? "None"}\nDeadline: {Deadline}\nDifficulty: {Difficulty}\nCredits: {Credits}\nWorkload: {ComputeWorkload()}\nDescription: {TaskDescription}\nIs Completed: {(IsCompleted ? "Yes" : "No")}\n";
        }

    }
}
