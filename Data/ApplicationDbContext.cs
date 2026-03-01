using Microsoft.EntityFrameworkCore;
using Clinic_BD.Data.Entities;

namespace Clinic_BD.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<Doctor> Doctors { get; set; }
    public DbSet<Patient> Patients { get; set; }
    public DbSet<Specialty> Specialties { get; set; }
    public DbSet<Appointment> Appointments { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<Visit> Visits { get; set; }
    public DbSet<Diagnosis> Diagnoses { get; set; }
    public DbSet<Prescription> Prescriptions { get; set; }
    public DbSet<MedicalDocument> MedicalDocuments { get; set; }
    public DbSet<ExaminationRoom> ExaminationRooms { get; set; }
    public DbSet<TestType> TestTypes { get; set; }
    public DbSet<TestOrder> TestOrders { get; set; }
    public DbSet<TestResult> TestResults { get; set; }
    public DbSet<TestAppointment> TestAppointments { get; set; }
    public DbSet<DiagnosisTest> DiagnosisTests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Visit>().ToTable(tb => tb.HasTrigger("trg_UpdateReferralAfterVisit"));
        modelBuilder.Entity<Patient>().ToTable(tb => tb.HasTrigger("trg_Patients_Audit"));
        modelBuilder.Entity<Appointment>().ToTable(tb => tb.HasTrigger("ANY_TRIGGER_NAME"));


        // 1. Мапінг ПАЦІЄНТІВ
        modelBuilder.Entity<Patient>(entity =>
        {
            entity.Property(e => e.PatientId).HasColumnName("patient_id").ValueGeneratedOnAdd();
            entity.Property(e => e.FullName).HasColumnName("full_name").IsRequired().HasMaxLength(100);
            entity.Property(e => e.BirthDate).HasColumnName("birth_date").IsRequired();
            entity.Property(e => e.Address).HasColumnName("address").HasMaxLength(200).HasDefaultValue("");
            entity.Property(e => e.Phone).HasColumnName("phone").HasMaxLength(20).HasDefaultValue("");
            entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100).HasDefaultValue("");
            entity.Property(e => e.FamilyDoctorId).HasColumnName("family_doctor_id");
            entity.Property(e => e.RegistrationDate).HasColumnName("registration_date")
                .HasDefaultValueSql("CAST(GETDATE() AS DATE)");
            entity.Property(e => e.BloodType).HasColumnName("blood_type").HasMaxLength(5).HasDefaultValue("");
            entity.Property(e => e.Allergies).HasColumnName("allergies").HasDefaultValue("");
            entity.Property(e => e.Password).HasColumnName("password").HasMaxLength(100).HasDefaultValue("12345")
                .IsRequired();
            entity.HasOne(p => p.FamilyDoctor)
                .WithMany()
                .HasForeignKey(p => p.FamilyDoctorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 2. Мапінг ЛІКАРІВ
        modelBuilder.Entity<Doctor>(entity =>
        {
            entity.ToTable("Doctors");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.SpecialtyId).HasColumnName("specialty_id");
            entity.Property(e => e.RoomNumber).HasColumnName("room_number");
            entity.Property(e => e.WorkPhone).HasColumnName("work_phone");
            entity.Property(e => e.PersonalPhone).HasColumnName("personal_phone");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.CreateAt)
                .HasColumnName("created_at")
                .HasDefaultValueSql("GETDATE()");
        });

        // 3. Мапінг ЗАПИСІВ
        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.ToTable("Appointments");
            entity.HasKey(e => e.AppointmentId);
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id").ValueGeneratedOnAdd();
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.ReferralId).HasColumnName("referral_id");
            entity.Property(e => e.AppointmentDate).HasColumnName("appointment_date");
            entity.Property(e => e.AppointmentTime).HasColumnName("appointment_time");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("заплановано");
            entity.Property(e => e.Notes).HasColumnName("notes");
            //entity.Property(e => e.CreatedAt).HasColumnName("created_at").HasDefaultValueSql("GETDATE()");

            entity.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(a => a.Referral)
                .WithMany()
                .HasForeignKey(a => a.ReferralId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 4. Мапінг СПЕЦІАЛЬНОСТЕЙ
        modelBuilder.Entity<Specialty>(entity =>
        {
            entity.ToTable("Specialties");
            entity.HasKey(e => e.SpecialtyId);

            entity.Property(e => e.Code).HasColumnName("code");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.IsFamily).HasColumnName("is_family");
            entity.Property(e => e.Description).HasColumnName("description");
        });
        // лістинг інших конфігурацій
        // 5. Мапінг НАПРАВЛЕНЬ
        modelBuilder.Entity<Referral>(entity =>
        {
            entity.ToTable("Referrals");
            entity.HasKey(e => e.ReferralId);

            entity.HasOne(r => r.Patient)
                .WithMany()
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.FromDoctor)
                .WithMany()
                .HasForeignKey(r => r.FromDoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(r => r.ToSpecialty)
                .WithMany()
                .HasForeignKey(r => r.ToSpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 6. Мапінг ДІАГНОЗІВ
        modelBuilder.Entity<Diagnosis>(entity =>
        {
            entity.ToTable("Diagnoses");
            entity.HasKey(e => e.DiagnosisId);
            entity.Property(e => e.DiagnosisId).HasColumnName("diagnosis_id").ValueGeneratedOnAdd();
            entity.Property(e => e.IcdCode).HasColumnName("icd_code").HasMaxLength(10);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(200);
            entity.Property(e => e.SpecialtyId).HasColumnName("specialty_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Symptoms).HasColumnName("symptoms");
            entity.Property(e => e.TypicalTreatment).HasColumnName("typical_treatment");

            entity.HasOne(d => d.Specialty)
                .WithMany()
                .HasForeignKey(d => d.SpecialtyId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 7. Мапінг ВІЗИТІВ
        modelBuilder.Entity<Visit>(entity =>
        {
            entity.ToTable("Visits");
            entity.HasKey(e => e.VisitId);
            entity.Property(e => e.VisitId).HasColumnName("visit_id").ValueGeneratedOnAdd();
            entity.Property(e => e.AppointmentId).HasColumnName("appointment_id");
            entity.Property(e => e.DiagnosisId).HasColumnName("diagnosis_id");
            entity.Property(e => e.Symptoms).HasColumnName("symptoms");
            entity.Property(e => e.Examination).HasColumnName("examination");
            entity.Property(e => e.TreatmentPlan).HasColumnName("treatment_plan");
            entity.Property(e => e.NextVisitDate).HasColumnName("next_visit_date");
            entity.Property(e => e.ReferralNeeded).HasColumnName("referral_needed").HasDefaultValue(false);
            entity.Property(e => e.VisitNotes).HasColumnName("visit_notes");
            entity.Property(e => e.VisitDate).HasColumnName("visit_date").HasDefaultValueSql("CAST(GETDATE() AS DATE)");

            entity.HasOne(v => v.Appointment)
                .WithOne()
                .HasForeignKey<Visit>(v => v.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(v => v.Diagnosis)
                .WithMany()
                .HasForeignKey(v => v.DiagnosisId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 8. Мапінг ВИДІВ ТЕСТІВ
        modelBuilder.Entity<TestType>(entity =>
        {
            entity.ToTable("TestTypes");
            entity.HasKey(e => e.TestTypeId);
            entity.Property(e => e.TestTypeId).HasColumnName("test_type_id").ValueGeneratedOnAdd();
            entity.Property(e => e.Code).HasColumnName("code").HasMaxLength(20);
            entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(100);
            entity.Property(e => e.Category).HasColumnName("category").HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Preparation).HasColumnName("preparation");
            entity.Property(e => e.DurationMin).HasColumnName("duration_min");
            entity.Property(e => e.Cost).HasColumnName("cost").HasDefaultValue(0.00m);
            entity.Property(e => e.NormalRange).HasColumnName("normal_range");
        });

        // 9. Мапінг НАПРАВЛЕНЬ НА ТЕСТ
        modelBuilder.Entity<TestOrder>(entity =>
        {
            entity.ToTable("TestOrders");
            entity.HasKey(e => e.OrderId);
    
            entity.Property(e => e.OrderId)
                .HasColumnName("order_id")
                .ValueGeneratedOnAdd();

            entity.Property(e => e.VisitId)
                .HasColumnName("visit_id")
                .IsRequired(false);

            entity.Property(e => e.TestTypeId)
                .HasColumnName("test_type_id")
                .IsRequired();

            entity.Property(e => e.DoctorId)
                .HasColumnName("doctor_id")
                .IsRequired();

            entity.Property(e => e.PatientId)
                .HasColumnName("patient_id")
                .IsRequired();

            entity.Property(e => e.OrderDate)
                .HasColumnName("order_date")
                .HasDefaultValueSql("CAST(GETDATE() AS DATE)");

            entity.Property(e => e.Priority)
                .HasColumnName("priority")
                .HasMaxLength(20)
                .HasDefaultValue("плановий")
                .IsRequired();

            entity.Property(e => e.Status)
                .HasColumnName("status")
                .HasMaxLength(20)
                .HasDefaultValue("призначено")
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasColumnName("notes")
                .IsRequired(false);

            entity.Property(e => e.RequiredForDiagnosis)
                .HasColumnName("required_for_diagnosis")
                .HasDefaultValue(false);

            entity.HasOne(to => to.Visit)
                .WithMany()
                .HasForeignKey(to => to.VisitId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(to => to.TestType)
                .WithMany()
                .HasForeignKey(to => to.TestTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(to => to.Doctor)
                .WithMany()
                .HasForeignKey(to => to.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(to => to.Patient)
                .WithMany()
                .HasForeignKey(to => to.PatientId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 10. Мапінг КАБІНЕТІВ ОБСТЕЖЕННЯ
        modelBuilder.Entity<ExaminationRoom>(entity =>
        {
            entity.ToTable("ExaminationRooms");
            entity.HasKey(e => e.RoomId);
            entity.Property(e => e.RoomId).HasColumnName("room_id").ValueGeneratedOnAdd();
            entity.Property(e => e.RoomNumber).HasColumnName("room_number").HasMaxLength(10);
            entity.Property(e => e.RoomType).HasColumnName("room_type").HasMaxLength(50);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.ResponsibleDoctorId).HasColumnName("responsible_doctor_id");
            entity.Property(e => e.ScheduleJson).HasColumnName("schedule_json");
            entity.Property(e => e.EquipmentList).HasColumnName("equipment_list");
            entity.Property(e => e.MaxPatientsPerDay).HasColumnName("max_patients_per_day").HasDefaultValue(20);

            entity.HasOne(er => er.ResponsibleDoctor)
                .WithMany()
                .HasForeignKey(er => er.ResponsibleDoctorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 11. Мапінг ЗАПИСІВ НА ТЕСТ
        modelBuilder.Entity<TestAppointment>(entity =>
        {
            entity.ToTable("TestAppointments");
            entity.HasKey(e => e.TestAppId);
            entity.Property(e => e.TestAppId).HasColumnName("test_app_id").ValueGeneratedOnAdd();
            entity.Property(e => e.OrderId).HasColumnName("order_id");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.ScheduledDate).HasColumnName("scheduled_date");
            entity.Property(e => e.ScheduledTime).HasColumnName("scheduled_time");
            entity.Property(e => e.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("заплановано");
            entity.Property(e => e.TechnicianId).HasColumnName("technician_id");
            entity.Property(e => e.ActualStartTime).HasColumnName("actual_start_time");
            entity.Property(e => e.ActualEndTime).HasColumnName("actual_end_time");
            entity.Property(e => e.Notes).HasColumnName("notes");

            entity.HasOne(ta => ta.TestOrder)
                .WithMany()
                .HasForeignKey(ta => ta.OrderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ta => ta.ExaminationRoom)
                .WithMany()
                .HasForeignKey(ta => ta.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ta => ta.Patient)
                .WithMany()
                .HasForeignKey(ta => ta.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(ta => ta.Technician)
                .WithMany()
                .HasForeignKey(ta => ta.TechnicianId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 12. Мапінг РЕЗУЛЬТАТІВ ТЕСТІВ
        modelBuilder.Entity<TestResult>(entity =>
        {
            entity.ToTable("TestResults");
            entity.HasKey(e => e.ResultId);
            entity.Property(e => e.ResultId).HasColumnName("result_id").ValueGeneratedOnAdd();
            entity.Property(e => e.TestAppId).HasColumnName("test_app_id");
            entity.Property(e => e.PerformedBy).HasColumnName("performed_by");
            entity.Property(e => e.ResultDate).HasColumnName("result_date").HasDefaultValueSql("GETDATE()");
            entity.Property(e => e.ResultText).HasColumnName("result_text");
            entity.Property(e => e.ResultJson).HasColumnName("result_json");
            entity.Property(e => e.AttachmentPath).HasColumnName("attachment_path").HasMaxLength(500);
            entity.Property(e => e.Conclusion).HasColumnName("conclusion");
            entity.Property(e => e.IsAbnormal).HasColumnName("is_abnormal");
            entity.Property(e => e.ReviewedByDoctorId).HasColumnName("reviewed_by_doctor_id");
            entity.Property(e => e.ReviewNotes).HasColumnName("review_notes");

            entity.HasOne(tr => tr.TestAppointment)
                .WithOne()
                .HasForeignKey<TestResult>(tr => tr.TestAppId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(tr => tr.PerformedByDoctor)
                .WithMany()
                .HasForeignKey(tr => tr.PerformedBy)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(tr => tr.ReviewedByDoctor)
                .WithMany()
                .HasForeignKey(tr => tr.ReviewedByDoctorId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // 13. Мапінг РЕЦЕПТІВ
        modelBuilder.Entity<Prescription>(entity =>
        {
            entity.ToTable("Prescriptions");
            entity.HasKey(e => e.PrescriptionId);
            entity.Property(e => e.PrescriptionId).HasColumnName("prescription_id").ValueGeneratedOnAdd();
            entity.Property(e => e.VisitId).HasColumnName("visit_id");
            entity.Property(e => e.Medication).HasColumnName("medication").HasMaxLength(200);
            entity.Property(e => e.Dosage).HasColumnName("dosage").HasMaxLength(50);
            entity.Property(e => e.Frequency).HasColumnName("frequency").HasMaxLength(50);
            entity.Property(e => e.DurationDays).HasColumnName("duration_days");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.Instructions).HasColumnName("instructions");
            entity.Property(e => e.RefillsAllowed).HasColumnName("refills_allowed").HasDefaultValue(0);
            entity.Property(e => e.RefillsUsed).HasColumnName("refills_used").HasDefaultValue(0);
            entity.Property(e => e.PrescribedBy).HasColumnName("prescribed_by");

            entity.HasOne(p => p.Visit)
                .WithMany()
                .HasForeignKey(p => p.VisitId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(p => p.PrescribingDoctor)
                .WithMany()
                .HasForeignKey(p => p.PrescribedBy)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 14. Мапінг МЕДИЧНИХ ДОКУМЕНТІВ
        modelBuilder.Entity<MedicalDocument>(entity =>
        {
            entity.ToTable("MedicalDocuments");
            entity.HasKey(e => e.DocumentId);
            entity.Property(e => e.DocumentId).HasColumnName("document_id").ValueGeneratedOnAdd();
            entity.Property(e => e.PatientId).HasColumnName("patient_id");
            entity.Property(e => e.DoctorId).HasColumnName("doctor_id");
            entity.Property(e => e.DocumentType).HasColumnName("document_type").HasMaxLength(50);
            entity.Property(e => e.IssueDate).HasColumnName("issue_date").HasDefaultValueSql("CAST(GETDATE() AS DATE)");
            entity.Property(e => e.FilePath).HasColumnName("file_path").HasMaxLength(500);
            entity.Property(e => e.DocumentText).HasColumnName("document_text");
            entity.Property(e => e.Notes).HasColumnName("notes");

            entity.HasOne(md => md.Patient)
                .WithMany()
                .HasForeignKey(md => md.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(md => md.Doctor)
                .WithMany()
                .HasForeignKey(md => md.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // 15. Мапінг ДІАГНОЗИ-ТЕСТИ
        modelBuilder.Entity<DiagnosisTest>(entity =>
        {
            entity.ToTable("DiagnosisTests");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();
            entity.Property(e => e.DiagnosisId).HasColumnName("diagnosis_id");
            entity.Property(e => e.TestTypeId).HasColumnName("test_type_id");
            entity.Property(e => e.IsMandatory).HasColumnName("is_mandatory").HasDefaultValue(true);
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.RecommendedFrequency).HasColumnName("recommended_frequency").HasMaxLength(50);

            entity.HasIndex(e => new { e.DiagnosisId, e.TestTypeId }).IsUnique();

            entity.HasOne(dt => dt.Diagnosis)
                .WithMany()
                .HasForeignKey(dt => dt.DiagnosisId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(dt => dt.TestType)
                .WithMany()
                .HasForeignKey(dt => dt.TestTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Doctor>(entity => { entity.Property(e => e.FullName).HasColumnName("full_name"); });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        const string connectionString = "Server=localhost;Database=Kursova;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False";

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}