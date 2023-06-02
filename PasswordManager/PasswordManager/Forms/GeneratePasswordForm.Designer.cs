namespace PasswordManager.Forms
{
    partial class GeneratePasswordForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneratePasswordForm));
            this.btnGenerate = new System.Windows.Forms.Button();
            this.cbUpperCase = new System.Windows.Forms.CheckBox();
            this.cbLowerCase = new System.Windows.Forms.CheckBox();
            this.cbNumerals = new System.Windows.Forms.CheckBox();
            this.cbSymblos = new System.Windows.Forms.CheckBox();
            this.tbPassword = new System.Windows.Forms.TextBox();
            this.tbLength = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnGenerate
            // 
            this.btnGenerate.Location = new System.Drawing.Point(317, 58);
            this.btnGenerate.Name = "btnGenerate";
            this.btnGenerate.Size = new System.Drawing.Size(90, 23);
            this.btnGenerate.TabIndex = 0;
            this.btnGenerate.Text = "Генерировать";
            this.btnGenerate.UseVisualStyleBackColor = true;
            this.btnGenerate.Click += new System.EventHandler(this.btnGenerate_Click);
            // 
            // cbUpperCase
            // 
            this.cbUpperCase.AutoSize = true;
            this.cbUpperCase.Location = new System.Drawing.Point(12, 13);
            this.cbUpperCase.Name = "cbUpperCase";
            this.cbUpperCase.Size = new System.Drawing.Size(54, 17);
            this.cbUpperCase.TabIndex = 1;
            this.cbUpperCase.Text = "AAAA";
            this.cbUpperCase.UseVisualStyleBackColor = true;
            // 
            // cbLowerCase
            // 
            this.cbLowerCase.AutoSize = true;
            this.cbLowerCase.Location = new System.Drawing.Point(72, 13);
            this.cbLowerCase.Name = "cbLowerCase";
            this.cbLowerCase.Size = new System.Drawing.Size(50, 17);
            this.cbLowerCase.TabIndex = 2;
            this.cbLowerCase.Text = "aaaa";
            this.cbLowerCase.UseVisualStyleBackColor = true;
            // 
            // cbNumerals
            // 
            this.cbNumerals.AutoSize = true;
            this.cbNumerals.Location = new System.Drawing.Point(128, 13);
            this.cbNumerals.Name = "cbNumerals";
            this.cbNumerals.Size = new System.Drawing.Size(50, 17);
            this.cbNumerals.TabIndex = 3;
            this.cbNumerals.Text = "1234";
            this.cbNumerals.UseVisualStyleBackColor = true;
            // 
            // cbSymblos
            // 
            this.cbSymblos.AutoSize = true;
            this.cbSymblos.Location = new System.Drawing.Point(184, 13);
            this.cbSymblos.Name = "cbSymblos";
            this.cbSymblos.Size = new System.Drawing.Size(54, 17);
            this.cbSymblos.TabIndex = 4;
            this.cbSymblos.Text = "%$!@";
            this.cbSymblos.UseVisualStyleBackColor = true;
            // 
            // tbPassword
            // 
            this.tbPassword.Location = new System.Drawing.Point(72, 61);
            this.tbPassword.Name = "tbPassword";
            this.tbPassword.Size = new System.Drawing.Size(239, 20);
            this.tbPassword.TabIndex = 5;
            // 
            // tbLength
            // 
            this.tbLength.Location = new System.Drawing.Point(12, 61);
            this.tbLength.Name = "tbLength";
            this.tbLength.Size = new System.Drawing.Size(42, 20);
            this.tbLength.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 42);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Длина";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(69, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Пароль";
            // 
            // GeneratePasswordForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 102);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tbLength);
            this.Controls.Add(this.tbPassword);
            this.Controls.Add(this.cbSymblos);
            this.Controls.Add(this.cbNumerals);
            this.Controls.Add(this.cbLowerCase);
            this.Controls.Add(this.cbUpperCase);
            this.Controls.Add(this.btnGenerate);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "GeneratePasswordForm";
            this.Text = "Генерация пароля";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnGenerate;
        private System.Windows.Forms.CheckBox cbUpperCase;
        private System.Windows.Forms.CheckBox cbLowerCase;
        private System.Windows.Forms.CheckBox cbNumerals;
        private System.Windows.Forms.CheckBox cbSymblos;
        private System.Windows.Forms.TextBox tbPassword;
        private System.Windows.Forms.TextBox tbLength;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}