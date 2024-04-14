using System;
using System.Collections.Generic;
using GiaSuService.EntityModel;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.AppDbContext;

public partial class DvgsDbContext : DbContext
{
    public DvgsDbContext()
    {
    }

    public DvgsDbContext(DbContextOptions<DvgsDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<IdentityCard> IdentityCards { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<RequestStatus> RequestStatuses { get; set; }

    public virtual DbSet<RequestTutorForm> RequestTutorForms { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<SessionDate> SessionDates { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<StatusType> StatusTypes { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<TransactionHistory> TransactionHistories { get; set; }

    public virtual DbSet<Tutor> Tutors { get; set; }

    public virtual DbSet<TutorApplyForm> TutorApplyForms { get; set; }

    public virtual DbSet<TutorStatusDetail> TutorStatusDetails { get; set; }

    public virtual DbSet<TutorType> TutorTypes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseNpgsql("Name=ConnectionStrings:TutorConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("account_pkey");

            entity.ToTable("account");

            entity.HasIndex(e => e.Email, "account_email_key").IsUnique();

            entity.HasIndex(e => e.Phone, "account_phone_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasColumnName("avatar");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.LockEnable).HasColumnName("lock_enable");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("account_roleid_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_pkey");

            entity.ToTable("customer");

            entity.HasIndex(e => e.AccountId, "customer_accountid_key").IsUnique();

            entity.HasIndex(e => e.IdentityId, "customer_identityid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AddressDetail)
                .HasMaxLength(100)
                .HasColumnName("address_detail");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            entity.Property(e => e.IdentityId).HasColumnName("identity_id");

            entity.HasOne(d => d.Account).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_accountid_fkey");

            entity.HasOne(d => d.District).WithMany(p => p.Customers)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_districtid_fkey");

            entity.HasOne(d => d.Identity).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.IdentityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_identityid_fkey");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("district_pkey");

            entity.ToTable("district");

            entity.HasIndex(e => e.Name, "district_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.ProvinceId).HasColumnName("province_id");

            entity.HasOne(d => d.Province).WithMany(p => p.Districts)
                .HasForeignKey(d => d.ProvinceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("district_provinceid_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pkey");

            entity.ToTable("employee");

            entity.HasIndex(e => e.AccountId, "employee_accountid_key").IsUnique();

            entity.HasIndex(e => e.IdentityId, "employee_identityid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AddressDetail)
                .HasMaxLength(100)
                .HasColumnName("address_detail");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            entity.Property(e => e.IdentityId).HasColumnName("identity_id");

            entity.HasOne(d => d.Account).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_accountid_fkey");

            entity.HasOne(d => d.District).WithMany(p => p.Employees)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_districtid_fkey");

            entity.HasOne(d => d.Identity).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.IdentityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_identityid_fkey");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("grade_pkey");

            entity.ToTable("grade");

            entity.HasIndex(e => e.Name, "grade_name_key").IsUnique();

            entity.HasIndex(e => e.Value, "grade_value_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fee)
                .HasColumnType("money")
                .HasColumnName("fee");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<IdentityCard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("identitycard_pkey");

            entity.ToTable("identity_card");

            entity.HasIndex(e => e.IdentityNumber, "identitycard_identitynumber_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('identitycard_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.BackIdentityCard)
                .HasMaxLength(255)
                .HasColumnName("back_identity_card");
            entity.Property(e => e.FrontIdentityCard)
                .HasMaxLength(255)
                .HasColumnName("front_identity_card");
            entity.Property(e => e.IdentityNumber)
                .HasMaxLength(20)
                .HasColumnName("identity_number");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("province_pkey");

            entity.ToTable("province");

            entity.HasIndex(e => e.Name, "province_provincename_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<RequestStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("requeststatus_pkey");

            entity.ToTable("request_status");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('requeststatus_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Context).HasColumnName("context");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.TutorRequestId).HasColumnName("tutor_request_id");

            entity.HasOne(d => d.TutorRequest).WithMany(p => p.RequestStatuses)
                .HasForeignKey(d => d.TutorRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requeststatus_tutorrequestid_fkey");
        });

        modelBuilder.Entity<RequestTutorForm>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tutorrequestform_pkey");

            entity.ToTable("request_tutor_form");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('tutorrequestform_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.AdditionalDetail).HasColumnName("additional_detail");
            entity.Property(e => e.AddressDetail)
                .HasMaxLength(255)
                .HasColumnName("address_detail");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.ExpiredDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expired_date");
            entity.Property(e => e.GradeId).HasColumnName("grade_id");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.Students).HasColumnName("students");
            entity.Property(e => e.SubjectId).HasColumnName("subject_id");

            entity.HasOne(d => d.Customer).WithMany(p => p.RequestTutorForms)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_customerid_fkey");

            entity.HasOne(d => d.District).WithMany(p => p.RequestTutorForms)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_districtid_fkey");

            entity.HasOne(d => d.Grade).WithMany(p => p.RequestTutorForms)
                .HasForeignKey(d => d.GradeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_gradeid_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.RequestTutorForms)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_statusid_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.RequestTutorForms)
                .HasForeignKey(d => d.SubjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_subjectid_fkey");

            entity.HasMany(d => d.Sessions).WithMany(p => p.TutorRequests)
                .UsingEntity<Dictionary<string, object>>(
                    "TutorRequestSession",
                    r => r.HasOne<SessionDate>().WithMany()
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorrequestsession_sessionid_fkey"),
                    l => l.HasOne<RequestTutorForm>().WithMany()
                        .HasForeignKey("TutorRequestId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorrequestsession_tutorrequestid_fkey"),
                    j =>
                    {
                        j.HasKey("TutorRequestId", "SessionId").HasName("tutorrequestsession_pkey");
                        j.ToTable("tutor_request_session");
                        j.IndexerProperty<int>("TutorRequestId").HasColumnName("tutor_request_id");
                        j.IndexerProperty<int>("SessionId").HasColumnName("session_id");
                    });
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.HasIndex(e => e.Name, "role_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
        });

        modelBuilder.Entity<SessionDate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sessiondate_pkey");

            entity.ToTable("session_date");

            entity.HasIndex(e => e.Name, "sessiondate_name_key").IsUnique();

            entity.HasIndex(e => e.Value, "sessiondate_value_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('sessiondate_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status");

            entity.HasIndex(e => new { e.Name, e.StatusTypeId }, "uq_name_status_type_id").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.StatusTypeId).HasColumnName("status_type_id");

            entity.HasOne(d => d.StatusType).WithMany(p => p.Statuses)
                .HasForeignKey(d => d.StatusTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("status_statustypeid_fkey");
        });

        modelBuilder.Entity<StatusType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("statustype_pkey");

            entity.ToTable("status_type");

            entity.HasIndex(e => e.Type, "statustype_type_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('statustype_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Type)
                .HasMaxLength(50)
                .HasColumnName("type");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subject_pkey");

            entity.ToTable("subject");

            entity.HasIndex(e => e.Name, "subject_name_key").IsUnique();

            entity.HasIndex(e => e.Value, "subject_value_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<TransactionHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactionhistory_pkey");

            entity.ToTable("transaction_history");

            entity.HasIndex(e => new { e.TutorId, e.FormId, e.TypeTransaction }, "ck_tutorid_formid_typetransaction").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('transactionhistory_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Context)
                .HasMaxLength(255)
                .HasColumnName("context");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.EmployeeId).HasColumnName("employee_id");
            entity.Property(e => e.FormId).HasColumnName("form_id");
            entity.Property(e => e.PaymentAmount)
                .HasColumnType("money")
                .HasColumnName("payment_amount");
            entity.Property(e => e.PaymentDate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("payment_date");
            entity.Property(e => e.TutorId).HasColumnName("tutor_id");
            entity.Property(e => e.TypeTransaction).HasColumnName("type_transaction");

            entity.HasOne(d => d.Employee).WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.EmployeeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactionhistory_employeeid_fkey");

            entity.HasOne(d => d.Form).WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.FormId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactionhistory_formid_fkey");

            entity.HasOne(d => d.Tutor).WithMany(p => p.TransactionHistories)
                .HasForeignKey(d => d.TutorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transaction_history_tutor_id_fkey");
        });

        modelBuilder.Entity<Tutor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tutor_pkey");

            entity.ToTable("tutor");

            entity.HasIndex(e => e.AccountId, "tutor_accountid_key").IsUnique();

            entity.HasIndex(e => e.IdentityId, "tutor_identityid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.AcademicYearFrom).HasColumnName("academic_year_from");
            entity.Property(e => e.AcademicYearTo).HasColumnName("academic_year_to");
            entity.Property(e => e.AccountId).HasColumnName("account_id");
            entity.Property(e => e.AdditionalInfo)
                .HasMaxLength(255)
                .HasColumnName("additional_info");
            entity.Property(e => e.AddressDetail)
                .HasMaxLength(255)
                .HasColumnName("address_detail");
            entity.Property(e => e.Area)
                .HasMaxLength(100)
                .HasColumnName("area");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.College)
                .HasMaxLength(100)
                .HasColumnName("college");
            entity.Property(e => e.DistrictId).HasColumnName("district_id");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            entity.Property(e => e.IdentityId).HasColumnName("identity_id");
            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValueSql("true")
                .HasColumnName("is_active");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TutorTypeId).HasColumnName("tutor_type_id");

            entity.HasOne(d => d.Account).WithOne(p => p.Tutor)
                .HasForeignKey<Tutor>(d => d.AccountId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutor_accountid_fkey");

            entity.HasOne(d => d.District).WithMany(p => p.Tutors)
                .HasForeignKey(d => d.DistrictId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutor_districtid_fkey");

            entity.HasOne(d => d.Identity).WithOne(p => p.Tutor)
                .HasForeignKey<Tutor>(d => d.IdentityId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutor_identityid_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Tutors)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_tutor_status");

            entity.HasOne(d => d.TutorType).WithMany(p => p.Tutors)
                .HasForeignKey(d => d.TutorTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutor_tutor_type_id_fkey");

            entity.HasMany(d => d.Districts).WithMany(p => p.TutorsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "TutorTeachingArea",
                    r => r.HasOne<District>().WithMany()
                        .HasForeignKey("DistrictId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorteachingareas_districtid_fkey"),
                    l => l.HasOne<Tutor>().WithMany()
                        .HasForeignKey("TutorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorteachingareas_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("TutorId", "DistrictId").HasName("tutorteachingareas_pkey");
                        j.ToTable("tutor_teaching_area");
                        j.IndexerProperty<int>("TutorId").HasColumnName("tutor_id");
                        j.IndexerProperty<int>("DistrictId").HasColumnName("district_id");
                    });

            entity.HasMany(d => d.Grades).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "TutorGrade",
                    r => r.HasOne<Grade>().WithMany()
                        .HasForeignKey("GradeId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorgrade_gradeid_fkey"),
                    l => l.HasOne<Tutor>().WithMany()
                        .HasForeignKey("TutorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorgrade_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("TutorId", "GradeId").HasName("tutorgrade_pkey");
                        j.ToTable("tutor_grade");
                        j.IndexerProperty<int>("TutorId").HasColumnName("tutor_id");
                        j.IndexerProperty<int>("GradeId").HasColumnName("grade_id");
                    });

            entity.HasMany(d => d.Sessions).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "TutorSession",
                    r => r.HasOne<SessionDate>().WithMany()
                        .HasForeignKey("SessionId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorsession_sessionid_fkey"),
                    l => l.HasOne<Tutor>().WithMany()
                        .HasForeignKey("TutorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorsession_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("TutorId", "SessionId").HasName("tutorsession_pkey");
                        j.ToTable("tutor_session");
                        j.IndexerProperty<int>("TutorId").HasColumnName("tutor_id");
                        j.IndexerProperty<int>("SessionId").HasColumnName("session_id");
                    });

            entity.HasMany(d => d.Subjects).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "TutorSubject",
                    r => r.HasOne<Subject>().WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorsubject_subjectid_fkey"),
                    l => l.HasOne<Tutor>().WithMany()
                        .HasForeignKey("TutorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorsubject_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("TutorId", "SubjectId").HasName("tutorsubject_pkey");
                        j.ToTable("tutor_subject");
                        j.IndexerProperty<int>("TutorId").HasColumnName("tutor_id");
                        j.IndexerProperty<int>("SubjectId").HasColumnName("subject_id");
                    });
        });

        modelBuilder.Entity<TutorApplyForm>(entity =>
        {
            entity.HasKey(e => new { e.TutorRequestId, e.TutorId }).HasName("tutorqueue_pkey");

            entity.ToTable("tutor_apply_form");

            entity.Property(e => e.TutorRequestId).HasColumnName("tutor_request_id");
            entity.Property(e => e.TutorId).HasColumnName("tutor_id");
            entity.Property(e => e.EnterDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("enter_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");

            entity.HasOne(d => d.Status).WithMany(p => p.TutorApplyForms)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorqueue_statusid_fkey");

            entity.HasOne(d => d.Tutor).WithMany(p => p.TutorApplyForms)
                .HasForeignKey(d => d.TutorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorqueue_tutorid_fkey");

            entity.HasOne(d => d.TutorRequest).WithMany(p => p.TutorApplyForms)
                .HasForeignKey(d => d.TutorRequestId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorqueue_tutorrequestid_fkey");
        });

        modelBuilder.Entity<TutorStatusDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("registerstatusdetail_pkey");

            entity.ToTable("tutor_status_detail");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('registerstatusdetail_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Context).HasColumnName("context");
            entity.Property(e => e.CreateDate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("create_date");
            entity.Property(e => e.StatusId).HasColumnName("status_id");
            entity.Property(e => e.TutorId).HasColumnName("tutor_id");

            entity.HasOne(d => d.Status).WithMany(p => p.TutorStatusDetails)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("registerstatusdetail_statusid_fkey");

            entity.HasOne(d => d.Tutor).WithMany(p => p.TutorStatusDetails)
                .HasForeignKey(d => d.TutorId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("registerstatusdetail_tutorid_fkey");
        });

        modelBuilder.Entity<TutorType>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tutor_type_pkey");

            entity.ToTable("tutor_type");

            entity.HasIndex(e => e.Name, "tutor_type_name_key").IsUnique();

            entity.HasIndex(e => e.Value, "tutor_type_value_key").IsUnique();

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('tutor_type_tutor_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(20)
                .HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
