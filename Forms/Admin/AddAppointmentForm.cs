using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Clinic_BD.Data;
using Clinic_BD.Data.Entities;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace Clinic_BD.Forms.Admin
{
    [DesignerCategory("Code")]
    [DesignTimeVisible(false)]
    public sealed class AddAppointmentForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Appointment NewAppointment { get; private set; }
        
        private ComboBox cbP, cbD;
        private DateTimePicker dtp;
        private TextBox txtSearchPatient;
        private TextBox txtNotes;
        private List<Patient> _allPatients;

        public AddAppointmentForm(ApplicationDbContext db)
        {
            _allPatients = db.Patients.AsNoTracking().ToList() ?? new List<Patient>();
            InitializeManualComponent(db);
        }

        private void InitializeManualComponent(ApplicationDbContext db)
        {
            this.Text = "Новий запис прийому";
            this.Size = new Size(420, 580);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            var lblSearch = new Label { Text = "Швидкий пошук пацієнта:", Top = 15, Left = 20, Width = 340 };
            txtSearchPatient = new TextBox { Top = 35, Left = 20, Width = 360 };
            txtSearchPatient.TextChanged += (s, e) => {
                var text = txtSearchPatient.Text.ToLower().Trim();
                cbP.DataSource = _allPatients.Where(p => p.FullName.ToLower().Contains(text)).ToList();
            };

            var lblP = new Label { Text = "Пацієнт:", Top = 70, Left = 20 };
            cbP = new ComboBox { Top = 90, Left = 20, Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };
            cbP.DisplayMember = "FullName";
            cbP.ValueMember = "PatientId";
            cbP.DataSource = _allPatients;

            var lblD = new Label { Text = "Лікар (спеціальність):", Top = 130, Left = 20 };
            cbD = new ComboBox { Top = 150, Left = 20, Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };

            var doctorsList = db.Doctors
                .Include(d => d.Specialty)
                .AsNoTracking()
                .ToList()
                .Select(d => new {
                    d.DoctorId,
                    FullNameWithSpecialty = $"{d.FullName} ({d.Specialty?.Name ?? "Без спеціальності"})"
                })
                .ToList();

            cbD.DataSource = doctorsList;
            cbD.DisplayMember = "FullNameWithSpecialty";
            cbD.ValueMember = "DoctorId";

            var lblDate = new Label { Text = "Дата та час прийому:", Top = 190, Left = 20 };
            dtp = new DateTimePicker { 
                Top = 210, Left = 20, Width = 360, 
                Format = DateTimePickerFormat.Custom, 
                CustomFormat = "dd.MM.yyyy HH:mm" 
            };

            var lblNotes = new Label { Text = "Скарги пацієнта / Нотатки:", Top = 250, Left = 20 };
            txtNotes = new TextBox { 
                Top = 270, Left = 20, Width = 360, 
                Height = 120, Multiline = true, 
                ScrollBars = ScrollBars.Vertical 
            };

            var btn = new Button { 
                Text = "📅 Створити запис", 
                Top = 420, Left = 110, Width = 180, Height = 45, 
                BackColor = Color.LightGreen,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            btn.Click += (s, e) => {
                if (cbP.SelectedValue == null || cbD.SelectedValue == null) {
                    MessageBox.Show("Будь ласка, оберіть пацієнта та лікаря!", "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (dtp.Value < DateTime.Now) {
                    MessageBox.Show("Не можна створити запис на минулу дату або час!", "Помилка валідації", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try {
                    DateTime fullDateTime = dtp.Value;
                    TimeSpan cleanTime = new TimeSpan(fullDateTime.Hour, fullDateTime.Minute, 0);

                    NewAppointment = new Appointment {
                        PatientId = (int)cbP.SelectedValue,
                        DoctorId = (int)cbD.SelectedValue,
                        ReferralId = 1, 
                        AppointmentDate = fullDateTime.Date,
                        AppointmentTime = cleanTime,
                        Status = "заплановано",
                        Notes = string.IsNullOrWhiteSpace(txtNotes.Text) ? "Плановий візит" : txtNotes.Text.Trim(),
                        CreateAt = DateTime.Now
                    };
                    
                    var newVisit = new Visit {
                        Appointment = NewAppointment,
                        VisitDate = NewAppointment.AppointmentDate,
                        Symptoms = NewAppointment.Notes,
                        ReferralNeeded = false,
                        VisitNotes = "Автоматично створений після запису"
                    };

                    using (var dbContext = new ApplicationDbContext()) {
                        dbContext.Appointments.Add(NewAppointment);
                        dbContext.Visits.Add(newVisit);
                        dbContext.SaveChanges();
                    }
                    this.DialogResult = DialogResult.OK;
                } 
                catch (Exception ex) {
                    MessageBox.Show($"Помилка при збереженні: {ex.Message}", "Помилка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            this.Controls.AddRange(new Control[] { 
                lblSearch, txtSearchPatient, 
                lblP, cbP, 
                lblD, cbD, 
                lblDate, dtp, 
                lblNotes, txtNotes, 
                btn 
            });
        }
    }
}