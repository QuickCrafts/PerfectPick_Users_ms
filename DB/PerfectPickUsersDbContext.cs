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

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;Database=PerfectPickUsers_DB;Integrated Security=True;Persist Security Info=False;Pooling=False;Multiple Active Result Sets=False;Connect Timeout=60;Encrypt=True;Trust Server Certificate=True;Command Timeout=0");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserEmail);

            entity.Property(e => e.UserEmail)
                .HasMaxLength(320)
                .IsUnicode(false)
                .HasColumnName("User_Email");
            entity.Property(e => e.UserIsAdmin).HasColumnName("User_isAdmin");
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .HasColumnName("User_Name");
            entity.Property(e => e.UserPassword)
                .HasMaxLength(60)
                .IsUnicode(false)
                .HasColumnName("User_Password");
            entity.Property(e => e.UserSurname)
                .HasMaxLength(50)
                .HasColumnName("User_Surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
