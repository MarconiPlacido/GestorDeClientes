using ClosedXML.Excel;
using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace GestorDeClientes.Core.Services.importacaoXML
{

    public class ImportacaoService : IImportacao
    {
        public AppDbContext Banco { get; set; }
        public ImportacaoService(AppDbContext banco)
        {
            Banco = banco;
        }


        public async Task<ResultadoImportacaoDto> ImportarClientesAsync(IFormFile arquivo, Guid UsuarioId)
            {
                var resultado = new ResultadoImportacaoDto();
                

                using var stream = new MemoryStream();
                await arquivo.CopyToAsync(stream);

                using var workbook = new XLWorkbook(stream);
                var planilha = workbook.Worksheet(1);

                var linhas = planilha.RowsUsed().Skip(1);

                foreach (var linha in linhas)
                {
                    resultado.TotalLinhas++;

                    try
                    {
                        var nomeCliente = linha.Cell(1).GetString().Trim();
                        var documento = linha.Cell(2).GetString().Trim();
                        var nomeContato = linha.Cell(3).GetString().Trim();
                        var email = linha.Cell(4).GetString().Trim();
                        var telefone = linha.Cell(5).GetString().Trim();

                        if (string.IsNullOrWhiteSpace(documento))
                        {
                            resultado.Erros.Add($"Linha {linha.RowNumber()}: Documento vazio.");
                            continue;
                        }

                        var cliente = await Banco.Clientes
                            .Include(c => c.Contatos)
                            .FirstOrDefaultAsync(c => c.Documento == documento);

                        if (cliente == null)
                        {
                            cliente = new ClienteModels
                            {
                                Id = Guid.NewGuid(),
                                Nome = nomeCliente,
                                Documento = documento,
                                UsuarioId = UsuarioId, 
                                Contatos = new List<ContatoModel>()
                            };

                            Banco.Clientes.Add(cliente);
                            resultado.ClientesCriados++;
                        }

                        bool contatoExiste = cliente.Contatos!
                            .Any(c => c.Email == email || c.Telefone == telefone);

                        if (contatoExiste)
                        {
                            resultado.ContatosIgnorados++;
                            continue;
                        }

                        cliente.Contatos!.Add(new ContatoModel
                        {
                            Id = Guid.NewGuid(),
                            Nome = nomeContato,
                            Email = email,
                            Telefone = telefone,
                            ClienteId = cliente.Id
                        });

                        resultado.ContatosCriados++;
                    }
                    catch (Exception ex)
                    {
                        resultado.Erros.Add($"Linha {linha.RowNumber()}: {ex.Message}");
                    }
                }

                await Banco.SaveChangesAsync();
                return resultado;
            }
    }

}