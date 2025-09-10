using IssuesReportingApp.Models;
using IssuesReportingApp.DataStructures;

namespace IssuesReportingApp
{
    public partial class ImprovementIdeasForm : Form
    {
        private static CustomStack<ImprovementIdea> submittedIdeas = new CustomStack<ImprovementIdea>();
        private static int nextId = 1;

        public ImprovementIdeasForm()
        {
            InitializeComponent();
        }

        private void ImprovementIdeasForm_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            cmbCategory.Items.AddRange(new string[]
            {
                "Parks & Recreation",
                "Transportation",
                "Public Safety",
                "Environmental",
                "Technology & Digital Services",
                "Community Services",
                "Infrastructure",
                "Economic Development",
                "Health & Wellness",
                "Education",
                "Other"
            });

            cmbPriority.Items.AddRange(new string[]
            {
                "Low",
                "Medium",
                "High",
                "Critical"
            });
            cmbPriority.SelectedIndex = 1;

            cmbEstimatedCost.Items.AddRange(new string[]
            {
                "Under R1,000",
                "R1,000 - R5,000",
                "R5,000 - R25,000",
                "R25,000 - R100,000",
                "Over R100,000",
                "Unknown"
            });

            UpdateProgressBar();
        }

        private void btnAttachFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All Files (*.*)|*.*|Images (*.jpg;*.jpeg;*.png;*.gif)|*.jpg;*.jpeg;*.png;*.gif|Documents (*.pdf;*.doc;*.docx;*.txt)|*.pdf;*.doc;*.docx;*.txt";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        if (!lstAttachedFiles.Items.Contains(fileName))
                        {
                            lstAttachedFiles.Items.Add(fileName);
                        }
                    }
                    UpdateProgressBar();
                }
            }
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            if (lstAttachedFiles.SelectedIndex >= 0)
            {
                lstAttachedFiles.Items.RemoveAt(lstAttachedFiles.SelectedIndex);
                UpdateProgressBar();
            }
            else
            {
                MessageBox.Show("Please select a file to remove.", "No File Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateForm())
            {
                ImprovementIdea idea = new ImprovementIdea
                {
                    Id = nextId++,
                    Title = txtTitle.Text.Trim(),
                    Category = cmbCategory.SelectedItem?.ToString() ?? string.Empty,
                    Description = rtbDescription.Text.Trim(),
                    SubmitterName = txtSubmitterName.Text.Trim(),
                    SubmitterEmail = txtSubmitterEmail.Text.Trim(),
                    Priority = cmbPriority.SelectedItem?.ToString() ?? "Medium",
                    ExpectedBenefit = rtbExpectedBenefit.Text.Trim(),
                    EstimatedCost = cmbEstimatedCost.SelectedItem?.ToString() ?? "Unknown",
                    AttachedFiles = lstAttachedFiles.Items.Cast<string>().ToList()
                };

                submittedIdeas.Push(idea);
                MessageBox.Show($"Thank you! Your improvement idea '{idea.Title}' has been submitted successfully.\n\nIdea ID: {idea.Id}\nSubmitted on: {idea.SubmittedDate:yyyy-MM-dd HH:mm}", 
                              "Idea Submitted Successfully", 
                              MessageBoxButtons.OK, 
                              MessageBoxIcon.Information);

                ClearForm();
            }
        }

        private bool ValidateForm()
        {
            CustomLinkedList<string> errors = new CustomLinkedList<string>();

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
                errors.Add("- Title is required");

            if (cmbCategory.SelectedIndex == -1)
                errors.Add("- Category must be selected");

            if (string.IsNullOrWhiteSpace(rtbDescription.Text))
                errors.Add("- Description is required");

            if (string.IsNullOrWhiteSpace(txtSubmitterName.Text))
                errors.Add("- Your name is required");

            if (string.IsNullOrWhiteSpace(txtSubmitterEmail.Text))
                errors.Add("- Email address is required");
            else if (!IsValidEmail(txtSubmitterEmail.Text.Trim()))
                errors.Add("- Please enter a valid email address");

            if (string.IsNullOrWhiteSpace(rtbExpectedBenefit.Text))
                errors.Add("- Expected benefit description is required");

            if (errors.Count > 0)
            {
                string errorMessage = "Please correct the following errors:\n\n" + string.Join("\n", errors.ToList());
                MessageBox.Show(errorMessage, "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void UpdateProgressBar()
        {
            int completedFields = 0;
            int totalFields = 7;

            if (!string.IsNullOrWhiteSpace(txtTitle.Text)) completedFields++;
            if (cmbCategory.SelectedIndex != -1) completedFields++;
            if (!string.IsNullOrWhiteSpace(rtbDescription.Text)) completedFields++;
            if (!string.IsNullOrWhiteSpace(txtSubmitterName.Text)) completedFields++;
            if (!string.IsNullOrWhiteSpace(txtSubmitterEmail.Text)) completedFields++;
            if (!string.IsNullOrWhiteSpace(rtbExpectedBenefit.Text)) completedFields++;
            if (cmbPriority.SelectedIndex != -1) completedFields++;

            int progressPercentage = (int)((double)completedFields / totalFields * 100);
            progressBar.Value = progressPercentage;
            if (progressPercentage == 100)
            {
                lblEngagement.Text = "🎉 Great! Your idea is ready to submit!";
                lblEngagement.ForeColor = Color.Green;
            }
            else if (progressPercentage >= 70)
            {
                lblEngagement.Text = "👍 Almost there! Just a few more details needed.";
                lblEngagement.ForeColor = Color.Orange;
            }
            else if (progressPercentage >= 40)
            {
                lblEngagement.Text = "📝 Good progress! Keep filling out the details.";
                lblEngagement.ForeColor = Color.Blue;
            }
            else
            {
                lblEngagement.Text = "💡 Share your brilliant idea to improve our community!";
                lblEngagement.ForeColor = Color.Gray;
            }
        }

        private void ClearForm()
        {
            txtTitle.Clear();
            cmbCategory.SelectedIndex = -1;
            rtbDescription.Clear();
            txtSubmitterName.Clear();
            txtSubmitterEmail.Clear();
            rtbExpectedBenefit.Clear();
            cmbPriority.SelectedIndex = 1;
            cmbEstimatedCost.SelectedIndex = -1;
            lstAttachedFiles.Items.Clear();
            UpdateProgressBar();
        }

        private void btnBackToMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnViewSubmittedIdeas_Click(object sender, EventArgs e)
        {
            if (submittedIdeas.Count == 0)
            {
                MessageBox.Show("No ideas have been submitted yet.", "No Ideas", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string ideasSummary = $"Submitted Ideas ({submittedIdeas.Count}):\n\n";
            var ideasList = submittedIdeas.ToList();
            foreach (var idea in ideasList.OrderByDescending(i => i.SubmittedDate))
            {
                ideasSummary += $"ID: {idea.Id} | {idea.Title}\n";
                ideasSummary += $"Category: {idea.Category} | Priority: {idea.Priority}\n";
                ideasSummary += $"Submitted: {idea.SubmittedDate:yyyy-MM-dd HH:mm}\n";
                ideasSummary += $"By: {idea.SubmitterName}\n\n";
            }

            MessageBox.Show(ideasSummary, "Submitted Ideas", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void txtTitle_TextChanged(object sender, EventArgs e) => UpdateProgressBar();
        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e) => UpdateProgressBar();
        private void rtbDescription_TextChanged(object sender, EventArgs e) => UpdateProgressBar();
        private void txtSubmitterName_TextChanged(object sender, EventArgs e) => UpdateProgressBar();
        private void txtSubmitterEmail_TextChanged(object sender, EventArgs e) => UpdateProgressBar();
        private void rtbExpectedBenefit_TextChanged(object sender, EventArgs e) => UpdateProgressBar();
        private void cmbPriority_SelectedIndexChanged(object sender, EventArgs e) => UpdateProgressBar();
    }
}