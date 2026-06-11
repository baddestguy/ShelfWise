using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace ShelfWise.Repository.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class ShelfWiseDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("ShelfWise.Domain.Models.Book", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("Author").IsRequired();
                b.Property<string>("Genre").IsRequired();
                b.Property<string>("Title").IsRequired();
                b.Property<int>("TotalCopies");
                b.Property<int>("Category");
                b.HasKey("Id");
                b.ToTable("Books");
            });

            modelBuilder.Entity("ShelfWise.Domain.Models.User", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<string>("FirstName").IsRequired();
                b.Property<string>("LastName").IsRequired();
                b.HasKey("Id");
                b.ToTable("Users");
            });

            modelBuilder.Entity("ShelfWise.Domain.Models.BorrowRecord", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("BookId");
                b.Property<DateTime>("CheckedOutAt");
                b.Property<DateTime?>("DueAt");
                b.Property<DateTime?>("ReturnedAt");
                b.Property<int>("UserId");
                b.HasKey("Id");
                b.HasIndex("BookId");
                b.HasIndex("UserId");
                b.ToTable("BorrowRecords");
            });

            modelBuilder.Entity("ShelfWise.Domain.Models.HoldRecord", b =>
            {
                b.Property<int>("Id").ValueGeneratedOnAdd();
                b.Property<int>("BookId");
                b.Property<DateTime>("CreatedAt");
                b.Property<DateTime?>("ExpiresAt");
                b.Property<DateTime?>("NotifiedAt");
                b.Property<int>("UserId");
                b.HasKey("Id");
                b.HasIndex("BookId");
                b.HasIndex("UserId");
                b.ToTable("HoldRecords");
            });

            modelBuilder.Entity("ShelfWise.Domain.Models.BorrowRecord", b =>
            {
                b.HasOne("ShelfWise.Domain.Models.Book")
                    .WithMany()
                    .HasForeignKey("BookId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("ShelfWise.Domain.Models.User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity("ShelfWise.Domain.Models.HoldRecord", b =>
            {
                b.HasOne("ShelfWise.Domain.Models.Book")
                    .WithMany()
                    .HasForeignKey("BookId")
                    .OnDelete(DeleteBehavior.Cascade);

                b.HasOne("ShelfWise.Domain.Models.User")
                    .WithMany()
                    .HasForeignKey("UserId")
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
