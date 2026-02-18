using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using GestorDeClientes.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestorDeClientes.Core.Services.Senha
{
    public class SenhaService : ISenha
    {
        private readonly IConfiguration configuracoes;
        public SenhaService(IConfiguration config)
        {
            configuracoes = config;
        }
        
        public void CriarSenhaHash(string Senha, out byte [] SenhaHash, out byte [] SenhaSalt)
        {
            using(var Hmac = new HMACSHA512())
            {
                SenhaSalt = Hmac.Key;
                SenhaHash = Hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Senha));
            }
        }

        public bool VerificarSenha (string Senha, byte [] SenhaHash, byte [] SenhaSalt)
        {
            using(var Hmac = new HMACSHA512(SenhaSalt))
            {
                var CompHash = Hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(Senha));
                return CompHash.SequenceEqual(SenhaHash);
            }
        }

        public string CriarToken(UsuarioModel usuario)
        {     //informações que irão dentro do meu token//
            List<Claim> claim = new List<Claim>()
            {
                new Claim("Id",usuario.Id.ToString()),
                new Claim("Nome",usuario.Nome.ToString()),
                new Claim("Cpf",usuario.Cpf.ToString())
            };
           //crendenciais/padrão, olhar o UTF8,token aleatoria a principio no appsetting, pois em seguida será criptografada//
            var keyString = configuracoes["Jwt:Key"];

            if (string.IsNullOrWhiteSpace(keyString))
                throw new Exception("Jwt:Key não configurado no appsettings.json");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyString)
);

            //escolhendo tipo de criptografia a usar//
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            //criando o token em si e passando as informações/paramentros que ele terá//
            var token = new JwtSecurityToken(
                claims:claim,
                expires: DateTime.Now.AddDays(2),
                signingCredentials: cred
            );
            //criação do jason web token em si, transformando os passos anteriores em string//
            var Jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return Jwt;
        }
    }
}