namespace DotNet_Lab01_Core;

public sealed class Reminder : IDisposable
{
    private readonly IReminderNotifier _notifier;
    private readonly HashSet<string> _shownReminders = new();
    private bool _disposed;

    public Reminder(IReminderNotifier notifier)
    {
        _notifier = notifier;
    }

    public void CheckDeadlines(IEnumerable<Course> courses)
    {
        ThrowIfDisposed();

        DateTime now = DateTime.Now;

        foreach (Course course in courses)
        {
            NotifyIfNeeded(
                key: $"course-{course.Id}",
                title: $"Course: {course.CourseName}",
                deadline: course.Deadline,
                warnBefore: TimeSpan.FromDays(7),
                now: now
            );

            foreach (ParacTask task in course.Tasks)
            {
                NotifyIfNeeded(
                    key: $"task-{task.Id}",
                    title: $"Task: {task.TaskName}",
                    deadline: task.Deadline,
                    warnBefore: TimeSpan.FromDays(1),
                    now: now
                );
            }
        }
    }

    private void NotifyIfNeeded(string key, string title, DateTime deadline, TimeSpan warnBefore, DateTime now)
    {
        TimeSpan timeLeft = deadline - now;

        if (timeLeft > warnBefore)
            return;

        string state = timeLeft.TotalSeconds < 0
            ? "Deadline is overdue."
            : $"Deadline is close: {deadline:dd.MM.yyyy HH:mm}.";

        string reminderKey = $"{key}-{state}";

        if (!_shownReminders.Add(reminderKey))
            return;

        _notifier.Show(title, state);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Reminder));
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        if (_notifier is IDisposable disposableNotifier)
            disposableNotifier.Dispose();

        _disposed = true;
    }
}
