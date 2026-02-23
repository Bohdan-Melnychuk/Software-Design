using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Clinic_BD.Data;
using Clinic_BD.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Clinic_BD.Forms.Auth;


namespace Clinic_BD.Forms.Admin
{
    public partial class AdminMainForm : Form
    {
        private ApplicationDbContext _db;
        private TabControl _tabControl;
        private ToolStripTextBox _txtSearch;
        private ToolStripComboBox _cbSearchColumn;
        private const string SearchPlaceholder = "Пошук...";

        private string _lastSelectedTab = "";

        public AdminMainForm()
        {
            this.WindowState = FormWindowState.Maximized;
            _db = new ApplicationDbContext();
            SetupUI();
        }

        private void SaveCurrentTab()
        {
            if (_tabControl.SelectedTab != null)
            {
                _lastSelectedTab = _tabControl.SelectedTab.Text;
            }
        }

        private void RestoreLastTab()
        {
            if (!string.IsNullOrEmpty(_lastSelectedTab))
            {
                foreach (TabPage tab in _tabControl.TabPages)
                {
                    if (tab.Text == _lastSelectedTab)
                    {
                        _tabControl.SelectedTab = tab;
                        break;
                    }
                }
            }
        }

        private void SetupUI()
        {
            this.Text = "Панель керування адміністратора | Clinic System";
            this.Size = new Size(1300, 750);
            this.StartPosition = FormStartPosition.CenterScreen;

            _tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            _tabControl.SelectedIndexChanged += (s, e) => UpdateSearchColumns();

            var toolStrip = new ToolStrip
                { Height = 50, BackColor = Color.White, Dock = DockStyle.Top, RenderMode = ToolStripRenderMode.System };

            var btnBack = new Button
            {
                Text = "← Назад", Location = new Point(1460, 1), Size = new Size(75, 25), FlatStyle = FlatStyle.Flat
            };
            btnBack.Click += (s, e) =>
            {
                foreach (Form f in Application.OpenForms)
                {
                    if (f is LoginForm)
                    {
                        f.Show();
                        break;
                    }
                }

                this.Hide();
            };

            var btnSave = new ToolStripButton("💾 Зберегти")
            {
                ForeColor = Color.ForestGreen
            };
            btnSave.Click += (s, e) => SaveChanges();

            var btnRefresh = new ToolStripButton("🔄 Оновити")
            {
                Margin = new Padding(5, 0, 0, 0)
            };
            btnRefresh.Click += (s, e) => LoadAllData();

            var btnAdd = new ToolStripButton("➕ Додати")
            {
                Margin = new Padding(5, 0, 0, 0),
                BackColor = Color.LightGreen
            };
            btnAdd.Click += (s, e) => AddNewRow();

            var btnAddApp = new ToolStripButton("📅 Новий запис")
            {
                BackColor = Color.LightBlue,
                Margin = new Padding(5, 0, 0, 0)
            };
            btnAddApp.Click += (s, e) => OpenAddAppointment();

            var btnAddReferral = new ToolStripButton("📜 Нове направлення")
            {
                BackColor = Color.LightCyan,
                Margin = new Padding(5, 0, 0, 0)
            };
            btnAddReferral.Click += (s, e) => OpenAddReferral();

            var btnAddTestOrder = new ToolStripButton("🧪 Направлення на тест")
            {
                BackColor = Color.LightCoral,
                Margin = new Padding(5, 0, 0, 0)
            };
            btnAddTestOrder.Click += (s, e) => OpenAddTestOrder();

            var btnEdit = new ToolStripButton("📝 Редагувати")
            {
                Margin = new Padding(5, 0, 0, 0),
                BackColor = Color.LightYellow
            };
            btnEdit.Click += (s, e) => EditSelected();

            var btnDelete = new ToolStripButton("❌ Видалити")
            {
                Margin = new Padding(5, 0, 0, 0),
                BackColor = Color.MistyRose
            };
            btnDelete.Click += (s, e) => DeleteSelected();

            toolStrip.Items.AddRange(new ToolStripItem[]
            {
                btnSave,
                btnRefresh,
                btnAdd,
                btnAddApp,
                btnAddReferral,
                btnAddTestOrder,
                btnEdit,
                btnDelete,
                new ToolStripSeparator()
            });
            this.Controls.Add(btnBack);

            toolStrip.Items.Add(new ToolStripLabel("🔍 Критерій:"));
            _cbSearchColumn = new ToolStripComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 150 };

