using ClosedXML.Excel;
using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorDeClientes.Core.Services.importacaoCorrigida
{
    public class ProcessadorImportacao : IProcessadorImportacao
{
    private readonly AppDbContext _banco;

    public ProcessadorImportacao(AppDbContext banco)
    {
        _banco = banco;
    }

    public async Task<ResultadoImportacaoDto> ProcessarAsync(
        Stream arquivo,
        Guid usuarioId, Guid importacaoId)
    {
        var resultado = new ResultadoImportacaoDto();

        using var workbook = new XLWorkbook(arquivo);
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

                var cliente = await _banco.Clientes
                    .Include(c => c.Contatos)
                    .FirstOrDefaultAsync(c => c.Documento == documento);

                if (cliente == null)
                {
                    cliente = new ClienteModels
                    {
                        Id = Guid.NewGuid(),
                        Nome = nomeCliente,
                        Documento = documento,
                        UsuarioId = usuarioId,
                        Contatos = new List<ContatoModel>()
                    };

                    _banco.Clientes.Add(cliente);
                    resultado.ClientesCriados++;
                }

                if (cliente.Contatos!.Any(c =>
                    c.Email == email || c.Telefone == telefone))
                {
                    resultado.ContatosIgnorados++;
                    continue;
                }

                cliente.Contatos.Add(new ContatoModel
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

        await _banco.SaveChangesAsync();
        return resultado;
    }
}

}