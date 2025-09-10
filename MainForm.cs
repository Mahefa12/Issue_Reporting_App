namespace IssuesReportingApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnReportIssues_Click(object sender, EventArgs e)
        {
            ReportIssuesForm reportForm = new ReportIssuesForm();
            reportForm.ShowDialog();
        }

        private void btnLocalEvents_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature will be implemented in a future update.", 
                          "Feature Not Available", 
                          MessageBoxButtons.OK, 
                          MessageBoxIcon.Information);
        }

        private void btnServiceStatus_Click(object sender, EventArgs e)
        {
            MessageBox.Show("This feature will be implemented in a future update.", 
                          "Feature Not Available", 
                          MessageBoxButtons.OK, 
                          MessageBoxIcon.Information);
        }

        private void btnImprovementIdeas_Click(object sender, EventArgs e)
        {
            ImprovementIdeasForm improvementForm = new ImprovementIdeasForm();
            improvementForm.ShowDialog();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.CenterToScreen();
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }
    }
}