﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VehicleLoanSystem.Data;

#nullable disable

namespace VehicleLoanSystem.Migrations
{
    [DbContext(typeof(VehicleLoanSystemContext))]
    [Migration("20240327030941_initialcreate")]
    partial class initialcreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            MySqlModelBuilderExtensions.AutoIncrementColumns(modelBuilder);

            modelBuilder.Entity("VehicleLoanSystem.Models.Loan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<byte[]>("CibilImage")
                        .HasColumnType("longblob");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<byte[]>("IdentityImage")
                        .HasColumnType("longblob");

                    b.Property<byte[]>("IncomeImage")
                        .HasColumnType("longblob");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<double>("LoanAmount")
                        .HasColumnType("double");

                    b.Property<DateTime>("LoanDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("LoanGrant")
                        .HasColumnType("longtext");

                    b.Property<int>("LoanPlanId")
                        .HasColumnType("int");

                    b.Property<string>("LoanPurpose")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("LoanTypeId")
                        .HasColumnType("int");

                    b.Property<double>("MonthlyPayableAmount")
                        .HasColumnType("double");

                    b.Property<double>("MonthlyPenalty")
                        .HasColumnType("double");

                    b.Property<decimal>("Phone")
                        .HasColumnType("decimal(65,30)");

                    b.Property<string>("RejectionReason")
                        .HasColumnType("longtext");

                    b.Property<double>("Salary")
                        .HasColumnType("double");

                    b.Property<double>("TotalPayableAmount")
                        .HasColumnType("double");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<string>("credit_score")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Loans");
                });

            modelBuilder.Entity("VehicleLoanSystem.Models.LoanPlan", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<double>("Interest")
                        .HasColumnType("double");

                    b.Property<int>("Month")
                        .HasColumnType("int");

                    b.Property<double>("MonthlyOverDuePenalty")
                        .HasColumnType("double");

                    b.HasKey("Id");

                    b.ToTable("LoanPlans");
                });

            modelBuilder.Entity("VehicleLoanSystem.Models.LoanType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("LoanDescription")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("LoanTypeName")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("LoanTypes");
                });

            modelBuilder.Entity("VehicleLoanSystem.Models.Payment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("LoanCovered")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LoanStatus")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("NextPaymentDate")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("PayedAmount")
                        .HasColumnType("double");

                    b.Property<DateTime>("PayedDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("PayedMonth")
                        .HasColumnType("datetime(6)");

                    b.Property<double>("PenaltyPaymentAmount")
                        .HasColumnType("double");

                    b.Property<double>("RemainingLoanAmount")
                        .HasColumnType("double");

                    b.Property<double>("RemainingMonthPayment")
                        .HasColumnType("double");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Payments");
                });

            modelBuilder.Entity("VehicleLoanSystem.Models.UserAccount", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    MySqlPropertyBuilderExtensions.UseMySqlIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("User_Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("User_Password")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.ToTable("Accounts");
                });
#pragma warning restore 612, 618
        }
    }
}
