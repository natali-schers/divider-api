using Microsoft.EntityFrameworkCore;
using Divider.Models;

namespace Divider.Data;

public class DividerDbContext : DbContext
{
    public DividerDbContext(DbContextOptions<DividerDbContext> options) : base(options)
    {
    }

    public DbSet<Group> Groups => Set<Group>();

    public DbSet<Member> Members => Set<Member>();

    public DbSet<Expense> Expenses => Set<Expense>()
        ;
    public DbSet<ExpenseSplit> ExpenseSplits => Set<ExpenseSplit>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Um Member pertence a um Group
        modelBuilder.Entity<Member>()
            .HasOne(m => m.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(m => m.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Uma Expense pertence a um Group
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.Group)
            .WithMany(g => g.Expenses)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Uma Expense tem um Member pagador
        modelBuilder.Entity<Expense>()
            .HasOne(e => e.PaidByMember)
            .WithMany(m => m.ExpensesPaid)
            .HasForeignKey(e => e.PaidByMemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Um ExpenseSplit pertence a uma Expense
        modelBuilder.Entity<ExpenseSplit>()
            .HasOne(es => es.Expense)
            .WithMany(e => e.Splits)
            .HasForeignKey(es => es.ExpenseId)
            .OnDelete(DeleteBehavior.Cascade);

        // Um ExpenseSplit pertence a um Member
        modelBuilder.Entity<ExpenseSplit>()
            .HasOne(es => es.Member)
            .WithMany(m => m.ExpenseSplits)
            .HasForeignKey(es => es.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        // Amount como decimal com precisão definida
        modelBuilder.Entity<Expense>()
            .Property(e => e.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<ExpenseSplit>()
            .Property(es => es.Amount)
            .HasPrecision(18, 2);

        // Configuração de índice único para o campo Email do User
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}