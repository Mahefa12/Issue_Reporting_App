namespace IssuesReportingApp
{
    partial class ImprovementIdeasForm
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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblIdeaTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblCategory = new System.Windows.Forms.Label();
            this.cmbCategory = new System.Windows.Forms.ComboBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.rtbDescription = new System.Windows.Forms.RichTextBox();
            this.lblSubmitterName = new System.Windows.Forms.Label();
            this.txtSubmitterName = new System.Windows.Forms.TextBox();
            this.lblSubmitterEmail = new System.Windows.Forms.Label();
            this.txtSubmitterEmail = new System.Windows.Forms.TextBox();
            this.lblExpectedBenefit = new System.Windows.Forms.Label();
            this.rtbExpectedBenefit = new System.Windows.Forms.RichTextBox();
            this.lblPriority = new System.Windows.Forms.Label();
            this.cmbPriority = new System.Windows.Forms.ComboBox();
            this.lblEstimatedCost = new System.Windows.Forms.Label();
            this.cmbEstimatedCost = new System.Windows.Forms.ComboBox();
            this.lblAttachedFiles = new System.Windows.Forms.Label();
            this.lstAttachedFiles = new System.Windows.Forms.ListBox();
            this.btnAttachFile = new System.Windows.Forms.Button();
            this.btnRemoveFile = new System.Windows.Forms.Button();
            this.lblEngagement = new System.Windows.Forms.Label();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.btnSubmit = new System.Windows.Forms.Button();
            this.btnBackToMenu = new System.Windows.Forms.Button();
            this.btnViewSubmittedIdeas = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.ForeColor = System.Drawing.Color.DarkBlue;
            this.lblTitle.Location = new System.Drawing.Point(12, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(398, 26);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Submit Municipal Improvement Ideas";

            this.lblIdeaTitle.AutoSize = true;
            this.lblIdeaTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblIdeaTitle.Location = new System.Drawing.Point(14, 50);
            this.lblIdeaTitle.Name = "lblIdeaTitle";
            this.lblIdeaTitle.Size = new System.Drawing.Size(74, 15);
            this.lblIdeaTitle.TabIndex = 1;
            this.lblIdeaTitle.Text = "Idea Title:";

            this.txtTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTitle.Location = new System.Drawing.Point(17, 68);
            this.txtTitle.Name = "txtTitle";
            this.txtTitle.Size = new System.Drawing.Size(400, 21);
            this.txtTitle.TabIndex = 2;
            this.txtTitle.TextChanged += new System.EventHandler(this.txtTitle_TextChanged);

            this.lblCategory.AutoSize = true;
            this.lblCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCategory.Location = new System.Drawing.Point(14, 100);
            this.lblCategory.Name = "lblCategory";
            this.lblCategory.Size = new System.Drawing.Size(69, 15);
            this.lblCategory.TabIndex = 3;
            this.lblCategory.Text = "Category:";

            this.cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCategory.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbCategory.FormattingEnabled = true;
            this.cmbCategory.Location = new System.Drawing.Point(17, 118);
            this.cmbCategory.Name = "cmbCategory";
            this.cmbCategory.Size = new System.Drawing.Size(200, 23);
            this.cmbCategory.TabIndex = 4;
            this.cmbCategory.SelectedIndexChanged += new System.EventHandler(this.cmbCategory_SelectedIndexChanged);

            this.lblDescription.AutoSize = true;
            this.lblDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDescription.Location = new System.Drawing.Point(14, 150);
            this.lblDescription.Name = "lblDescription";
            this.lblDescription.Size = new System.Drawing.Size(83, 15);
            this.lblDescription.TabIndex = 5;
            this.lblDescription.Text = "Description:";

            this.rtbDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbDescription.Location = new System.Drawing.Point(17, 168);
            this.rtbDescription.Name = "rtbDescription";
            this.rtbDescription.Size = new System.Drawing.Size(400, 80);
            this.rtbDescription.TabIndex = 6;
            this.rtbDescription.Text = "";
            this.rtbDescription.TextChanged += new System.EventHandler(this.rtbDescription_TextChanged);

            this.lblSubmitterName.AutoSize = true;
            this.lblSubmitterName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubmitterName.Location = new System.Drawing.Point(14, 260);
            this.lblSubmitterName.Name = "lblSubmitterName";
            this.lblSubmitterName.Size = new System.Drawing.Size(83, 15);
            this.lblSubmitterName.TabIndex = 7;
            this.lblSubmitterName.Text = "Your Name:";

            this.txtSubmitterName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSubmitterName.Location = new System.Drawing.Point(17, 278);
            this.txtSubmitterName.Name = "txtSubmitterName";
            this.txtSubmitterName.Size = new System.Drawing.Size(200, 21);
            this.txtSubmitterName.TabIndex = 8;
            this.txtSubmitterName.TextChanged += new System.EventHandler(this.txtSubmitterName_TextChanged);

            this.lblSubmitterEmail.AutoSize = true;
            this.lblSubmitterEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSubmitterEmail.Location = new System.Drawing.Point(230, 260);
            this.lblSubmitterEmail.Name = "lblSubmitterEmail";
            this.lblSubmitterEmail.Size = new System.Drawing.Size(95, 15);
            this.lblSubmitterEmail.TabIndex = 9;
            this.lblSubmitterEmail.Text = "Email Address:";
            // 
            // txtSubmitterEmail
            // 
            this.txtSubmitterEmail.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSubmitterEmail.Location = new System.Drawing.Point(233, 278);
            this.txtSubmitterEmail.Name = "txtSubmitterEmail";
            this.txtSubmitterEmail.Size = new System.Drawing.Size(184, 21);
            this.txtSubmitterEmail.TabIndex = 10;
            this.txtSubmitterEmail.TextChanged += new System.EventHandler(this.txtSubmitterEmail_TextChanged);
            // 
            // lblExpectedBenefit
            // 
            this.lblExpectedBenefit.AutoSize = true;
            this.lblExpectedBenefit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblExpectedBenefit.Location = new System.Drawing.Point(14, 310);
            this.lblExpectedBenefit.Name = "lblExpectedBenefit";
            this.lblExpectedBenefit.Size = new System.Drawing.Size(116, 15);
            this.lblExpectedBenefit.TabIndex = 11;
            this.lblExpectedBenefit.Text = "Expected Benefit:";
            // 
            // rtbExpectedBenefit
            this.rtbExpectedBenefit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbExpectedBenefit.Location = new System.Drawing.Point(17, 328);
            this.rtbExpectedBenefit.Name = "rtbExpectedBenefit";
            this.rtbExpectedBenefit.Size = new System.Drawing.Size(400, 60);
            this.rtbExpectedBenefit.TabIndex = 12;
            this.rtbExpectedBenefit.Text = "";
            this.rtbExpectedBenefit.TextChanged += new System.EventHandler(this.rtbExpectedBenefit_TextChanged);

            this.lblPriority.AutoSize = true;
            this.lblPriority.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPriority.Location = new System.Drawing.Point(14, 400);
            this.lblPriority.Name = "lblPriority";
            this.lblPriority.Size = new System.Drawing.Size(56, 15);
            this.lblPriority.TabIndex = 13;
            this.lblPriority.Text = "Priority:";

            this.cmbPriority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPriority.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbPriority.FormattingEnabled = true;
            this.cmbPriority.Location = new System.Drawing.Point(17, 418);
            this.cmbPriority.Name = "cmbPriority";
            this.cmbPriority.Size = new System.Drawing.Size(120, 23);
            this.cmbPriority.TabIndex = 14;
            this.cmbPriority.SelectedIndexChanged += new System.EventHandler(this.cmbPriority_SelectedIndexChanged);

            this.lblEstimatedCost.AutoSize = true;
            this.lblEstimatedCost.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEstimatedCost.Location = new System.Drawing.Point(150, 400);
            this.lblEstimatedCost.Name = "lblEstimatedCost";
            this.lblEstimatedCost.Size = new System.Drawing.Size(105, 15);
            this.lblEstimatedCost.TabIndex = 15;
            this.lblEstimatedCost.Text = "Estimated Cost:";

            this.cmbEstimatedCost.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEstimatedCost.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbEstimatedCost.FormattingEnabled = true;
            this.cmbEstimatedCost.Location = new System.Drawing.Point(153, 418);
            this.cmbEstimatedCost.Name = "cmbEstimatedCost";
            this.cmbEstimatedCost.Size = new System.Drawing.Size(150, 23);
            this.cmbEstimatedCost.TabIndex = 16;

            this.lblAttachedFiles.AutoSize = true;
            this.lblAttachedFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAttachedFiles.Location = new System.Drawing.Point(14, 450);
            this.lblAttachedFiles.Name = "lblAttachedFiles";
            this.lblAttachedFiles.Size = new System.Drawing.Size(98, 15);
            this.lblAttachedFiles.TabIndex = 17;
            this.lblAttachedFiles.Text = "Attached Files:";

            this.lstAttachedFiles.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lstAttachedFiles.FormattingEnabled = true;
            this.lstAttachedFiles.Location = new System.Drawing.Point(17, 468);
            this.lstAttachedFiles.Name = "lstAttachedFiles";
            this.lstAttachedFiles.Size = new System.Drawing.Size(300, 69);
            this.lstAttachedFiles.TabIndex = 18;

            this.btnAttachFile.BackColor = System.Drawing.Color.LightGray;
            this.btnAttachFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAttachFile.ForeColor = System.Drawing.Color.Black;
            this.btnAttachFile.Location = new System.Drawing.Point(330, 468);
            this.btnAttachFile.Name = "btnAttachFile";
            this.btnAttachFile.Size = new System.Drawing.Size(87, 30);
            this.btnAttachFile.TabIndex = 19;
            this.btnAttachFile.Text = "Attach File";
            this.btnAttachFile.UseVisualStyleBackColor = false;
            this.btnAttachFile.Click += new System.EventHandler(this.btnAttachFile_Click);

            this.btnRemoveFile.BackColor = System.Drawing.Color.DarkGray;
            this.btnRemoveFile.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRemoveFile.ForeColor = System.Drawing.Color.White;
            this.btnRemoveFile.Location = new System.Drawing.Point(330, 507);
            this.btnRemoveFile.Name = "btnRemoveFile";
            this.btnRemoveFile.Size = new System.Drawing.Size(87, 30);
            this.btnRemoveFile.TabIndex = 20;
            this.btnRemoveFile.Text = "Remove File";
            this.btnRemoveFile.UseVisualStyleBackColor = false;
            this.btnRemoveFile.Click += new System.EventHandler(this.btnRemoveFile_Click);

            this.lblEngagement.AutoSize = true;
            this.lblEngagement.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEngagement.ForeColor = System.Drawing.Color.Gray;
            this.lblEngagement.Location = new System.Drawing.Point(14, 550);
            this.lblEngagement.Name = "lblEngagement";
            this.lblEngagement.Size = new System.Drawing.Size(291, 15);
            this.lblEngagement.TabIndex = 21;
            this.lblEngagement.Text = "💡 Share your brilliant idea to improve our community!";
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(17, 575);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(400, 15);
            this.progressBar.TabIndex = 22;
            // 
            // btnSubmit
            // 
            this.btnSubmit.BackColor = System.Drawing.Color.Black;
            this.btnSubmit.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSubmit.ForeColor = System.Drawing.Color.White;
            this.btnSubmit.Location = new System.Drawing.Point(17, 605);
            this.btnSubmit.Name = "btnSubmit";
            this.btnSubmit.Size = new System.Drawing.Size(120, 35);
            this.btnSubmit.TabIndex = 23;
            this.btnSubmit.Text = "Submit Idea";
            this.btnSubmit.UseVisualStyleBackColor = false;
            this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);

            this.btnBackToMenu.BackColor = System.Drawing.Color.Gray;
            this.btnBackToMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBackToMenu.ForeColor = System.Drawing.Color.White;
            this.btnBackToMenu.Location = new System.Drawing.Point(297, 605);
            this.btnBackToMenu.Name = "btnBackToMenu";
            this.btnBackToMenu.Size = new System.Drawing.Size(120, 35);
            this.btnBackToMenu.TabIndex = 24;
            this.btnBackToMenu.Text = "Back to Menu";
            this.btnBackToMenu.UseVisualStyleBackColor = false;
            this.btnBackToMenu.Click += new System.EventHandler(this.btnBackToMenu_Click);

            this.btnViewSubmittedIdeas.BackColor = System.Drawing.Color.LightGray;
            this.btnViewSubmittedIdeas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnViewSubmittedIdeas.ForeColor = System.Drawing.Color.Black;
            this.btnViewSubmittedIdeas.Location = new System.Drawing.Point(150, 605);
            this.btnViewSubmittedIdeas.Name = "btnViewSubmittedIdeas";
            this.btnViewSubmittedIdeas.Size = new System.Drawing.Size(130, 35);
            this.btnViewSubmittedIdeas.TabIndex = 25;
            this.btnViewSubmittedIdeas.Text = "View Submitted Ideas";
            this.btnViewSubmittedIdeas.UseVisualStyleBackColor = false;
            this.btnViewSubmittedIdeas.Click += new System.EventHandler(this.btnViewSubmittedIdeas_Click);

            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(434, 655);
            this.MaximizeBox = true;
            this.MinimumSize = new System.Drawing.Size(434, 655);
            this.Controls.Add(this.btnViewSubmittedIdeas);
            this.Controls.Add(this.btnBackToMenu);
            this.Controls.Add(this.btnSubmit);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.lblEngagement);
            this.Controls.Add(this.btnRemoveFile);
            this.Controls.Add(this.btnAttachFile);
            this.Controls.Add(this.lstAttachedFiles);
            this.Controls.Add(this.lblAttachedFiles);
            this.Controls.Add(this.cmbEstimatedCost);
            this.Controls.Add(this.lblEstimatedCost);
            this.Controls.Add(this.cmbPriority);
            this.Controls.Add(this.lblPriority);
            this.Controls.Add(this.rtbExpectedBenefit);
            this.Controls.Add(this.lblExpectedBenefit);
            this.Controls.Add(this.txtSubmitterEmail);
            this.Controls.Add(this.lblSubmitterEmail);
            this.Controls.Add(this.txtSubmitterName);
            this.Controls.Add(this.lblSubmitterName);
            this.Controls.Add(this.rtbDescription);
            this.Controls.Add(this.lblDescription);
            this.Controls.Add(this.cmbCategory);
            this.Controls.Add(this.lblCategory);
            this.Controls.Add(this.txtTitle);
            this.Controls.Add(this.lblIdeaTitle);
            this.Controls.Add(this.lblTitle);
            this.Name = "ImprovementIdeasForm";
            this.Text = "Municipal Improvement Ideas";
            this.Load += new System.EventHandler(this.ImprovementIdeasForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblIdeaTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.RichTextBox rtbDescription;
        private System.Windows.Forms.Label lblSubmitterName;
        private System.Windows.Forms.TextBox txtSubmitterName;
        private System.Windows.Forms.Label lblSubmitterEmail;
        private System.Windows.Forms.TextBox txtSubmitterEmail;
        private System.Windows.Forms.Label lblExpectedBenefit;
        private System.Windows.Forms.RichTextBox rtbExpectedBenefit;
        private System.Windows.Forms.Label lblPriority;
        private System.Windows.Forms.ComboBox cmbPriority;
        private System.Windows.Forms.Label lblEstimatedCost;
        private System.Windows.Forms.ComboBox cmbEstimatedCost;
        private System.Windows.Forms.Label lblAttachedFiles;
        private System.Windows.Forms.ListBox lstAttachedFiles;
        private System.Windows.Forms.Button btnAttachFile;
        private System.Windows.Forms.Button btnRemoveFile;
        private System.Windows.Forms.Label lblEngagement;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnBackToMenu;
        private System.Windows.Forms.Button btnViewSubmittedIdeas;
    }
}