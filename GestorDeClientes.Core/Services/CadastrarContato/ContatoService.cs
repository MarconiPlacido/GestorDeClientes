using AutoMapper;
using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace GestorDeClientes.Core.Services.CadastrarContato
{
    public class ContatoService : Icontato
    {
        private readonly AppDbContext Banco;
        private readonly IMapper mapper;
        public ContatoService(AppDbContext Db, IMapper _automapper)
        {
            Banco = Db;
            mapper = _automapper;
        }


        ResponseModels<ContatoModel> RespostaServico = new();


        public async Task<bool> VerificarExistencia(ContatoDto ContatoVerificar)
        {
            var resposta = await Banco.Contato.FirstOrDefaultAsync
            (Contatos=>Contatos.Email == ContatoVerificar.Email || Contatos.Telefone ==ContatoVerificar.Telefone);
            if (resposta == null)
            {
                return false;
            }
            return true;
        }

        public async Task<ResponseModels<List<ContatoModel>>> BuscarContato(string documentoDoCliente)
            {
                var response = new ResponseModels<List<ContatoModel>>();

                var cliente = await Banco.Clientes
                    .Include(c => c.Contatos)
                    .FirstOrDefaultAsync(c => c.Documento == documentoDoCliente);

                if (cliente == null)
                {
                    response.Status = false;
                    response.Mensagem = "Cliente não encontrado.";
                    response.Dados = new List<ContatoModel>();
                    return response;
                }

                response.Status = true;
                response.Dados = cliente.Contatos ?? new List<ContatoModel>();
                response.Mensagem = "Contatos encontrados com sucesso.";

                return response;
            }


        public async Task<ResponseModels<ContatoModel>> CriarContato(ContatoDto Contato, string Documento)
        {
            try
            {
                var resposta =await VerificarExistencia(Contato);

                if(resposta == false)
                {
                    var cliente1 = await Banco.Clientes.FirstOrDefaultAsync(cliente=> cliente.Documento == Documento);

                    var contatoMapper = mapper.Map<ContatoModel>(Contato);
                    contatoMapper.ClienteId = cliente1.Id;
                    

                    Banco.Contato.Add(contatoMapper);
                    var salar = await Banco.SaveChangesAsync();
                    
                    RespostaServico.Dados = contatoMapper;
                    RespostaServico.Mensagem = "Contato cadastrado com sucesso";
                    RespostaServico.Status = true;
                }
                else
                {
                    RespostaServico.Dados = null;
                    RespostaServico.Mensagem = "Contato já cadastrado";
                    RespostaServico.Status = false;
                }
            }
            catch (Exception ex)
                {
                    RespostaServico.Dados = null;
                    RespostaServico.Status = false;
                    
                    var mensagemErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    RespostaServico.Mensagem = $"Erro: {mensagemErro}";
                }
            return RespostaServico;
        }

        public async Task<ResponseModels<ContatoModel>> BuscarContatoPorId(Guid Id)
        {
            
            try
            {
                
                var resposta = await Banco.Contato.FirstOrDefaultAsync(contato => contato.Id == Id);

                if(resposta == null)
                {
                    RespostaServico.Dados = resposta;
                    RespostaServico.Mensagem = "Usuario não encontrado";
                    RespostaServico.Status = false;
                }
                else
                {
             
                    RespostaServico.Dados = resposta;
                    RespostaServico.Mensagem = "cliente encontrado com sucesso";
                    RespostaServico.Status = true;
                }
            }
            catch (Exception ex)
                {
                    RespostaServico.Dados = null;
                    RespostaServico.Status = false;
                    
                    var mensagemErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    RespostaServico.Mensagem = $"Erro: {mensagemErro}";
                }
            return RespostaServico;
        }

        public async Task<ResponseModels<ContatoModel>> EditarContato (Guid Id, ContatoDto contatoAtualizado)
        {
            
            try
            {
                var resposta = await Banco.Contato.FirstOrDefaultAsync(contato=> contato.Id == Id);

                if(resposta == null)
                {
                    RespostaServico.Dados = resposta;
                    RespostaServico.Mensagem = "Usuario não encontrado";
                    RespostaServico.Status = false;
                }
                else
                {
                    resposta.Nome = contatoAtualizado.Nome;
                    resposta.Telefone = contatoAtualizado.Telefone;
                    resposta.Email = contatoAtualizado.Email;
                    Banco.Contato.Update(resposta);
                    await Banco.SaveChangesAsync();


                    RespostaServico.Dados = resposta;
                    RespostaServico.Mensagem = "Editado com sucesso";
                    RespostaServico.Status = true;
                }
            }
            catch (Exception ex)
                {
                    RespostaServico.Dados = null;
                    RespostaServico.Status = false;
                    
                    var mensagemErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    RespostaServico.Mensagem = $"Erro: {mensagemErro}";
                }
            return RespostaServico;
        }


        public async Task<ResponseModels<ContatoModel>> ExcluirContato (Guid Id)
        {
            
            try
            {
                var resposta = await Banco.Contato.FirstOrDefaultAsync(contato=> contato.Id == Id);

                if(resposta == null)
                {
                    RespostaServico.Dados = resposta;
                    RespostaServico.Mensagem = "Contato não encontrado";
                    RespostaServico.Status = false;
                }
                else
                {
            
                    Banco.Remove(resposta);
                    await Banco.SaveChangesAsync();


                    RespostaServico.Dados = resposta;
                    RespostaServico.Mensagem = "excluido com sucesso";
                    RespostaServico.Status = true;
                }
            }
            catch (Exception ex)
                {
                    RespostaServico.Dados = null;
                    RespostaServico.Status = false;
                    
                    var mensagemErro = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
                    RespostaServico.Mensagem = $"Erro no Banco: {mensagemErro}";
                }
            return RespostaServico;
        }


    }


}
