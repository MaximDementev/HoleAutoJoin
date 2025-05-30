using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace HoleAutoJoin.UI
{
    public partial class HelpForm : Form
    {
        #region Fields
        private readonly List<HelpLink> _helpLinks;
        #endregion

        #region Constructor
        public HelpForm()
        {
            _helpLinks = InitializeHelpLinks();
            InitializeComponent();
            SetupLinks();
        }
        #endregion

        #region Private Methods
        private List<HelpLink> InitializeHelpLinks()
        {
            return new List<HelpLink>
            {
                new HelpLink("Revit: Получение задания на отверстия", "https://bz.krgp.ru/kb/revit-poluchenie-zadaniya-na-otverstiya/"),
                new HelpLink("Citrus: Отверстия", "https://bz.krgp.ru/kb/citrus-otverstiya/"),
                new HelpLink("Revit: Поиск пустых отверстий", "https://bz.krgp.ru/kb/revit-poisk-pustyh-otverstij/")
            };
        }

        private void SetupLinks()
        {
            int yPosition = 60;
            const int linkHeight = 30;
            const int margin = 10;

            foreach (var helpLink in _helpLinks)
            {
                var linkLabel = new LinkLabel
                {
                    Text = helpLink.Title,
                    Location = new Point(20, yPosition),
                    Size = new Size(this.Width - 60, linkHeight),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular),
                    LinkColor = Color.Blue,
                    Tag = helpLink.Url
                };

                linkLabel.LinkClicked += LinkLabel_LinkClicked;
                this.Controls.Add(linkLabel);

                yPosition += linkHeight + margin;
            }

            // Обновляем размер формы под количество ссылок
            this.Height = yPosition + 80;
        }

        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (sender is LinkLabel linkLabel && linkLabel.Tag is string url)
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось открыть ссылку: {ex.Message}", "Ошибка",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Windows Form Designer generated code
        private void InitializeComponent()
        {
            this.lblTitle = new Label();
            this.btnClose = new Button();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point, ((byte)(204)));
            this.lblTitle.Location = new Point(20, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(200, 20);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Справка по отверстиям";

            // btnClose
            this.btnClose.Anchor = ((AnchorStyles)((AnchorStyles.Bottom | AnchorStyles.Right)));
            this.btnClose.Location = new Point(297, 200);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Закрыть";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new EventHandler(this.btnClose_Click);

            // HelpForm
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(400, 240);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HelpForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Справка по отверстиям";
            this.ResumeLayout(false);
            this.PerformLayout();
        }
        #endregion

        #region Windows Form Designer generated variables
        private Label lblTitle;
        private Button btnClose;
        #endregion

        #region Helper Classes
        private class HelpLink
        {
            public string Title { get; }
            public string Url { get; }

            public HelpLink(string title, string url)
            {
                Title = title ?? throw new ArgumentNullException(nameof(title));
                Url = url ?? throw new ArgumentNullException(nameof(url));
            }
        }
        #endregion
    }
}
