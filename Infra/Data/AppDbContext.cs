using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cotacao> Cotacao { get; set; }
        public DbSet<CotacaoBeneficiario> CotacaoBeneficiario { get; set; }
        public DbSet<CotacaoCobertura> CotacaoCobertura
        { get; set; }
        public DbSet<Produto> Produto { get; set; }
        public DbSet<Parceiro> Parceiro { get; set; }
        public DbSet<TipoParentesco> TipoParentesco { get; set; }
        public DbSet<Cobertura> Cobertura { get; set; }
        public DbSet<FaixaIdade> FaixaIdade { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Produto>().HasKey(p => p.Id);
            modelBuilder.Entity<Parceiro>().HasKey(p => p.Id);
            modelBuilder.Entity<TipoParentesco>().HasKey(tp => tp.Id);
            modelBuilder.Entity<Cobertura>().HasKey(c => c.Id);
            modelBuilder.Entity<FaixaIdade>().HasKey(f => f.Id);

            modelBuilder.Entity<Cotacao>()
                .HasOne<Produto>()
                .WithMany()
                .HasForeignKey(c => c.IdProduto)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cotacao>()
                .HasOne<Parceiro>()
                .WithMany()
                .HasForeignKey(c => c.IdParceiro)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CotacaoBeneficiario>()
                .HasOne<CotacaoBeneficiario>()
                .WithMany()
                .HasForeignKey(b => b.IdCotacao);

            modelBuilder.Entity<CotacaoCobertura>()
                .HasOne<CotacaoCobertura>()
                .WithMany()
                .HasForeignKey(c => c.IdCobertura);

        }
    }

}
