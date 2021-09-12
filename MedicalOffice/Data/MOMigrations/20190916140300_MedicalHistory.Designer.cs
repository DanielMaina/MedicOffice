﻿// <auto-generated />
using System;
using MedicalOffice.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace MedicalOffice.Data.MOMigrations
{
    [DbContext(typeof(MedicalOfficeContext))]
    [Migration("20190916140300_MedicalHistory")]
    partial class MedicalHistory
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("MO")
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MedicalOffice.Models.Condition", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ConditionName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.ToTable("Conditions");
                });

            modelBuilder.Entity("MedicalOffice.Models.Doctor", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.ToTable("Doctors");
                });

            modelBuilder.Entity("MedicalOffice.Models.Patient", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime?>("DOB");

                    b.Property<int>("DoctorID");

                    b.Property<byte>("ExpYrVisits");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50);

                    b.Property<string>("OHIP")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<long>("Phone");

                    b.Property<string>("eMail")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("ID");

                    b.HasIndex("DoctorID");

                    b.HasIndex("OHIP")
                        .IsUnique();

                    b.ToTable("Patients");
                });

            modelBuilder.Entity("MedicalOffice.Models.PatientCondition", b =>
                {
                    b.Property<int>("ConditionID");

                    b.Property<int>("PatientID");

                    b.HasKey("ConditionID", "PatientID");

                    b.HasIndex("PatientID");

                    b.ToTable("PatientConditions");
                });

            modelBuilder.Entity("MedicalOffice.Models.Patient", b =>
                {
                    b.HasOne("MedicalOffice.Models.Doctor", "Doctor")
                        .WithMany("Patients")
                        .HasForeignKey("DoctorID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MedicalOffice.Models.PatientCondition", b =>
                {
                    b.HasOne("MedicalOffice.Models.Condition", "Condition")
                        .WithMany("PatientConditions")
                        .HasForeignKey("ConditionID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MedicalOffice.Models.Patient", "Patient")
                        .WithMany("PatientConditions")
                        .HasForeignKey("PatientID")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
