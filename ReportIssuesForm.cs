using IssuesReportingApp.Models;
using IssuesReportingApp.DataStructures;

namespace IssuesReportingApp
{
    public partial class ReportIssuesForm : Form
    {
        private static CustomLinkedList<Issue> reportedIssues = new CustomLinkedList<Issue>();
        private CustomLinkedList<string> attachedFiles = new CustomLinkedList<string>();
        private CustomQueue<string> encouragingMessages;
        
        private void InitializeEncouragingMessages()
        {
            encouragingMessages = new CustomQueue<string>();
            encouragingMessages.Enqueue("Thank you for being an active citizen!");
            encouragingMessages.Enqueue("Your report helps improve our community!");
            encouragingMessages.Enqueue("Every report makes a difference!");
            encouragingMessages.Enqueue("Together we build a better city!");
            encouragingMessages.Enqueue("Your voice matters in our community!");
        }
        private Random random = new Random();

        public ReportIssuesForm()
        {
            InitializeComponent();
            InitializeEncouragingMessages();
        }

        private void ReportIssuesForm_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            cmbCategory.Items.AddRange(new string[] {
                "Sanitation",
                "Roads",
                "Utilities",
                "Public Safety",
                "Parks and Recreation",
                "Street Lighting",
                "Water and Sewage",
                "Noise Complaints",
                "Other"
            });

            UpdateEngagementMessage();
            progressBarEngagement.Value = 0;
            progressBarEngagement.Maximum = 100;
        }

        private void btnAttachFile_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "All Files (*.*)|*.*|Images (*.jpg;*.jpeg;*.png;*.gif;*.bmp)|*.jpg;*.jpeg;*.png;*.gif;*.bmp|Documents (*.pdf;*.doc;*.docx;*.txt)|*.pdf;*.doc;*.docx;*.txt";
                openFileDialog.FilterIndex = 1;
                openFileDialog.Multiselect = true;
                openFileDialog.Title = "Select files to attach";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    foreach (string fileName in openFileDialog.FileNames)
                    {
                        if (!attachedFiles.Contains(fileName))
                        {
                            attachedFiles.Add(fileName);
                        }
                    }
                    UpdateAttachedFilesList();
                    UpdateProgressBar();
                }
            }
        }

        private void UpdateAttachedFilesList()
        {
            lstAttachedFiles.Items.Clear();
            foreach (string file in attachedFiles)
            {
                lstAttachedFiles.Items.Add(Path.GetFileName(file));
            }
            lblAttachedCount.Text = $"Attached Files: {attachedFiles.Count}";
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtLocation.Text))
            {
                MessageBox.Show("Please enter a location for the issue.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtLocation.Focus();
                return;
            }

            if (cmbCategory.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a category for the issue.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbCategory.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(rtbDescription.Text))
            {
                MessageBox.Show("Please provide a description of the issue.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                rtbDescription.Focus();
                return;
            }


            Issue newIssue = new Issue(txtLocation.Text.Trim(), cmbCategory.SelectedItem.ToString(), rtbDescription.Text.Trim())
            {
                Id = reportedIssues.Count + 1
            };


            foreach (string file in attachedFiles)
            {
                newIssue.AttachedFiles.Add(file);
            }


            reportedIssues.Add(newIssue);


            MessageBox.Show($"Issue reported successfully!\n\nIssue ID: {newIssue.Id}\nLocation: {newIssue.Location}\nCategory: {newIssue.Category}\nStatus: {newIssue.Status}\n\nThank you for helping improve our community!", 
                          "Report Submitted", 
                          MessageBoxButtons.OK, 
                          MessageBoxIcon.Information);


            ClearForm();


            UpdateEngagementMessage();
            progressBarEngagement.Value = 100;


            lblTotalReports.Text = $"Total Reports Submitted: {reportedIssues.Count}";
        }

        private void ClearForm()
        {
            txtLocation.Clear();
            cmbCategory.SelectedIndex = -1;
            rtbDescription.Clear();
            attachedFiles.Clear();
            UpdateAttachedFilesList();
            progressBarEngagement.Value = 0;
        }

        private void UpdateEngagementMessage()
        {
            if (encouragingMessages.Count > 0)
            {
                lblEngagement.Text = encouragingMessages.GetRandomItem(random);
            }
        }

        private void UpdateProgressBar()
        {
            int progress = 0;
            if (!string.IsNullOrWhiteSpace(txtLocation.Text)) progress += 25;
            if (cmbCategory.SelectedIndex != -1) progress += 25;
            if (!string.IsNullOrWhiteSpace(rtbDescription.Text)) progress += 25;
            if (attachedFiles.Count > 0) progress += 25;

            progressBarEngagement.Value = progress;
        }

        private void btnBackToMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRemoveFile_Click(object sender, EventArgs e)
        {
            if (lstAttachedFiles.SelectedIndex != -1)
            {
                int selectedIndex = lstAttachedFiles.SelectedIndex;
                var filesList = attachedFiles.ToList();
                if (selectedIndex < filesList.Count)
                {
                    string fileToRemove = filesList[selectedIndex];
                    attachedFiles.Remove(fileToRemove);
                    UpdateAttachedFilesList();
                    UpdateProgressBar();
                }
            }
            else
            {
                MessageBox.Show("Please select a file to remove.", "No File Selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void txtLocation_TextChanged(object sender, EventArgs e)
        {
            UpdateProgressBar();
        }

        private void cmbCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateProgressBar();
        }

        private void rtbDescription_TextChanged(object sender, EventArgs e)
        {
            UpdateProgressBar();
        }

        public static List<Issue> GetReportedIssues()
        {
            return reportedIssues.ToList();
        }
    }
}