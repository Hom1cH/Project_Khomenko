namespace DotNet_Lab01_Core
{
    public class Course
    {
        private string? _courseName;
        private int _credits;
        private static int _nextId = 0;
        public int Id { get; }

        private DateTime _startDate;
        private DateTime _endDate;
        public string CourseName
        {
            get => _courseName ?? string.Empty;
            set { if (value != null) _courseName = value; }
        }
        public int Credits
        {
            get => _credits;
            set { if (value > 0) _credits = value; }
        }

        public DateTime StartDate
        {
            get => _startDate;
            set { _startDate = value; }
        }

        public DateTime EndDate
        {
            get => _endDate;
            set { _endDate = value; }
        }
        
        public int ReceivedCredits { get; set; }
        public bool IsCompleted { get; set; }
        public Course(){}
        public Course(string coursename,int credits,DateTime startdate,DateTime enddate)
        {
            Id = _nextId++;
            if(coursename != null){_courseName = coursename;}
            if(credits > 0){_credits = credits;}
            _startDate = startdate;
            _endDate = enddate;
            ReceivedCredits = 0;
            IsCompleted = false;
        }

        public override string ToString()
        {
            return $"ID: {Id}\nCourse name: {_courseName}\nCredits: {_credits}\nStart date: {_startDate}\nEnd date: {_endDate}\nIsCompleted: {(IsCompleted ? "Yes" : "No")}\n";
        }
    }
}

