using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DotNet_Lab01_Core;

namespace WinFormsApp
{
    public partial class Form1 : Form
    {
        private readonly BindingSource _courseBindingSource = new BindingSource();
        private readonly string _dataDirectory;
        private readonly string _jsonFilePath;
        private readonly string _xmlFilePath;
        private readonly string _logFilePath;
        private readonly ResourceManager _logger;
        private readonly CourseManager _courseManager;
        private readonly TaskManager _taskManager;
        private readonly CourseController _courseController;
        public Form1()
        {
            InitializeComponent();

            _dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");
            Directory.CreateDirectory(_dataDirectory);

            _jsonFilePath = Path.Combine(_dataDirectory, "courses.json");
            _xmlFilePath = Path.Combine(_dataDirectory, "courses.xml");
            _logFilePath = Path.Combine(_dataDirectory, "logs.log");

            _logger = new ResourceManager(_logFilePath);
            _courseManager = new CourseManager(_logger);
            _taskManager = new TaskManager(_courseManager, _logger);
            _courseController = new CourseController(_courseManager, _taskManager, _logger);

            _logger.WriteLog("WinForms main form initialized.");
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            CourseJsonStorage.LoadCourses(_jsonFilePath, _courseManager, _taskManager, _logger);
            _courseManager.EnableJsonAutoSave(_jsonFilePath);

            ConfigureCoursesGrid();
            LoadCoursesToGrid();
            _logger.WriteLog($"Form1_Load completed. Courses count: {_courseManager.GetCourses().Count}");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            _logger.Dispose();
            base.OnFormClosed(e);
        }

        //========================== Grid methods ==========================
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dataGridView1.Rows[e.RowIndex].DataBoundItem is not Course course)
                return;

            EditCourse(course);
        }

        private void ConfigureCoursesGrid()
        {
            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.ReadOnly = true;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.MultiSelect = false;
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dataGridView1.DataSource = _courseBindingSource;
        }

        private void LoadCoursesToGrid()
        {
            _courseBindingSource.DataSource = null;
            _courseBindingSource.DataSource = _courseManager.GetCourses();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using AddCourseForm form = new AddCourseForm();

            if (form.ShowDialog(this) == DialogResult.OK)
            {
                _courseController.CreateCourse(
                    form.CourseTitle,
                    form.CourseDescription,
                    form.CourseCredits,
                    form.CourseDifficulty,
                    form.CourseDeadline
                );

                LoadCoursesToGrid();
            }
        }

        private void EditCourse(Course course)
        {
            using AddCourseForm form = new AddCourseForm(course);

            if (form.ShowDialog(this) != DialogResult.OK)
                return;

            _courseManager.RenameCourse(course.Id, form.CourseTitle);
            _courseManager.ChangeCourseDescription(course.Id, form.CourseDescription);
            _courseManager.UpdateCourseCredits(course.Id, form.CourseCredits);
            _courseManager.UpdateCourseDifficulty(course.Id, form.CourseDifficulty);
            _courseManager.ChangeCourseDeadline(course.Id, form.CourseDeadline);

            LoadCoursesToGrid();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show(
                    "Select a course to delete.",
                    "Delete course",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                return;
            }

            if (dataGridView1.SelectedRows[0].DataBoundItem is not Course selectedCourse)
            {
                MessageBox.Show(
                    "Cannot determine selected course.",
                    "Delete course",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            DialogResult result = MessageBox.Show(
                $"Delete course \"{selectedCourse.CourseName}\"?",
                "Confirm delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
                return;

            if (!_courseManager.RemoveCourse(selectedCourse.Id))
            {
                MessageBox.Show(
                    "Course was not found.",
                    "Delete course",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );

                return;
            }

            LoadCoursesToGrid();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = "Save courses",
                Filter = "JSON files (*.json)|*.json|XML files (*.xml)|*.xml",
                FileName = "courses.json",
                InitialDirectory = _dataDirectory
            };

            if (saveDialog.ShowDialog(this) != DialogResult.OK)
                return;

            SaveBySelectedFileType(saveDialog.FileName, saveDialog.FilterIndex);

            MessageBox.Show(
                "Courses saved.",
                "Saved",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using SaveFileDialog saveDialog = new SaveFileDialog
            {
                Title = "Export courses",
                Filter = "XML files (*.xml)|*.xml|JSON files (*.json)|*.json",
                FileName = "courses.xml",
                InitialDirectory = _dataDirectory
            };

            if (saveDialog.ShowDialog(this) != DialogResult.OK)
                return;

            SaveBySelectedFileType(saveDialog.FileName, saveDialog.FilterIndex == 1 ? 2 : 1);

            MessageBox.Show(
                "Courses exported.",
                "Exported",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void SaveBySelectedFileType(string filePath, int filterIndex)
        {
            if (filterIndex == 1 || Path.GetExtension(filePath).Equals(".json", StringComparison.OrdinalIgnoreCase))
            {
                CourseJsonStorage.SaveCourses(_courseManager.GetCourses(), filePath, _logger);
                return;
            }

            CourseXmlExporter.ExportActiveCourses(_courseManager.GetCourses(), filePath, _logger);
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }
}
