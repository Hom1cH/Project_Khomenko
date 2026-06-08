namespace DotNet_Lab01_Core
{
    public class Course : EducationUnit, ICompute
    {
        private int _credits;

        public string CourseName
        {
            get => Title;
            set { if (value != null) Title = value; }
        }

        public string? CourseDescription
        {
            get => Description;
            set { if (value != null) Description = value; }
        }

        public int Credits
        {
            get => _credits;
            set { if (value > 0) _credits = value; }
        }

        public int ReceivedCredits { get; set; }
        public List<ParacTask> Tasks { get; set; } = new();

        public Course() : base(string.Empty, string.Empty, DateTime.Now) { }

        public Course(string coursename, int credits, DateTime enddate)
            : base(coursename, $"Course with {credits} credits", enddate)
        {
            if (credits > 0) _credits = credits;
            Difficulty = 50;
            ReceivedCredits = 0;
        }

        // Compatibility constructor: accepts explicit start date and end date (used by older sample projects)
        public Course(string coursename, int credits, DateTime startDate, DateTime endDate)
            : base(coursename, $"Course with {credits} credits", endDate)
        {
            if (credits > 0) _credits = credits;
            CreatedAt = startDate;
            Deadline = endDate;
            Difficulty = 50;
            ReceivedCredits = 0;
        }

        public Course(string coursename,string description, int credits, DateTime enddate)
            : base(coursename, description, enddate)
        {
            if (credits > 0) _credits = credits;
            Description = description;
            Difficulty = 50;
            ReceivedCredits = 0;
        }

        // Compatibility method for older code samples
        public int GetCourseDuration()
        {
            return (Deadline - CreatedAt).Days;
        }

        public override void Start()
        {
            Activate();
            UpdateProgress(0);
            Console.WriteLine($"Course '{CourseName}' started.");
        }

        public override void Complete()
        {
            ReceivedCredits = Credits;
            MarkCompleted();
            Console.WriteLine($"Course '{CourseName}' completed.");
        }

        public override void ShowInfo()
        {
            Console.WriteLine("====================================================");
            Console.WriteLine($"Course №{Id}:");
            base.ShowInfo();
            Console.Write(
                $"Credits: {Credits}\n" +
                $"Course workload: {ComputeWorkload()}\n" +
                $"Received credits: {ReceivedCredits}\n" +
                $"Tasks count: {Tasks.Count}\n"
            );
        }
        
        public double ComputeWorkload()
        {
            return WorkloadCalculator.CalculateImportance(Credits, Difficulty, CreatedAt, Deadline);
        }

        public override string ToString()
        {
            return $"ID: {Id}\nCourse name: {CourseName}\nCredits: {_credits}\nDifficulty: {Difficulty}\nWorkload: {ComputeWorkload()}\nStart date: {CreatedAt}\nDeadline: {Deadline}\nTasks count: {Tasks.Count}\n";
        }

        public void UpdateReceivedCredits(int value){
            if(value <= Credits && value >=0 ){
                ReceivedCredits = value;
            }
        }

        public void AddTask(ParacTask task)
        {
            task.CourseId = Id;
            Tasks.Add(task);
        }

        public bool RemoveTask(int taskId)
        {
            ParacTask? task = Tasks.FirstOrDefault(item => item.Id == taskId);

            if (task == null)
                return false;

            task.CourseId = null;
            return Tasks.Remove(task);
        }

        public void ClearTasks()
        {
            foreach (ParacTask task in Tasks)
            {
                task.CourseId = null;
            }

            Tasks.Clear();
        }

        public List<ParacTask> GetTasksSortedByDeadline()
        {
            return Tasks
                .OrderBy(task => task.Deadline)
                .ToList();
        }

        public List<ParacTask> GetTasksSortedByDifficulty()
        {
            return Tasks
                .OrderByDescending(task => task.Difficulty)
                .ToList();
        }
        public void RecalculateFromTasks()
        {
            if (Tasks.Count == 0)
            {
                Progress = 0;
                ReceivedCredits = 0;
                return;
            }

            // Прогрес = середнє по всіх тасках
            Progress = (int)Math.Round(Tasks.Average(t => t.Progress));

            // Кредити завершених тасків
            int completedCredits = Tasks
                .Where(t => t.Status == UnitStatus.Completed)
                .Sum(t => t.Credits);

            int totalTaskCredits = Tasks.Sum(t => t.Credits);

            if (totalTaskCredits <= Credits)
            {
                // Пряма сума — таски не перевищують максимум курсу
                ReceivedCredits = completedCredits;
            }
            else
            {
                // Пропорційний розрахунок:
                // totalTaskCredits = 100%
                // completedCredits = x%
                // ReceivedCredits = Credits курсу * x%
                double ratio = (double)completedCredits / totalTaskCredits;
                ReceivedCredits = (int)Math.Round(Credits * ratio);
            }

            // Оновити статус курсу
            if (Progress == 0)
                Status = UnitStatus.NotStarted;
            else if (Progress == 100)
                Status = UnitStatus.Completed;
            else if (Status != UnitStatus.Paused && Status != UnitStatus.Archived)
                Status = UnitStatus.InProgress;
        }
    }
}
