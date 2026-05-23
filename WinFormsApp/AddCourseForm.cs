using DotNet_Lab01_Core;

namespace WinFormsApp
{
    public partial class AddCourseForm : Form
    {
        public AddCourseForm()
        {
            InitializeComponent();
            AcceptButton = Add;
            CancelButton = button2;
            CourseDeadline = DateTime.Now.AddDays(90);
        }

        public AddCourseForm(Course course) : this()
        {
            Text = "Edit course";
            headerLabel.Text = "Редагуй курс";
            Add.Text = "Save";

            titleTextBox.Text = course.CourseName;
            difficultyNumeric.Value = Clamp(course.Difficulty, 1, 100);
            creditsNumeric.Value = Clamp(course.Credits, 1, 100);
            descriptionTextBox.Text = course.CourseDescription ?? "";
            deadlinePicker.Value = course.Deadline > DateTimePicker.MinimumDateTime
                ? course.Deadline
                : DateTime.Now.AddDays(90);
        }

        public string CourseTitle => titleTextBox.Text.Trim();
        public int CourseDifficulty => (int)difficultyNumeric.Value;
        public int CourseCredits => (int)creditsNumeric.Value;
        public string CourseDescription => descriptionTextBox.Text.Trim();
        public DateTime CourseDeadline
        {
            get => deadlinePicker.Value;
            private set => deadlinePicker.Value = value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
                return;

            DialogResult = DialogResult.OK;
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(titleTextBox.Text))
            {
                ShowValidationMessage("Enter course title.");
                return false;
            }

            if (deadlinePicker.Value <= DateTime.Now)
            {
                ShowValidationMessage("Deadline must be in the future.");
                return false;
            }

            return true;
        }

        private static decimal Clamp(int value, int min, int max)
        {
            if (value < min)
                return min;

            if (value > max)
                return max;

            return value;
        }

        private static void ShowValidationMessage(string message)
        {
            MessageBox.Show(
                message,
                "Validation",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
            );
        }

        private void AddCourseForm_Load(object sender, EventArgs e)
        {
        }
    }
}
