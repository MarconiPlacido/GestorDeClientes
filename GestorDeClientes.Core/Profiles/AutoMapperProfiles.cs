using AutoMapper;
using GestorDeClientes.Core.DTO;
using GestorDeClientes.Core.Models;

namespace GestorDeClientes.Core.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap< ClienteDto, ClienteModels >();
            CreateMap< CriarUsuarioDto, UsuarioModel >();
            CreateMap<ContatoDto, ContatoModel>();
        }
    }
}