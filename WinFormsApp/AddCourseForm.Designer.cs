namespace WinFormsApp
{
    partial class AddCourseForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            titleTextBox = new TextBox();
            difficultyNumeric = new NumericUpDown();
            creditsNumeric = new NumericUpDown();
            descriptionTextBox = new TextBox();
            deadlinePicker = new DateTimePicker();
            Add = new Button();
            button2 = new Button();
            headerLabel = new Label();
            titleLabel = new Label();
            difficultyLabel = new Label();
            creditsLabel = new Label();
            descriptionLabel = new Label();
            deadlineLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)difficultyNumeric).BeginInit();
            ((System.ComponentModel.ISupportInitialize)creditsNumeric).BeginInit();
            SuspendLayout();
            // 
            // titleTextBox
            // 
            titleTextBox.Location = new Point(138, 65);
            titleTextBox.Name = "titleTextBox";
            titleTextBox.Size = new Size(280, 23);
            titleTextBox.TabIndex = 0;
            // 
            // difficultyNumeric
            // 
            difficultyNumeric.Location = new Point(138, 94);
            difficultyNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            difficultyNumeric.Name = "difficultyNumeric";
            difficultyNumeric.Size = new Size(280, 23);
            difficultyNumeric.TabIndex = 1;
            difficultyNumeric.Value = new decimal(new int[] { 50, 0, 0, 0 });
            // 
            // creditsNumeric
            // 
            creditsNumeric.Location = new Point(138, 123);
            creditsNumeric.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            creditsNumeric.Name = "creditsNumeric";
            creditsNumeric.Size = new Size(280, 23);
            creditsNumeric.TabIndex = 2;
            creditsNumeric.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // descriptionTextBox
            // 
            descriptionTextBox.Location = new Point(138, 181);
            descriptionTextBox.Multiline = true;
            descriptionTextBox.Name = "descriptionTextBox";
            descriptionTextBox.Size = new Size(280, 72);
            descriptionTextBox.TabIndex = 4;
            // 
            // deadlinePicker
            // 
            deadlinePicker.Location = new Point(138, 152);
            deadlinePicker.Name = "deadlinePicker";
            deadlinePicker.Size = new Size(280, 23);
            deadlinePicker.TabIndex = 3;
            // 
            // Add
            // 
            Add.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            Add.Location = new Point(262, 268);
            Add.Name = "Add";
            Add.Size = new Size(75, 27);
            Add.TabIndex = 5;
            Add.Text = "OK";
            Add.UseVisualStyleBackColor = true;
            Add.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button2.Location = new Point(343, 268);
            button2.Name = "button2";
            button2.Size = new Size(75, 27);
            button2.TabIndex = 6;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // headerLabel
            // 
            headerLabel.AutoSize = true;
            headerLabel.Font = new Font("Segoe UI", 16F);
            headerLabel.Location = new Point(138, 18);
            headerLabel.Name = "headerLabel";
            headerLabel.Size = new Size(128, 30);
            headerLabel.TabIndex = 7;
            headerLabel.Text = "Додай курс";
            // 
            // titleLabel
            // 
            titleLabel.AutoSize = true;
            titleLabel.Location = new Point(12, 68);
            titleLabel.Name = "titleLabel";
            titleLabel.Size = new Size(42, 15);
            titleLabel.TabIndex = 8;
            titleLabel.Text = "Назва:";
            // 
            // difficultyLabel
            // 
            difficultyLabel.AutoSize = true;
            difficultyLabel.Location = new Point(12, 96);
            difficultyLabel.Name = "difficultyLabel";
            difficultyLabel.Size = new Size(111, 15);
            difficultyLabel.TabIndex = 9;
            difficultyLabel.Text = "Складність(1...100):";
            // 
            // creditsLabel
            // 
            creditsLabel.AutoSize = true;
            creditsLabel.Location = new Point(12, 125);
            creditsLabel.Name = "creditsLabel";
            creditsLabel.Size = new Size(96, 15);
            creditsLabel.TabIndex = 10;
            creditsLabel.Text = "Кредити(1...100):";
            // 
            // descriptionLabel
            // 
            descriptionLabel.AutoSize = true;
            descriptionLabel.Location = new Point(12, 184);
            descriptionLabel.Name = "descriptionLabel";
            descriptionLabel.Size = new Size(39, 15);
            descriptionLabel.TabIndex = 11;
            descriptionLabel.Text = "Опис:";
            // 
            // deadlineLabel
            // 
            deadlineLabel.AutoSize = true;
            deadlineLabel.Location = new Point(12, 156);
            deadlineLabel.Name = "deadlineLabel";
            deadlineLabel.Size = new Size(56, 15);
            deadlineLabel.TabIndex = 12;
            deadlineLabel.Text = "Deadline:";
            // 
            // AddCourseForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(430, 307);
            Controls.Add(deadlineLabel);
            Controls.Add(descriptionLabel);
            Controls.Add(creditsLabel);
            Controls.Add(difficultyLabel);
            Controls.Add(titleLabel);
            Controls.Add(headerLabel);
            Controls.Add(button2);
            Controls.Add(Add);
            Controls.Add(deadlinePicker);
            Controls.Add(descriptionTextBox);
            Controls.Add(creditsNumeric);
            Controls.Add(difficultyNumeric);
            Controls.Add(titleTextBox);
            MinimumSize = new Size(446, 346);
            Name = "AddCourseForm";
            Text = "Course";
            Load += AddCourseForm_Load;
            ((System.ComponentModel.ISupportInitialize)difficultyNumeric).EndInit();
            ((System.ComponentModel.ISupportInitialize)creditsNumeric).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox titleTextBox;
        private NumericUpDown difficultyNumeric;
        private NumericUpDown creditsNumeric;
        private TextBox descriptionTextBox;
        private DateTimePicker deadlinePicker;
        private Button Add;
        private Button button2;
        private Label headerLabel;
        private Label titleLabel;
        private Label difficultyLabel;
        private Label creditsLabel;
        private Label descriptionLabel;
        private Label deadlineLabel;
    }
}
