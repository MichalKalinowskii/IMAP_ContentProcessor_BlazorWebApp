using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace IMAP_ContentProcessor_BlazorWebApp.Infrastructure.Models;

public partial class ZamowieniaImapContext : DbContext
{
    public ZamowieniaImapContext()
    {
    }

    public ZamowieniaImapContext(DbContextOptions<ZamowieniaImapContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Mail> Mail { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;port=3306;database=zamowieniaImap;user=root;password=root");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Mail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.HasIndex(e => e.TokenId, "TokenID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.Eml).HasColumnName("eml");
            entity.Property(e => e.TokenId).HasColumnName("TokenID");

            entity.HasOne(d => d.Token).WithMany(p => p.Mail)
                .HasForeignKey(d => d.TokenId)
                .HasConstraintName("Mail_ibfk_1");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("OrderDetail");

            entity.HasIndex(e => e.MailId, "MailID");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MailId).HasColumnName("MailID");
            entity.Property(e => e.Price)
                .HasPrecision(10)
                .HasColumnName("price");
            entity.Property(e => e.ProductName)
                .HasMaxLength(2048)
                .HasColumnName("productName");
            entity.Property(e => e.ProductQuantity).HasColumnName("productQuantity");

            entity.HasOne(d => d.Mail).WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.MailId)
                .HasConstraintName("OrderDetail_ibfk_1");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("Token");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccessToken)
                .HasMaxLength(2048)
                .HasColumnName("access_token");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.EmailGeneratedId).HasColumnName("emailGeneratedID");
            entity.Property(e => e.RefreshToken)
                .HasMaxLength(2048)
                .HasColumnName("refresh_token");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
