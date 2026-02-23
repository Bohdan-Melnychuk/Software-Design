using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Clinic_BD.Data;
using Clinic_BD.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clinic_BD.Forms.Auth
{
    public partial class RegistrationForm : Form
    {
        private TextBox txtName, txtAddress, txtPhone, txtEmail, txtPassword;
        private DateTimePicker dtpBirthDate;
        private ComboBox cmbFamilyDoctor;
        private Label lblPhoneError, lblEmailError;
        private Button btnReg;
        private CheckBox chkShowPassword;
        private Panel passwordPanel;

        private readonly Regex _ukrainianPhoneRegex = new Regex(
            @"^(\+?380|0)(39|50|63|66|67|68|73|91|92|93|94|95|96|97|98|99)\d{7}$",
            RegexOptions.Compiled
        );

        private readonly Regex _emailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z.]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public RegistrationForm()
        {
            SetupUI();
            LoadFamilyDoctors();
        }

        private void SetupUI()
        {
            this.Text = "Реєстрація нового пацієнта";
            this.Size = new Size(450, 750);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.White;

            var lblTitle = new Label
            {
                Text = "📝 РЕЄСТРАЦІЯ ПАЦІЄНТА",
                Size = new Size(300, 25),
                Location = new Point(75, 40),
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.DarkSlateBlue
            };
            this.Controls.Add(lblTitle);

            var btnBack = new Button
            {
                Text = "← Назад",
                Location = new Point(10, 10),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.LightGray,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9)
            };
            btnBack.Click += (s, e) =>
            {
                Form loginForm = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f is PatientLoginForm);
                if (loginForm != null)
                {
                    loginForm.Show();
                }
                else
                {
                    new PatientLoginForm().Show();
                }
                this.Close();
            };
            this.Controls.Add(btnBack);

            int startY = 70;

            txtName = CreateField("ПІБ (Повне ім'я):*", ref startY);
            
            dtpBirthDate = CreateDatePicker("Дата народження:*", ref startY);
            dtpBirthDate.MaxDate = DateTime.Today;
            dtpBirthDate.MinDate = DateTime.Today.AddYears(-120);
            
            txtAddress = CreateField("Адреса проживання:", ref startY);
            
            CreatePhoneField(ref startY);
            
            CreateEmailField(ref startY);
            
            CreateFamilyDoctorField(ref startY);
            
            CreatePasswordField(ref startY);

            btnReg = new Button
            {
                Text = "✅ ЗАРЕЄСТРУВАТИСЯ",
                Size = new Size(300, 50),
                Location = new Point(75, startY + 20),
                BackColor = Color.FromArgb(40, 167, 69),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnReg.Click += BtnReg_Click;
            this.Controls.Add(btnReg);

            var lblRequired = new Label
            {
                Text = "* - обов'язкові поля",
                Location = new Point(75, startY + 75),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8, FontStyle.Italic)
            };
            this.Controls.Add(lblRequired);
        }

        private TextBox CreateField(string labelText, ref int y, bool isPass = false)
        {
            var lbl = new Label
            {
                Text = labelText,
                Location = new Point(50, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(lbl);

            var tb = new TextBox
            {
                Location = new Point(50, y + 20),
                Width = 330,
                UseSystemPasswordChar = isPass,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(tb);

            y += 65;
            return tb;
        }

        private void CreatePhoneField(ref int y)
        {
            var lbl = new Label
            {
                Text = "Номер телефону:*",
                Location = new Point(50, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(lbl);

            txtPhone = new TextBox
            {
                Location = new Point(50, y + 20),
                Width = 330,
                Font = new Font("Segoe UI", 10)
            };
            txtPhone.TextChanged += ValidatePhone;
            this.Controls.Add(txtPhone);

            lblPhoneError = new Label
            {
                Text = "Невірний формат (приклад: +380501234567 або 0501234567)",
                Location = new Point(50, y + 45),
                Width = 330,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8),
                Visible = false
            };
            this.Controls.Add(lblPhoneError);

            y += 70;
        }

        private void CreateEmailField(ref int y)
        {
            var lbl = new Label
            {
                Text = "Електронна пошта:*",
                Location = new Point(50, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(lbl);

            txtEmail = new TextBox
            {
                Location = new Point(50, y + 20),
                Width = 330,
                Font = new Font("Segoe UI", 10)
            };
            txtEmail.TextChanged += ValidateEmail;
            this.Controls.Add(txtEmail);

            lblEmailError = new Label
            {
                Text = "Невірний формат email (приклад: name@domain.com)",
                Location = new Point(50, y + 45),
                Width = 330,
                ForeColor = Color.Red,
                Font = new Font("Segoe UI", 8),
                Visible = false
            };
            this.Controls.Add(lblEmailError);

            y += 70;
        }

        private void CreateFamilyDoctorField(ref int y)
        {
            var lbl = new Label
            {
                Text = "Сімейний лікар:",
                Location = new Point(50, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(lbl);

            cmbFamilyDoctor = new ComboBox
            {
                Location = new Point(50, y + 20),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(cmbFamilyDoctor);

            y += 65;
        }
        
        private void CreatePasswordField(ref int y)
        {
            var lbl = new Label
            {
                Text = "Пароль:*",
                Location = new Point(50, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(lbl);

            passwordPanel = new Panel
            {
                Location = new Point(50, y + 20),
                Width = 330,
                Height = 30
            };

            txtPassword = new TextBox
            {
                Location = new Point(0, 0),
                Width = 250,
                Height = 25,
                UseSystemPasswordChar = true,
                Font = new Font("Segoe UI", 10),
                Text = "12345"
            };

            chkShowPassword = new CheckBox
            {
                Text = "Показати",
                Location = new Point(260, 2),
                Width = 100,
                Height = 25,
                Font = new Font("Segoe UI", 9),
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

            passwordPanel.Controls.Add(txtPassword);
            passwordPanel.Controls.Add(chkShowPassword);
            
            this.Controls.Add(passwordPanel);

            y += 65;
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
            
            if (chkShowPassword.Checked)
            {
                chkShowPassword.Text = "Приховати";
                chkShowPassword.ForeColor = Color.Blue;
            }
            else
            {
                chkShowPassword.Text = "Показати";
                chkShowPassword.ForeColor = Color.Black;
            }
            
            int cursorPosition = txtPassword.SelectionStart;
            txtPassword.SelectionStart = cursorPosition;
        }

        private DateTimePicker CreateDatePicker(string labelText, ref int y)
        {
            var lbl = new Label
            {
                Text = labelText,
                Location = new Point(50, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            this.Controls.Add(lbl);

            var dtp = new DateTimePicker
            {
                Location = new Point(50, y + 20),
                Width = 330,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd.MM.yyyy",
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(dtp);

            y += 65;
            return dtp;
        }

        private void LoadFamilyDoctors()
        {
            try
            {
                using (var db = new ApplicationDbContext())
                {
                    var familyDoctors = db.Doctors
                        .Include(d => d.Specialty)
                        .Where(d => d.Specialty != null && d.Specialty.IsFamily && d.IsAcceptingNewPatients)
                        .Select(d => new
                        {
                            d.DoctorId,
                            DisplayName = $"{d.FullName} ({d.Specialty.Name})"
                        })
                        .ToList();

                    cmbFamilyDoctor.DisplayMember = "DisplayName";
                    cmbFamilyDoctor.ValueMember = "DoctorId";
                    cmbFamilyDoctor.DataSource = familyDoctors;

                    if (cmbFamilyDoctor.Items.Count > 0)
                    {
                        cmbFamilyDoctor.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження списку лікарів: {ex.Message}", 
                    "Попередження", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void ValidatePhone(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPhone.Text))
            {
                lblPhoneError.Visible = true;
                lblPhoneError.Text = "Телефон обов'язковий для заповнення";
                return;
            }

            if (!_ukrainianPhoneRegex.IsMatch(txtPhone.Text))
            {
                lblPhoneError.Visible = true;
                lblPhoneError.Text = "Невірний формат (приклад: +380501234567 або 0501234567)";
            }
            else
            {
                lblPhoneError.Visible = false;
            }
        }

        private void ValidateEmail(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                lblEmailError.Visible = true;
                lblEmailError.Text = "Email обов'язковий для заповнення";
                return;
            }

            if (!_emailRegex.IsMatch(txtEmail.Text))
            {
                lblEmailError.Visible = true;
                lblEmailError.Text = "Невірний формат email (приклад: name@domain.com)";
            }
            else
            {
                lblEmailError.Visible = false;
            }
        }

        private bool ValidateForm()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                txtName.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                txtName.BackColor = Color.White;
            }

            ValidatePhone(null, null);
            if (lblPhoneError.Visible)
            {
                txtPhone.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                txtPhone.BackColor = Color.White;
            }

            ValidateEmail(null, null);
            if (lblEmailError.Visible)
            {
                txtEmail.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                txtEmail.BackColor = Color.White;
            }

            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                txtPassword.BackColor = Color.MistyRose;
                isValid = false;
            }
            else
            {
                txtPassword.BackColor = Color.White;
            }

            int age = CalculateAge(dtpBirthDate.Value);
            if (age < 0 || age > 120)
            {
                MessageBox.Show("Будь ласка, введіть коректну дату народження", 
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                isValid = false;
            }
            return isValid;
        }

        private int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (birthDate > DateTime.Now.AddYears(-age)) age--;
            return age;
        }

        private void BtnReg_Click(object sender, EventArgs e)
        {
            if (!ValidateForm())
            {
                MessageBox.Show("Будь ласка, виправте помилки у формі", 
                    "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    if (db.Patients.Any(p => p.Email == txtEmail.Text.Trim()))
                    {
                        MessageBox.Show("Користувач з таким email вже зареєстрований", 
                            "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtEmail.Focus();
                        return;
                    }

                    if (db.Patients.Any(p => p.Phone == txtPhone.Text.Trim()))
                    {
                        MessageBox.Show("Користувач з таким телефоном вже зареєстрований", 
                            "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        txtPhone.Focus();
                        return;
                    }

                    int? familyDoctorId = null;
                    
                    if (cmbFamilyDoctor.SelectedValue != null)
                    {
                        familyDoctorId = (int)cmbFamilyDoctor.SelectedValue;
                    }
                    else
                    {
                        int age = CalculateAge(dtpBirthDate.Value);
                        int defaultDoctorId = (age < 18) ? 2 : 1;
                        
                        if (db.Doctors.Any(d => d.DoctorId == defaultDoctorId))
                        {
                            familyDoctorId = defaultDoctorId;
                        }
                    }

                    var newPatient = new Patient
                    {
                        FullName = txtName.Text.Trim(),
                        BirthDate = dtpBirthDate.Value,
                        Address = txtAddress.Text?.Trim() ?? "",
                        Phone = txtPhone.Text.Trim(),
                        Email = txtEmail.Text.Trim(),
                        Password = txtPassword.Text.Trim(),
                        FamilyDoctorId = familyDoctorId,
                        RegistrationDate = DateTime.Now,
                        BloodType = null,
                        Allergies = null
                    };

                    db.Patients.Add(newPatient);
                    db.SaveChanges();

                    string doctorInfo = familyDoctorId.HasValue 
                        ? $"ID лікаря: {familyDoctorId}" 
                        : "лікар не призначений";

                    MessageBox.Show(
                        $"✅ РЕЄСТРАЦІЯ УСПІШНА!\n\n" +
                        $"Вітаємо, {newPatient.FullName}!\n" +
                        $"Email: {newPatient.Email}\n" +
                        $"Пароль: {newPatient.Password}\n" +
                        $"Сімейний лікар: {doctorInfo}",
                        "Успіх", 
                        MessageBoxButtons.OK, 
                        MessageBoxIcon.Information);

                    new PatientLoginForm().Show();
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException?.Message ?? ex.Message;
                MessageBox.Show($"Помилка при реєстрації: {msg}", 
                    "Помилка БД", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}