            _txtSearch = new ToolStripTextBox
            {
                Width = 200, Text = SearchPlaceholder, ForeColor = Color.Gray, BorderStyle = BorderStyle.FixedSingle
            };
            _txtSearch.Enter += (s, e) =>
            {
                if (_txtSearch.Text == SearchPlaceholder)
                {
                    _txtSearch.Text = "";
                    _txtSearch.ForeColor = Color.Black;
                }
            };
            _txtSearch.Leave += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(_txtSearch.Text))
                {
                    _txtSearch.Text = SearchPlaceholder;
                    _txtSearch.ForeColor = Color.Gray;
                }
            };
            _txtSearch.TextChanged += (s, e) => FilterData(_txtSearch.Text);

            toolStrip.Items.AddRange(new ToolStripItem[] { _cbSearchColumn, _txtSearch });

            this.Controls.Add(_tabControl);
            this.Controls.Add(toolStrip);

            LoadAllData();
        }

        private void LoadAllData()
        {
            try
            {
                SaveCurrentTab();

                _db = new ApplicationDbContext();
                _tabControl.TabPages.Clear();
                
                var doctors = _db.Doctors.Include(d => d.Specialty).ToList();
                var specialties = _db.Specialties.ToList();
                var diagnoses = _db.Diagnoses.Include(d => d.Specialty).ToList();
                var testTypes = _db.TestTypes.ToList();
                var examinationRooms = _db.ExaminationRooms.Include(er => er.ResponsibleDoctor).ToList();

                var patientsData = _db.Patients
                    .Include(p => p.FamilyDoctor)
                    .ToList()
                    .Select(p => new
                    {
                        ID = p.PatientId,
                        ПІБ = p.FullName ?? "Не вказано",
                        Дата_народження = p.BirthDate.ToString("dd.MM.yyyy"),
                        Вік = CalculateAge(p.BirthDate),
                        Адреса = p.Address ?? "---",
                        Телефон = p.Phone ?? "---",
                        Email = p.Email ?? "---",
                        Сімейний_лікар = p.FamilyDoctor != null ? p.FamilyDoctor.FullName : "Не призначено",
                        Дата_реєстрації =
                            p.RegistrationDate != null ? p.RegistrationDate.ToString("dd.MM.yyyy") : "---",
                        Група_крові = p.BloodType ?? "---",
                        Алергії = p.Allergies ?? "---"
                    })
                    .ToList();


                var appointmentsData = _db.Appointments
                    .Select(a => new
                    {
                        ID = a.AppointmentId,
                        PatientId = a.PatientId,
                        Пацієнт = a.Patient != null ? a.Patient.FullName : "ID: " + a.PatientId,
                        DoctorId = a.DoctorId,
                        Лікар = a.Doctor != null ? a.Doctor.FullName : "ID: " + a.DoctorId,
                        Дата = a.AppointmentDate.HasValue ? a.AppointmentDate.Value.ToShortDateString() : "---",
                        Час = a.AppointmentTime.HasValue ? a.AppointmentTime.Value.ToString(@"hh\:mm") : "---",
                        Статус = !string.IsNullOrEmpty(a.Status) ? a.Status : "заплановано",
                        Нотатки = !string.IsNullOrEmpty(a.Notes) ? a.Notes : "",
                        Створено = a.CreateAt.HasValue ? a.CreateAt.Value.ToShortDateString() : ""
                    })
                    .ToList();

                var referralsData = _db.Referrals
                    .Select(r => new
                    {
                        ID = r.ReferralId,
                        Пацієнт_ID = r.PatientId,
                        Пацієнт = r.Patient != null ? r.Patient.FullName : "ID: " + r.PatientId,
                        Лікар_ID = r.FromDoctorId,
                        Лікар = r.FromDoctor != null ? r.FromDoctor.FullName : "ID: " + r.FromDoctorId,
                        Спеціальність_ID = r.ToSpecialtyId,
                        Спеціальність = r.ToSpecialty != null ? r.ToSpecialty.Name : "ID: " + r.ToSpecialtyId,
                        Причина = !string.IsNullOrEmpty(r.Reason)
                            ? (r.Reason.Length > 50 ? r.Reason.Substring(0, 47) + "..." : r.Reason)
                            : "---",
                        Дата_направлення = r.ReferralDate.HasValue ? r.ReferralDate.Value.ToShortDateString() : "---",
                        Дійсне_до = r.ExpiryDate.HasValue ? r.ExpiryDate.Value.ToShortDateString() : "---",
                        Статус = r.Used ? "✅ Використано" : "⏳ Очікує",
                        Пріоритет = !string.IsNullOrEmpty(r.Priority) ? r.Priority : "нормальний",
                        Нотатки = !string.IsNullOrEmpty(r.Notes)
                            ? (r.Notes.Length > 30 ? r.Notes.Substring(0, 27) + "..." : r.Notes)
                            : "---"
                    })
                    .ToList();

                var doctorsData = _db.Doctors
                    .Include(d => d.Specialty)
                    .Select(d => new
                    {
                        ID = d.DoctorId,
                        ПІБ = d.FullName ?? "Не вказано",
                        Спеціальність = d.Specialty != null ? d.Specialty.Name : "Не вказано",
                        SpecialtyId = d.SpecialtyId,
                        Кабінет = d.RoomNumber ?? "---",
                        Робочий_телефон = d.WorkPhone ?? "---",
                        Особистий_телефон = d.PersonalPhone ?? "---",
                        Email = d.Email ?? "---",
                        Дата_створення = d.CreateAt.HasValue
                            ? d.CreateAt.Value.ToString("dd.MM.yyyy")
                            : "---",
                        Пароль = d.Password,
                        Приймає_нових = d.IsAcceptingNewPatients
                    })
                    .ToList();

                var visitsData = _db.Visits
                    .Include(v => v.Appointment)
                    .ThenInclude(a => a.Patient)
                    .Include(v => v.Appointment)
                    .ThenInclude(a => a.Doctor)
                    .Include(v => v.Diagnosis)
                    .Select(v => new
                    {
                        ID = v.VisitId,
                        Запис_ID = v.AppointmentId,
                        Пацієнт = v.Appointment != null && v.Appointment.Patient != null
                            ? v.Appointment.Patient.FullName
                            : "---",
                        Лікар = v.Appointment != null && v.Appointment.Doctor != null
                            ? v.Appointment.Doctor.FullName
                            : "---",
                        Діагноз = v.Diagnosis != null ? v.Diagnosis.Name : "---",
                        Симптоми = !string.IsNullOrEmpty(v.Symptoms)
                            ? (v.Symptoms.Length > 30 ? v.Symptoms.Substring(0, 27) + "..." : v.Symptoms)
                            : "---",
                        Дата_візиту = v.VisitDate.HasValue ? v.VisitDate.Value.ToString("dd.MM.yyyy") : "---",
                        Наступний_візит = v.NextVisitDate.HasValue
                            ? v.NextVisitDate.Value.ToString("dd.MM.yyyy")
                            : "---"
                    })
                    .ToList();

                var testOrdersData = _db.TestOrders
                    .Include(to => to.Visit)
                    .ThenInclude(v => v.Appointment)
                    .ThenInclude(a => a.Patient)
                    .Include(to => to.TestType)
                    .Include(to => to.Doctor)
                    .Select(to => new
                    {
                        ID = to.OrderId,
                        Візит_ID = to.VisitId,
                        Пацієнт = to.Visit != null && to.Visit.Appointment != null &&
                                  to.Visit.Appointment.Patient != null
                            ? to.Visit.Appointment.Patient.FullName
                            : "---",
                        Тест = to.TestType != null ? to.TestType.Name : "---",
                        Лікар = to.Doctor != null ? to.Doctor.FullName : "---",
                        Дата_направлення = to.OrderDate.HasValue ? to.OrderDate.Value.ToString("dd.MM.yyyy") : "---",
                        Пріоритет = !string.IsNullOrEmpty(to.Priority) ? to.Priority : "плановий",
                        Статус = !string.IsNullOrEmpty(to.Status) ? to.Status : "призначено"
                    })
                    .ToList();

                var testAppointmentsData = _db.TestAppointments
                    .Include(ta => ta.TestOrder)
                    .ThenInclude(to => to.TestType)
                    .Include(ta => ta.Patient)
                    .Include(ta => ta.ExaminationRoom)
                    .Select(ta => new
                    {
                        ID = ta.TestAppId,
                        Направлення_ID = ta.OrderId,
                        Пацієнт = ta.Patient != null ? ta.Patient.FullName : "---",
                        Тест = ta.TestOrder != null && ta.TestOrder.TestType != null
                            ? ta.TestOrder.TestType.Name
                            : "---",
                        Кабінет = ta.ExaminationRoom != null ? ta.ExaminationRoom.RoomNumber : "---",
                        Дата = ta.ScheduledDate.ToString("dd.MM.yyyy"),
                        Час = ta.ScheduledTime.ToString(@"hh\:mm"),
                        Статус = !string.IsNullOrEmpty(ta.Status) ? ta.Status : "заплановано"
                    })
                    .ToList();

                var prescriptionsData = _db.Prescriptions
                    .Include(p => p.Visit)
                    .ThenInclude(v => v.Appointment)
                    .ThenInclude(a => a.Patient)
                    .Include(p => p.PrescribingDoctor)
                    .Select(p => new
                    {
                        ID = p.PrescriptionId,
                        Візит_ID = p.VisitId,
                        Пацієнт = p.Visit != null && p.Visit.Appointment != null && p.Visit.Appointment.Patient != null
                            ? p.Visit.Appointment.Patient.FullName
                            : "---",
                        Ліки = p.Medication,
                        Дозування = p.Dosage,
                        Частота = !string.IsNullOrEmpty(p.Frequency) ? p.Frequency : "---",
                        Тривалість = p.DurationDays.HasValue ? $"{p.DurationDays} дн." : "---",
                        Виписав = p.PrescribingDoctor != null ? p.PrescribingDoctor.FullName : "---"
                    })
                    .ToList();

                var medicalDocumentsData = _db.MedicalDocuments
                    .Include(md => md.Patient)
                    .Include(md => md.Doctor)
                    .Select(md => new
                    {
                        ID = md.DocumentId,
                        Пацієнт = md.Patient != null ? md.Patient.FullName : "---",
                        Лікар = md.Doctor != null ? md.Doctor.FullName : "---",
                        Тип_документу = md.DocumentType,
                        Дата_видачі = md.IssueDate.HasValue ? md.IssueDate.Value.ToString("dd.MM.yyyy") : "---"
                    })
                    .ToList();

                var testResultsData = _db.TestResults
                    .Include(tr => tr.TestAppointment)
                    .ThenInclude(ta => ta.Patient)
                    .Include(tr => tr.PerformedByDoctor)
                    .Select(tr => new
                    {
                        ID = tr.ResultId,
                        Запис_ID = tr.TestAppId,
                        Пацієнт = tr.TestAppointment != null && tr.TestAppointment.Patient != null
                            ? tr.TestAppointment.Patient.FullName
                            : "---",
                        Виконав = tr.ReviewedByDoctor != null ? tr.ReviewedByDoctor.FullName : "---",
                        Перевірив = tr.ReviewedByDoctor != null ? tr.ReviewedByDoctor.FullName : "---",
                        Дата_результату = tr.ResultDate.HasValue
                            ? tr.ResultDate.Value.ToString("dd.MM.yyyy HH:mm")
                            : "---",
                        Результат = !string.IsNullOrEmpty(tr.ResultText)
                            ? (tr.ResultText.Length > 30 ? tr.ResultText.Substring(0, 27) + "..." : tr.ResultText)
                            : "---",
                        Висновок = tr.Conclusion,
                        Аномальний = tr.IsAbnormal.HasValue ? (tr.IsAbnormal.Value ? "✅ Так" : "❌ Ні") : "---",
                        Нотатки_лікаря = tr.ReviewNotes
                    })
                    .ToList();

                var diagnosisTestsData = _db.DiagnosisTests
                    .Include(dt => dt.Diagnosis)
                    .Include(dt => dt.TestType)
                    .Select(dt => new
                    {
                        ID = dt.Id,
                        Діагноз = dt.Diagnosis != null ? dt.Diagnosis.Name : "---",
                        Тест = dt.TestType != null ? dt.TestType.Name : "---",
                        Обовязковий = dt.IsMandatory ? "✅ Так" : "❌ Ні",
                        Рекомендована_частота = !string.IsNullOrEmpty(dt.RecommendedFrequency)
                            ? dt.RecommendedFrequency
                            : "---"
                    })
                    .ToList();

                CreateStatisticsTab();

                AddTableTab("👨‍👩‍👧‍👦 Пацієнти", patientsData);
                AddTableTab("👨‍⚕️ Лікарі", doctorsData);

                AddTableTab("🧬 Спеціальності", specialties);
                AddTableTab("🧪 Види тестів", testTypes);
                AddTableTab("🚪 Кабінети", examinationRooms);
                AddTableTab("🩺 Діагнози", diagnoses);

                AddTableTab("📋 Записи", appointmentsData);
                AddTableTab("🏥 Візити", visitsData);
                AddTableTab("📜 Направлення", referralsData);
                AddTableTab("🔬 Направлення на тест", testOrdersData);
                AddTableTab("📅 Записи на тест", testAppointmentsData);

                AddTableTab("📊 Результати тестів", testResultsData);
                AddTableTab("🔗 Діагнози-Тести", diagnosisTestsData);
                AddTableTab("💊 Рецепти", prescriptionsData);
                AddTableTab("📄 Мед-документи", medicalDocumentsData);

                UpdateSearchColumns();
                RestoreLastTab();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження даних: {ex.Message}\nДеталі: {ex.InnerException?.Message}");
            }
        }

        private int CalculateAge(DateTime birthDate)
        {
            int age = DateTime.Now.Year - birthDate.Year;
            if (birthDate > DateTime.Now.AddYears(-age)) age--;
            return age;
        }

        private void CreateStatisticsTab()
        {
            var tabPage = new TabPage("📊 Статистика");
            var panel = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Padding = new Padding(20) };

            int yPos = 20;

            AddStatSectionTitle(panel, "📈 ЗАГАЛЬНА СТАТИСТИКА", ref yPos);

            using (var db = new ApplicationDbContext())
            {
                int totalPatients = db.Patients.Count();
                int totalDoctors = db.Doctors.Count();
                int totalAppointments = db.Appointments.Count();
                int totalVisits = db.Visits.Count();
                int totalReferrals = db.Referrals.Count();
                int totalPrescriptions = db.Prescriptions.Count();

                AddStatCard(panel, "👨‍👩‍👧‍👦 Пацієнти", totalPatients.ToString(), Color.FromArgb(52, 152, 219), ref yPos);
                AddStatCard(panel, "👨‍⚕️ Лікарі", totalDoctors.ToString(), Color.FromArgb(46, 204, 113), ref yPos);
                AddStatCard(panel, "📅 Записи", totalAppointments.ToString(), Color.FromArgb(155, 89, 182), ref yPos);
                AddStatCard(panel, "🏥 Візити", totalVisits.ToString(), Color.FromArgb(241, 196, 15), ref yPos);
                AddStatCard(panel, "📜 Направлення", totalReferrals.ToString(), Color.FromArgb(230, 126, 34), ref yPos);
                AddStatCard(panel, "💊 Рецепти", totalPrescriptions.ToString(), Color.FromArgb(231, 76, 60), ref yPos);

                yPos += 40;
                AddStatSectionTitle(panel, "📊 СТАТИСТИКА ЗАПИСІВ", ref yPos);

                var appointmentsByStatus = db.Appointments
                    .GroupBy(a => a.Status)
                    .Select(g => new { Status = g.Key, Count = g.Count() })
                    .ToList();

                foreach (var item in appointmentsByStatus)
                {
                    string status = item.Status ?? "Не вказано";
                    string statusText = GetStatusText(status);
                    Color statusColor = GetStatusColor(status);
                    AddStatRow(panel, statusText, item.Count.ToString(), statusColor, ref yPos);
                }

                yPos += 20;
                AddStatSectionTitle(panel, "🔬 СТАТИСТИКА НАПРАВЛЕНЬ", ref yPos);

                int usedReferrals = db.Referrals.Count(r => r.Used);
                int unusedReferrals = db.Referrals.Count(r => !r.Used);

                AddStatRow(panel, "✅ Використані", usedReferrals.ToString(), Color.Green, ref yPos);
                AddStatRow(panel, "⏳ Очікують", unusedReferrals.ToString(), Color.Orange, ref yPos);

                yPos += 20;
                AddStatSectionTitle(panel, "📅 СЬОГОДНІШНІ ЗАПИСИ", ref yPos);

                var today = DateTime.Today;
                var todayAppointments = db.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Doctor)
                    .Where(a => a.AppointmentDate == today)
                    .OrderBy(a => a.AppointmentTime)
                    .ToList();

                if (todayAppointments.Any())
                {
                    foreach (var app in todayAppointments)
                    {
                        string time = app.AppointmentTime?.ToString(@"hh\:mm") ?? "??";
                        string patient = app.Patient?.FullName ?? "Невідомо";
                        string doctor = app.Doctor?.FullName ?? "Невідомо";
                        AddStatRow(panel, $"{time} - {patient}", $"до {doctor}", Color.Black, ref yPos);
                    }
                }
                else
                {
                    AddStatRow(panel, "На сьогодні немає записів", "", Color.Gray, ref yPos);
                }
            }

            tabPage.Controls.Add(panel);
            _tabControl.TabPages.Add(tabPage);
        }

        private void AddStatSectionTitle(Panel panel, string title, ref int yPos)
        {
            var lbl = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                Location = new Point(20, yPos),
                AutoSize = true
            };
            panel.Controls.Add(lbl);
            yPos += 35;
        }

        private void AddStatCard(Panel panel, string label, string value, Color color, ref int yPos)
        {
            int cardsInRow = (panel.Controls.Count - 1) % 3;
            int xPos = 20 + (cardsInRow * 220);

            var cardPanel = new Panel
            {
                Location = new Point(xPos, yPos),
                Size = new Size(200, 80),
                BackColor = color,
                BorderStyle = BorderStyle.FixedSingle
            };

            var lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 10),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 30),
                AutoSize = true
            };

            cardPanel.Controls.Add(lblLabel);
            cardPanel.Controls.Add(lblValue);
            panel.Controls.Add(cardPanel);

            if (cardsInRow == 2)
            {
                yPos += 100;
            }
        }

        private void AddStatRow(Panel panel, string label, string value, Color color, ref int yPos)
        {
            var lblLabel = new Label
            {
                Text = label,
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                Location = new Point(40, yPos),
                AutoSize = true
            };

            var lblValue = new Label
            {
                Text = value,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = color,
                Location = new Point(300, yPos),
                AutoSize = true
            };

            panel.Controls.Add(lblLabel);
            panel.Controls.Add(lblValue);
            yPos += 30;
        }

        private void AddTableTab(string title, object dataSource)
        {
            var tabPage = new TabPage(title);
            var grid = new DataGridView
            {
                Dock = DockStyle.Fill,
                DataSource = dataSource,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
                BackgroundColor = Color.White,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                ReadOnly = true,
                AllowUserToOrderColumns = true
            };

            grid.DataError += (s, e) => e.ThrowException = false;
            tabPage.Controls.Add(grid);
            _tabControl.TabPages.Add(tabPage);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _db?.Dispose();
            Application.Exit();
        }

        private void EditSelected()
        {
            var grid = _tabControl.SelectedTab?.Controls.OfType<DataGridView>().FirstOrDefault();
            if (grid?.CurrentRow == null) return;

            string tab = _tabControl.SelectedTab?.Text ?? "";

            try
            {
                if (tab.Contains("📋 Записи") || tab.Contains("Запис") && !tab.Contains("на тест"))
                {
                    var appointment = GetEntityFromRow(grid) as Appointment;
                    if (appointment != null)
                    {
                        using (var form = new EditAppointmentStatusForm(appointment))
                        {
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                var entry = _db.Entry(appointment);
                                entry.Property(a => a.Status).IsModified = true;
                                entry.Property(a => a.Notes).IsModified = true;

                                SaveChanges();
                            }
                        }

                        return;
                    }
                }

                object entity = GetEntityFromRow(grid);
                if (entity != null)
                {
                    if (entity is Appointment app)
                    {
                        using (var form = new EditAppointmentStatusForm(app))
                        {
                            if (form.ShowDialog() == DialogResult.OK)
                            {
                                var entry = _db.Entry(app);
                                entry.Property(a => a.Status).IsModified = true;
                                entry.Property(a => a.Notes).IsModified = true;
                                SaveChanges();
                            }
                        }
                    }
                    else
                    {
                        using (var f = new UniversalEditForm(entity, tab))
                        {
                            if (f.ShowDialog() == DialogResult.OK)
                                SaveChanges();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при редагуванні: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private object? GetEntityFromRow(DataGridView grid)
        {
            if (grid.CurrentRow == null || grid.CurrentRow.DataBoundItem == null)
                return null;

            string tab = _tabControl.SelectedTab?.Text ?? "";
            var dataItem = grid.CurrentRow.DataBoundItem;

            if (dataItem is Patient || dataItem is Doctor || dataItem is Specialty ||
                dataItem is Referral || dataItem is Appointment || dataItem is Visit ||
                dataItem is Diagnosis || dataItem is TestType || dataItem is ExaminationRoom ||
                dataItem is TestOrder || dataItem is TestAppointment || dataItem is TestResult ||
                dataItem is Prescription || dataItem is MedicalDocument || dataItem is DiagnosisTest)
            {
                return dataItem;
            }

            try
            {
                int id = 0;

                string[] possibleIdColumns =
                {
                    "ID", "Id",
                    "PatientId", "DoctorId", "AppointmentId", "ReferralId", "SpecialtyId",
                    "DiagnosisId", "VisitId", "TestTypeId", "RoomId", "OrderId",
                    "TestAppId", "ResultId", "PrescriptionId", "DocumentId"
                };

                foreach (var columnName in possibleIdColumns)
                {
                    if (grid.Columns.Contains(columnName))
                    {
                        var cellValue = grid.CurrentRow.Cells[columnName].Value;
                        if (cellValue != null && cellValue != DBNull.Value)
                        {
                            id = Convert.ToInt32(cellValue);
                            break;
                        }
                    }
                }

                if (id == 0)
                {
                    foreach (DataGridViewCell cell in grid.CurrentRow.Cells)
                    {
                        if (cell.Value != null && int.TryParse(cell.Value.ToString(), out id))
                        {
                            break;
                        }
                    }
                }

                if (id == 0) return null;

                if (tab.Contains("Запис") && !tab.Contains("на тест") || tab.Contains("📋 Записи"))
                {
                    return _db.Appointments
                        .Include(a => a.Patient)
                        .Include(a => a.Doctor)
                        .FirstOrDefault(a => a.AppointmentId == id);
                }

                if (tab.Contains("Пацієнт"))
                    return _db.Patients.Find(id);

                if (tab.Contains("Лікар"))
                    return _db.Doctors.Find(id);

                if (tab.Contains("Спеціальност"))
                    return _db.Specialties.Find(id);

                if (tab.Contains("Направлення на тест"))
                    return _db.TestOrders.Find(id);

                if (tab.Contains("Направлення") && !tab.Contains("на тест"))
                    return _db.Referrals.Find(id);

                if (tab.Contains("Записи на тест"))
                    return _db.TestAppointments.Find(id);

                if (tab.Contains("Візит"))
                    return _db.Visits.Find(id);

                if (tab.Contains("Діагноз"))
                    return _db.Diagnoses.Find(id);

                if (tab.Contains("Вид тест"))
                    return _db.TestTypes.Find(id);

                if (tab.Contains("Кабінет"))
                    return _db.ExaminationRooms.Find(id);

                if (tab.Contains("Результат"))
                    return _db.TestResults.Find(id);

                if (tab.Contains("Рецепт"))
                    return _db.Prescriptions.Find(id);

                if (tab.Contains("документ"))
                    return _db.MedicalDocuments.Find(id);

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка отримання сутності: {ex.Message}");
                return null;
            }
        }

        private void DeleteSelected()
        {
            var grid = _tabControl.SelectedTab?.Controls.OfType<DataGridView>().FirstOrDefault();
            if (grid?.CurrentRow == null) return;

            var item = GetEntityFromRow(grid);
            if (item == null)
            {
                MessageBox.Show("Не вдалося знайти об'єкт для видалення.");
                return;
            }

            if (MessageBox.Show("Ви впевнені, що хочете видалити цей запис?", "Підтвердження", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                try
                {
                    _db.Remove(item);
                    _db.SaveChanges();
                    LoadAllData();
                    MessageBox.Show("Успішно видалено!");
                }
                catch (DbUpdateException)
                {
                    MessageBox.Show("Неможливо видалити цей запис! На нього посилаються дані в інших таблицях.\n" +
                                    "Спочатку видаліть пов'язані візити, результати або рецепти.",
                        "Помилка цілісності", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    _db.Entry(item).State = EntityState.Unchanged;
                    _db.ChangeTracker.Clear();
                    _db = new ApplicationDbContext();
                    LoadAllData();
                }
            }
        }

        private void SaveChanges()
        {
            try
            {
                var validationErrors = new List<string>();

                foreach (var entry in _db.ChangeTracker.Entries()
                             .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
                {
                    var validationContext = new ValidationContext(entry.Entity);
                    var validationResults = new List<ValidationResult>();

                    if (!Validator.TryValidateObject(entry.Entity, validationContext, validationResults, true))
                    {
                        foreach (var validationResult in validationResults)
                        {
                            validationErrors.Add($"{entry.Entity.GetType().Name}: {validationResult.ErrorMessage}");
                        }
                    }
                }

                if (validationErrors.Any())
                {
                    MessageBox.Show($"Помилки валідації:\n{string.Join("\n", validationErrors)}",
                        "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int createdVisits = 0;
                foreach (var entry in _db.ChangeTracker.Entries<Appointment>()
                             .Where(e => e.State == EntityState.Added))
                {
                    bool visitExists = _db.Visits.Any(v => v.AppointmentId == entry.Entity.AppointmentId);

                    if (!visitExists && entry.Entity.AppointmentDate.HasValue)
                    {
                        var newVisit = new Visit
                        {
                            AppointmentId = entry.Entity.AppointmentId,
                            VisitDate = entry.Entity.AppointmentDate.Value,
                            Symptoms = entry.Entity.Notes ?? "Плановий візит",
                            ReferralNeeded = false,
                            VisitNotes = "Автоматично створений після запису"
                        };
                        _db.Visits.Add(newVisit);
                        createdVisits++;
                    }
                }

                if (_db.ChangeTracker.HasChanges())
                {
                    int saved = _db.SaveChanges();
                    string message = $"Збережено змін: {saved}";
                    if (createdVisits > 0)
                    {
                        message += $"\nАвтоматично створено візитів: {createdVisits}";
                    }

                    MessageBox.Show(message, "Успіх",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAllData();
                }
                else
                {
                    MessageBox.Show("Немає змін для збереження.", "Інформація",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (DbUpdateException ex)
            {
                string errorMessage = "Помилка збереження в базу:\n";
                errorMessage += ex.InnerException?.Message ?? ex.Message;

                MessageBox.Show(errorMessage, "Помилка бази даних",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);

                _db.ChangeTracker.Clear();
                _db = new ApplicationDbContext();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Загальна помилка: {ex.Message}\n{ex.InnerException?.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                _db = new ApplicationDbContext();
            }
        }

        private void CreateTestResultForCompletedTest(int testAppointmentId)
        {
            try
            {
                var testApp = _db.TestAppointments
                    .Include(ta => ta.TestOrder)
                    .ThenInclude(to => to.TestType)
                    .FirstOrDefault(ta => ta.TestAppId == testAppointmentId);

                if (testApp == null) return;

                bool resultExists = _db.TestResults.Any(tr => tr.TestAppId == testAppointmentId);
                if (resultExists) return;

                var newResult = new TestResult
                {
                    TestAppId = testAppointmentId,
                    PerformedBy = testApp.TechnicianId ?? 1,
                    ResultDate = DateTime.Now,
                    ResultText =
                        $"Тест '{testApp.TestOrder?.TestType?.Name ?? "невідомий"}' виконано. Очікуйте висновок лікаря.",
                    Conclusion = "Очікує обробки",
                    IsAbnormal = null,
                    ReviewedByDoctorId = testApp.TestOrder?.DoctorId
                };

                _db.TestResults.Add(newResult);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка створення результату: {ex.Message}");
            }
        }

        private void OpenAddAppointment()
        {
            using (var form = new AddAppointmentForm(_db))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    SaveChanges();
                    LoadAllData();
                }
            }
        }

        private void OpenAddReferral()
        {
            using (var form = new AddReferralForm(_db))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    _db.Referrals.Add(form.NewReferral);
                    SaveChanges();
                    LoadAllData();
                }
            }
        }

        private void OpenAddTestOrder()
        {
            try
            {
                using (var form = new AddTestOrderForm(_db))
                {
                    if (form.ShowDialog() == DialogResult.OK)
                    {
                        LoadAllData();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося відкрити форму: {ex.Message}",
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddNewRow()
        {
            var currentTab = _tabControl.SelectedTab?.Text;
            if (string.IsNullOrEmpty(currentTab)) return;

            try
            {
                switch (currentTab)
                {
                    case string tab when tab.Contains("👨‍👩‍👧‍👦 Пацієнти") ||
                                         tab.Contains("Пацієнт"):
                        AddNewPatient();
                        break;

                    case string tab when tab.Contains("👨‍⚕️ Лікарі") ||
                                         tab.Contains("Лікар"):
                        AddNewDoctor();
                        break;

                    case string tab when tab.Contains("🧬 Спеціальності") ||
                                         tab.Contains("Спеціальність"):
                        AddNewSpecialty();
                        break;

                    case string tab when tab.Contains("🧪 Види тестів") ||
                                         tab.Contains("Вид тест"):
                        AddNewTestType();
                        break;

                    case string tab when tab.Contains("🚪 Кабінети") ||
                                         tab.Contains("Кабінет"):
                        AddNewExaminationRoom();
                        break;

                    case string tab when tab.Contains("🩺 Діагнози") ||
                                         tab.Contains("Діагноз") && !tab.Contains("Тест"):
                        AddNewDiagnosis();
                        break;

                    case string tab when tab.Contains("📋 Записи") && !tab.Contains("на тест"):
                        OpenAddAppointment();
                        break;

                    case string tab when tab.Contains("📜 Направлення") && !tab.Contains("на тест"):
                        OpenAddReferral();
                        break;

                    case string tab when tab.Contains("🏥 Візити") ||
                                         tab.Contains("Візит"):
                        AddNewVisit();
                        break;

                    case string tab when tab.Contains("🔬 Направлення на тест"):
                        OpenAddTestOrder();
                        break;

                    case string tab when tab.Contains("📅 Записи на тест"):
                        AddNewTestAppointment();
                        break;

                    case string tab when tab.Contains("💊 Рецепти") ||
                                         tab.Contains("Рецепт"):
                        AddNewPrescription();
                        break;

                    case string tab when tab.Contains("📄 Мед-документи") ||
                                         tab.Contains("Меддокумент"):
                        AddNewMedicalDocument();
                        break;

                    case string tab when tab.Contains("📊 Результати тестів") ||
                                         tab.Contains("Результат тест"):
                        AddNewTestResult();
                        break;

                    case string tab when tab.Contains("🔗 Діагнози-Тести"):
                        AddNewDiagnosisTest();
                        break;

                    default:
                        MessageBox.Show($"Додавання для таблиці '{currentTab}' не підтримується",
                            "Інформація", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка при додаванні: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddNewPatient()
        {
            var newPatient = new Patient
            {
                RegistrationDate = DateTime.Now,
                BirthDate = DateTime.Today.AddYears(-30)
            };

            using (var form = new UniversalEditForm(newPatient, "Новий пацієнт"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var patientToAdd = form.TargetObject as Patient;
                        if (patientToAdd != null)
                        {
                            _db.Patients.Add(patientToAdd);
                            SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження пацієнта: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewDoctor()
        {
            var newDoctor = new Doctor();

            using (var form = new UniversalEditForm(newDoctor, "Новий лікар"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var doctorToAdd = form.TargetObject as Doctor;
                        if (doctorToAdd != null)
                        {
                            _db.Doctors.Add(doctorToAdd);
                            SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження лікаря: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewSpecialty()
        {
            var newSpecialty = new Specialty();

            using (var form = new UniversalEditForm(newSpecialty, "Нова спеціальність"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var specialtyToAdd = form.TargetObject as Specialty;
                        if (specialtyToAdd != null)
                        {
                            specialtyToAdd.SpecialtyId = 0; //
                            _db.Specialties.Add(specialtyToAdd);
                            SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження спеціальності: {ex.Message}", "Помилка",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void AddNewDiagnosis()
        {
            var newDiagnosis = new Diagnosis();

            using (var form = new UniversalEditForm(newDiagnosis, "Новий діагноз"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _db.Diagnoses.Add(newDiagnosis);
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження діагнозу: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewTestType()
        {
            var newTestType = new TestType();

            using (var form = new UniversalEditForm(newTestType, "Новий вид тесту"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _db.TestTypes.Add(newTestType);
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження виду тесту: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewExaminationRoom()
        {
            var newRoom = new ExaminationRoom();

            using (var form = new UniversalEditForm(newRoom, "Новий кабінет"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _db.ExaminationRooms.Add(newRoom);
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження кабінету: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewVisit()
        {
            var newVisit = new Visit
            {
                VisitDate = DateTime.Now
            };

            using (var form = new UniversalEditForm(newVisit, "Новий візит"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _db.Visits.Add(newVisit);
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження візиту: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewTestAppointment()
        {
            var newTestAppointment = new TestAppointment
            {
                ScheduledDate = DateTime.Now,
                ScheduledTime = DateTime.Now.TimeOfDay
            };

            using (var form = new UniversalEditForm(newTestAppointment, "Новий запис на тест"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _db.TestAppointments.Add(newTestAppointment);
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження запису на тест: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewPrescription()
        {
            var newPrescription = new Prescription
            {
                StartDate = DateTime.Now
            };

            using (var form = new UniversalEditForm(newPrescription, "Новий рецепт"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _db.Prescriptions.Add(newPrescription);
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження рецепту: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewMedicalDocument()
        {
            var newDocument = new MedicalDocument
            {
                IssueDate = DateTime.Now
            };

            using (var form = new UniversalEditForm(newDocument, "Новий медичний документ"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _db.MedicalDocuments.Add(newDocument);
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження документу: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewTestResult()
        {
            var newTestResult = new TestResult
            {
                ResultDate = DateTime.Now
            };

            using (var form = new UniversalEditForm(newTestResult, "Новий результат тесту"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _db.TestResults.Add(newTestResult);
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження результату тесту: {ex.Message}");
                    }
                }
            }
        }

        private void AddNewDiagnosisTest()
        {
            var newDiagnosisTest = new DiagnosisTest();

            using (var form = new UniversalEditForm(newDiagnosisTest, "Новий зв'язок діагноз-тест"))
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        _db.DiagnosisTests.Add(newDiagnosisTest);
                        SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Помилка збереження зв'язку: {ex.Message}");
                    }
                }
            }
        }

        private void UpdateSearchColumns()
        {
            var grid = _tabControl.SelectedTab?.Controls.OfType<DataGridView>().FirstOrDefault();
            if (grid == null) return;
            _cbSearchColumn.Items.Clear();
            _cbSearchColumn.Items.Add("Усі стовпці");
            foreach (DataGridViewColumn col in grid.Columns) _cbSearchColumn.Items.Add(col.HeaderText);
            _cbSearchColumn.SelectedIndex = 0;
        }

        private void FilterData(string text)
        {
            if (text == SearchPlaceholder) return;
            var grid = _tabControl.SelectedTab?.Controls.OfType<DataGridView>().FirstOrDefault();
            if (grid == null) return;

            string searchText = text.ToLower().Trim();
            string selectedCol = _cbSearchColumn.SelectedItem?.ToString();

            CurrencyManager cm = (CurrencyManager)grid.BindingContext[grid.DataSource];
            cm.SuspendBinding();

            foreach (DataGridViewRow row in grid.Rows)
            {
                if (row.IsNewRow) continue;
                bool isVisible = false;

                if (string.IsNullOrWhiteSpace(searchText)) isVisible = true;
                else if (selectedCol == "Усі стовпці")
                    isVisible = row.Cells.Cast<DataGridViewCell>()
                        .Any(c => c.Value?.ToString().ToLower().Contains(searchText) == true);
                else
                {
                    var cell = row.Cells.Cast<DataGridViewCell>()
                        .FirstOrDefault(c => grid.Columns[c.ColumnIndex].HeaderText == selectedCol);
                    isVisible = cell?.Value?.ToString().ToLower().Contains(searchText) == true;
                }

                row.Visible = isVisible;
            }

            cm.ResumeBinding();
        }

        private string GetStatusText(string status)
        {
            return status switch
            {
                "заплановано" => "📅 Заплановано",
                "відвідано" => "✅ Відвідано",
                "скасовано" => "❌ Скасовано",
                "перенесено" => "⏳ Перенесено",
                _ => status ?? "Невідомо"
            };
        }

        private Color GetStatusColor(string status)
        {
            return status switch
            {
                "заплановано" => Color.Blue,
                "відвідано" => Color.Green,
                "скасовано" => Color.Red,
                "перенесено" => Color.Orange,
                _ => Color.Black
            };
        }
    }

    public class UniversalEditForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object TargetObject { get; private set; }

        private Dictionary<Control, PropertyInfo> _controlProperties = new Dictionary<Control, PropertyInfo>();

        public UniversalEditForm(object obj, string title)
        {
            TargetObject = obj;
            this.Text = "Редагування: " + title;
            this.Size = new Size(500, 800);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            var mainPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(20),
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            var properties = obj.GetType().GetProperties()
                .Where(p => p.CanWrite &&
                            !p.PropertyType.IsClass ||
                            p.PropertyType == typeof(string) ||
                            p.PropertyType.IsEnum)
                .ToList();

            foreach (var prop in properties)
            {
                if (prop.PropertyType.IsClass &&
                    prop.PropertyType != typeof(string) &&
                    !prop.PropertyType.IsEnum)
                    continue;

                var label = new Label
                {
                    Text = GetDisplayName(prop) + ":",
                    Width = 400,
                    Height = 25,
                    Font = new Font(this.Font, FontStyle.Bold)
                };
                mainPanel.Controls.Add(label);

                Control inputControl = CreateInputControl(prop, obj);
                if (inputControl != null)
                {
                    inputControl.Width = 400;
                    inputControl.Height = 35;

                    if (prop.PropertyType == typeof(string) &&
                        (prop.Name.Contains("Notes") || prop.Name.Contains("Description")))
                    {
                        inputControl.Height = 80;
                    }

                    _controlProperties[inputControl] = prop;
                    mainPanel.Controls.Add(inputControl);
                }
            }

            var saveButton = new Button
            {
                Text = "💾 ЗБЕРЕГТИ",
                Width = 200,
                Height = 45,
                BackColor = Color.LightGreen,
                Font = new Font(this.Font, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            saveButton.Location = new Point(
                (this.ClientSize.Width - saveButton.Width) / 2
            );
            saveButton.Click += (s, e) =>
            {
                SaveChanges();
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            var buttonPanel = new Panel
            {
                Width = mainPanel.Width,
                Height = 60,
                Margin = new Padding(0, 20, 0, 0)
            };
            buttonPanel.Controls.Add(saveButton);
            saveButton.Left = (buttonPanel.Width - saveButton.Width) / 2;

            mainPanel.Controls.Add(buttonPanel);
            this.Controls.Add(mainPanel);
        }

        private string GetDisplayName(PropertyInfo prop)
        {
            var dict = new Dictionary<string, string>
            {
                { "FullName", "Повне ім'я" },
                { "BirthDate", "Дата народження" },
                { "Address", "Адреса" },
                { "Phone", "Телефон" },
                { "Email", "Електронна пошта" },
                { "RegistrationDate", "Дата реєстрації" },
                { "BloodType", "Група крові" },
                { "Allergies", "Алергії" },
                { "Password", "Пароль" },
                { "RoomNumber", "Кабінет" },
                { "WorkPhone", "Робочий телефон" },
                { "PersonalPhone", "Особистий телефон" },
                { "SpecialtyId", "ID спеціальності" },
                { "Code", "Код" },
                { "Name", "Назва" },
                { "IsFamily", "Сімейний лікар" },
                { "Description", "Опис" }
            };

            return dict.ContainsKey(prop.Name) ? dict[prop.Name] : prop.Name;
        }

        private Control CreateInputControl(PropertyInfo prop, object obj)
        {
            var propType = prop.PropertyType;
            var currentValue = prop.GetValue(obj);

            if (propType == typeof(DateTime?) || propType == typeof(DateTime))
            {
                var datePicker = new DateTimePicker
                {
                    Format = DateTimePickerFormat.Custom,
                    CustomFormat = "dd.MM.yyyy",
                    ShowUpDown = false,
                    ShowCheckBox = propType == typeof(DateTime?)
                };

                if (currentValue != null)
                {
                    datePicker.Value = (DateTime)currentValue;
                }

                return datePicker;
            }
            else if (propType == typeof(bool) || propType == typeof(bool?))
            {
                var checkBox = new CheckBox
                {
                    Text = "",
                    Checked = currentValue != null && (bool)currentValue,
                    ThreeState = propType == typeof(bool?)
                };

                return checkBox;
            }
            else if (propType.IsEnum)
            {
                var comboBox = new ComboBox
                {
                    DropDownStyle = ComboBoxStyle.DropDownList
                };

                foreach (var value in Enum.GetValues(propType))
                {
                    comboBox.Items.Add(value);
                }

                if (currentValue != null)
                {
                    comboBox.SelectedItem = currentValue;
                }

                return comboBox;
            }
            else
            {
                var textBox = new TextBox();
                textBox.Text = currentValue?.ToString() ?? "";

                if (prop.Name.Contains("Password", StringComparison.OrdinalIgnoreCase))
                {
                    textBox.UseSystemPasswordChar = true;
                }

                if (propType == typeof(string) &&
                    (prop.Name.Contains("Notes") ||
                     prop.Name.Contains("Description") ||
                     prop.Name.Contains("Allergies")))
                {
                    textBox.Multiline = true;
                    textBox.Height = 80;
                    textBox.ScrollBars = ScrollBars.Vertical;
                }

                return textBox;
            }
        }

        private void SaveChanges()
        {
            foreach (var kvp in _controlProperties)
            {
                var control = kvp.Key;
                var prop = kvp.Value;

                try
                {
                    object newValue = GetValueFromControl(control, prop.PropertyType);
                    prop.SetValue(TargetObject, newValue);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка при збереженні {prop.Name}: {ex.Message}");
                }
            }

            if (TargetObject is TestAppointment testApp &&
                testApp.Status == "виконано" &&
                testApp.TestAppId > 0)
            {
                var adminForm = Application.OpenForms.OfType<AdminMainForm>().FirstOrDefault();
                if (adminForm != null)
                {
                    var method = adminForm.GetType().GetMethod("CreateTestResultForCompletedTest",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    if (method != null)
                    {
                        method.Invoke(adminForm, new object[] { testApp.TestAppId });
                    }
                }
            }
        }

        private object GetValueFromControl(Control control, Type targetType)
        {
            if (control is DateTimePicker datePicker)
            {
                if (datePicker.ShowCheckBox && !datePicker.Checked)
                    return targetType == typeof(DateTime?) ? null : DateTime.MinValue;

                return datePicker.Value;
            }
            else if (control is CheckBox checkBox)
            {
                if (targetType == typeof(bool?))
                    return checkBox.CheckState == CheckState.Indeterminate ? (bool?)null : checkBox.Checked;

                return checkBox.Checked;
            }
            else if (control is ComboBox comboBox)
            {
                return comboBox.SelectedItem;
            }
            else if (control is TextBox textBox)
            {
                string text = textBox.Text.Trim();

                if (string.IsNullOrEmpty(text))
                {
                    if (targetType == typeof(string))
                    {
                        if (textBox.UseSystemPasswordChar)
                            return "12345";

                        return "";
                    }

                    return targetType.IsValueType ? Activator.CreateInstance(targetType) : null;
                }

                if (targetType == typeof(int) || targetType == typeof(int?))
                {
                    if (int.TryParse(text, out int intResult)) return intResult;
                    return 0;
                }

                if (targetType == typeof(decimal) || targetType == typeof(decimal?))
                {
                    if (decimal.TryParse(text, out decimal decResult)) return decResult;
                    return 0m;
                }

                if (targetType == typeof(string))
                    return text;
            }

            return null;
        }
    }

    public class EditAppointmentStatusForm : Form
    {
        private Appointment _appointment;
        private ComboBox cmbStatus;
        private TextBox txtNotes;
        private Button btnSave;
        private Button btnCancel;
        private Label lblInfo;

        public EditAppointmentStatusForm(Appointment appointment)
        {
            _appointment = appointment;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "Зміна статусу запису";
            this.Size = new Size(450, 250);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            lblInfo = new Label
            {
                Text = $"Запис #{_appointment.AppointmentId} | " +
                       $"Дата: {(_appointment.AppointmentDate?.ToString("dd.MM.yyyy") ?? "не вказано")} " +
                       $"Час: {(_appointment.AppointmentTime?.ToString(@"hh\:mm") ?? "не вказано")}",
                Location = new Point(20, 20),
                Width = 400,
                Font = new Font("Segoe UI", 9, FontStyle.Italic),
                ForeColor = Color.Gray
            };

            var lblStatus = new Label
            {
                Text = "Статус:",
                Location = new Point(20, 60),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            cmbStatus = new ComboBox
            {
                Location = new Point(120, 57),
                Width = 250,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            cmbStatus.Items.AddRange(new object[]
            {
                "заплановано",
                "відвідано",
                "скасовано",
                "перенесено"
            });
            cmbStatus.SelectedItem = _appointment.Status ?? "заплановано";

            var lblNotes = new Label
            {
                Text = "Нотатки:",
                Location = new Point(20, 100),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            txtNotes = new TextBox
            {
                Location = new Point(120, 97),
                Width = 250,
                Text = _appointment.Notes ?? "",
                Font = new Font("Segoe UI", 10)
            };

            btnSave = new Button
            {
                Text = "💾 Зберегти",
                Location = new Point(120, 150),
                Size = new Size(120, 35),
                BackColor = Color.LightGreen,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;

            btnCancel = new Button
            {
                Text = "Скасувати",
                Location = new Point(250, 150),
                Size = new Size(120, 35),
                BackColor = Color.LightCoral,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            this.Controls.AddRange(new Control[]
            {
                lblInfo,
                lblStatus, cmbStatus,
                lblNotes, txtNotes,
                btnSave, btnCancel
            });
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            _appointment.Status = cmbStatus.SelectedItem.ToString();
            _appointment.Notes = txtNotes.Text;

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}