using System.Data.Common;
using AutoMapper;
using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.Dto;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;
using GestorDeClientes.Core.Services.Senha;

using Microsoft.EntityFrameworkCore;

namespace GestorDeClientes.Core.Services.Cadastrar
{
    public class CadastrarService : ICadastrar
    {
        readonly private AppDbContext Banco;
        private readonly ISenha Isenha;

        private readonly IMapper mapper;
        public CadastrarService(AppDbContext Db, ISenha SenhaInterface, IMapper _automapper)
        {
            Banco = Db;
            Isenha = SenhaInterface;
            mapper = _automapper;
        }
        public bool VerificarExistencia(CriarUsuarioDto UsuarioVerificar)
        {

            var resposta = Banco.UsuarioGestor.FirstOrDefault
            (usuario=>usuario.Cpf == UsuarioVerificar.Cpf || usuario.Login == UsuarioVerificar.Login);
            if (resposta == null)
            {
                return false;
            }
            return true;}
    
        public async Task<ResponseModels<UsuarioModel>> Cadastrar ( CriarUsuarioDto Usuario)
        {
            ResponseModels<UsuarioModel> RespostaServico = new();

            try
            {
                var resposta = VerificarExistencia(Usuario);
                if (resposta == false)
                {

                    var UsuarioModel = mapper.Map<UsuarioModel>(Usuario);
                    Isenha.CriarSenhaHash(Usuario.Senha,out byte [] SenhaHash, out byte [] SenhaSalt);

                    UsuarioModel _usuario = new();
                    _usuario.Nome = Usuario.Nome;
                    _usuario.Cpf = Usuario.Cpf;
                    _usuario.Login = Usuario.Login;
                    
                    _usuario.SenhaHash = SenhaHash;
                    _usuario.SenhaSalt = SenhaSalt;

                    
                    Banco.UsuarioGestor.Add(_usuario);
                    await Banco.SaveChangesAsync();

                    RespostaServico.Dados = UsuarioModel;
                    RespostaServico.Mensagem = "Usuario criado com Sucesso";
                    return RespostaServico;
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
                RespostaServico.Mensagem = ex.Message;
                RespostaServico.Status = false;
            }
            return RespostaServico;
           
            
        }


        public async Task<ResponseModels<string>> Login ( LoginDto UsuarioDto)
        {
            ResponseModels<string> RespostaServico = new();

            try
            {
                var usuario = await Banco.UsuarioGestor.FirstOrDefaultAsync(usuarios=> usuarios.Login == UsuarioDto.Login);
                if(usuario == null)
                {
                    RespostaServico.Mensagem = "Login inexistente";
                    RespostaServico.Status = false;
                    return RespostaServico;
                }
                if (!Isenha.VerificarSenha(UsuarioDto.Senha, usuario.SenhaHash,usuario.SenhaSalt)) 
                {
                    RespostaServico.Mensagem = " Senha invalida";
                    RespostaServico.Status = false;
                    return RespostaServico;
                }

                var token = Isenha.CriarToken(usuario);
                RespostaServico.Dados = token;
                RespostaServico.Mensagem = "usuario logado com sucesso";
                RespostaServico.Status = true;
            }
            catch (Exception ex)
            {
                RespostaServico.Dados = null;
                RespostaServico.Mensagem = ex.Message;
                RespostaServico.Status = false;
                return RespostaServico;
            }
            return RespostaServico;
        }
    }
}