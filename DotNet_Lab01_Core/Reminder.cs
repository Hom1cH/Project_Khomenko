namespace DotNet_Lab01_Core
{
    public class Reminder
    {
        private string? _message;
        public string Message
        { 
            get { return _message; }
            set { if(value != null) _message = value; }
        }
        public DateTime ReminderTime { get; set; }
        public bool IsTriggered { get; set; }
        public int Priority { get; set; }

        public Reminder() { }
        public Reminder(string message, DateTime reminderTime, int priority)
        {
            Message = message;
            ReminderTime = reminderTime;
            Priority = priority;
            IsTriggered = false;
        }

        public override string ToString()
        {
            return $"Reminder: {Message}\nTime: {ReminderTime}\nPriority: {Priority}\nIs Triggered: {(IsTriggered ? "Yes" : "No")}\n";
        }
    }
}