using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Clinic_BD.Data;
using Clinic_BD.Forms.Patients;

namespace Clinic_BD.Forms.Auth
{
    public partial class PatientLoginForm : Form
    {
        private TextBox txtLogin;
        private TextBox txtPass;

        public PatientLoginForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Вхід для пацієнта";
            this.Size = new Size(350, 350);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            
            var lbl = new Label { 
                Text = "Вхід", 
                Dock = DockStyle.Top, Height = 80, 
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(0, 50, 0, 0)
            };

            txtLogin = new TextBox { PlaceholderText = "Email або Телефон", Width = 250, Location = new Point(45, 100) };
            txtPass = new TextBox { PlaceholderText = "Пароль", Width = 250, Location = new Point(45, 150), UseSystemPasswordChar = true };
            
            var btnEnter = new Button { 
                Text = "УВІЙТИ", 
                Location = new Point(45, 210), Size = new Size(250, 40),
                BackColor = Color.DodgerBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat 
            };
            btnEnter.Click += BtnEnter_Click;

            var lnkReg = new LinkLabel { 
                Text = "Ще не зареєстровані? Створити акаунт", 
                Location = new Point(45, 270), Size = new Size(250, 30), TextAlign = ContentAlignment.MiddleCenter
            };
            lnkReg.LinkClicked += (s, e) => { new RegistrationForm().Show(); this.Hide(); };

            var btnBack = new Button { Text = "← Назад", Location = new Point(10, 10), Size = new Size(75, 30), FlatStyle = FlatStyle.Flat };
            btnBack.Click += (s, e) => {
                foreach (Form f in Application.OpenForms) { if (f is LoginForm) { f.Show(); break; } }
                this.Close();
            };

            this.Controls.Add(btnBack);
            this.Controls.Add(lbl);
            this.Controls.AddRange(new Control[] { txtLogin, txtPass, btnEnter, lnkReg });
        }

        private void BtnEnter_Click(object sender, EventArgs e)
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var patient = db.Patients.FirstOrDefault(p => 
                        (p.Email == txtLogin.Text || p.Phone == txtLogin.Text) && 
                        p.Password == txtPass.Text);

                    if (patient != null)
                    {
                        new PatientMainForm(patient).Show(); 
                        this.Hide();
                    }
                    else { MessageBox.Show("Невірний логін або пароль!"); }
                }
            }
            catch (Exception ex) { MessageBox.Show($"Помилка: {ex.Message}"); }
        }
    }
}