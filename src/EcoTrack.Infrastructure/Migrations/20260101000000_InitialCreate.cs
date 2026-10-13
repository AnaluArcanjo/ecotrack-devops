using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ET_TIPO_RESIDUO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorLixeira = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Instrucao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ET_TIPO_RESIDUO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ET_USUARIO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Perfil = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ET_USUARIO", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ET_PONTO_COLETA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Endereco = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Cidade = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    Cep = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: false),
                    Latitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    Longitude = table.Column<decimal>(type: "decimal(10,7)", nullable: true),
                    CapacidadeKg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    OcupacaoAtual = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ET_PONTO_COLETA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ET_PONTO_COLETA_ET_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "ET_USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ET_NOTIFICACAO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Lida = table.Column<bool>(type: "bit", nullable: false),
                    DataLeitura = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ET_NOTIFICACAO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ET_NOTIFICACAO_ET_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "ET_USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ET_RELATORIO",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PeriodoInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PeriodoFim = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalKg = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    TotalColetas = table.Column<long>(type: "bigint", nullable: false),
                    Co2EvitadoKg = table.Column<decimal>(type: "decimal(12,2)", nullable: false),
                    GeradoPorId = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ET_RELATORIO", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ET_RELATORIO_ET_USUARIO_GeradoPorId",
                        column: x => x.GeradoPorId,
                        principalTable: "ET_USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ET_ALERTA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PontoColetaId = table.Column<int>(type: "int", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Resolvido = table.Column<bool>(type: "bit", nullable: false),
                    DataResolucao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ET_ALERTA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ET_ALERTA_ET_PONTO_COLETA_PontoColetaId",
                        column: x => x.PontoColetaId,
                        principalTable: "ET_PONTO_COLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ET_COLETA",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PontoColetaId = table.Column<int>(type: "int", nullable: false),
                    TipoResiduoId = table.Column<int>(type: "int", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    QuantidadeKg = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DataAgendamento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DataColeta = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Observacao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DataAtualizacao = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Ativo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ET_COLETA", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ET_COLETA_ET_PONTO_COLETA_PontoColetaId",
                        column: x => x.PontoColetaId,
                        principalTable: "ET_PONTO_COLETA",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ET_COLETA_ET_TIPO_RESIDUO_TipoResiduoId",
                        column: x => x.TipoResiduoId,
                        principalTable: "ET_TIPO_RESIDUO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ET_COLETA_ET_USUARIO_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "ET_USUARIO",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            // ===== Índices =====
            migrationBuilder.CreateIndex(name: "IX_ET_USUARIO_Email", table: "ET_USUARIO", column: "Email", unique: true);
            migrationBuilder.CreateIndex(name: "IX_ET_TIPO_RESIDUO_Nome", table: "ET_TIPO_RESIDUO", column: "Nome", unique: true);
            migrationBuilder.CreateIndex(name: "IX_ET_PONTO_COLETA_UsuarioId", table: "ET_PONTO_COLETA", column: "UsuarioId");
            migrationBuilder.CreateIndex(name: "IX_ET_NOTIFICACAO_UsuarioId", table: "ET_NOTIFICACAO", column: "UsuarioId");
            migrationBuilder.CreateIndex(name: "IX_ET_RELATORIO_GeradoPorId", table: "ET_RELATORIO", column: "GeradoPorId");
            migrationBuilder.CreateIndex(name: "IX_ET_ALERTA_PontoColetaId", table: "ET_ALERTA", column: "PontoColetaId");
            migrationBuilder.CreateIndex(name: "IX_ET_COLETA_PontoColetaId", table: "ET_COLETA", column: "PontoColetaId");
            migrationBuilder.CreateIndex(name: "IX_ET_COLETA_TipoResiduoId", table: "ET_COLETA", column: "TipoResiduoId");
            migrationBuilder.CreateIndex(name: "IX_ET_COLETA_UsuarioId", table: "ET_COLETA", column: "UsuarioId");

            // ===== Seed: Tipos de Resíduo =====
            migrationBuilder.InsertData(
                table: "ET_TIPO_RESIDUO",
                columns: new[] { "Id", "Nome", "Descricao", "CorLixeira", "Instrucao", "DataCriacao", "Ativo" },
                values: new object[,]
                {
                    { 1, "Papel/Papelão", "Jornais, revistas, caixas, cadernos e papéis em geral", "AZUL", "Amasse papéis e separe papelão. Evite materiais molhados ou engordurados.", new DateTime(2026,1,1), true },
                    { 2, "Plástico", "Embalagens, garrafas PET, sacolas e utensílios plásticos", "VERMELHO", "Enxágue as embalagens antes de descartar. Amasse garrafas PET.", new DateTime(2026,1,1), true },
                    { 3, "Vidro", "Garrafas, potes, frascos e recipientes de vidro", "VERDE", "Enxágue os recipientes. Não quebre o vidro. Cuidado com cacos.", new DateTime(2026,1,1), true },
                    { 4, "Metal", "Latas de alumínio, aço, ferragens e embalagens metálicas", "AMARELO", "Enxágue as latas. Amasse para economizar espaço.", new DateTime(2026,1,1), true },
                    { 5, "Eletrônico", "Celulares, computadores, pilhas, baterias e eletrodomésticos", "LARANJA", "Nunca descarte em lixo comum. Procure pontos de coleta específicos.", new DateTime(2026,1,1), true },
                    { 6, "Orgânico", "Restos de alimentos, cascas de frutas e verduras", "MARROM", "Ideal para compostagem. Separe de embalagens.", new DateTime(2026,1,1), true }
                });

            // ===== Seed: Usuários =====
            migrationBuilder.InsertData(
                table: "ET_USUARIO",
                columns: new[] { "Id", "Nome", "Email", "SenhaHash", "Perfil", "DataCriacao", "Ativo" },
                values: new object[,]
                {
                    { 1, "Administrador EcoTrack", "admin@ecotrack.com.br", "$2b$11$rt8A9jGPuERfFd3/fGWnAeLZiueXr07HsM9tzx61wN2pWSOrPVKRS", "Admin", new DateTime(2026,1,1), true },
                    { 2, "Empresa Exemplo Ltda", "empresa@exemplo.com.br", "$2b$11$rt8A9jGPuERfFd3/fGWnAeLZiueXr07HsM9tzx61wN2pWSOrPVKRS", "Empresa", new DateTime(2026,1,1), true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ET_COLETA");
            migrationBuilder.DropTable(name: "ET_ALERTA");
            migrationBuilder.DropTable(name: "ET_RELATORIO");
            migrationBuilder.DropTable(name: "ET_NOTIFICACAO");
            migrationBuilder.DropTable(name: "ET_PONTO_COLETA");
            migrationBuilder.DropTable(name: "ET_TIPO_RESIDUO");
            migrationBuilder.DropTable(name: "ET_USUARIO");
        }
    }
}
