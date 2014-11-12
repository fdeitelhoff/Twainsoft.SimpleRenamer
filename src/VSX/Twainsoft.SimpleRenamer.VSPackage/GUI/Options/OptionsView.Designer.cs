namespace Twainsoft.SimpleRenamer.VSPackage.GUI.Options
{
    partial class OptionsView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.changeAssemblyInfo = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.changeProjectProperties = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.changeProjectReferences = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.changeProjectReferences);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.changeAssemblyInfo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.changeProjectProperties);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(427, 293);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Renaming Options";
            // 
            // changeAssemblyInfo
            // 
            this.changeAssemblyInfo.AutoSize = true;
            this.changeAssemblyInfo.Location = new System.Drawing.Point(9, 122);
            this.changeAssemblyInfo.Name = "changeAssemblyInfo";
            this.changeAssemblyInfo.Size = new System.Drawing.Size(160, 17);
            this.changeAssemblyInfo.TabIndex = 3;
            this.changeAssemblyInfo.Text = "Change AssemblyInfo Data?";
            this.changeAssemblyInfo.UseVisualStyleBackColor = true;
            this.changeAssemblyInfo.CheckedChanged += new System.EventHandler(this.changeAssemblyInfo_CheckedChanged);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(6, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(421, 32);
            this.label2.TabIndex = 2;
            this.label2.Text = "Change the AssemblyInfo.cs file (AssemblyTitle and AssemblyProduct) after the pro" +
    "ject was renamed?";
            // 
            // changeProjectProperties
            // 
            this.changeProjectProperties.AutoSize = true;
            this.changeProjectProperties.Location = new System.Drawing.Point(9, 51);
            this.changeProjectProperties.Name = "changeProjectProperties";
            this.changeProjectProperties.Size = new System.Drawing.Size(155, 17);
            this.changeProjectProperties.TabIndex = 1;
            this.changeProjectProperties.Text = "Change Project Properties?";
            this.changeProjectProperties.UseVisualStyleBackColor = true;
            this.changeProjectProperties.CheckedChanged += new System.EventHandler(this.changeProjectProperties_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(6, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(421, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Change the Project Properties (DefaultNamespace and AssemblyName) after the proje" +
    "ct was renamed?";
            // 
            // changeProjectReferences
            // 
            this.changeProjectReferences.AutoSize = true;
            this.changeProjectReferences.Location = new System.Drawing.Point(9, 181);
            this.changeProjectReferences.Name = "changeProjectReferences";
            this.changeProjectReferences.Size = new System.Drawing.Size(163, 17);
            this.changeProjectReferences.TabIndex = 5;
            this.changeProjectReferences.Text = "Change Project References?";
            this.changeProjectReferences.UseVisualStyleBackColor = true;
            this.changeProjectReferences.CheckedChanged += new System.EventHandler(this.changeProjectReferences_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.Location = new System.Drawing.Point(6, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(421, 18);
            this.label3.TabIndex = 4;
            this.label3.Text = "Change references from other projects to the renamed one?";
            // 
            // OptionsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "OptionsView";
            this.Size = new System.Drawing.Size(433, 299);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox changeProjectProperties;
        private System.Windows.Forms.CheckBox changeAssemblyInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox changeProjectReferences;
        private System.Windows.Forms.Label label3;
    }
}
