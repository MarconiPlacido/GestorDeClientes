using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using GestorDeClientes.Core.Models;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Services.Cadastrar;
using GestorDeClientes.Core.Dto;
namespace GestorDeClientes.Controllers
{
        [ApiController]
        [Route("api/[controller]")] 
    public class RegistrarController : ControllerBase
    {
        readonly private ICadastrar service;
    
        public RegistrarController( ICadastrar Service)
        {
            service = Service;
        }

        [HttpPost]
        public async Task<ActionResult> Cadastrar ([FromBody] CriarUsuarioDto Usuario)
        {
            var resultado = await service.Cadastrar(Usuario);
            if(resultado.Status == true)
            {
                return BadRequest(resultado.Mensagem);
                
            }
           return Created("",resultado.Mensagem);
        }

        [HttpPost("login")]

        public async Task<ActionResult> Login ([FromBody] LoginDto Login)
        {
            var resultado = await service.Login(Login);
            
            return Ok(resultado);
        }

    };
}