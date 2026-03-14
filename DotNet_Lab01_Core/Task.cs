namespace DotNet_Lab01_Core
{
    public class Task
    {
        private string? _taskName;
        private DateTime _deadline;
        private int _difficulty;
        private int _credits;
        public string TaskName { 
            get { return _taskName; }
			set { if(value != null) _taskName = value; }
        }
        public string? TaskDescription { get; set; }
        public bool IsCompleted { get; set; }

        public Task(){}
        public Task(string taskName,DateTime deadline,int difficulty,int credits,string taskDescription)
        {
            if(difficulty > 0 && difficulty <= 100){_difficulty = difficulty;}
            if(credits > 0){_credits = credits;}
            TaskDescription = taskDescription;
            TaskName = taskName;
            _deadline = deadline;
            IsCompleted = false;
        }
    }
}
