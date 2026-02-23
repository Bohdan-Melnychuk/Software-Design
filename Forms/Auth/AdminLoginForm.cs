using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Clinic_BD.Data;
using Clinic_BD.Forms.Admin; 

namespace Clinic_BD.Forms.Auth
{
    public partial class AdminLoginForm : Form
    {
        private TextBox txtLogin;
        private TextBox txtPass;

        public AdminLoginForm()
        {
            SetupUI();
        }

        private void SetupUI()
        {
            this.Text = "Вхід для Адміністратора";
            this.Size = new Size(350, 350);
            this.StartPosition = FormStartPosition.CenterScreen;

            var lbl = new Label { 
                Text = "Панель управління лікаря", 
                Dock = DockStyle.Top, Height = 80, 
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Padding = new Padding(0, 50, 0, 0)
            };

            txtLogin = new TextBox { PlaceholderText = "Email лікаря", Width = 250, Location = new Point(45, 110) };
            txtPass = new TextBox { PlaceholderText = "Пароль", Width = 250, Location = new Point(45, 150), UseSystemPasswordChar = true };
            
            var btnEnter = new Button { 
                Text = "Увійти в систему", 
                Location = new Point(45, 190), Size = new Size(250, 40), 
                BackColor = Color.PaleGreen, FlatStyle = FlatStyle.Flat 
            };
            btnEnter.Click += BtnEnter_Click;

            var btnBack = new Button { Text = "← Назад", Location = new Point(10, 10), Size = new Size(75, 30), FlatStyle = FlatStyle.Flat };
            btnBack.Click += (s, e) => {
                foreach (Form f in Application.OpenForms) { if (f is LoginForm) { f.Show(); break; } }
                this.Close();
            };

            this.Controls.Add(btnBack);
            this.Controls.AddRange(new Control[] { lbl, txtLogin, txtPass, btnEnter });
        }

        private void BtnEnter_Click(object sender, EventArgs e)
        {
            string loginInput = txtLogin.Text.Trim();
            string passwordInput = txtPass.Text.Trim();

            if (string.IsNullOrWhiteSpace(loginInput) || string.IsNullOrWhiteSpace(passwordInput))
            {
                MessageBox.Show("Заповніть усі поля!");
                return;
            }

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var doctor = db.Doctors.FirstOrDefault(d => 
                        d.Email == loginInput && d.Password == passwordInput);

                    if (doctor != null)
                    {
                        var adminForm = new AdminMainForm();
                        adminForm.Show();
                        this.Hide(); 
                    }
                    else
                    {
                        MessageBox.Show("Невірний логін або пароль адміністратора!", "Помилка доступу", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Критична помилка БД: {ex.Message}");
            }
        }
    }
}