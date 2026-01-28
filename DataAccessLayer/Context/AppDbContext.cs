using System;
using System.Collections.Generic;
using DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<audit_log> audit_logs { get; set; }

    public virtual DbSet<central_kitchen> central_kitchens { get; set; }

    public virtual DbSet<delivery> deliveries { get; set; }

    public virtual DbSet<franchise_store> franchise_stores { get; set; }

    public virtual DbSet<inventory> inventories { get; set; }

    public virtual DbSet<order> orders { get; set; }

    public virtual DbSet<order_item> order_items { get; set; }

    public virtual DbSet<product> products { get; set; }

    public virtual DbSet<product_category> product_categories { get; set; }

    public virtual DbSet<receipt> receipts { get; set; }

    public virtual DbSet<role> roles { get; set; }

    public virtual DbSet<user> users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<audit_log>(entity =>
        {
            entity.HasKey(e => e.id).HasName("audit_logs_pkey");

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.action).HasMaxLength(30);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.entity_id).HasMaxLength(40);
            entity.Property(e => e.entity_type).HasMaxLength(50);
            entity.Property(e => e.user_id).HasMaxLength(40);

            entity.HasOne(d => d.user).WithMany(p => p.audit_logs)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("audit_logs_user_id_fkey");
        });

        modelBuilder.Entity<central_kitchen>(entity =>
        {
            entity.HasKey(e => e.id).HasName("central_kitchens_pkey");

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.address).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(150);
        });

        modelBuilder.Entity<delivery>(entity =>
        {
            entity.HasKey(e => e.id).HasName("deliveries_pkey");

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.delivery_time).HasColumnType("timestamp without time zone");
            entity.Property(e => e.order_id).HasMaxLength(40);
            entity.Property(e => e.status).HasMaxLength(20);

            entity.HasOne(d => d.order).WithMany(p => p.deliveries)
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("deliveries_order_id_fkey");
        });

        modelBuilder.Entity<franchise_store>(entity =>
        {
            entity.HasKey(e => e.id).HasName("franchise_stores_pkey");

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.address).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(150);
            entity.Property(e => e.status).HasMaxLength(20);
        });

        modelBuilder.Entity<inventory>(entity =>
        {
            entity.HasKey(e => e.id).HasName("inventory_pkey");

            entity.ToTable("inventory");

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.location_id).HasMaxLength(40);
            entity.Property(e => e.location_type).HasMaxLength(20);
            entity.Property(e => e.product_id).HasMaxLength(40);
            entity.Property(e => e.quantity).HasPrecision(12, 2);
            entity.Property(e => e.updated_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.product).WithMany(p => p.inventories)
                .HasForeignKey(d => d.product_id)
                .HasConstraintName("inventory_product_id_fkey");
        });

        modelBuilder.Entity<order>(entity =>
        {
            entity.HasKey(e => e.id).HasName("orders_pkey");

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.expected_delivery_at).HasColumnType("timestamp without time zone");
            entity.Property(e => e.kitchen_id).HasMaxLength(40);
            entity.Property(e => e.status).HasMaxLength(20);
            entity.Property(e => e.store_id).HasMaxLength(40);

            entity.HasOne(d => d.kitchen).WithMany(p => p.orders)
                .HasForeignKey(d => d.kitchen_id)
                .HasConstraintName("orders_kitchen_id_fkey");

            entity.HasOne(d => d.store).WithMany(p => p.orders)
                .HasForeignKey(d => d.store_id)
                .HasConstraintName("orders_store_id_fkey");
        });

        modelBuilder.Entity<order_item>(entity =>
        {
            entity.HasKey(e => new { e.order_id, e.product_id }).HasName("order_items_pkey");

            entity.Property(e => e.order_id).HasMaxLength(40);
            entity.Property(e => e.product_id).HasMaxLength(40);
            entity.Property(e => e.quantity).HasPrecision(12, 2);

            entity.HasOne(d => d.order).WithMany(p => p.order_items)
                .HasForeignKey(d => d.order_id)
                .HasConstraintName("order_items_order_id_fkey");

            entity.HasOne(d => d.product).WithMany(p => p.order_items)
                .HasForeignKey(d => d.product_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_items_product_id_fkey");
        });

        modelBuilder.Entity<product>(entity =>
        {
            entity.HasKey(e => e.id).HasName("products_pkey");

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.category_id).HasMaxLength(40);
            entity.Property(e => e.name).HasMaxLength(150);
            entity.Property(e => e.type).HasMaxLength(30);
            entity.Property(e => e.unit).HasMaxLength(20);

            entity.HasOne(d => d.category).WithMany(p => p.products)
                .HasForeignKey(d => d.category_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("products_category_id_fkey");
        });

        modelBuilder.Entity<product_category>(entity =>
        {
            entity.HasKey(e => e.id).HasName("product_categories_pkey");

            entity.HasIndex(e => e.name, "product_categories_name_key").IsUnique();

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.name).HasMaxLength(100);
        });

        modelBuilder.Entity<receipt>(entity =>
        {
            entity.HasKey(e => e.id).HasName("receipts_pkey");

            entity.HasIndex(e => e.order_id, "receipts_order_id_key").IsUnique();

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.order_id).HasMaxLength(40);
            entity.Property(e => e.quality_note).HasMaxLength(255);
            entity.Property(e => e.received_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.order).WithOne(p => p.receipt)
                .HasForeignKey<receipt>(d => d.order_id)
                .HasConstraintName("receipts_order_id_fkey");
        });

        modelBuilder.Entity<role>(entity =>
        {
            entity.HasKey(e => e.id).HasName("roles_pkey");

            entity.HasIndex(e => e.name, "roles_name_key").IsUnique();

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.name).HasMaxLength(50);
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.HasIndex(e => e.email, "users_email_key").IsUnique();

            entity.Property(e => e.id).HasMaxLength(40);
            entity.Property(e => e.created_at)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.password_hash).HasMaxLength(255);
            entity.Property(e => e.status).HasMaxLength(20);

            entity.HasMany(d => d.roles).WithMany(p => p.users)
                .UsingEntity<Dictionary<string, object>>(
                    "user_role",
                    r => r.HasOne<role>().WithMany()
                        .HasForeignKey("role_id")
                        .HasConstraintName("user_roles_role_id_fkey"),
                    l => l.HasOne<user>().WithMany()
                        .HasForeignKey("user_id")
                        .HasConstraintName("user_roles_user_id_fkey"),
                    j =>
                    {
                        j.HasKey("user_id", "role_id").HasName("user_roles_pkey");
                        j.ToTable("user_roles");
                        j.IndexerProperty<string>("user_id").HasMaxLength(40);
                        j.IndexerProperty<string>("role_id").HasMaxLength(40);
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
