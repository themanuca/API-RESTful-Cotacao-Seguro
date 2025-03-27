using Domain.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Infra.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Cotacao> Cotacoes { get; set; }
        public DbSet<CotacaoBeneficiario> CotacaoBeneficiarios { get; set; }
        public DbSet<CotacaoCobertura> CotacaoCoberturas { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Parceiro> Parceiros { get; set; }
        public DbSet<TipoParentesco> TiposParentesco { get; set; }
        public DbSet<Cobertura> Coberturas { get; set; }
        public DbSet<FaixaIdade> FaixasIdade { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar chaves primárias explicitamente
            modelBuilder.Entity<Produto>().HasKey(p => p.Id);
            modelBuilder.Entity<Parceiro>().HasKey(p => p.Id);
            modelBuilder.Entity<TipoParentesco>().HasKey(tp => tp.Id);
            modelBuilder.Entity<Cobertura>().HasKey(c => c.Id);
            modelBuilder.Entity<FaixaIdade>().HasKey(f => f.Id);
        }
    }
}
