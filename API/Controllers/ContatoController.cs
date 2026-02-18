using GestorDeClientes.Core.Data;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;
using GestorDeClientes.Core.Services.CadastrarCliente;
using GestorDeClientes.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GestorDeClientes.Controllers
{       
        [ApiController]
        [Route("api/[controller]")] 
    public class ContatoController : ControllerBase
    {   
        private readonly Icontato Service;
        public ContatoController(Icontato ContatoService)
        {
            Service = ContatoService;
        }

        [HttpPost("{DocumentoDoClienteResponsavel}")]
        public async Task<ActionResult<ResponseModels<ContatoModel>>> CriarContato(ContatoDto Contato, string DocumentoDoClienteResponsavel)
        {
            var Resultado = await Service.CriarContato(Contato, DocumentoDoClienteResponsavel);
            
            if(Resultado.Status == true)
            {
                return Created("",Resultado.Mensagem);
            }

            return BadRequest (Resultado.Mensagem);
        }





        [HttpGet("Buscar/{Id}")]
        public async Task<ActionResult<ResponseModels<ContatoModel>>> BuscarContatoPorId(Guid Id)
        {
            
            var Resultado = await Service.BuscarContatoPorId(Id);

            if(Resultado.Status == true)
            {
               return Ok(Resultado);
            }

            return BadRequest(Resultado.Mensagem);

        }


        [HttpPut("Editar/{Id}")]
        public async Task<ActionResult<ResponseModels<ContatoModel>>> EditarContato (Guid Id, ContatoDto contatoAtualizado)
        {
            var Resultado = await Service.EditarContato(Id, contatoAtualizado);

            if(Resultado.Status == true)
            {
               return Ok(Resultado);
            }

            return BadRequest(Resultado.Mensagem);
        }

        [HttpDelete("Delete/{Id}")]

        public async Task<ActionResult<ResponseModels<ContatoModel>>> ExcluirContato (Guid Id)
        {
            var Resultado = await Service.ExcluirContato(Id);

            if(Resultado.Status == true)
            {
               return Ok();
            }

            return BadRequest(Resultado.Mensagem);
        }

    }
}