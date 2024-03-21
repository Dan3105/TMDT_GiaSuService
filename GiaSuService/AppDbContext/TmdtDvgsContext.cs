using System;
using System.Collections.Generic;
using GiaSuService.EntityModel;
using Microsoft.EntityFrameworkCore;

namespace GiaSuService.AppDbContext;

public partial class TmdtDvgsContext : DbContext
{
    public TmdtDvgsContext()
    {
    }

    public TmdtDvgsContext(DbContextOptions<TmdtDvgsContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Configpricehistory> Configpricehistories { get; set; }

    public virtual DbSet<District> Districts { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Province> Provinces { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sessiondate> Sessiondates { get; set; }

    public virtual DbSet<Subject> Subjects { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<Tutormatchrequestqueue> Tutormatchrequestqueues { get; set; }

    public virtual DbSet<Tutorprofile> Tutorprofiles { get; set; }

    public virtual DbSet<Tutorrequestform> Tutorrequestforms { get; set; }

    public virtual DbSet<Tutorrequesthistorystatus> Tutorrequesthistorystatuses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("account_pkey");

            entity.ToTable("account");

            entity.HasIndex(e => e.Email, "account_email_key").IsUnique();

            entity.HasIndex(e => e.Identitycard, "account_identitycard_key").IsUnique();

            entity.HasIndex(e => e.Phone, "account_phone_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Addressdetail)
                .HasMaxLength(255)
                .HasColumnName("addressdetail");
            entity.Property(e => e.Avatar)
                .HasMaxLength(255)
                .HasDefaultValueSql("''::character varying")
                .HasColumnName("avatar");
            entity.Property(e => e.Backidentitycard)
                .HasMaxLength(255)
                .HasColumnName("backidentitycard");
            entity.Property(e => e.Birth).HasColumnName("birth");
            entity.Property(e => e.Createdate)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdate");
            entity.Property(e => e.Districtid).HasColumnName("districtid");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.Frontidentitycard)
                .HasMaxLength(255)
                .HasColumnName("frontidentitycard");
            entity.Property(e => e.Fullname)
                .HasMaxLength(255)
                .HasColumnName("fullname");
            entity.Property(e => e.Gender)
                .HasMaxLength(5)
                .HasColumnName("gender");
            entity.Property(e => e.Identitycard)
                .HasMaxLength(20)
                .HasColumnName("identitycard");
            entity.Property(e => e.Lockenable).HasColumnName("lockenable");
            entity.Property(e => e.Passwordhash)
                .HasMaxLength(255)
                .HasColumnName("passwordhash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Roleid)
                .ValueGeneratedOnAdd()
                .HasColumnName("roleid");

            entity.HasOne(d => d.District).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.Districtid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("account_districtid_fkey");

            entity.HasOne(d => d.Role).WithMany(p => p.Accounts)
                .HasForeignKey(d => d.Roleid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("account_roleid_fkey");
        });

        modelBuilder.Entity<Configpricehistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("configpricehistory_pkey");

            entity.ToTable("configpricehistory");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Typetutor).HasColumnName("typetutor");
            entity.Property(e => e.Accountwriteid).HasColumnName("accountwriteid");
            entity.Property(e => e.Commission).HasColumnName("commission");
            entity.Property(e => e.Createdate)
                .HasDefaultValueSql("now()")
                .HasColumnName("createdate");
            entity.Property(e => e.Gradeid).HasColumnName("gradeid");
            entity.Property(e => e.Nsessions).HasColumnName("nsessions");
            entity.Property(e => e.Price).HasColumnName("price");

            entity.HasOne(d => d.Accountwrite).WithMany(p => p.Configpricehistories)
                .HasForeignKey(d => d.Accountwriteid)
                .HasConstraintName("configpricehistory_accountwriteid_fkey");

            entity.HasOne(d => d.Grade).WithMany(p => p.Configpricehistories)
                .HasForeignKey(d => d.Gradeid)
                .HasConstraintName("configpricehistory_gradeid_fkey");
        });

        modelBuilder.Entity<District>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("district_pkey");

            entity.ToTable("district");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Districtname)
                .HasMaxLength(50)
                .HasColumnName("districtname");
            entity.Property(e => e.Provinceid)
                .ValueGeneratedOnAdd()
                .HasColumnName("provinceid");

            entity.HasOne(d => d.Province).WithMany(p => p.Districts)
                .HasForeignKey(d => d.Provinceid)
                .HasConstraintName("district_provinceid_fkey");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("grade_pkey");

            entity.ToTable("grade");

