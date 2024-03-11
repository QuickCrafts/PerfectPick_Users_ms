using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace _PerfectPickUsers_MS.DB;

public partial class PerfectPickUsersDbContext : DbContext
{
    public PerfectPickUsersDbContext()
    {
    }

    public PerfectPickUsersDbContext(DbContextOptions<PerfectPickUsersDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<ResToken> ResTokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(Environment.GetEnvironmentVariable("connectionString"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IdCountry);

            entity.Property(e => e.IdCountry).HasColumnName("id_country");
            entity.Property(e => e.Code2)
                .HasMaxLength(2)
                .IsUnicode(false)
                .HasColumnName("code_2");
            entity.Property(e => e.Code3)
                .HasMaxLength(3)
                .IsUnicode(false)
                .HasColumnName("code_3");
            entity.Property(e => e.Name)
                .HasMaxLength(70)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<ResToken>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.ToTable("Res_Token");

            entity.Property(e => e.IdUser)
                .ValueGeneratedNever()
                .HasColumnName("Id_user");
            entity.Property(e => e.Token)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("token");

            entity.HasOne(d => d.IdUserNavigation).WithOne(p => p.ResToken)
                .HasForeignKey<ResToken>(d => d.IdUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Res_Token_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.IdUser);

            entity.HasIndex(e => e.Email, "UniqueEmail_Users").IsUnique();

            entity.Property(e => e.IdUser).HasColumnName("id_user");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("avatar_url");
            entity.Property(e => e.Birthdate)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("birthdate");
            entity.Property(e => e.CreatedTime)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("created_time");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("first_name");
            entity.Property(e => e.Gender)
                .HasMaxLength(1)
                .IsUnicode(false)
                .HasColumnName("gender");
            entity.Property(e => e.IdCountry).HasColumnName("id_country");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("last_name");
            entity.Property(e => e.Password)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Setup).HasColumnName("setup");
            entity.Property(e => e.Verified).HasColumnName("verified");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
