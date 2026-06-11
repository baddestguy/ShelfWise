using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShelfWise.Domain.Models;

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

            modelBuilder.Entity<Book>(b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                b.Property<string>("Author")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<int>("Available")
                    .HasColumnType("integer");

                b.Property<int>("Category")
                    .HasColumnType("integer");

                b.Property<int>("OnHold")
                    .HasColumnType("integer");

                b.Property<string>("Genre")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<string>("Title")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<int>("TotalCopies")
                    .HasColumnType("integer");

                b.HasKey("Id");

                b.ToTable("Books");
            });
        }
    }
}
