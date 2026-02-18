using GestorDeClientes.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorDeClientes.Core.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base (options)
        {
            
        }

        public DbSet<UsuarioModel> UsuarioGestor {get; set;}
        public DbSet<ClienteModels> Clientes {get; set;}
        public DbSet<ContatoModel> Contato {get; set;}
    }

}