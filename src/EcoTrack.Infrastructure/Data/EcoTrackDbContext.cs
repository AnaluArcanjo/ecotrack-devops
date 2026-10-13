using EcoTrack.Domain.Entities;
using EcoTrack.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace EcoTrack.Infrastructure.Data;

public class EcoTrackDbContext : DbContext
{
    public EcoTrackDbContext(DbContextOptions<EcoTrackDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<PontoColeta> PontosColeta => Set<PontoColeta>();
    public DbSet<TipoResiduo> TiposResiduo => Set<TipoResiduo>();
    public DbSet<Coleta> Coletas => Set<Coleta>();
    public DbSet<Alerta> Alertas => Set<Alerta>();
    public DbSet<Notificacao> Notificacoes => Set<Notificacao>();
    public DbSet<Relatorio> Relatorios => Set<Relatorio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ===================== Usuario =====================
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("ET_USUARIO");
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.SenhaHash).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Perfil).HasConversion<string>().HasMaxLength(20);
        });

        // ===================== PontoColeta =====================
        modelBuilder.Entity<PontoColeta>(entity =>
        {
            entity.ToTable("ET_PONTO_COLETA");
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Endereco).IsRequired().HasMaxLength(255);
            entity.Property(e => e.Cidade).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Estado).IsRequired().HasMaxLength(2);
            entity.Property(e => e.Cep).IsRequired().HasMaxLength(9);
            entity.Property(e => e.Latitude).HasColumnType("decimal(10,7)");
            entity.Property(e => e.Longitude).HasColumnType("decimal(10,7)");
            entity.Property(e => e.CapacidadeKg).HasColumnType("decimal(10,2)");
            entity.Property(e => e.OcupacaoAtual).HasColumnType("decimal(10,2)");
            entity.Ignore(e => e.PercentualOcupacao);
            entity.Ignore(e => e.CapacidadeCritica);

            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.PontosColeta)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===================== TipoResiduo =====================
        modelBuilder.Entity<TipoResiduo>(entity =>
        {
            entity.ToTable("ET_TIPO_RESIDUO");
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Nome).IsUnique();
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.CorLixeira).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Instrucao).IsRequired().HasMaxLength(1000);
        });

        // ===================== Coleta =====================
        modelBuilder.Entity<Coleta>(entity =>
        {
            entity.ToTable("ET_COLETA");
            entity.Property(e => e.QuantidadeKg).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            entity.Property(e => e.Observacao).HasMaxLength(500);

            entity.HasOne(e => e.PontoColeta)
                  .WithMany(p => p.Coletas)
                  .HasForeignKey(e => e.PontoColetaId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.TipoResiduo)
                  .WithMany(t => t.Coletas)
                  .HasForeignKey(e => e.TipoResiduoId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.Coletas)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===================== Alerta =====================
        modelBuilder.Entity<Alerta>(entity =>
        {
            entity.ToTable("ET_ALERTA");
            entity.Property(e => e.Tipo).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.Mensagem).IsRequired().HasMaxLength(500);

            entity.HasOne(e => e.PontoColeta)
                  .WithMany(p => p.Alertas)
                  .HasForeignKey(e => e.PontoColetaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===================== Notificacao =====================
        modelBuilder.Entity<Notificacao>(entity =>
        {
            entity.ToTable("ET_NOTIFICACAO");
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Mensagem).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.Tipo).HasConversion<string>().HasMaxLength(20);

            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.Notificacoes)
                  .HasForeignKey(e => e.UsuarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===================== Relatorio =====================
        modelBuilder.Entity<Relatorio>(entity =>
        {
            entity.ToTable("ET_RELATORIO");
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            entity.Property(e => e.TotalKg).HasColumnType("decimal(12,2)");
            entity.Property(e => e.Co2EvitadoKg).HasColumnType("decimal(12,2)");

            entity.HasOne(e => e.GeradoPor)
                  .WithMany(u => u.Relatorios)
                  .HasForeignKey(e => e.GeradoPorId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // ===================== Seed Data =====================
        modelBuilder.Entity<TipoResiduo>().HasData(
            new TipoResiduo
            {
                Id = 1, Nome = "Papel/Papelão",
                Descricao = "Jornais, revistas, caixas, cadernos e papéis em geral",
                CorLixeira = "AZUL",
                Instrucao = "Amasse papéis e separe papelão. Evite materiais molhados ou engordurados.",
                DataCriacao = new DateTime(2026, 1, 1)
            },
            new TipoResiduo
            {
                Id = 2, Nome = "Plástico",
                Descricao = "Embalagens, garrafas PET, sacolas e utensílios plásticos",
                CorLixeira = "VERMELHO",
                Instrucao = "Enxágue as embalagens antes de descartar. Amasse garrafas PET.",
                DataCriacao = new DateTime(2026, 1, 1)
            },
            new TipoResiduo
            {
                Id = 3, Nome = "Vidro",
                Descricao = "Garrafas, potes, frascos e recipientes de vidro",
                CorLixeira = "VERDE",
                Instrucao = "Enxágue os recipientes. Não quebre o vidro. Cuidado com cacos.",
                DataCriacao = new DateTime(2026, 1, 1)
            },
            new TipoResiduo
            {
                Id = 4, Nome = "Metal",
                Descricao = "Latas de alumínio, aço, ferragens e embalagens metálicas",
                CorLixeira = "AMARELO",
                Instrucao = "Enxágue as latas. Amasse para economizar espaço.",
                DataCriacao = new DateTime(2026, 1, 1)
            },
            new TipoResiduo
            {
                Id = 5, Nome = "Eletrônico",
                Descricao = "Celulares, computadores, pilhas, baterias e eletrodomésticos",
                CorLixeira = "LARANJA",
                Instrucao = "Nunca descarte em lixo comum. Procure pontos de coleta específicos.",
                DataCriacao = new DateTime(2026, 1, 1)
            },
            new TipoResiduo
            {
                Id = 6, Nome = "Orgânico",
                Descricao = "Restos de alimentos, cascas de frutas e verduras",
                CorLixeira = "MARROM",
                Instrucao = "Ideal para compostagem. Separe de embalagens.",
                DataCriacao = new DateTime(2026, 1, 1)
            }
        );

        // Senha "Admin@123" — hash BCrypt válido (verificado via round-trip), igual para os dois usuários de seed
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1, Nome = "Administrador EcoTrack", Email = "admin@ecotrack.com.br",
                SenhaHash = "$2b$11$rt8A9jGPuERfFd3/fGWnAeLZiueXr07HsM9tzx61wN2pWSOrPVKRS",
                Perfil = PerfilUsuario.Admin, DataCriacao = new DateTime(2026, 1, 1)
            },
            new Usuario
            {
                Id = 2, Nome = "Empresa Exemplo Ltda", Email = "empresa@exemplo.com.br",
                SenhaHash = "$2b$11$rt8A9jGPuERfFd3/fGWnAeLZiueXr07HsM9tzx61wN2pWSOrPVKRS",
                Perfil = PerfilUsuario.Empresa, DataCriacao = new DateTime(2026, 1, 1)
            }
        );

        base.OnModelCreating(modelBuilder);
    }
}
