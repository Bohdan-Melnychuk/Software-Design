using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Clinic_BD.Data;
using Clinic_BD.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.ComponentModel;

namespace Clinic_BD.Forms.Admin
{
    public partial class AddTestOrderForm : Form
    {
        private ApplicationDbContext _db;
        private ComboBox _cbPatient;
        private ComboBox _cbDoctor;
        private ComboBox _cbTestType;
        private ComboBox _cbVisit;
        private ComboBox _cbRoom;
        private DateTimePicker _dtpDate;
        private DateTimePicker _dtpTime;
        private RadioButton _rbPlanned;
        private RadioButton _rbUrgent;
        private CheckBox _chkCreateAppointment;
        private CheckBox _chkForDiagnosis;
        private TextBox _txtReason;
        private TextBox _txtNotes;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestOrder CreatedTestOrder { get; private set; }
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public TestAppointment CreatedTestAppointment { get; private set; }

        public AddTestOrderForm(ApplicationDbContext db)
        {
            _db = db;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            this.Text = "Нове направлення на тест";
            this.Size = new Size(600, 900);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Font = new Font("Segoe UI", 10);

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                Padding = new Padding(15)
            };

            int top = 15;
            int left = 15;
            int width = 550;

            var lblPatient = new Label
            {
                Text = "Пацієнт: *",
                Top = top,
                Left = left,
                Width = width,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            top += 25;

            _cbPatient = new ComboBox
            {
                Top = top,
                Left = left,
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DropDownHeight = 200
            };
            _cbPatient.SelectedIndexChanged += CbPatient_SelectedIndexChanged;
            top += 45;

            var lblDoctor = new Label
            {
                Text = "Лікар: *",
                Top = top,
                Left = left,
                Width = width,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            top += 25;

            _cbDoctor = new ComboBox
            {
                Top = top,
                Left = left,
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DropDownHeight = 200
            };
            top += 45;

            var lblTestType = new Label
            {
                Text = "Вид тесту: *",
                Top = top,
                Left = left,
                Width = width,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            top += 25;

            _cbTestType = new ComboBox
            {
                Top = top,
                Left = left,
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList,
                DropDownHeight = 200
            };
            _cbTestType.SelectedIndexChanged += CbTestType_SelectedIndexChanged;
            top += 45;

            var lblReason = new Label
            {
                Text = "Причина: *",
                Top = top,
                Left = left,
                Width = width,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            top += 25;

            _txtReason = new TextBox
            {
                Top = top,
                Left = left,
                Width = width,
                Height = 60,
                Multiline = true
            };
            top += 70;

            var lblVisit = new Label
            {
                Text = "Візит (необов'язково):",
                Top = top,
                Left = left,
                Width = width
            };
            top += 25;

            _cbVisit = new ComboBox
            {
                Top = top,
                Left = left,
                Width = width,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Enabled = false
            };
            top += 50;

            _chkForDiagnosis = new CheckBox
            {
                Text = "Тест необхідний для діагнозу",
                Top = top,
                Left = left,
                Width = width
            };
            top += 35;

            _chkCreateAppointment = new CheckBox
            {
                Text = "Створити запис на тест",
                Top = top,
                Left = left,
                Width = width,
                Checked = true,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            _chkCreateAppointment.CheckedChanged += ChkCreateAppointment_CheckedChanged;
            top += 35;

            var dateTimePanel = new Panel
            {
                Top = top,
                Left = left,
                Width = width,
                Height = 35,
                Enabled = false
            };

            var lblDate = new Label
            {
                Text = "Дата:",
                Top = 8,
                Left = 0,
                Width = 50
            };

            _dtpDate = new DateTimePicker
            {
                Top = 5,
                Left = 50,
                Width = 120,
                Value = DateTime.Today.AddDays(1),
                Format = DateTimePickerFormat.Short
            };

            var lblTime = new Label
            {
                Text = "Час:",
                Top = 8,
                Left = 180,
                Width = 50
            };

            _dtpTime = new DateTimePicker
            {
                Top = 5,
                Left = 230,
                Width = 100,
                Value = DateTime.Today.AddHours(10),
                Format = DateTimePickerFormat.Time,
                ShowUpDown = true
            };

            var lblRoom = new Label
            {
                Text = "Кабінет:",
                Top = 8,
                Left = 340,
                Width = 60
            };

            _cbRoom = new ComboBox
            {
                Top = 5,
                Left = 400,
                Width = 150,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            dateTimePanel.Controls.AddRange(new Control[] { lblDate, _dtpDate, lblTime, _dtpTime, lblRoom, _cbRoom });
            top += 45;

            var lblPriority = new Label
            {
                Text = "Пріоритет:",
                Top = top,
                Left = left,
                Width = width
            };
            top += 25;

            var priorityPanel = new Panel
            {
                Top = top,
                Left = left,
                Width = width,
                Height = 35
            };

            _rbPlanned = new RadioButton
            {
                Text = "Плановий",
                Left = 0,
                Top = 8,
                Width = 100,
                Checked = true
            };

            _rbUrgent = new RadioButton
            {
                Text = "Терміновий",
                Left = 110,
                Top = 8,
                Width = 100
            };

            priorityPanel.Controls.AddRange(new Control[] { _rbPlanned, _rbUrgent });
            top += 45;

            var lblNotes = new Label
            {
                Text = "Нотатки:",
                Top = top,
                Left = left,
                Width = width
            };
            top += 25;

            _txtNotes = new TextBox
            {
                Top = top,
                Left = left,
                Width = width,
                Height = 70,
                Multiline = true
            };
            top += 85;

            var btnSave = new Button
            {
                Text = "Зберегти",
                Top = top,
                Left = left + 150,
                Width = 120,
                Height = 40,
                BackColor = Color.LightGreen,
                Font = new Font(this.Font, FontStyle.Bold)
            };
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button
            {
                Text = "Скасувати",
                Top = top,
                Left = left + 280,
                Width = 120,
                Height = 40,
                BackColor = Color.LightGray
            };
            btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            mainPanel.Controls.AddRange(new Control[]
            {
                lblPatient, _cbPatient,
                lblDoctor, _cbDoctor,
                lblTestType, _cbTestType,
                lblReason, _txtReason,
                lblVisit, _cbVisit,
                _chkForDiagnosis,
                _chkCreateAppointment,
                dateTimePanel,
                lblPriority, priorityPanel,
                lblNotes, _txtNotes,
                btnSave, btnCancel
            });

            this.Controls.Add(mainPanel);
        }

        private void LoadData()
        {
            try
            {
                var patients = _db.Patients
                    .OrderBy(p => p.FullName)
                    .Select(p => new
                    {
                        p.PatientId,
                        DisplayText = $"{p.FullName} (ID: {p.PatientId})"
                    })
                    .ToList();

                _cbPatient.DisplayMember = "DisplayText";
                _cbPatient.ValueMember = "PatientId";
                _cbPatient.DataSource = patients;

                var doctors = _db.Doctors
                    .Include(d => d.Specialty)
                    .OrderBy(d => d.FullName)
                    .Select(d => new
                    {
                        d.DoctorId,
                        DisplayText = $"{d.FullName} ({d.Specialty.Name})"
                    })
                    .ToList();

                _cbDoctor.DisplayMember = "DisplayText";
                _cbDoctor.ValueMember = "DoctorId";
                _cbDoctor.DataSource = doctors;

                var testTypes = _db.TestTypes
                    .OrderBy(t => t.Name)
                    .Select(t => new
                    {
                        t.TestTypeId,
                        DisplayText = $"{t.Name} ({t.Code})"
                    })
                    .ToList();

                _cbTestType.DisplayMember = "DisplayText";
                _cbTestType.ValueMember = "TestTypeId";
                _cbTestType.DataSource = testTypes;

                var rooms = _db.ExaminationRooms
                    .OrderBy(r => r.RoomNumber)
                    .Select(r => new
                    {
                        r.RoomId,
                        DisplayText = $"Каб. {r.RoomNumber} ({r.RoomType})"
                    })
                    .ToList();

                _cbRoom.DisplayMember = "DisplayText";
                _cbRoom.ValueMember = "RoomId";
                _cbRoom.DataSource = rooms;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка завантаження: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CbPatient_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cbPatient.SelectedValue == null) return;

            try
            {
                int patientId = (int)_cbPatient.SelectedValue;

                var visits = _db.Visits
                    .Include(v => v.Appointment)
                    .ThenInclude(a => a.Doctor)  
                    .Where(v => v.Appointment != null && v.Appointment.PatientId == patientId)
                    .OrderByDescending(v => v.VisitDate)
                    .Take(5)
                    .ToList();

                var visitList = new System.Collections.ArrayList
                {
                    new { VisitId = 0, DisplayText = "-- Без візиту --" }
                };

                foreach (var visit in visits)
                {
                    string doctorName = visit.Appointment?.Doctor?.FullName ?? "Невідомий лікар";
            
                    visitList.Add(new
                    {
                        visit.VisitId,
                        DisplayText = $"Візит #{visit.VisitId} від {visit.VisitDate:dd.MM.yyyy} (Лікар: {doctorName})"
                    });
                }

                _cbVisit.DataSource = visitList;
                _cbVisit.DisplayMember = "DisplayText";
                _cbVisit.ValueMember = "VisitId";
                _cbVisit.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не вдалося завантажити візити: {ex.Message}\n\n" +
                                $"Деталі: {ex.InnerException?.Message}", 
                    "Попередження", 
                    MessageBoxButtons.OK, 
                    MessageBoxIcon.Warning);
                _cbVisit.Enabled = false;
            }
        }

        private void CbTestType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_cbTestType.SelectedValue == null || !_chkCreateAppointment.Checked) return;

            try
            {
                int testTypeId = (int)_cbTestType.SelectedValue;
                var testType = _db.TestTypes.Find(testTypeId);

                if (testType?.DurationMin > 0)
                {
                    _dtpTime.Value = DateTime.Today
                        .AddHours(10)
                        .AddMinutes((double)testType.DurationMin);
                }
            }
            catch { /**/ }
        }

        private void ChkCreateAppointment_CheckedChanged(object sender, EventArgs e)
        {
            bool enabled = _chkCreateAppointment.Checked;
    
            if (_dtpDate.Parent is Panel dateTimePanel)
            {
                dateTimePanel.Enabled = enabled;
            }
        }

        private string GetPriority()
        {
            return _rbUrgent.Checked ? "терміновий" : "плановий";
        }

        private bool ValidateForm()
        {
            var errors = new StringBuilder();

            if (_cbPatient.SelectedValue == null)
                errors.AppendLine("• Оберіть пацієнта");
            
            if (_cbDoctor.SelectedValue == null)
                errors.AppendLine("• Оберіть лікаря");
            
            if (_cbTestType.SelectedValue == null)
                errors.AppendLine("• Оберіть вид тесту");
            
            if (string.IsNullOrWhiteSpace(_txtReason.Text))
                errors.AppendLine("• Вкажіть причину направлення");

            if (_chkCreateAppointment.Checked)
            {
                if (_dtpDate.Value < DateTime.Today)
                    errors.AppendLine("• Дата не може бути в минулому");
                
                if (_cbRoom.SelectedValue == null)
                    errors.AppendLine("• Оберіть кабінет");
            }

            if (errors.Length > 0)
            {
                MessageBox.Show($"Виправте помилки:\n\n{errors}", 
                    "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            try
            {
                Cursor = Cursors.WaitCursor;

                CreatedTestOrder = new TestOrder
                {
                    PatientId = (int)_cbPatient.SelectedValue,
                    DoctorId = (int)_cbDoctor.SelectedValue,
                    TestTypeId = (int)_cbTestType.SelectedValue,
                    VisitId = _cbVisit.SelectedValue != null && (int)_cbVisit.SelectedValue > 0 ? (int?)_cbVisit.SelectedValue : null,
                    OrderDate = DateTime.Now,
                    Priority = GetPriority(),
                    Status = "призначено",
                    Notes = _txtReason.Text.Trim(),
                    RequiredForDiagnosis = _chkForDiagnosis.Checked
                };

                _db.TestOrders.Add(CreatedTestOrder);
                _db.SaveChanges();

                if (_chkCreateAppointment.Checked)
                {
                    CreatedTestAppointment = new TestAppointment
                    {
                        OrderId = CreatedTestOrder.OrderId,
                        RoomId = (int)_cbRoom.SelectedValue,
                        PatientId = CreatedTestOrder.PatientId,
                        ScheduledDate = _dtpDate.Value.Date,
                        ScheduledTime = _dtpTime.Value.TimeOfDay,
                        Status = "заплановано",
                        Notes = _txtNotes.Text.Trim()
                    };

                    _db.TestAppointments.Add(CreatedTestAppointment);
                    _db.SaveChanges();
                }

                string message = $"Направлення успішно створено!\n\n" +
                               $"ID: {CreatedTestOrder.OrderId}\n" +
                               $"Пацієнт: {_cbPatient.Text}\n" +
                               $"Тест: {_cbTestType.Text}";

                if (_chkCreateAppointment.Checked && CreatedTestAppointment != null)
                {
                    message += $"\n\nЗапис на тест створено:\n" +
                             $"Дата: {CreatedTestAppointment.ScheduledDate:dd.MM.yyyy}\n" +
                             $"Час: {CreatedTestAppointment.ScheduledTime:hh\\:mm}";
                }

                MessageBox.Show(message, "Успіх", 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (DbUpdateException dbEx)
            {
                string errorMsg = dbEx.InnerException?.Message ?? dbEx.Message;
                
                if (errorMsg.Contains("patient_id"))
                {
                    errorMsg += "\n\nМожлива причина: відсутність стовпця 'patient_id' в таблиці TestOrders.\n" +
                               "Виконайте в SQL: ALTER TABLE TestOrders ADD patient_id INT NOT NULL";
                }
                
                MessageBox.Show($"Помилка бази даних:\n\n{errorMsg}", 
                    "Помилка збереження", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка: {ex.Message}", 
                    "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }
    }
}