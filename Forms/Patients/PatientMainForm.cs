using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Clinic_BD.Data;
using Clinic_BD.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Clinic_BD.Forms.Patients
{
    public partial class PatientMainForm : Form
    {
        private ApplicationDbContext _db;
        private Patient _currentPatient;
        private Panel contentPanel;
        private Label lblWelcome;

        public PatientMainForm(Patient patient)
        {
            _db = new ApplicationDbContext();

            _currentPatient = _db.Patients
                .Include(p => p.FamilyDoctor)
                .ThenInclude(d => d.Specialty)
                .FirstOrDefault(p => p.PatientId == patient.PatientId);

            if (_currentPatient == null)
            {
                MessageBox.Show("Помилка завантаження даних пацієнта", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            SetupUI();
            ShowProfile(null, null);
        }

        private void SetupUI()
        {
            this.Text = $"Особистий кабінет: {_currentPatient?.FullName ?? "Пацієнт"}";
            this.Size = new Size(1000, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.White;

            var topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.FromArgb(0, 120, 215),
                Padding = new Padding(20)
            };

            var lblTitle = new Label
            {
                Text = "ОСОБИСТИЙ КАБІНЕТ ПАЦІЄНТА",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                Location = new Point(20, 15),
                AutoSize = true
            };

            lblWelcome = new Label
            {
                Text = $"Вітаємо, {_currentPatient.FullName}!",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 12),
                Location = new Point(20, 50),
                AutoSize = true
            };

            topPanel.Controls.Add(lblTitle);
            topPanel.Controls.Add(lblWelcome);

            var panelMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 250,
                BackColor = Color.FromArgb(240, 240, 240),
                Padding = new Padding(10)
            };

            var lblMenu = new Label
            {
                Text = "МЕНЮ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                Location = new Point(10, 15),
                AutoSize = true
            };
            panelMenu.Controls.Add(lblMenu);

            panelMenu.Controls.Add(CreateMenuButton("👤 МІЙ ПРОФІЛЬ", 50, ShowProfile));
            panelMenu.Controls.Add(CreateMenuButton("📅 ЗАПИСАТИСЬ ДО ЛІКАРЯ", 100, ShowAppointment));
            panelMenu.Controls.Add(CreateMenuButton("📋 МОЇ ЗАПИСИ", 150, ShowMyAppointments));
            panelMenu.Controls.Add(CreateMenuButton("🔬 МОЇ НАПРАВЛЕННЯ", 200, ShowReferrals));
            panelMenu.Controls.Add(CreateMenuButton("📊 РЕЗУЛЬТАТИ АНАЛІЗІВ", 250, ShowTestResults));
            panelMenu.Controls.Add(CreateMenuButton("🩺 МОЇ ДІАГНОЗИ", 300, ShowDiagnoses));
            panelMenu.Controls.Add(CreateMenuButton("💊 МОЇ РЕЦЕПТИ", 350, ShowPrescriptions));

            var separator = new Label
            {
                Text = "______________________________",
                Location = new Point(10, 400),
                AutoSize = true,
                ForeColor = Color.Gray
            };
            panelMenu.Controls.Add(separator);

            panelMenu.Controls.Add(CreateMenuButton("🚪 ВИХІД", 430, Logout));

            contentPanel = new Panel
            {
                Location = new Point(260, 110),
                Size = new Size(720, 550),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                AutoScroll = true
            };

            this.Controls.Add(contentPanel);
            this.Controls.Add(panelMenu);
            this.Controls.Add(topPanel);
        }

        private Button CreateMenuButton(string text, int y, EventHandler clickHandler)
        {
            var btn = new Button
            {
                Text = text,
                Location = new Point(10, y),
                Size = new Size(220, 40),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.White,
                Cursor = Cursors.Hand,
                FlatAppearance = { BorderColor = Color.LightGray }
            };
            btn.Click += clickHandler;
            return btn;
        }

        private void ShowProfile(object sender, EventArgs e)
        {
            contentPanel.Controls.Clear();

            int yPos = 20;
            int labelWidth = 150;
            int valueWidth = 450;

            AddSectionTitle("👤 МІЙ ПРОФІЛЬ", ref yPos);

            AddInfoRow("ПІБ:", _currentPatient.FullName, ref yPos, labelWidth, valueWidth);
            AddInfoRow("Дата народження:", _currentPatient.BirthDate.ToString("dd.MM.yyyy"), ref yPos, labelWidth,
                valueWidth);
            AddInfoRow("Вік:", $"{CalculateAge(_currentPatient.BirthDate)} років", ref yPos, labelWidth, valueWidth);

            if (!string.IsNullOrEmpty(_currentPatient.Address))
                AddInfoRow("Адреса:", _currentPatient.Address, ref yPos, labelWidth, valueWidth);

            if (!string.IsNullOrEmpty(_currentPatient.Phone))
                AddInfoRow("Телефон:", _currentPatient.Phone, ref yPos, labelWidth, valueWidth);

            if (!string.IsNullOrEmpty(_currentPatient.Email))
                AddInfoRow("Email:", _currentPatient.Email, ref yPos, labelWidth, valueWidth);

            AddInfoRow("Дата реєстрації:",
                _currentPatient.RegistrationDate.ToString("dd.MM.yyyy"),
                ref yPos, labelWidth, valueWidth);

            if (!string.IsNullOrEmpty(_currentPatient.BloodType))
                AddInfoRow("Група крові:", _currentPatient.BloodType, ref yPos, labelWidth, valueWidth);

            if (!string.IsNullOrEmpty(_currentPatient.Allergies))
                AddInfoRow("Алергії:", _currentPatient.Allergies, ref yPos, labelWidth, valueWidth);

            yPos += 20;

            AddSectionTitle("👨‍⚕️ СІМЕЙНИЙ ЛІКАР", ref yPos);

            if (_currentPatient.FamilyDoctor != null)
            {
                AddInfoRow("ПІБ лікаря:", _currentPatient.FamilyDoctor.FullName ?? "Не вказано", ref yPos, labelWidth,
                    valueWidth);

                string specialtyName = _currentPatient.FamilyDoctor.Specialty?.Name ?? "Не вказано";
                AddInfoRow("Спеціальність:", specialtyName, ref yPos, labelWidth, valueWidth);

                string room = !string.IsNullOrEmpty(_currentPatient.FamilyDoctor.RoomNumber)
                    ? $"Кабінет №{_currentPatient.FamilyDoctor.RoomNumber}"
                    : "Не вказано";
                AddInfoRow("Кабінет:", room, ref yPos, labelWidth, valueWidth);

                string phone = !string.IsNullOrEmpty(_currentPatient.FamilyDoctor.WorkPhone)
                    ? _currentPatient.FamilyDoctor.WorkPhone
                    : "Не вказано";
                AddInfoRow("Телефон:", phone, ref yPos, labelWidth, valueWidth);
            }
            else
            {
                var lblNoDoctor = new Label
                {
                    Text = "❌ Сімейний лікар не призначений",
                    Font = new Font("Segoe UI", 10, FontStyle.Italic),
                    ForeColor = Color.Red,
                    Location = new Point(40, yPos),
                    AutoSize = true
                };
                contentPanel.Controls.Add(lblNoDoctor);
            }
        }

        private void ShowAppointment(object sender, EventArgs e)
        {
            contentPanel.Controls.Clear();
            int yPos = 20;

            AddSectionTitle("📅 ЗАПИС ДО ЛІКАРЯ", ref yPos);

            using (var db = new ApplicationDbContext())
            {
                var doctors = db.Doctors
                    .Include(d => d.Specialty)
                    .Where(d => d.IsAcceptingNewPatients)
                    .Select(d => new
                    {
                        d.DoctorId,
                        DisplayName = $"{d.FullName} - {d.Specialty.Name} (каб.{d.RoomNumber})"
                    })
                    .ToList();

                if (!doctors.Any())
                {
                    ShowMessage("На жаль, зараз немає доступних лікарів для запису");
                    return;
                }

                var lblDoctor = new Label
                {
                    Text = "Оберіть лікаря:",
                    Location = new Point(30, yPos),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true
                };
                contentPanel.Controls.Add(lblDoctor);
                yPos += 25;

                var cmbDoctor = new ComboBox
                {
                    Location = new Point(30, yPos),
                    Width = 500,
                    DropDownStyle = ComboBoxStyle.DropDownList,
                    DataSource = doctors,
                    DisplayMember = "DisplayName",
                    ValueMember = "DoctorId"
                };
                contentPanel.Controls.Add(cmbDoctor);
                yPos += 40;

                var lblDate = new Label
                {
                    Text = "Оберіть дату та час:",
                    Location = new Point(30, yPos),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true
                };
                contentPanel.Controls.Add(lblDate);
                yPos += 25;

                var dtpDate = new DateTimePicker
                {
                    Location = new Point(30, yPos),
                    Width = 200,
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "dd.MM.yyyy HH:mm",
                    MinDate = DateTime.Today,
                    MaxDate = DateTime.Today.AddMonths(1)
                };
                contentPanel.Controls.Add(dtpDate);
                yPos += 40;

                var lblNotes = new Label
                {
                    Text = "Опишіть причину звернення:",
                    Location = new Point(30, yPos),
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    AutoSize = true
                };
                contentPanel.Controls.Add(lblNotes);
                yPos += 25;

                var txtNotes = new TextBox
                {
                    Location = new Point(30, yPos),
                    Width = 500,
                    Height = 80,
                    Multiline = true,
                    ScrollBars = ScrollBars.Vertical
                };
                contentPanel.Controls.Add(txtNotes);
                yPos += 100;

                var btnSave = new Button
                {
                    Text = "✅ ЗАПИСАТИСЬ",
                    Location = new Point(30, yPos),
                    Size = new Size(200, 40),
                    BackColor = Color.LightGreen,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold)
                };

                btnSave.Click += (s, args) =>
                {
                    try
                    {
                        var newAppointment = new Appointment
                        {
                            PatientId = _currentPatient.PatientId,
                            DoctorId = (int)cmbDoctor.SelectedValue,
                            AppointmentDate = dtpDate.Value.Date,
                            AppointmentTime = dtpDate.Value.TimeOfDay,
                            Status = "заплановано",
                            Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? "Плановий візит" : txtNotes.Text,
                            CreateAt = DateTime.Now
                        };

                        using (var ctx = new ApplicationDbContext())
                        {
                            ctx.Appointments.Add(newAppointment);

                            var newVisit = new Visit
                            {
                                Appointment = newAppointment,
                                VisitDate = newAppointment.AppointmentDate,
                                Symptoms = newAppointment.Notes,
                                VisitNotes = "Автоматично створений після запису"
                            };
                            ctx.Visits.Add(newVisit);

                            ctx.SaveChanges();
                        }

                        MessageBox.Show("Ви успішно записались до лікаря!", "Успіх",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);

                        ShowMyAppointments(null, null);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка при записі: {ex.Message}", "Помилка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                };

                contentPanel.Controls.Add(btnSave);
            }
        }

        private void ShowMyAppointments(object sender, EventArgs e)
        {
            contentPanel.Controls.Clear();
            int yPos = 20;

            AddSectionTitle("📋 МОЇ ЗАПИСИ ДО ЛІКАРЯ", ref yPos);

            using (var db = new ApplicationDbContext())
            {
                var appointments = db.Appointments
                    .Include(a => a.Doctor)
                    .ThenInclude(d => d.Specialty)
                    .Where(a => a.PatientId == _currentPatient.PatientId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ThenByDescending(a => a.AppointmentTime)
                    .ToList();

                if (!appointments.Any())
                {
                    ShowMessage("У вас немає записів до лікарів");
                    return;
                }

                foreach (var app in appointments)
                {
                    CreateAppointmentCard(app, ref yPos);
                }
            }
        }

        private void CreateAppointmentCard(Appointment app, ref int yPos)
        {
            var cardPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(650, 130),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            int currentX = 10;
            int currentY = 15;

            var lblStatus = new Label
            {
                Text = GetStatusText(app.Status),
                Location = new Point(520, 15),
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = GetStatusColor(app.Status)
            };
            cardPanel.Controls.Add(lblStatus);

            AddCardLabelWithOffset(cardPanel, "👨‍⚕️ Лікар:", $"{app.Doctor?.FullName} ({app.Doctor?.Specialty?.Name})",
                ref currentX, ref currentY, 80);

            AddCardLabelWithOffset(cardPanel, "📅 Дата:",
                $"{app.AppointmentDate:dd.MM.yyyy} о {app.AppointmentTime:hh\\:mm}",
                ref currentX, ref currentY, 80);

            AddCardLabelWithOffset(cardPanel, "📝 Причина:", app.Notes ?? "Не вказано",
                ref currentX, ref currentY, 80, true);

            if (app.AppointmentDate > DateTime.Today ||
                (app.AppointmentDate == DateTime.Today && app.AppointmentTime > DateTime.Now.TimeOfDay))
            {
                var btnCancel = new Button
                {
                    Text = "✖ Скасувати",
                    Location = new Point(540, 80),
                    Size = new Size(90, 30),
                    BackColor = Color.LightCoral,
                    FlatStyle = FlatStyle.Flat,
                    Tag = app
                };
                btnCancel.Click += (s, args) => CancelAppointment((Appointment)((Button)s).Tag);
                cardPanel.Controls.Add(btnCancel);
            }

            contentPanel.Controls.Add(cardPanel);
            yPos += 140;
        }

        private void ShowReferrals(object sender, EventArgs e)
        {
            contentPanel.Controls.Clear();
            int yPos = 20;

            AddSectionTitle("🔬 МОЇ НАПРАВЛЕННЯ", ref yPos);

            try
            {
                using (var db = new ApplicationDbContext())
                {
                    Console.WriteLine($"Завантаження направлень для пацієнта ID: {_currentPatient.PatientId}");
                    var referrals = db.Referrals
                        .Include(r => r.FromDoctor)
                        .Include(r => r.ToSpecialty)
                        .Where(r => r.PatientId == _currentPatient.PatientId)
                        .OrderByDescending(r => r.ReferralDate)
                        .ToList();

                    if (!referrals.Any())
                    {
                        ShowMessage("У вас немає направлень");
                        return;
                    }

                    foreach (var referral in referrals)
                    {
                        CreateReferralCard(referral, ref yPos);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження направлень: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CreateReferralCard(Referral referral, ref int yPos)
        {
            var cardPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(650, 220),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            int startX = 15;
            int startY = 15;
            int currentX = startX;
            int currentY = startY;

            try
            {
                Console.WriteLine($"Створення картки направлення ID: {referral.ReferralId}");
                Console.WriteLine($"  Used: {referral.Used}");
                Console.WriteLine($"  FromDoctor: {referral.FromDoctor?.FullName ?? "NULL"}");
                Console.WriteLine($"  ToSpecialty: {referral.ToSpecialty?.Name ?? "NULL"}");
                Console.WriteLine($"  ReferralDate: {referral.ReferralDate?.ToString() ?? "NULL"}");
                Console.WriteLine($"  ExpiryDate: {referral.ExpiryDate?.ToString() ?? "NULL"}");
                Console.WriteLine($"  Priority: {referral.Priority ?? "NULL"}");
                Console.WriteLine($"  Reason: {referral.Reason ?? "NULL"}");
                Console.WriteLine($"  Notes: {referral.Notes ?? "NULL"}");

                var statusLabel = new Label
                {
                    Text = referral.Used ? "✅ Використано" : "⏳ Очікує",
                    Location = new Point(500, 15),
                    AutoSize = true,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    ForeColor = referral.Used ? Color.Green : Color.Orange
                };
                cardPanel.Controls.Add(statusLabel);

                string doctorName = "Невідомо";
                if (referral.FromDoctor != null && !string.IsNullOrEmpty(referral.FromDoctor.FullName))
                {
                    doctorName = referral.FromDoctor.FullName;
                }

                AddLabelValue(cardPanel, "👨‍⚕️ Лікар:", doctorName, ref currentX, ref currentY);

                string specialtyName = "Невідомо";
                if (referral.ToSpecialty != null && !string.IsNullOrEmpty(referral.ToSpecialty.Name))
                {
                    specialtyName = referral.ToSpecialty.Name;
                }

                AddLabelValue(cardPanel, "🔬 Спеціаліст:", specialtyName, ref currentX, ref currentY);

                string referralDateStr = "Дата не вказана";
                if (referral.ReferralDate.HasValue)
                {
                    referralDateStr = referral.ReferralDate.Value.ToString("dd.MM.yyyy");
                }

                AddLabelValue(cardPanel, "📅 Дата:", referralDateStr, ref currentX, ref currentY);

                string expiryDateStr = "Термін не вказано";
                if (referral.ExpiryDate.HasValue)
                {
                    expiryDateStr = referral.ExpiryDate.Value.ToString("dd.MM.yyyy");
                }

                AddLabelValue(cardPanel, "⏰ Дійсне до:", expiryDateStr, ref currentX, ref currentY);

                if (!string.IsNullOrEmpty(referral.Priority) && referral.Priority != "нормальний")
                {
                    string priorityText = referral.Priority switch
                    {
                        "терміновий" => "🔴 Терміновий",
                        "високий" => "🟠 Високий",
                        _ => referral.Priority
                    };

                    var priorityLabel = new Label
                    {
                        Text = $"Пріоритет: {priorityText}",
                        Location = new Point(500, currentY - 20),
                        Font = new Font("Segoe UI", 9, FontStyle.Italic),
                        ForeColor = Color.Gray,
                        AutoSize = true
                    };
                    cardPanel.Controls.Add(priorityLabel);
                }

                if (!string.IsNullOrEmpty(referral.Reason))
                {
                    currentX = startX;
                    currentY += 10;

                    var reasonLabel = new Label
                    {
                        Text = "📝 Причина:",
                        Location = new Point(currentX, currentY),
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        AutoSize = true
                    };
                    cardPanel.Controls.Add(reasonLabel);

                    var reasonValue = new Label
                    {
                        Text = referral.Reason,
                        Location = new Point(currentX + 90, currentY),
                        Width = 450,
                        Font = new Font("Segoe UI", 9),
                        AutoSize = false
                    };
                    cardPanel.Controls.Add(reasonValue);
                    currentY += 30;
                }

                if (!string.IsNullOrEmpty(referral.Notes))
                {
                    currentX = startX;

                    var notesLabel = new Label
                    {
                        Text = "📋 Нотатки:",
                        Location = new Point(currentX, currentY),
                        Font = new Font("Segoe UI", 9, FontStyle.Bold),
                        AutoSize = true
                    };
                    cardPanel.Controls.Add(notesLabel);

                    var notesValue = new Label
                    {
                        Text = referral.Notes,
                        Location = new Point(currentX + 90, currentY),
                        Width = 450,
                        Font = new Font("Segoe UI", 9),
                        AutoSize = false
                    };
                    cardPanel.Controls.Add(notesValue);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка при створенні картки: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                cardPanel.Controls.Clear();
                var errorLabel = new Label
                {
                    Text = $"❌ Помилка відображення: {ex.Message}",
                    Location = new Point(20, 20),
                    AutoSize = true,
                    ForeColor = Color.Red
                };
                cardPanel.Controls.Add(errorLabel);
            }

            contentPanel.Controls.Add(cardPanel);
            yPos += 240;
        }

        private void AddLabelValue(Panel card, string labelText, string valueText, ref int x, ref int y)
        {
            try
            {
                var label = new Label
                {
                    Text = labelText,
                    Location = new Point(x, y),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    AutoSize = true
                };
                card.Controls.Add(label);

                var value = new Label
                {
                    Text = valueText ?? "Не вказано",
                    Location = new Point(x + 90, y),
                    Width = 350,
                    Font = new Font("Segoe UI", 9),
                    AutoSize = false,
                    TextAlign = ContentAlignment.MiddleLeft
                };
                card.Controls.Add(value);

                y += 25;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка в AddLabelValue: {ex.Message}");
            }
        }

        private void AddCardLabelWithOffset(Panel card, string label, string value,
            ref int x, ref int y, int labelWidth, bool isLong = false)
        {
            var lbl = new Label
            {
                Text = label,
                Location = new Point(x, y),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true
            };
            card.Controls.Add(lbl);

            var val = new Label
            {
                Text = value ?? "Не вказано",
                Location = new Point(x + labelWidth + 15, y),
                Width = isLong ? 430 : 300,
                Font = new Font("Segoe UI", 9),
                AutoSize = false
            };
            card.Controls.Add(val);

            y += 25;
        }

        private void ShowTestResults(object sender, EventArgs e)
        {
            contentPanel.Controls.Clear();
            int yPos = 20;

            AddSectionTitle("📊 РЕЗУЛЬТАТИ АНАЛІЗІВ", ref yPos);

            using (var db = new ApplicationDbContext())
            {
                var testResults = db.TestResults
                    .Include(tr => tr.TestAppointment)
                    .ThenInclude(ta => ta.TestOrder)
                    .ThenInclude(to => to.TestType)
                    .Include(tr => tr.PerformedByDoctor)
                    .Include(tr => tr.ReviewedByDoctor)
                    .Where(tr => tr.TestAppointment.PatientId == _currentPatient.PatientId)
                    .OrderByDescending(tr => tr.ResultDate)
                    .ToList();

                if (!testResults.Any())
                {
                    ShowMessage("У вас немає результатів аналізів");
                    return;
                }

                foreach (var result in testResults)
                {
                    CreateTestResultCard(result, ref yPos);
                }
            }
        }

        private void CreateTestResultCard(TestResult result, ref int yPos)
        {
            var cardPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(650, 200),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            int currentX = 10;
            int currentY = 15;

            var lblTestName = new Label
            {
                Text = result.TestAppointment?.TestOrder?.TestType?.Name ?? "Невідомий тест",
                Location = new Point(currentX, currentY - 5),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblTestName);
            currentY += 25;

            string resultDate = result.ResultDate != null
                ? result.ResultDate.Value.ToString("dd.MM.yyyy HH:mm")
                : "Дата не вказана";
            AddCardLabelWithOffset(cardPanel, "📅 Дата:", resultDate, ref currentX, ref currentY, 80);

            string performer = result.PerformedByDoctor?.FullName ?? "Невідомо";
            AddCardLabelWithOffset(cardPanel, "👨‍🔬 Виконав:", performer, ref currentX, ref currentY, 80);

            currentX = 10;
            currentY += 5;

            var lblResultLabel = new Label
            {
                Text = "📊 Результат:",
                Location = new Point(currentX, currentY),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblResultLabel);

            var lblResult = new Label
            {
                Text = result.ResultText ?? "Не вказано",
                Location = new Point(currentX + 100, currentY),
                Width = 430,
                Font = new Font("Segoe UI", 9),
                AutoSize = false,
                ForeColor = result.IsAbnormal == true ? Color.Red : Color.Black
            };
            cardPanel.Controls.Add(lblResult);
            currentY += 25;

            if (!string.IsNullOrEmpty(result.Conclusion))
            {
                currentX = 10;

                var lblConclusionLabel = new Label
                {
                    Text = "📝 Висновок:",
                    Location = new Point(currentX, currentY),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    AutoSize = true
                };
                cardPanel.Controls.Add(lblConclusionLabel);

                var lblConclusion = new Label
                {
                    Text = result.Conclusion,
                    Location = new Point(currentX + 100, currentY),
                    Width = 430,
                    Font = new Font("Segoe UI", 9),
                    AutoSize = false
                };
                cardPanel.Controls.Add(lblConclusion);
                currentY += 25;
            }

            if (result.IsAbnormal == true)
            {
                var lblAbnormal = new Label
                {
                    Text = "⚠️ Відхилення від норми!",
                    Location = new Point(500, currentY - 20),
                    AutoSize = true,
                    ForeColor = Color.Red,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };
                cardPanel.Controls.Add(lblAbnormal);
            }

            contentPanel.Controls.Add(cardPanel);
            yPos += 210;
        }

        private void ShowDiagnoses(object sender, EventArgs e)
        {
            contentPanel.Controls.Clear();
            int yPos = 20;

            AddSectionTitle("🩺 МОЇ ДІАГНОЗИ", ref yPos);

            using (var db = new ApplicationDbContext())
            {
                var visits = db.Visits
                    .Include(v => v.Appointment)
                    .ThenInclude(a => a.Doctor)
                    .Include(v => v.Diagnosis)
                    .Where(v => v.Appointment.PatientId == _currentPatient.PatientId
                                && v.DiagnosisId != null)
                    .OrderByDescending(v => v.VisitDate)
                    .ToList();

                if (!visits.Any())
                {
                    ShowMessage("У вас немає встановлених діагнозів");
                    return;
                }

                foreach (var visit in visits)
                {
                    CreateDiagnosisCard(visit, ref yPos);
                }
            }
        }

        private void CreateDiagnosisCard(Visit visit, ref int yPos)
        {
            var cardPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(650, 210),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            int currentX = 10;
            int currentY = 15;

            string diagnosisName = visit.Diagnosis?.Name ?? "Невідомий діагноз";
            var lblDiagnosis = new Label
            {
                Text = diagnosisName,
                Location = new Point(currentX, currentY - 5),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblDiagnosis);
            currentY += 25;

            if (!string.IsNullOrEmpty(visit.Diagnosis?.IcdCode))
            {
                AddCardLabelWithOffset(cardPanel, "🔖 Код МКХ:", visit.Diagnosis.IcdCode,
                    ref currentX, ref currentY, 80);
            }

            string doctorName = visit.Appointment?.Doctor?.FullName ?? "Невідомо";
            AddCardLabelWithOffset(cardPanel, "👨‍⚕️ Лікар:", doctorName, ref currentX, ref currentY, 80);

            string visitDate = visit.VisitDate != null
                ? visit.VisitDate.Value.ToString("dd.MM.yyyy")
                : "Дата не вказана";
            AddCardLabelWithOffset(cardPanel, "📅 Дата:", visitDate, ref currentX, ref currentY, 80);

            if (!string.IsNullOrEmpty(visit.Symptoms))
            {
                currentX = 10;
                currentY += 5;

                var lblSymptomsLabel = new Label
                {
                    Text = "🤒 Симптоми:",
                    Location = new Point(currentX, currentY),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    AutoSize = true
                };
                cardPanel.Controls.Add(lblSymptomsLabel);

                var lblSymptoms = new Label
                {
                    Text = visit.Symptoms,
                    Location = new Point(currentX + 100, currentY),
                    Width = 430,
                    Font = new Font("Segoe UI", 9),
                    AutoSize = false
                };
                cardPanel.Controls.Add(lblSymptoms);
                currentY += 25;
            }

            if (!string.IsNullOrEmpty(visit.TreatmentPlan))
            {
                currentX = 10;

                var lblTreatmentLabel = new Label
                {
                    Text = "💊 Лікування:",
                    Location = new Point(currentX, currentY),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    AutoSize = true
                };
                cardPanel.Controls.Add(lblTreatmentLabel);

                var lblTreatment = new Label
                {
                    Text = visit.TreatmentPlan,
                    Location = new Point(currentX + 100, currentY),
                    Width = 430,
                    Font = new Font("Segoe UI", 9),
                    AutoSize = false
                };
                cardPanel.Controls.Add(lblTreatment);
            }

            contentPanel.Controls.Add(cardPanel);
            yPos += 220;
        }

        private void ShowPrescriptions(object sender, EventArgs e)
        {
            contentPanel.Controls.Clear();
            int yPos = 20;

            AddSectionTitle("💊 МОЇ РЕЦЕПТИ", ref yPos);

            using (var db = new ApplicationDbContext())
            {
                var prescriptions = db.Prescriptions
                    .Include(p => p.Visit)
                    .ThenInclude(v => v.Appointment)
                    .ThenInclude(a => a.Doctor)
                    .Include(p => p.PrescribingDoctor)
                    .Where(p => p.Visit.Appointment.PatientId == _currentPatient.PatientId)
                    .OrderByDescending(p => p.StartDate)
                    .ToList();

                if (!prescriptions.Any())
                {
                    ShowMessage("У вас немає рецептів");
                    return;
                }

                foreach (var prescription in prescriptions)
                {
                    CreatePrescriptionCard(prescription, ref yPos);
                }
            }
        }

        private void CreatePrescriptionCard(Prescription prescription, ref int yPos)
        {
            var cardPanel = new Panel
            {
                Location = new Point(20, yPos),
                Size = new Size(650, 180),
                BackColor = Color.FromArgb(250, 250, 250),
                BorderStyle = BorderStyle.FixedSingle
            };

            int currentX = 10;
            int currentY = 15;

            var lblMedication = new Label
            {
                Text = prescription.Medication,
                Location = new Point(currentX, currentY - 5),
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                AutoSize = true
            };
            cardPanel.Controls.Add(lblMedication);
            currentY += 25;

            string dosage = !string.IsNullOrEmpty(prescription.Dosage) ? prescription.Dosage.Trim() : "Не вказано";
            AddCardLabelWithOffset(cardPanel, "💊 Дозування:", dosage, ref currentX, ref currentY, 80);

            string frequency = !string.IsNullOrEmpty(prescription.Frequency) ? prescription.Frequency : "Не вказано";
            AddCardLabelWithOffset(cardPanel, "⏰ Частота:", frequency, ref currentX, ref currentY, 80);

            string period;
            if (prescription.StartDate != null && prescription.EndDate != null)
            {
                period = $"{prescription.StartDate.Value:dd.MM.yyyy} - {prescription.EndDate.Value:dd.MM.yyyy}";
            }
            else if (prescription.StartDate != null)
            {
                period = $"з {prescription.StartDate.Value:dd.MM.yyyy}";
            }
            else
            {
                period = "Термін не вказано";
            }

            AddCardLabelWithOffset(cardPanel, "📅 Період:", period, ref currentX, ref currentY, 80);

            string doctorName = prescription.PrescribingDoctor?.FullName ?? "Невідомо";
            AddCardLabelWithOffset(cardPanel, "👨‍⚕️ Лікар:", doctorName, ref currentX, ref currentY, 80);

            if (!string.IsNullOrEmpty(prescription.Instructions))
            {
                currentX = 10;
                currentY += 5;

                var lblInstructionsLabel = new Label
                {
                    Text = "📝 Інструкції:",
                    Location = new Point(currentX, currentY),
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    AutoSize = true
                };
                cardPanel.Controls.Add(lblInstructionsLabel);

                var lblInstructions = new Label
                {
                    Text = prescription.Instructions,
                    Location = new Point(currentX + 100, currentY),
                    Width = 430,
                    Font = new Font("Segoe UI", 9),
                    AutoSize = false
                };
                cardPanel.Controls.Add(lblInstructions);
            }

            contentPanel.Controls.Add(cardPanel);
            yPos += 190;
        }

        private void AddSectionTitle(string title, ref int yPos)
        {
            var lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                Location = new Point(20, yPos),
                AutoSize = true
            };
            contentPanel.Controls.Add(lbl);
            yPos += 35;
        }

        private void ShowMessage(string message)
        {
            var lbl = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(50, 50),
                AutoSize = true
            };
            contentPanel.Controls.Add(lbl);
        }

        private void AddInfoRow(string label, string value, ref int yPos, int labelWidth, int valueWidth)
        {
            var lbl = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Location = new Point(30, yPos),
                Width = labelWidth,
                TextAlign = ContentAlignment.MiddleRight
            };

            var val = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 10),
                Location = new Point(30 + labelWidth + 10, yPos),
                Width = valueWidth,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleLeft
            };

            contentPanel.Controls.Add(lbl);
            contentPanel.Controls.Add(val);
            yPos += 30;
        }

        private string GetStatusText(string status)
        {
            return status switch
            {
                "заплановано" => "📅 Заплановано",
                "виконано" => "✅ Виконано",
                "скасовано" => "❌ Скасовано",
                "перенесено" => "⏳ Перенесено",
                _ => status
            };
        }

        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "заплановано" => Color.Blue,
                "виконано" => Color.Green,
                "скасовано" => Color.Red,
                "перенесено" => Color.Orange,
                _ => Color.Black
            };
        }

        private int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (birthDate > DateTime.Now.AddYears(-age)) age--;
            return age;
        }

        private void CancelAppointment(Appointment appointment)
        {
            var result = MessageBox.Show("Ви дійсно хочете скасувати запис?", "Підтвердження",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    using (var db = new ApplicationDbContext())
                    {
                        var app = db.Appointments.Find(appointment.AppointmentId);
                        if (app != null)
                        {
                            app.Status = "скасовано";
                            db.SaveChanges();
                            MessageBox.Show("Запис скасовано", "Успіх",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                            ShowMyAppointments(null, null);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Помилка: {ex.Message}", "Помилка",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Logout(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Ви дійсно хочете вийти?", "Вихід",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _db?.Dispose();
                var loginForm = new Clinic_BD.Forms.Auth.LoginForm();
                loginForm.Show();
                this.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _db?.Dispose();
            base.OnFormClosing(e);
        }
    }
}