            entity.HasIndex(e => e.Gradename, "grade_gradename_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Gradename)
                .HasMaxLength(50)
                .HasColumnName("gradename");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Province>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("province_pkey");

            entity.ToTable("province");

            entity.HasIndex(e => e.Provincename, "province_provincename_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Provincename)
                .HasMaxLength(50)
                .HasColumnName("provincename");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pkey");

            entity.ToTable("role");

            entity.HasIndex(e => e.Rolename, "role_rolename_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Rolename)
                .HasMaxLength(20)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Sessiondate>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("sessiondate_pkey");

            entity.ToTable("sessiondate");

            entity.HasIndex(e => e.Sessiondate1, "sessiondate_sessiondate_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Sessiondate1)
                .HasMaxLength(25)
                .HasColumnName("sessiondate");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Subject>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("subject_pkey");

            entity.ToTable("subject");

            entity.HasIndex(e => e.Subjectname, "subject_subjectname_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Subjectname)
                .HasMaxLength(100)
                .HasColumnName("subjectname");
            entity.Property(e => e.Value).HasColumnName("value");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("transaction_pkey");

            entity.ToTable("transaction");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Transactiontype).HasColumnName("transactiontype");
            entity.Property(e => e.Paymentmethod).HasColumnName("paymenttype");
            entity.Property(e => e.Accountpayid).HasColumnName("accountpayid");
            entity.Property(e => e.Accountwriteid).HasColumnName("accountwriteid");
            entity.Property(e => e.Commission).HasColumnName("commission");
            entity.Property(e => e.Context).HasColumnName("context");
            entity.Property(e => e.Createdate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createdate");
            entity.Property(e => e.Paymentdate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paymentdate");
            entity.Property(e => e.Paymentdeadline)
                .HasDefaultValueSql("(CURRENT_TIMESTAMP + '5 days'::interval)")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("paymentdeadline");
            entity.Property(e => e.Price).HasColumnName("price");
            entity.Property(e => e.Tutorformid).HasColumnName("tutorformid");

            entity.HasOne(d => d.Accountpay).WithMany(p => p.TransactionAccountpays)
                .HasForeignKey(d => d.Accountpayid)
                .HasConstraintName("transaction_accountpayid_fkey");

            entity.HasOne(d => d.Accountwrite).WithMany(p => p.TransactionAccountwrites)
                .HasForeignKey(d => d.Accountwriteid)
                .HasConstraintName("transaction_accountwriteid_fkey");

            entity.HasOne(d => d.Tutorform).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.Tutorformid)
                .HasConstraintName("transaction_tutorformid_fkey");
        });

        modelBuilder.Entity<Tutormatchrequestqueue>(entity =>
        {
            entity.HasKey(e => new { e.Formid, e.Tutorid }).HasName("classtutorqueue_pkey");

            entity.ToTable("tutormatchrequestqueue");

            entity.Property(e => e.Formid).HasColumnName("formid");
            entity.Property(e => e.Queuestatus).HasColumnName("status");
            entity.Property(e => e.Tutorid).HasColumnName("tutorid");
            entity.Property(e => e.Applydate)
                .HasDefaultValueSql("now()")
                .HasColumnName("applydate");

            entity.HasOne(d => d.Form).WithMany(p => p.Tutormatchrequestqueues)
                .HasForeignKey(d => d.Formid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("classtutorqueue_classid_fkey");

            entity.HasOne(d => d.Tutor).WithMany(p => p.Tutormatchrequestqueues)
                .HasForeignKey(d => d.Tutorid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("classtutorqueue_tutorid_fkey");
        });

        modelBuilder.Entity<Tutorprofile>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("tutorprofile_pkey");

            entity.ToTable("tutorprofile");

            entity.HasIndex(e => e.Accountid, "uk_tutor_account").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Academicyearfrom).HasColumnName("academicyearfrom");
            entity.Property(e => e.Academicyearto).HasColumnName("academicyearto");
            entity.Property(e => e.Accountid)
                .ValueGeneratedOnAdd()
                .HasColumnName("accountid");
            entity.Property(e => e.Additionalinfo).HasColumnName("additionalinfo");
            entity.Property(e => e.Area)
                .HasMaxLength(255)
                .HasColumnName("area");
            entity.Property(e => e.College)
                .HasMaxLength(255)
                .HasColumnName("college");
            entity.Property(e => e.Currentstatus)
                .HasColumnName("currentstatus");
            entity.Property(e => e.Formstatus)
            .HasColumnName("formstatus");
            entity.HasOne(d => d.Account).WithOne(p => p.Tutorprofile)
                .HasForeignKey<Tutorprofile>(d => d.Accountid)
                .HasConstraintName("tutorprofile_accountid_fkey");

            entity.HasMany(d => d.Districts).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "Tutorteachingarea",
                    r => r.HasOne<District>().WithMany()
                        .HasForeignKey("Districtid")
                        .HasConstraintName("tutormovingavailable_districtid_fkey"),
                    l => l.HasOne<Tutorprofile>().WithMany()
                        .HasForeignKey("Tutorid")
                        .HasConstraintName("tutormovingavailable_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("Tutorid", "Districtid").HasName("tutormovingavailable_pkey");
                        j.ToTable("tutorteachingarea");
                        j.IndexerProperty<int>("Tutorid")
                            .HasDefaultValueSql("nextval('tutormovingavailable_tutorid_seq'::regclass)")
                            .HasColumnName("tutorid");
                        j.IndexerProperty<int>("Districtid")
                            .HasDefaultValueSql("nextval('tutormovingavailable_districtid_seq'::regclass)")
                            .HasColumnName("districtid");
                    });

            entity.HasMany(d => d.Grades).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "Tutorgrade",
                    r => r.HasOne<Grade>().WithMany()
                        .HasForeignKey("Gradeid")
                        .HasConstraintName("tutorgrade_gradeid_fkey"),
                    l => l.HasOne<Tutorprofile>().WithMany()
                        .HasForeignKey("Tutorid")
                        .HasConstraintName("tutorgrade_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("Tutorid", "Gradeid").HasName("tutorgrade_pkey");
                        j.ToTable("tutorgrade");
                        j.IndexerProperty<int>("Tutorid")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("tutorid");
                        j.IndexerProperty<int>("Gradeid")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("gradeid");
                    });

            entity.HasMany(d => d.Sessions).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "Tutorsession",
                    r => r.HasOne<Sessiondate>().WithMany()
                        .HasForeignKey("Sessionid")
                        .HasConstraintName("tutorsession_sessionid_fkey"),
                    l => l.HasOne<Tutorprofile>().WithMany()
                        .HasForeignKey("Tutorid")
                        .HasConstraintName("tutorsession_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("Tutorid", "Sessionid").HasName("tutorsession_pkey");
                        j.ToTable("tutorsession");
                        j.IndexerProperty<int>("Tutorid")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("tutorid");
                        j.IndexerProperty<int>("Sessionid")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("sessionid");
                    });

            entity.HasMany(d => d.Subjects).WithMany(p => p.Tutors)
                .UsingEntity<Dictionary<string, object>>(
                    "Tutorsubject",
                    r => r.HasOne<Subject>().WithMany()
                        .HasForeignKey("Subjectid")
                        .HasConstraintName("tutorsubject_subjectid_fkey"),
                    l => l.HasOne<Tutorprofile>().WithMany()
                        .HasForeignKey("Tutorid")
                        .HasConstraintName("tutorsubject_tutorid_fkey"),
                    j =>
                    {
                        j.HasKey("Tutorid", "Subjectid").HasName("tutorsubject_pkey");
                        j.ToTable("tutorsubject");
                        j.IndexerProperty<int>("Tutorid")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("tutorid");
                        j.IndexerProperty<int>("Subjectid")
                            .ValueGeneratedOnAdd()
                            .HasColumnName("subjectid");
                    });
        });

        modelBuilder.Entity<Tutorrequestform>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("classprofile_pkey");

            entity.ToTable("tutorrequestform");
            entity.Property(e => e.Status)
            .HasColumnName("status");
            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('classprofile_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Additionaldetail).HasColumnName("additionaldetail");
            entity.Property(e => e.Addressdetail)
                .HasMaxLength(255)
                .HasColumnName("addressdetail");
            entity.Property(e => e.Createddate)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone")
                .HasColumnName("createddate");
            entity.Property(e => e.Districtid).HasColumnName("districtid");
            entity.Property(e => e.Expireddate)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("expireddate");
            entity.Property(e => e.Gradeid).HasColumnName("gradeid");
            entity.Property(e => e.Nsessions)
                .HasDefaultValueSql("2")
                .HasColumnName("nsessions");
            entity.Property(e => e.Nstudents)
                .HasDefaultValueSql("1")
                .HasColumnName("nstudents");
            entity.Property(e => e.Subjectid).HasColumnName("subjectid");
            entity.Property(e => e.Tutorid).HasColumnName("tutorid");

            entity.HasOne(d => d.District).WithMany(p => p.Tutorrequestforms)
                .HasForeignKey(d => d.Districtid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("classprofile_districtid_fkey");

            entity.HasOne(d => d.Grade).WithMany(p => p.Tutorrequestforms)
                .HasForeignKey(d => d.Gradeid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("classprofile_gradeid_fkey");

            entity.HasOne(d => d.Subject).WithMany(p => p.Tutorrequestforms)
                .HasForeignKey(d => d.Subjectid)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("classprofile_subjectid_fkey");

            entity.HasOne(d => d.Tutor).WithMany(p => p.Tutorrequestforms)
                .HasForeignKey(d => d.Tutorid)
                .HasConstraintName("tutorrequestform_tutorid_fkey");
        });

        modelBuilder.Entity<Tutorrequesthistorystatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("statushistoryclass_pkey");

            entity.ToTable("tutorrequesthistorystatus");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('statushistoryclass_statusid_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Commitername)
                .HasDefaultValueSql("nextval('statushistoryclass_commitername_seq'::regclass)")
                .HasColumnName("commitername");
            entity.Property(e => e.Statusdetail).HasColumnName("statusdetail");
            entity.Property(e => e.Tutorrequestformid)
                .HasDefaultValueSql("nextval('statushistoryclass_classid_seq'::regclass)")
                .HasColumnName("tutorrequestformid");

            entity.HasOne(d => d.Tutorrequestform).WithMany(p => p.Tutorrequesthistorystatuses)
                .HasForeignKey(d => d.Tutorrequestformid)
                .HasConstraintName("statushistoryclass_classid_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
