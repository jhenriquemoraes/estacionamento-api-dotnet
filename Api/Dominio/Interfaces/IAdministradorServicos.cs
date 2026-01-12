using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Dominio.Entidade;

namespace MinimalAPI.Dominio.Interfaces
{
    public interface IAdministradorServicos
    {
        Administrador? Login(LoginDTOs loginDTOs);
        Administrador Incluir(Administrador administrador);
        Administrador? BuscarPorId (int id);
        List<Administrador> Todos (int? pagina);
    }
}