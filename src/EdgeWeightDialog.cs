using System;
using System.Drawing;
using System.Windows.Forms;

namespace MSTVisualizer
{
    public class EdgeWeightDialog : Form
    {
        public int Weight { get; private set; }
        private NumericUpDown numWeight;

        public EdgeWeightDialog(string from, string to)
        {
            this.Text = "Set Edge Weight";
            this.Size = new Size(340, 200);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(40, 44, 52);

            var lblTitle = new Label
            {
                Text = $"Edge: {from}  →  {to}",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = false,
                Width = 300,
                Height = 30,
                Location = new Point(15, 15),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var lblWeight = new Label
            {
                Text = "Enter edge weight (cost):",
                ForeColor = Color.FromArgb(180, 190, 200),
                Font = new Font("Segoe UI", 9),
                AutoSize = false,
                Width = 300,
                Height = 25,
                Location = new Point(15, 55)
            };

            numWeight = new NumericUpDown
            {
                Location = new Point(15, 80),
                Width = 120,
                Height = 30,
                Minimum = 1,
                Maximum = 9999,
                Value = 1,
                Font = new Font("Segoe UI", 12),
                BackColor = Color.FromArgb(55, 60, 70),
                ForeColor = Color.White
            };

            var btnOk = new Button
            {
                Text = "Add Edge",
                Location = new Point(15, 120),
                Width = 120,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(34, 139, 34),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                DialogResult = DialogResult.OK,
                Cursor = Cursors.Hand
            };
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.Click += (s, e) =>
            {
                Weight = (int)numWeight.Value;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            var btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(150, 120),
                Width = 80,
                Height = 35,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(100, 50, 50),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9),
                DialogResult = DialogResult.Cancel,
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancel;
            this.Controls.AddRange(new Control[] { lblTitle, lblWeight, numWeight, btnOk, btnCancel });
        }
    }
}
