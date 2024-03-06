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
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=PerfectPickUsers_DB;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Connect Timeout=60;Encrypt=True;Trust Server Certificate=True;Command Timeout=0");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Country>(entity =>
        {
            entity.HasKey(e => e.IdCountry);

            entity.Property(e => e.IdCountry)
                .ValueGeneratedNever()
                .HasColumnName("id_country");
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

            entity.Property(e => e.IdUser).HasColumnName("id_user");
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
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
