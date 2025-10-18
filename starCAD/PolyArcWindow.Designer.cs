namespace sCAD
{
    partial class PolyArcWindow
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
            this.SizeBox1 = new System.Windows.Forms.RichTextBox();
            this.SureButton = new System.Windows.Forms.Button();
            this.ShowSizeBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SizeBox1
            // 
            this.SizeBox1.Location = new System.Drawing.Point(13, 13);
            this.SizeBox1.Name = "SizeBox1";
            this.SizeBox1.Size = new System.Drawing.Size(219, 377);
            this.SizeBox1.TabIndex = 0;
            this.SizeBox1.Text = "";
            // 
            // SureButton
            // 
            this.SureButton.Location = new System.Drawing.Point(13, 399);
            this.SureButton.Name = "SureButton";
            this.SureButton.Size = new System.Drawing.Size(75, 30);
            this.SureButton.TabIndex = 1;
            this.SureButton.Text = "确定";
            this.SureButton.UseVisualStyleBackColor = true;
            this.SureButton.Click += new System.EventHandler(this.SureButton_Click);
            // 
            // ShowSizeBox1
            // 
            this.ShowSizeBox1.Location = new System.Drawing.Point(154, 400);
            this.ShowSizeBox1.Name = "ShowSizeBox1";
            this.ShowSizeBox1.Size = new System.Drawing.Size(78, 29);
            this.ShowSizeBox1.TabIndex = 2;
            this.ShowSizeBox1.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(95, 408);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "当前半径";
            // 
            // PolyArcWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(244, 438);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ShowSizeBox1);
            this.Controls.Add(this.SureButton);
            this.Controls.Add(this.SizeBox1);
            this.Name = "PolyArcWindow";
            this.Text = "PolyArcWindow";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox SizeBox1;
        private System.Windows.Forms.Button SureButton;
        private System.Windows.Forms.RichTextBox ShowSizeBox1;
        private System.Windows.Forms.Label label1;
    }
}