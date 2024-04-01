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

    public virtual DbSet<Configprice> Configprices { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Employee> Employees { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Identitycard> Identitycards { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Registerstatusdetail> Registerstatusdetails { get; set; }

    public virtual DbSet<Requeststatus> Requeststatuses { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sessiondate> Sessiondates { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<Statustype> Statustypes { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Transactionhistory> Transactionhistoryhistories { get; set; }

    public virtual DbSet<Tutor> Tutors { get; set; }

    public virtual DbSet<Tutorqueue> Tutorqueues { get; set; }

    public virtual DbSet<Tutorrequestform> Tutorrequestforms { get; set; }

    public virtual DbSet<Typetransaction> Typetransactions { get; set; }


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
            entity.Property(e => e.Createdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdate");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Lockenable)
                .HasDefaultValueSql("false")
                .HasColumnName("lockenable");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Roleid).HasColumnName("roleid");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("account_roleid_fkey");
        });

        modelBuilder.Entity<Configprice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("configprice_pkey");

            entity.ToTable("configprice");

            entity.HasIndex(e => new { e.Type, e.Gradeid }, "type_grade_unique").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("money")
                .HasColumnName("amount");
            entity.Property(e => e.Gradeid).HasColumnName("gradeid");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Grade).WithMany(p => p.Configprices)
                .HasForeignKey(d => d.Gradeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("configprice_gradeid_fkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("customer_pkey");

            entity.ToTable("customer");

            entity.HasIndex(e => e.Accountid, "customer_accountid_key").IsUnique();

            entity.HasIndex(e => e.Identityid, "customer_identityid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Addressdetail)
                .HasMaxLength(100)
                .HasColumnName("addressdetail");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.Districtid).HasColumnName("districtid");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            entity.Property(e => e.Identityid).HasColumnName("identityid");

            entity.HasOne(d => d.Account).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.Accountid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_accountid_fkey");

            entity.HasOne(d => d.District).WithMany(p => p.Customers)
                .HasForeignKey(d => d.Districtid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customer_districtid_fkey");

            entity.HasOne(d => d.Identity).WithOne(p => p.Customer)
                .HasForeignKey<Customer>(d => d.Identityid)
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
            entity.Property(e => e.Provinceid).HasColumnName("provinceid");

            entity.HasOne(d => d.Province).WithMany(p => p.Districts)
                .HasForeignKey(d => d.Provinceid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("district_provinceid_fkey");
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("employee_pkey");

            entity.ToTable("employee");

            entity.HasIndex(e => e.Accountid, "employee_accountid_key").IsUnique();

            entity.HasIndex(e => e.Identityid, "employee_identityid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Addressdetail)
                .HasMaxLength(100)
                .HasColumnName("addressdetail");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.Districtid).HasColumnName("districtid");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            entity.Property(e => e.Identityid).HasColumnName("identityid");

            entity.HasOne(d => d.Account).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.Accountid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_accountid_fkey");

            entity.HasOne(d => d.District).WithMany(p => p.Employees)
                .HasForeignKey(d => d.Districtid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("employee_districtid_fkey");

            entity.HasOne(d => d.Identity).WithOne(p => p.Employee)
                .HasForeignKey<Employee>(d => d.Identityid)
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
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Identitycard>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("identitycard_pkey");

            entity.ToTable("identitycard");

            entity.HasIndex(e => e.Identitynumber, "identitycard_identitynumber_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Backidentitycard)
                .HasMaxLength(255)
                .HasColumnName("backidentitycard");
            entity.Property(e => e.Frontidentitycard)
                .HasMaxLength(255)
                .HasColumnName("frontidentitycard");
            entity.Property(e => e.Identitynumber)
                .HasMaxLength(20)
                .HasColumnName("identitynumber");
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

        modelBuilder.Entity<Registerstatusdetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("registerstatusdetail_pkey");

            entity.ToTable("registerstatusdetail");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Context)
                .HasMaxLength(255)
                .HasColumnName("context");
            entity.Property(e => e.Reviewdate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("reviewdate");
            entity.Property(e => e.Statusid).HasColumnName("statusid");
            entity.Property(e => e.Tutorid).HasColumnName("tutorid");

            entity.HasOne(d => d.Status).WithMany(p => p.Registerstatusdetails)
                .HasForeignKey(d => d.Statusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("registerstatusdetail_statusid_fkey");

            entity.HasOne(d => d.Tutor).WithMany(p => p.Registerstatusdetails)
                .HasForeignKey(d => d.Tutorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("registerstatusdetail_tutorid_fkey");
        });

        modelBuilder.Entity<Requeststatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("requeststatus_pkey");

            entity.ToTable("requeststatus");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Context)
                .HasMaxLength(255)
                .HasColumnName("context");
            entity.Property(e => e.Createdate)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdate");
            entity.Property(e => e.Tutorrequestid).HasColumnName("tutorrequestid");

            entity.HasOne(d => d.Tutorrequest).WithMany(p => p.Requeststatuses)
                .HasForeignKey(d => d.Tutorrequestid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("requeststatus_tutorrequestid_fkey");
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

        modelBuilder.Entity<Sessiondate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sessiondate_pkey");

            entity.ToTable("sessiondate");

            entity.HasIndex(e => e.Name, "sessiondate_name_key").IsUnique();

            entity.HasIndex(e => e.Value, "sessiondate_value_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pkey");

            entity.ToTable("status");

            entity.HasIndex(e => e.Name, "status_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Statustypeid).HasColumnName("statustypeid");

            entity.HasOne(d => d.Statustype).WithMany(p => p.Statuses)
                .HasForeignKey(d => d.Statustypeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("status_statustypeid_fkey");
        });

        modelBuilder.Entity<Statustype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("statustype_pkey");

            entity.ToTable("statustype");

            entity.HasIndex(e => e.Type, "statustype_type_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
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

        modelBuilder.Entity<Transactionhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transactionhistory_pkey");

            entity.ToTable("transactionhistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Actualamount)
                .HasColumnType("money")
                .HasColumnName("actualamount");
            entity.Property(e => e.Context)
                .HasMaxLength(255)
                .HasColumnName("context");
            entity.Property(e => e.Createdate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("createdate");
            entity.Property(e => e.Employeeid).HasColumnName("employeeid");
            entity.Property(e => e.Formid).HasColumnName("formid");
            entity.Property(e => e.Payamount)
                .HasColumnType("money")
                .HasColumnName("payamount");
            entity.Property(e => e.Paymentdate).HasColumnName("paymentdate");
            entity.Property(e => e.Rate).HasColumnName("rate");
            entity.Property(e => e.Typetransactionid).HasColumnName("typetransactionid");

            entity.HasOne(d => d.Account).WithMany(p => p.Transactionhistoryhistories)
                .HasForeignKey(d => d.Accountid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactionhistory_accountid_fkey");

            entity.HasOne(d => d.Employee).WithMany(p => p.Transactionhistoryhistories)
                .HasForeignKey(d => d.Employeeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactionhistory_employeeid_fkey");

            entity.HasOne(d => d.Form).WithMany(p => p.Transactionhistoryhistories)
                .HasForeignKey(d => d.Formid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactionhistory_formid_fkey");

            entity.HasOne(d => d.Typetransaction).WithMany(p => p.Transactionhistoryhistories)
                .HasForeignKey(d => d.Typetransactionid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("transactionhistory_typetransactionid_fkey");
        });

        modelBuilder.Entity<Tutor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tutor_pkey");

            entity.ToTable("tutor");

            entity.HasIndex(e => e.Accountid, "tutor_accountid_key").IsUnique();

            entity.HasIndex(e => e.Identityid, "tutor_identityid_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Academicyearfrom).HasColumnName("academicyearfrom");
            entity.Property(e => e.Academicyearto).HasColumnName("academicyearto");
            entity.Property(e => e.Accountid).HasColumnName("accountid");
            entity.Property(e => e.Additionalinfo)
                .HasMaxLength(255)
                .HasColumnName("additionalinfo");
            entity.Property(e => e.Addressdetail)
                .HasMaxLength(255)
                .HasColumnName("addressdetail");
            entity.Property(e => e.Area)
                .HasMaxLength(100)
                .HasColumnName("area");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.College)
                .HasMaxLength(100)
                .HasColumnName("college");
            entity.Property(e => e.Districtid).HasColumnName("districtid");
            entity.Property(e => e.Fullname)
                .HasMaxLength(100)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .HasColumnName("gender");
            entity.Property(e => e.Identityid).HasColumnName("identityid");
            entity.Property(e => e.Typetutor).HasColumnName("typetutor");

            entity.Property(e => e.Isvalid).HasColumnName("isvalid");
            entity.Property(e => e.Isactive).HasColumnName("isactive");

            entity.HasOne(d => d.Account).WithOne(p => p.Tutor)
                .HasForeignKey<Tutor>(d => d.Accountid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutor_accountid_fkey");

            entity.HasOne(d => d.District).WithMany(p => p.Tutors)
                .HasForeignKey(d => d.Districtid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutor_districtid_fkey");

            entity.HasOne(d => d.Identity).WithOne(p => p.Tutor)
                .HasForeignKey<Tutor>(d => d.Identityid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutor_identityid_fkey");

            entity.HasMany(d => d.Districts).WithMany(p => p.TutorsNavigation)
                .UsingEntity<Dictionary<string, object>>(
                    "Tutorteachingarea",
                    r => r.HasOne<District>().WithMany()
                        .HasForeignKey("Districtid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorteachingareas_districtid_fkey"),
                    l => l.HasOne<Tutor>().WithMany()
                        .HasForeignKey("Tutorid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorteachingareas_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("Tutorid", "Districtid").HasName("tutorteachingareas_pkey");
                        j.ToTable("tutorteachingareas");
                        j.IndexerProperty<int>("Tutorid").HasColumnName("tutorid");
                        j.IndexerProperty<int>("Districtid").HasColumnName("districtid");
                    });

            entity.HasMany(d => d.Grades).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "Tutorgrade",
                    r => r.HasOne<Grade>().WithMany()
                        .HasForeignKey("Gradeid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorgrade_gradeid_fkey"),
                    l => l.HasOne<Tutor>().WithMany()
                        .HasForeignKey("Tutorid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorgrade_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("Tutorid", "Gradeid").HasName("tutorgrade_pkey");
                        j.ToTable("tutorgrade");
                        j.IndexerProperty<int>("Tutorid").HasColumnName("tutorid");
                        j.IndexerProperty<int>("Gradeid").HasColumnName("gradeid");
                    });

            entity.HasMany(d => d.Sessions).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "Tutorsession",
                    r => r.HasOne<Sessiondate>().WithMany()
                        .HasForeignKey("Sessionid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorsession_sessionid_fkey"),
                    l => l.HasOne<Tutor>().WithMany()
                        .HasForeignKey("Tutorid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorsession_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("Tutorid", "Sessionid").HasName("tutorsession_pkey");
                        j.ToTable("tutorsession");
                        j.IndexerProperty<int>("Tutorid").HasColumnName("tutorid");
                        j.IndexerProperty<int>("Sessionid").HasColumnName("sessionid");
                    });

            entity.HasMany(d => d.Subjects).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "Tutorsubject",
                    r => r.HasOne<Subject>().WithMany()
                        .HasForeignKey("Subjectid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorsubject_subjectid_fkey"),
                    l => l.HasOne<Tutor>().WithMany()
                        .HasForeignKey("Tutorid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorsubject_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("Tutorid", "Subjectid").HasName("tutorsubject_pkey");
                        j.ToTable("tutorsubject");
                        j.IndexerProperty<int>("Tutorid").HasColumnName("tutorid");
                        j.IndexerProperty<int>("Subjectid").HasColumnName("subjectid");
                    });
        });

        modelBuilder.Entity<Tutorqueue>(entity =>
        {
            entity.HasKey(e => new { e.Tutorrequestid, e.Tutorid }).HasName("tutorqueue_pkey");

            entity.ToTable("tutorqueue");

            entity.Property(e => e.Tutorrequestid).HasColumnName("tutorrequestid");
            entity.Property(e => e.Tutorid).HasColumnName("tutorid");
            entity.Property(e => e.Enterdate)
                .HasDefaultValueSql("CURRENT_DATE")
                .HasColumnName("enterdate");
            entity.Property(e => e.Statusid).HasColumnName("statusid");

            entity.HasOne(d => d.Status).WithMany(p => p.Tutorqueues)
                .HasForeignKey(d => d.Statusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorqueue_statusid_fkey");

            entity.HasOne(d => d.Tutor).WithMany(p => p.Tutorqueues)
                .HasForeignKey(d => d.Tutorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorqueue_tutorid_fkey");

            entity.HasOne(d => d.Tutorrequest).WithMany(p => p.Tutorqueues)
                .HasForeignKey(d => d.Tutorrequestid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorqueue_tutorrequestid_fkey");
        });

        modelBuilder.Entity<Tutorrequestform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tutorrequestform_pkey");

            entity.ToTable("tutorrequestform");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Additionaldetail).HasColumnName("additionaldetail");
            entity.Property(e => e.Addressdetail)
                .HasMaxLength(255)
                .HasColumnName("addressdetail");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Customerid).HasColumnName("customerid");
            entity.Property(e => e.Districtid).HasColumnName("districtid");
            entity.Property(e => e.Expireddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expireddate");
            entity.Property(e => e.Gradeid).HasColumnName("gradeid");
            entity.Property(e => e.Statusid).HasColumnName("statusid");
            entity.Property(e => e.Students).HasColumnName("students");
            entity.Property(e => e.Subjectid).HasColumnName("subjectid");

            entity.HasOne(d => d.Customer).WithMany(p => p.Tutorrequestforms)
                .HasForeignKey(d => d.Customerid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_customerid_fkey");

            entity.HasOne(d => d.District).WithMany(p => p.Tutorrequestforms)
                .HasForeignKey(d => d.Districtid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_districtid_fkey");

            entity.HasOne(d => d.Grade).WithMany(p => p.Tutorrequestforms)
                .HasForeignKey(d => d.Gradeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_gradeid_fkey");

            entity.HasOne(d => d.Status).WithMany(p => p.Tutorrequestforms)
                .HasForeignKey(d => d.Statusid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_statusid_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.Tutorrequestforms)
                .HasForeignKey(d => d.Subjectid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("tutorrequestform_subjectid_fkey");

            entity.HasMany(d => d.Sessions).WithMany(p => p.Tutorrequests)
                .UsingEntity<Dictionary<string, object>>(
                    "Tutorrequestsession",
                    r => r.HasOne<Sessiondate>().WithMany()
                        .HasForeignKey("Sessionid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorrequestsession_sessionid_fkey"),
                    l => l.HasOne<Tutorrequestform>().WithMany()
                        .HasForeignKey("Tutorrequestid")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("tutorrequestsession_tutorrequestid_fkey"),
                    j =>
                    {
                        j.HasKey("Tutorrequestid", "Sessionid").HasName("tutorrequestsession_pkey");
                        j.ToTable("tutorrequestsession");
                        j.IndexerProperty<int>("Tutorrequestid").HasColumnName("tutorrequestid");
                        j.IndexerProperty<int>("Sessionid").HasColumnName("sessionid");
                    });
        });

        modelBuilder.Entity<Typetransaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("typetransaction_pkey");

            entity.ToTable("typetransaction");

            entity.HasIndex(e => e.Name, "typetransaction_name_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
