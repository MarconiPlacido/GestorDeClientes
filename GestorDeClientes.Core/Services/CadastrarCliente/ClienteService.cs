using AutoMapper;

using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;
using GestorDeClientes.Core.Services.CadastrarCliente;
using Microsoft.EntityFrameworkCore;

namespace GestorDeClientes.Core.Services
{
    public class ClienteService : Icliente
    {
        private readonly AppDbContext Banco;
        private readonly IMapper mapper;
        public ClienteService(AppDbContext Db, IMapper _automapper)
        {
            Banco = Db;
            mapper = _automapper;
        }


        ResponseModels<ClienteModels> RespostaServico = new();


        public async Task<bool> VerificarExistencia(ClienteDto ClienteVerificar)
        {
            var resposta = await Banco.Clientes.FirstOrDefaultAsync
            (clientes=>clientes.Documento == ClienteVerificar.Documento);
            if (resposta == null)
            {
                return false;
            }
            return true;
        }

        public async Task<ClienteModels> BuscarCliente(string Documento)
        {
            var resposta = await Banco.Clientes.FirstOrDefaultAsync(Cliente=> Cliente.Documento == Documento);
            return resposta;
        }


        public async Task<ResponseModels<ClienteModels>> CriarCliente(ClienteDto Cliente, Guid usuarioId)
        {
            try
            {
                var resposta =await VerificarExistencia(Cliente);

                if(resposta == false)
                {
                    var clienteMapper = mapper.Map<ClienteModels>(Cliente);
                    clienteMapper.UsuarioId = usuarioId; 
                    clienteMapper.Usuario = await Banco.UsuarioGestor.FirstOrDefaultAsync(usuarios => usuarios.Id == usuarioId);
                    

                    Banco.Clientes.Add(clienteMapper);
                    var salar = await Banco.SaveChangesAsync();
                    
                    RespostaServico.Dados = null;
                    RespostaServico.Mensagem = "cliente cadastrado com sucesso";
                    RespostaServico.Status = true;
                }
                else
                {
                    RespostaServico.Dados = null;
                    RespostaServico.Mensagem = "Usuario já cadastrado";
                    RespostaServico.Status = false;
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

        public async Task<ResponseModels<ClienteModels>> BuscarClientePorId(string Documento)
        {
            
            try
            {
                
                var resposta = await BuscarCliente(Documento);

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
                    RespostaServico.Mensagem = $"Erro no Banco: {mensagemErro}";
                }
            return RespostaServico;
        }

        public async Task<ResponseModels<ClienteModels>> EditarCliente (string Documento, ClienteDto clienteAtualizado)
        {
            
            try
            {
                var resposta = await BuscarCliente(Documento);

                if(resposta == null)
                {
                    RespostaServico.Dados = resposta;
                    RespostaServico.Mensagem = "Usuario não encontrado";
                    RespostaServico.Status = false;
                }
                else
                {
                    resposta.Nome = clienteAtualizado.Nome;
                    resposta.Documento = clienteAtualizado.Documento;
                    resposta.Endereço = clienteAtualizado.Endereço;
                    Banco.Clientes.Update(resposta);
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
                    RespostaServico.Mensagem = $"Erro no Banco: {mensagemErro}";
                }
            return RespostaServico;
        }


        public async Task<ResponseModels<ClienteModels>> ExcluirCliente (string Documento)
        {
            
            try
            {
                var resposta = await BuscarCliente(Documento);

                if(resposta == null)
                {
                    RespostaServico.Dados = resposta;
                    RespostaServico.Mensagem = "Usuario não encontrado";
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
