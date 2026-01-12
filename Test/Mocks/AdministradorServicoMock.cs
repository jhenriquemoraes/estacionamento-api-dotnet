using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Dominio.Entidade;
using MinimalAPI.Dominio.Interfaces;

namespace Test.Mocks
{
    public class AdministradorServicoMock : IAdministradorServicos
    {
        private static List<Administrador> administradores = new List<Administrador>()
        {
            new Administrador{
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
            },
        };
        public Administrador? BuscarPorId(int id)
        {
           return administradores.Find(adm => adm.Id == id);
        }

        public Administrador Incluir(Administrador administrador)
        {
            administrador.Id = administradores.Count + 1;
            administradores.Add(administrador);

            return administrador;
        }

        public Administrador? Login(LoginDTOs loginDTOs)
        {
            return administradores.Find(adm => adm.Email == loginDTOs.Email && adm.Senha == loginDTOs.Senha);
        }

        public List<Administrador> Todos(int? pagina)
        {
            return administradores;
        }
    }
}