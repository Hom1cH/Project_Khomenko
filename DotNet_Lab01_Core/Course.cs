namespace DotNet_Lab01_Core
{
    public class Course
    {
        private string? _courseName;
        private int _credits;
        
        private DateTime _startDate;
        private DateTime _endDate;
        public int ReceivedCredits { get; set; }
        public bool IsCompleted { get; set; }
        public Course(){}
        public Course(string coursename,int credits,DateTime startdate,DateTime enddate)
        {
            if(coursename != null){_courseName = coursename;}
            if(credits > 0){_credits = credits;}
            _startDate = startdate;
            _endDate = enddate;
            ReceivedCredits = 0;
            IsCompleted = false;
        }

        public override string ToString()
        {
            return $"Course name: {_courseName}\nCredits: {_credits}\nStart date: {_startDate}\nEnd date: {_endDate}\nIsCompleted: {(IsCompleted ? "Yes" : "No")}";
        }
    }
}

