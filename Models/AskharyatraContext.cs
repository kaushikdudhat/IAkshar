using System;
using System.Collections.Generic;
using System.Configuration;
using Microsoft.EntityFrameworkCore;

namespace iAkshar.Models;

public partial class AskharyatraContext : DbContext
{
    private readonly IConfiguration _configuration;
    public AskharyatraContext()
    {
    }

    public AskharyatraContext(DbContextOptions<AskharyatraContext> options,IConfiguration configuration)
        : base(options)
    {
        _configuration = configuration;
    }

    public virtual DbSet<Attendence> Attendences { get; set; }

    public virtual DbSet<Mandal> Mandals { get; set; }

    public virtual DbSet<Pradesh> Pradeshes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Sabha> Sabhas { get; set; }

    public virtual DbSet<SabhaTrack> SabhaTracks { get; set; }

    public virtual DbSet<SabhaType> SabhaTypes { get; set; }

    public virtual DbSet<Suggestions> Suggestions { get; set; }

    public virtual DbSet<AksharUser> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(_configuration.GetConnectionString("askharyatraContext"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Attendence>(entity =>
        {
            entity.HasKey(e => e.AttendenceId).HasName("PK__Attenden__8A12B81F9E9ACE61");

            entity.ToTable("Attendence");

            entity.Property(e => e.AttendenceId).HasColumnName("attendence_id");
            entity.Property(e => e.Ispresent).HasColumnName("ispresent");
            entity.Property(e => e.Sabhatrackid).HasColumnName("sabhatrackid");
            entity.Property(e => e.Userid).HasColumnName("userid");

            entity.HasOne(d => d.Sabhatrack).WithMany(p => p.Attendences)
                .HasForeignKey(d => d.Sabhatrackid)
                .HasConstraintName("FK__Attendenc__sabha__5629CD9C");

            entity.HasOne(d => d.User).WithMany(p => p.Attendences)
                .HasForeignKey(d => d.Userid)
                .HasConstraintName("FK__Attendenc__useri__571DF1D5");
        });

        modelBuilder.Entity<Mandal>(entity =>
        {
            entity.HasKey(e => e.Mandalid).HasName("PK__Mandal__CCFD605E8B4ECF00");

            entity.ToTable("Mandal");

            entity.Property(e => e.Mandalid).HasColumnName("mandalid");
            entity.Property(e => e.Area)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("area");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("country");
            entity.Property(e => e.Mandalname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("mandalname");
            entity.Property(e => e.Pradeshid).HasColumnName("pradeshid");
            entity.Property(e => e.State)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("state");

            entity.HasOne(d => d.Pradesh).WithMany(p => p.Mandals)
                .HasForeignKey(d => d.Pradeshid)
                .HasConstraintName("FK__Mandal__pradeshi__398D8EEE");
        });

        modelBuilder.Entity<Pradesh>(entity =>
        {
            entity.HasKey(e => e.Pradeshid).HasName("PK__Pradesh__9CFD218E7AEDEF5B");

            entity.ToTable("Pradesh");

            entity.HasIndex(e => e.Pradeshname, "UQ__Pradesh__7274C23F3D88CD17").IsUnique();

            entity.Property(e => e.Pradeshid).HasColumnName("pradeshid");
            entity.Property(e => e.Pradeshname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("pradeshname");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("PK__Role__CD994BF24B2DEEC0");

            entity.ToTable("Role");

            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Rolename)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("rolename");
        });

        modelBuilder.Entity<Sabha>(entity =>
        {
            entity.HasKey(e => e.Sabhaid).HasName("PK__Sabha__5242669A924D8E4F");

            entity.ToTable("Sabha");

            entity.Property(e => e.Sabhaid).HasColumnName("sabhaid");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.Area)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("area");
            entity.Property(e => e.Contactpersonid).HasColumnName("contactpersonid");
            entity.Property(e => e.Googlemap)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("googlemap");
            entity.Property(e => e.Mandalid).HasColumnName("mandalid");
            entity.Property(e => e.Sabhaday)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sabhaday");
            entity.Property(e => e.Sabhaname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sabhaname");
            entity.Property(e => e.Sabhatime)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("sabhatime");
            entity.Property(e => e.Sabhatypeid).HasColumnName("sabhatypeid");

            entity.HasOne(d => d.Contactperson).WithMany(p => p.Sabhas)
                .HasForeignKey(d => d.Contactpersonid)
                .HasConstraintName("FK__Sabha__contactpe__49C3F6B7");

            entity.HasOne(d => d.Mandal).WithMany(p => p.Sabhas)
                .HasForeignKey(d => d.Mandalid)
                .HasConstraintName("FK__Sabha__mandalid__48CFD27E");

            entity.HasOne(d => d.Sabhatype).WithMany(p => p.Sabhas)
                .HasForeignKey(d => d.Sabhatypeid)
                .HasConstraintName("FK__Sabha__sabhatype__4AB81AF0");
        });

        modelBuilder.Entity<SabhaTrack>(entity =>
        {
            entity.HasKey(e => e.Sabhatrackid).HasName("PK__SabhaTra__3F19B55D20B088A2");

            entity.ToTable("SabhaTrack");

            entity.Property(e => e.Sabhatrackid).HasColumnName("sabhatrackid");
            entity.Property(e => e.Date)
                .HasColumnType("date")
                .HasColumnName("date");
            entity.Property(e => e.Sabhaid).HasColumnName("sabhaid");
            entity.Property(e => e.Topic)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("topic");

            entity.HasOne(d => d.Sabha).WithMany(p => p.SabhaTracks)
                .HasForeignKey(d => d.Sabhaid)
                .HasConstraintName("FK__SabhaTrac__sabha__5070F446");
        });

        modelBuilder.Entity<SabhaType>(entity =>
        {
            entity.HasKey(e => e.Sabhatypeid).HasName("PK__SabhaTyp__9F07F5D104E2D599");

            entity.ToTable("SabhaType");

            entity.HasIndex(e => e.Sabhatype1, "UQ__SabhaTyp__F37D93B89DC2CB20").IsUnique();

            entity.Property(e => e.Sabhatypeid).HasColumnName("sabhatypeid");
            entity.Property(e => e.Sabhatype1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("sabhatype");
        });

        modelBuilder.Entity<AksharUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__CB9A1CFF014908CD");

            entity.ToTable("AksharUser");

            entity.Property(e => e.UserId).HasColumnName("userId");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.BirthDate)
                .HasColumnType("date")
                .HasColumnName("birth_date");
            entity.Property(e => e.Bloodgroup)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("bloodgroup");
            entity.Property(e => e.Education)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("education");
            entity.Property(e => e.Educationstatus)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("educationstatus");
            entity.Property(e => e.Emailid)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("emailid");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstname");
            entity.Property(e => e.Followupby).HasColumnName("followupby");
            entity.Property(e => e.Isattending).HasColumnName("isattending");
            entity.Property(e => e.Isirregular).HasColumnName("isirregular");
            entity.Property(e => e.Iskaryakarta).HasColumnName("iskaryakarta");
            entity.Property(e => e.Ispresent).HasColumnName("ispresent");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastname");
            entity.Property(e => e.Maritalstatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("maritalstatus");
            entity.Property(e => e.Middlename)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("middlename");
            entity.Property(e => e.Mobile)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("mobile");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Pincode).HasColumnName("pincode");
            entity.Property(e => e.Referenceby).HasColumnName("referenceby");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.Sabhaid).HasColumnName("sabhaid");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("FK__User__roleid__30F848ED");

            entity.HasOne(d => d.Sabha).WithMany(p => p.Users)
                .HasForeignKey(d => d.Sabhaid)
                .HasConstraintName("FK_Sabha_User_sabhaid");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
