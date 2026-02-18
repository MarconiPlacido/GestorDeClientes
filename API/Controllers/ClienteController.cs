using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;
using GestorDeClientes.Core.Services.CadastrarCliente;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GestorDeClientes.Controllers
{       
        [ApiController]
        [Route("api/[controller]")] 
    public class ClienteController : ControllerBase
    {   
        private readonly Icliente Service;
        public ClienteController(Icliente clienteService)
        {
            Service = clienteService;
        }

        [Authorize]
        [HttpPost("criar")]
        public async Task<ActionResult<ClienteModels>> CriarCliente (ClienteDto Cliente)
        {
            var claimId = User.FindFirstValue("Id");

            if (string.IsNullOrEmpty(claimId) || !Guid.TryParse(claimId, out Guid usuarioGuid))
            {
                return BadRequest($"ID de usuário inválido no token. Valor recebido: {claimId}");
            }
            var Resultado = await Service.CriarCliente(Cliente, usuarioGuid);
            
            if(Resultado.Status == false)
            {
               return BadRequest (Resultado.Mensagem);
            }

            return Created("",Resultado.Mensagem);
        }

        [HttpGet]
        public async Task<ActionResult<ClienteModels>> BuscarClientePorId ([FromHeader]string Documento)
        {
            
            var Resultado = await Service.BuscarClientePorId(Documento);

            if(Resultado.Status == true)
            {
               return Ok(Resultado);
            }

            return BadRequest(Resultado.Mensagem);

        }


        [HttpPut("Editar")]
        public async Task<ActionResult<ResponseModels<ClienteModels>>> EditarCliente ([FromHeader]string Documento,[FromBody] ClienteDto clienteAtualizado)
        {
            var Resultado = await Service.EditarCliente(Documento, clienteAtualizado);

            if(Resultado.Status == true)
            {
               return Ok(Resultado);
            }

            return BadRequest(Resultado.Mensagem);
        }

        [HttpDelete("excluir")]

        public async Task<ActionResult<ResponseModels<ClienteModels>>>ExcluirCliente (string Documento)
        {
            var Resultado = await Service.ExcluirCliente(Documento);

            if(Resultado.Status == true)
            {
               return Ok();
            }

            return BadRequest(Resultado.Mensagem);
        }

    }
}