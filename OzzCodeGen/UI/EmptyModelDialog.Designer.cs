namespace OzzCodeGen.UI
{
    partial class EmptyModelDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnAccept = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.txtNamespace = new System.Windows.Forms.TextBox();
            this.lblNamespace = new System.Windows.Forms.Label();
            this.chkCreateDefaultEntity = new System.Windows.Forms.CheckBox();
            this.txtDefaultEntityName = new System.Windows.Forms.TextBox();
            this.lblDefaultEntityName = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnAccept
            // 
            this.btnAccept.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAccept.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAccept.Location = new System.Drawing.Point(180, 273);
            this.btnAccept.Name = "btnAccept";
            this.btnAccept.Size = new System.Drawing.Size(88, 28);
            this.btnAccept.TabIndex = 11;
            this.btnAccept.Text = "OK";
            this.btnAccept.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.CausesValidation = false;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(281, 273);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 28);
            this.btnCancel.TabIndex = 12;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Location = new System.Drawing.Point(12, 24);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(74, 13);
            this.lblProjectName.TabIndex = 2;
            this.lblProjectName.Text = "Project Name:";
            // 
            // txtProjectName
            // 
            this.txtProjectName.Location = new System.Drawing.Point(122, 21);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(247, 20);
            this.txtProjectName.TabIndex = 0;
            this.txtProjectName.Text = "Untitled";
            this.txtProjectName.Leave += new System.EventHandler(this.txtProjectName_Leave);
            // 
            // txtNamespace
            // 
            this.txtNamespace.Location = new System.Drawing.Point(122, 47);
            this.txtNamespace.Name = "txtNamespace";
            this.txtNamespace.Size = new System.Drawing.Size(247, 20);
            this.txtNamespace.TabIndex = 1;
            // 
            // lblNamespace
            // 
            this.lblNamespace.AutoSize = true;
            this.lblNamespace.Location = new System.Drawing.Point(12, 50);
            this.lblNamespace.Name = "lblNamespace";
            this.lblNamespace.Size = new System.Drawing.Size(103, 13);
            this.lblNamespace.TabIndex = 4;
            this.lblNamespace.Text = "Project Namespace:";
            // 
            // chkCreateDefaultEntity
            // 
            this.chkCreateDefaultEntity.AutoSize = true;
            this.chkCreateDefaultEntity.Location = new System.Drawing.Point(122, 74);
            this.chkCreateDefaultEntity.Name = "chkCreateDefaultEntity";
            this.chkCreateDefaultEntity.Size = new System.Drawing.Size(123, 17);
            this.chkCreateDefaultEntity.TabIndex = 2;
            this.chkCreateDefaultEntity.Text = "Create Default Entity";
            this.chkCreateDefaultEntity.UseVisualStyleBackColor = true;
            this.chkCreateDefaultEntity.CheckedChanged += new System.EventHandler(this.chkCreateDefaultEntity_CheckedChanged);
            // 
            // txtDefaultEntityName
            // 
            this.txtDefaultEntityName.Enabled = false;
            this.txtDefaultEntityName.Location = new System.Drawing.Point(122, 97);
            this.txtDefaultEntityName.Name = "txtDefaultEntityName";
            this.txtDefaultEntityName.Size = new System.Drawing.Size(247, 20);
            this.txtDefaultEntityName.TabIndex = 3;
            this.txtDefaultEntityName.Text = "AbstractEntity";
            // 
            // lblDefaultEntityName
            // 
            this.lblDefaultEntityName.AutoSize = true;
            this.lblDefaultEntityName.Location = new System.Drawing.Point(12, 100);
            this.lblDefaultEntityName.Name = "lblDefaultEntityName";
            this.lblDefaultEntityName.Size = new System.Drawing.Size(104, 13);
            this.lblDefaultEntityName.TabIndex = 15;
            this.lblDefaultEntityName.Text = "Default Entity Name:";
            // 
            // EmptyModelDialog
            // 
            this.AcceptButton = this.btnAccept;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(381, 313);
            this.Controls.Add(this.txtDefaultEntityName);
            this.Controls.Add(this.lblDefaultEntityName);
            this.Controls.Add(this.chkCreateDefaultEntity);
            this.Controls.Add(this.txtNamespace);
            this.Controls.Add(this.lblNamespace);
            this.Controls.Add(this.txtProjectName);
            this.Controls.Add(this.lblProjectName);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnAccept);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EmptyModelDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Empty Model";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnAccept;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.TextBox txtNamespace;
        private System.Windows.Forms.Label lblNamespace;
        private System.Windows.Forms.CheckBox chkCreateDefaultEntity;
        private System.Windows.Forms.TextBox txtDefaultEntityName;
        private System.Windows.Forms.Label lblDefaultEntityName;
    }
}