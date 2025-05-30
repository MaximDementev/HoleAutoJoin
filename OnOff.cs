using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Button;

public partial class OnOff : Form
{
    private AutoJoinOpening autoJoinOpening;

    public OnOff()
    {
        //if (autoJoinOpening == null) autoJoinOpening = new AutoJoinOpening();
        //checkBox.Checked = AutoJoinOpening.IsDelegateAdded;

        InitializeComponent();
    }


    private void Save(object sender, EventArgs e)
    {
        //autoJoinOpening.OnOrOff(checkBox.Checked);
        this.Hide();
    }
}
