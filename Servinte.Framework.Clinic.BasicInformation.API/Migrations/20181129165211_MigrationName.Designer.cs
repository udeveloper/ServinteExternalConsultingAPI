﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Servinte.Framework.Clinic.BasicInformation.Infraestructure;

namespace Servinte.Framework.Clinic.BasicInformation.API.Migrations
{
    [DbContext(typeof(ExternalConsultingContext))]
    [Migration("20181129165211_MigrationName")]
    partial class MigrationName
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024");

            modelBuilder.Entity("Servinte.Framework.Clinic.BasicInformation.Infraestructure.Patient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("Edad");

                    b.Property<string>("Genero");

                    b.Property<long>("Identificador");

                    b.Property<decimal>("MasaCorporal");

                    b.Property<string>("Nombre");

                    b.Property<long>("NumeroIdentificacion");

                    b.Property<decimal>("Peso");

                    b.Property<decimal>("SuperficieCorporal");

                    b.Property<decimal>("Talla");

                    b.Property<string>("TipoIdentificacion");

                    b.HasKey("Id");

                    b.ToTable("Patients");
                });
#pragma warning restore 612, 618
        }
    }
}
