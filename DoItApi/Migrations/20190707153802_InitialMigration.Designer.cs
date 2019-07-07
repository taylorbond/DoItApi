﻿// <auto-generated />
using System;
using DoItApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DoItApi.Migrations
{
    [DbContext(typeof(DoItDbContext))]
    [Migration("20190707153802_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DoItApi.Models.AlertTime", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("TaskId");

                    b.Property<DateTimeOffset>("Time");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("AlertTime");
                });

            modelBuilder.Entity("DoItApi.Models.Comment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("InsertedTime");

                    b.Property<string>("TaskId");

                    b.Property<string>("Text")
                        .IsRequired();

                    b.Property<DateTimeOffset>("UpdatedTime");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("TaskId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("DoItApi.Models.Task", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTimeOffset>("DueDateTime");

                    b.Property<string>("TaskDescription")
                        .IsRequired();

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Tasks");
                });

            modelBuilder.Entity("DoItApi.Models.AlertTime", b =>
                {
                    b.HasOne("DoItApi.Models.Task")
                        .WithMany("AlertTimes")
                        .HasForeignKey("TaskId");
                });

            modelBuilder.Entity("DoItApi.Models.Comment", b =>
                {
                    b.HasOne("DoItApi.Models.Task")
                        .WithMany("Comments")
                        .HasForeignKey("TaskId");
                });
#pragma warning restore 612, 618
        }
    }
}
