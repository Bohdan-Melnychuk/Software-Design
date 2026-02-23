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
    public partial class AddReferralForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Referral NewReferral { get; private set; }
        
        private ComboBox cbPatient, cbDoctor, cbSpecialty, cbPriority;
        private DateTimePicker dtpExpiry;
        private TextBox txtReason, txtNotes, txtSearchPatient;
        private List<Patient> _allPatients;

        public AddReferralForm(ApplicationDbContext db)
        {
            _allPatients = db.Patients.AsNoTracking().ToList() ?? new List<Patient>();
            InitializeManualComponent(db);
        }

        private void InitializeManualComponent(ApplicationDbContext db)
        {
            this.Text = "Створення нового направлення";
            this.Size = new Size(450, 750);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int left = 20, width = 390;

            var lblSearch = new Label { Text = "Швидкий пошук пацієнта:", Top = 15, Left = left, Width = width };
            txtSearchPatient = new TextBox { Top = 35, Left = left, Width = width };
            txtSearchPatient.TextChanged += (s, e) => {
                var text = txtSearchPatient.Text.ToLower().Trim();
                cbPatient.DataSource = _allPatients.Where(p => p.FullName.ToLower().Contains(text)).ToList();
            };

            var lblP = new Label { Text = "Пацієнт:", Top = 70, Left = left };
            cbPatient = new ComboBox { Top = 90, Left = left, Width = width, DropDownStyle = ComboBoxStyle.DropDownList };
            cbPatient.DisplayMember = "FullName";
            cbPatient.ValueMember = "PatientId";
            cbPatient.DataSource = _allPatients;

            var lblD = new Label { Text = "Лікар, що видає направлення:", Top = 130, Left = left };
            cbDoctor = new ComboBox { Top = 150, Left = left, Width = width, DropDownStyle = ComboBoxStyle.DropDownList };

            cbDoctor.DataSource = db.Doctors.Include(d => d.Specialty).AsNoTracking().ToList()
                .Select(d => new {
                    Id = d.DoctorId,
                    Name = $"{d.FullName} ({d.Specialty?.Name ?? "Без спеціальності"})"
                }).ToList();
            cbDoctor.DisplayMember = "Name";
            cbDoctor.ValueMember = "Id";

            var lblS = new Label { Text = "Направити до спеціальності:", Top = 190, Left = left };
            cbSpecialty = new ComboBox { Top = 210, Left = left, Width = width, DropDownStyle = ComboBoxStyle.DropDownList };
            
            cbSpecialty.DataSource = db.Specialties.AsNoTracking().ToList();
            cbSpecialty.DisplayMember = "Name";
            cbSpecialty.ValueMember = "SpecialtyId";

            var lblReason = new Label { Text = "Причина направлення:", Top = 250, Left = left };
            txtReason = new TextBox { Top = 270, Left = left, Width = width, Height = 60, Multiline = true };

            var lblPriority = new Label { Text = "Пріоритет:", Top = 340, Left = left };
            cbPriority = new ComboBox { Top = 360, Left = left, Width = width, DropDownStyle = ComboBoxStyle.DropDownList };
            cbPriority.Items.AddRange(new string[] { "нормальний", "терміновий", "високий", "критичний" });
            cbPriority.SelectedIndex = 0;

            var lblExpiry = new Label { Text = "Дійсне до:", Top = 400, Left = left };
            dtpExpiry = new DateTimePicker { Top = 420, Left = left, Width = width, Format = DateTimePickerFormat.Short };
            dtpExpiry.Value = DateTime.Now.AddMonths(1);

            var lblNotes = new Label { Text = "Додаткові нотатки:", Top = 460, Left = left };
            txtNotes = new TextBox { Top = 480, Left = left, Width = width, Height = 80, Multiline = true, ScrollBars = ScrollBars.Vertical };

            var btnSave = new Button { 
                Text = "📜 Видати направлення", 
                Top = 600, Left = 110, Width = 220, Height = 50, 
                BackColor = Color.LightCyan,
                Font = new Font(this.Font, FontStyle.Bold)
            };

            btnSave.Click += (s, e) => {
                if (cbPatient.SelectedItem != null && cbDoctor.SelectedItem != null && cbSpecialty.SelectedItem != null) {
                    try {
                        if (cbPatient.SelectedValue != null) {
                            NewReferral = new Referral {
                                PatientId = Convert.ToInt32(cbPatient.SelectedValue),
                                FromDoctorId = Convert.ToInt32(cbDoctor.SelectedValue),
                                ToSpecialtyId = Convert.ToInt32(cbSpecialty.SelectedValue),
                                Reason = txtReason.Text,
                                ReferralDate = DateTime.Now,
                                ExpiryDate = dtpExpiry.Value,
                                Used = false,
                                Priority = cbPriority.SelectedItem?.ToString() ?? "нормальний",
                                Notes = txtNotes.Text
                            };
                            this.DialogResult = DialogResult.OK;
                        }
                    } catch (Exception ex) {
                        MessageBox.Show($"Помилка: {ex.Message}");
                    }
                } else {
                    MessageBox.Show("Будь ласка, заповніть усі поля!");
                }
            };

            this.Controls.AddRange(new Control[] { 
                lblSearch, txtSearchPatient, lblP, cbPatient, 
                lblD, cbDoctor, lblS, cbSpecialty, 
                lblReason, txtReason, lblPriority, cbPriority,
                lblExpiry, dtpExpiry, lblNotes, txtNotes, btnSave 
            });
        }
    }
}