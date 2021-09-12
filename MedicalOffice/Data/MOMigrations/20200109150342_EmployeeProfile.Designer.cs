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
    [Migration("20200109150342_EmployeeProfile")]
    partial class EmployeeProfile
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("MO")
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("MedicalOffice.Models.Appointment", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("ApptReasonID");

                    b.Property<string>("Notes")
                        .IsRequired()
                        .HasMaxLength(2000);

                    b.Property<int>("PatientID");

                    b.Property<DateTime>("appDate");

                    b.Property<decimal>("extraFee");

                    b.HasKey("ID");

                    b.HasIndex("ApptReasonID");

                    b.HasIndex("PatientID");

                    b.ToTable("Appointments");
                });

            modelBuilder.Entity("MedicalOffice.Models.ApptReason", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ReasonName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("ID");

                    b.ToTable("ApptReasons");
                });

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

            modelBuilder.Entity("MedicalOffice.Models.DoctorSpecialty", b =>
                {
                    b.Property<int>("DoctorID");

                    b.Property<int>("SpecialtyID");

                    b.HasKey("DoctorID", "SpecialtyID");

                    b.HasIndex("SpecialtyID");

                    b.ToTable("DoctorSpecialties");
                });

            modelBuilder.Entity("MedicalOffice.Models.Employee", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("Active");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256);

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<string>("FavouriteIceCream")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Phone")
                        .HasMaxLength(10);

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256);

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<string>("eMail")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.HasKey("ID");

                    b.HasIndex("eMail")
                        .IsUnique();

                    b.ToTable("Employees");
                });

            modelBuilder.Entity("MedicalOffice.Models.MedicalTrial", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("TrialName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.ToTable("MedicalTrials");
                });

            modelBuilder.Entity("MedicalOffice.Models.Patient", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(256);

                    b.Property<DateTime?>("CreatedOn");

                    b.Property<DateTime?>("DOB");

                    b.Property<int>("DoctorID");

                    b.Property<byte>("ExpYrVisits");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<int?>("MedicalTrialID");

                    b.Property<string>("MiddleName")
                        .HasMaxLength(50);

                    b.Property<string>("OHIP")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<long>("Phone");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate();

                    b.Property<string>("UpdatedBy")
                        .HasMaxLength(256);

                    b.Property<DateTime?>("UpdatedOn");

                    b.Property<string>("eMail")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<byte[]>("imageContent");

                    b.Property<string>("imageFileName")
                        .HasMaxLength(100);

                    b.Property<string>("imageMimeType")
                        .HasMaxLength(256);

                    b.HasKey("ID");

                    b.HasIndex("DoctorID");

                    b.HasIndex("MedicalTrialID");

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

            modelBuilder.Entity("MedicalOffice.Models.Specialty", b =>
                {
                    b.Property<int>("ID")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("SpecialtyName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("ID");

                    b.ToTable("Specialties");
                });

            modelBuilder.Entity("MedicalOffice.Models.Appointment", b =>
                {
                    b.HasOne("MedicalOffice.Models.ApptReason", "ApptReason")
                        .WithMany("Appointments")
                        .HasForeignKey("ApptReasonID");

                    b.HasOne("MedicalOffice.Models.Patient", "Patient")
                        .WithMany("Appointments")
                        .HasForeignKey("PatientID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("MedicalOffice.Models.DoctorSpecialty", b =>
                {
                    b.HasOne("MedicalOffice.Models.Doctor", "Doctor")
                        .WithMany("DoctorSpecialties")
                        .HasForeignKey("DoctorID")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("MedicalOffice.Models.Specialty", "Specialty")
                        .WithMany("DoctorSpecialties")
                        .HasForeignKey("SpecialtyID")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("MedicalOffice.Models.Patient", b =>
                {
                    b.HasOne("MedicalOffice.Models.Doctor", "Doctor")
                        .WithMany("Patients")
                        .HasForeignKey("DoctorID")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("MedicalOffice.Models.MedicalTrial", "MedicalTrial")
                        .WithMany("Patients")
                        .HasForeignKey("MedicalTrialID");
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
