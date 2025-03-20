
partial class OnOff
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
        this.checkBox = new System.Windows.Forms.CheckBox();
        this.button1 = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // checkBox
        // 
        this.checkBox.AutoSize = true;
        this.checkBox.Location = new System.Drawing.Point(12, 12);
        this.checkBox.Name = "checkBox";
        this.checkBox.Size = new System.Drawing.Size(283, 17);
        this.checkBox.TabIndex = 0;
        this.checkBox.Text = "Автоматическое соединение отверстий с плитами";
        this.checkBox.UseVisualStyleBackColor = true;
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(94, 42);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(140, 23);
        this.button1.TabIndex = 1;
        this.button1.Text = "Применить";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(this.Save);
        // 
        // OnOff
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(322, 77);
        this.Controls.Add(this.button1);
        this.Controls.Add(this.checkBox);
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "OnOff";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.Text = "OnOff";
        this.TopMost = true;
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.CheckBox checkBox;
    private System.Windows.Forms.Button button1;
}