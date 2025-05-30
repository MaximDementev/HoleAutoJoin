using Autodesk.Revit.DB;
using HoleAutoJoin.Services;
using System;
using System.Windows.Forms;

namespace HoleAutoJoin.UI
{
    public partial class HoleJoinSettingsForm : System.Windows.Forms.Form
    {
        #region Fields
        private readonly IAutoJoinService _autoJoinService;
        private readonly IManualJoinService _manualJoinService;
        #endregion

        #region Constructor
        public HoleJoinSettingsForm(IAutoJoinService autoJoinService, IManualJoinService manualJoinService)
        {
            _autoJoinService = autoJoinService ?? throw new ArgumentNullException(nameof(autoJoinService));
            _manualJoinService = manualJoinService ?? throw new ArgumentNullException(nameof(manualJoinService));

            InitializeComponent();
            chkAutoJoin.Checked = _autoJoinService.IsEnabled;
        }
        #endregion

        #region Event Handlers
        private void chkAutoJoin_CheckedChanged(object sender, EventArgs e)
        {
            _autoJoinService.IsEnabled = chkAutoJoin.Checked;
        }

        private void btnManualJoin_Click(object sender, EventArgs e)
        {
            _manualJoinService.Raise();
            Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.chkAutoJoin = new System.Windows.Forms.CheckBox();
            this.btnManualJoin = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // chkAutoJoin
            // 
            this.chkAutoJoin.AutoSize = true;
            this.chkAutoJoin.Location = new System.Drawing.Point(12, 50);
            this.chkAutoJoin.Name = "chkAutoJoin";
            this.chkAutoJoin.Size = new System.Drawing.Size(280, 17);
            this.chkAutoJoin.TabIndex = 0;
            this.chkAutoJoin.Text = "Автоматически соединять отверстия с плитами";
            this.chkAutoJoin.UseVisualStyleBackColor = true;
            this.chkAutoJoin.CheckedChanged += new System.EventHandler(this.chkAutoJoin_CheckedChanged);
            // 
            // btnManualJoin
            // 
            this.btnManualJoin.Location = new System.Drawing.Point(12, 85);
            this.btnManualJoin.Name = "btnManualJoin";
            this.btnManualJoin.Size = new System.Drawing.Size(180, 23);
            this.btnManualJoin.TabIndex = 1;
            this.btnManualJoin.Text = "Соединить сейчас";
            this.btnManualJoin.UseVisualStyleBackColor = true;
            this.btnManualJoin.Click += new System.EventHandler(this.btnManualJoin_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(198, 85);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(94, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(236, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Настройки соединения отверстий";
            // 
            // HoleJoinSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 121);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnManualJoin);
            this.Controls.Add(this.chkAutoJoin);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HoleJoinSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Соединение отверстий";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        #region Windows Form Designer generated variables
        private System.Windows.Forms.CheckBox chkAutoJoin;
        private System.Windows.Forms.Button btnManualJoin;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label label1;
        #endregion
    }
}
