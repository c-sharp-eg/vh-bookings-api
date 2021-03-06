﻿// <auto-generated />
using System;
using Bookings.DAL;
using Bookings.Domain.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Bookings.DAL.Migrations
{
    [DbContext(typeof(BookingsDbContext))]
    [Migration("20190529132908_add_suitabilityAnswerTable")]
    partial class add_suitabilityAnswerTable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Bookings.Domain.Address", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("City");

                    b.Property<string>("County");

                    b.Property<string>("HouseNumber");

                    b.Property<string>("Postcode");

                    b.Property<string>("Street");

                    b.HasKey("Id");

                    b.ToTable("Address");
                });

            modelBuilder.Entity("Bookings.Domain.Case", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<bool>("IsLeadCase");

                    b.Property<string>("Name");

                    b.Property<string>("Number");

                    b.HasKey("Id");

                    b.ToTable("Case");
                });

            modelBuilder.Entity("Bookings.Domain.Hearing", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CaseTypeId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("HearingMediumType")
                        .HasColumnName("HearingMediumId");

                    b.Property<string>("HearingRoomName");

                    b.Property<int>("HearingTypeId");

                    b.Property<string>("HearingVenueName");

                    b.Property<string>("OtherInformation");

                    b.Property<DateTime>("ScheduledDateTime");

                    b.Property<int>("ScheduledDuration");

                    b.Property<int>("Status")
                        .HasColumnName("HearingStatusId");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("Id");

                    b.HasIndex("CaseTypeId");

                    b.HasIndex("HearingTypeId");

                    b.HasIndex("HearingVenueName");

                    b.ToTable("Hearing");

                    b.HasDiscriminator<int>("HearingMediumType");
                });

            modelBuilder.Entity("Bookings.Domain.HearingCase", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<long>("CaseId");

                    b.Property<Guid>("HearingId");

                    b.HasKey("Id");

                    b.HasIndex("HearingId");

                    b.HasIndex("CaseId", "HearingId")
                        .IsUnique();

                    b.ToTable("HearingCase");
                });

            modelBuilder.Entity("Bookings.Domain.HearingVenue", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Id");

                    b.HasKey("Name");

                    b.ToTable("HearingVenue");
                });

            modelBuilder.Entity("Bookings.Domain.Organisation", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Organisation");
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Participant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CaseRoleId");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Discriminator")
                        .IsRequired();

                    b.Property<string>("DisplayName");

                    b.Property<Guid>("HearingId");

                    b.Property<int>("HearingRoleId");

                    b.Property<Guid>("PersonId");

                    b.Property<string>("UpdatedBy");

                    b.Property<DateTime>("UpdatedDate");

                    b.HasKey("Id");

                    b.HasIndex("CaseRoleId");

                    b.HasIndex("HearingId");

                    b.HasIndex("HearingRoleId");

                    b.HasIndex("PersonId", "HearingId")
                        .IsUnique();

                    b.ToTable("Participant");

                    b.HasDiscriminator<string>("Discriminator").HasValue("Participant");
                });

            modelBuilder.Entity("Bookings.Domain.Person", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<long?>("AddressId");

                    b.Property<string>("ContactEmail");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("FirstName");

                    b.Property<string>("LastName");

                    b.Property<string>("MiddleNames");

                    b.Property<long?>("OrganisationId");

                    b.Property<string>("TelephoneNumber");

                    b.Property<string>("Title");

                    b.Property<DateTime>("UpdatedDate");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.HasIndex("ContactEmail")
                        .IsUnique()
                        .HasFilter("[ContactEmail] IS NOT NULL");

                    b.HasIndex("OrganisationId");

                    b.HasIndex("Username")
                        .IsUnique()
                        .HasFilter("[Username] IS NOT NULL");

                    b.ToTable("Person");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.CaseRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaseTypeId");

                    b.Property<int>("Group");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CaseTypeId");

                    b.ToTable("CaseRole");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.CaseType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CaseType");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.HearingRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaseRoleId");

                    b.Property<string>("Name");

                    b.Property<int>("UserRoleId");

                    b.HasKey("Id");

                    b.HasIndex("CaseRoleId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("HearingRole");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.HearingType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("CaseTypeId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("CaseTypeId");

                    b.ToTable("HearingType");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.UserRole", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("UserRole");
                });

            modelBuilder.Entity("Bookings.Domain.SuitabilityAnswer", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Data")
                        .IsRequired();

                    b.Property<string>("ExtendedData");

                    b.Property<string>("Key")
                        .IsRequired();

                    b.Property<Guid>("PersonId");

                    b.HasKey("Id");

                    b.HasIndex("PersonId");

                    b.ToTable("SuitabilityAnswer");
                });

            modelBuilder.Entity("Bookings.Domain.VideoHearing", b =>
                {
                    b.HasBaseType("Bookings.Domain.Hearing");

                    b.HasDiscriminator().HasValue(1);
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Individual", b =>
                {
                    b.HasBaseType("Bookings.Domain.Participants.Participant");

                    b.HasDiscriminator().HasValue("Individual");
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Judge", b =>
                {
                    b.HasBaseType("Bookings.Domain.Participants.Participant");

                    b.HasDiscriminator().HasValue("Judge");
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Representative", b =>
                {
                    b.HasBaseType("Bookings.Domain.Participants.Participant");

                    b.Property<string>("Representee");

                    b.Property<string>("SolicitorsReference");

                    b.HasDiscriminator().HasValue("Representative");
                });

            modelBuilder.Entity("Bookings.Domain.Hearing", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseType", "CaseType")
                        .WithMany()
                        .HasForeignKey("CaseTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Bookings.Domain.RefData.HearingType", "HearingType")
                        .WithMany()
                        .HasForeignKey("HearingTypeId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Bookings.Domain.HearingVenue", "HearingVenue")
                        .WithMany()
                        .HasForeignKey("HearingVenueName");
                });

            modelBuilder.Entity("Bookings.Domain.HearingCase", b =>
                {
                    b.HasOne("Bookings.Domain.Case", "Case")
                        .WithMany("HearingCases")
                        .HasForeignKey("CaseId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Bookings.Domain.Hearing", "Hearing")
                        .WithMany("HearingCases")
                        .HasForeignKey("HearingId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Bookings.Domain.Participants.Participant", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseRole", "CaseRole")
                        .WithMany()
                        .HasForeignKey("CaseRoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Bookings.Domain.Hearing", "Hearing")
                        .WithMany("Participants")
                        .HasForeignKey("HearingId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Bookings.Domain.RefData.HearingRole", "HearingRole")
                        .WithMany()
                        .HasForeignKey("HearingRoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Bookings.Domain.Person", "Person")
                        .WithMany()
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Bookings.Domain.Person", b =>
                {
                    b.HasOne("Bookings.Domain.Address", "Address")
                        .WithMany()
                        .HasForeignKey("AddressId");

                    b.HasOne("Bookings.Domain.Organisation", "Organisation")
                        .WithMany()
                        .HasForeignKey("OrganisationId");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.CaseRole", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseType")
                        .WithMany("CaseRoles")
                        .HasForeignKey("CaseTypeId");
                });

            modelBuilder.Entity("Bookings.Domain.RefData.HearingRole", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseRole")
                        .WithMany("HearingRoles")
                        .HasForeignKey("CaseRoleId");

                    b.HasOne("Bookings.Domain.RefData.UserRole", "UserRole")
                        .WithMany()
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Bookings.Domain.RefData.HearingType", b =>
                {
                    b.HasOne("Bookings.Domain.RefData.CaseType")
                        .WithMany("HearingTypes")
                        .HasForeignKey("CaseTypeId");
                });

            modelBuilder.Entity("Bookings.Domain.SuitabilityAnswer", b =>
                {
                    b.HasOne("Bookings.Domain.Person", "Person")
                        .WithMany("SuitabilityAnswers")
                        .HasForeignKey("PersonId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
