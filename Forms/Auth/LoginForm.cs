using System;
using System.Drawing;
using System.Windows.Forms;
using Clinic_BD.Forms.Admin; 

namespace Clinic_BD.Forms.Auth
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Медична система - Вибір";
            this.Size = new Size(400, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            var title = new Label { 
                Text = "Оберіть спосіб входу", 
                Dock = DockStyle.Top, Height = 80, 
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };

            var btnPatient = new Button { 
                Text = "Увійти як ПАЦІЄНТ", 
                Size = new Size(250, 50), Location = new Point(75, 120),
                BackColor = Color.LightBlue, FlatStyle = FlatStyle.Flat 
            };
            btnPatient.Click += (s, e) => {
                new PatientLoginForm().Show(); 
                this.Hide();
            };

            var btnDoctor = new Button { 
                Text = "Увійти як ЛІКАР (Адмін)", 
                Size = new Size(250, 50), Location = new Point(75, 200),
                BackColor = Color.LightGreen, FlatStyle = FlatStyle.Flat 
            };
            btnDoctor.Click += (s, e) => {
                new AdminLoginForm().Show(); 
                this.Hide();
            };
            
            this.Controls.AddRange(new Control[] { title, btnPatient, btnDoctor });
        }
    }
}