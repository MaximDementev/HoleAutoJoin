using Autodesk.Revit.DB;
using HoleAutoJoin.Services;
using HoleAutoJoin.Core;
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
            _autoJoinService = autoJoinService; // Can be null
            _manualJoinService = manualJoinService ?? throw new ArgumentNullException(nameof(manualJoinService));

            InitializeComponent();
            LoadSettingsToForm();
        }
        #endregion

        #region Private Methods
        private void LoadSettingsToForm()
        {
            var settings = SettingsManager.Instance.Settings;
            chkAutoJoin.Checked = settings.IsAutoJoinEnabled;
            chkShowNotifications.Checked = settings.ShowNotifications;
        }

        private void SaveSettingsFromForm()
        {
            var settings = SettingsManager.Instance.Settings;
            settings.IsAutoJoinEnabled = chkAutoJoin.Checked;
            settings.ShowNotifications = chkShowNotifications.Checked;
            SettingsManager.Instance.SaveSettings();
        }
        #endregion

        #region Event Handlers
        private void chkAutoJoin_CheckedChanged(object sender, EventArgs e)
        {
            if (_autoJoinService != null)
            {
                _autoJoinService.IsEnabled = chkAutoJoin.Checked;
            }
            SaveSettingsFromForm();
        }

        private void chkShowNotifications_CheckedChanged(object sender, EventArgs e)
        {
            SaveSettingsFromForm();
        }

        private void numTolerance_ValueChanged(object sender, EventArgs e)
        {
            SaveSettingsFromForm();
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

        private void btnResetDefaults_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Сбросить все настройки к значениям по умолчанию?",
                               "Подтверждение",
                               MessageBoxButtons.YesNo,
                               MessageBoxIcon.Question) == DialogResult.Yes)
            {
                SettingsManager.Instance.ResetToDefaults();
                LoadSettingsToForm();
                if (_autoJoinService != null)
                {
                    _autoJoinService.IsEnabled = SettingsManager.Instance.Settings.IsAutoJoinEnabled;
                }
            }
        }
        #endregion

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.chkAutoJoin = new System.Windows.Forms.CheckBox();
            this.btnManualJoin = new System.Windows.Forms.Button();
            this.btnClose = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.chkShowNotifications = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // chkAutoJoin
            // 
            this.chkAutoJoin.Location = new System.Drawing.Point(12, 50);
            this.chkAutoJoin.Name = "chkAutoJoin";
            this.chkAutoJoin.Size = new System.Drawing.Size(280, 35);
            this.chkAutoJoin.TabIndex = 0;
            this.chkAutoJoin.Text = "Автоматически соединять отверстия с плитами (настройка сохраняется при перезапуск" +
    "е Revit)";
            this.chkAutoJoin.UseVisualStyleBackColor = true;
            this.chkAutoJoin.CheckedChanged += new System.EventHandler(this.chkAutoJoin_CheckedChanged);
            // 
            // btnManualJoin
            // 
            this.btnManualJoin.Location = new System.Drawing.Point(12, 135);
            this.btnManualJoin.Name = "btnManualJoin";
            this.btnManualJoin.Size = new System.Drawing.Size(120, 23);
            this.btnManualJoin.TabIndex = 1;
            this.btnManualJoin.Text = "Соединить сейчас";
            this.btnManualJoin.UseVisualStyleBackColor = true;
            this.btnManualJoin.Click += new System.EventHandler(this.btnManualJoin_Click);
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(228, 135);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(64, 23);
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
            this.label1.Size = new System.Drawing.Size(237, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "Настройки соединения отверстий";
            // 
            // chkShowNotifications
            // 
            this.chkShowNotifications.AutoSize = true;
            this.chkShowNotifications.Location = new System.Drawing.Point(12, 100);
            this.chkShowNotifications.Name = "chkShowNotifications";
            this.chkShowNotifications.Size = new System.Drawing.Size(159, 17);
            this.chkShowNotifications.TabIndex = 4;
            this.chkShowNotifications.Text = "Показывать уведомления";
            this.chkShowNotifications.UseVisualStyleBackColor = true;
            this.chkShowNotifications.CheckedChanged += new System.EventHandler(this.chkShowNotifications_CheckedChanged);
            // 
            // HoleJoinSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(304, 171);
            this.Controls.Add(this.chkShowNotifications);
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
        private System.Windows.Forms.CheckBox chkShowNotifications;
        #endregion
    }
}
