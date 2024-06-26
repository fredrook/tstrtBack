﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Estacionamentos",
                columns: table => new
                {
                    IdEstacionamento = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estacionamentos", x => x.IdEstacionamento);
                });

            migrationBuilder.CreateTable(
                name: "Veiculos",
                columns: table => new
                {
                    IdVeiculo = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Tipo = table.Column<int>(type: "integer", nullable: false),
                    Placa = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    Cor = table.Column<string>(type: "text", nullable: true),
                    VagaId = table.Column<int>(type: "integer", nullable: false),
                    HoraEntrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Veiculos", x => x.IdVeiculo);
                });

            migrationBuilder.CreateTable(
                name: "Vagas",
                columns: table => new
                {
                    IdVaga = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ocupada = table.Column<bool>(type: "boolean", nullable: false),
                    TipoVaga = table.Column<int>(type: "integer", nullable: false),
                    VeiculoId = table.Column<int>(type: "integer", nullable: true),
                    EstacionamentoIdEstacionamento = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vagas", x => x.IdVaga);
                    table.ForeignKey(
                        name: "FK_Vagas_Estacionamentos_EstacionamentoIdEstacionamento",
                        column: x => x.EstacionamentoIdEstacionamento,
                        principalTable: "Estacionamentos",
                        principalColumn: "IdEstacionamento");
                    table.ForeignKey(
                        name: "FK_Vagas_Veiculos_VeiculoId",
                        column: x => x.VeiculoId,
                        principalTable: "Veiculos",
                        principalColumn: "IdVeiculo");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vagas_EstacionamentoIdEstacionamento",
                table: "Vagas",
                column: "EstacionamentoIdEstacionamento");

            migrationBuilder.CreateIndex(
                name: "IX_Vagas_VeiculoId",
                table: "Vagas",
                column: "VeiculoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Vagas");

            migrationBuilder.DropTable(
                name: "Estacionamentos");

            migrationBuilder.DropTable(
                name: "Veiculos");
        }
    }
}
