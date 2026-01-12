using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Dominio.Entidade;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Infraestrutura.Db;

namespace MinimalAPI.Dominio.Servicos
{
    public class AdministradorServico : IAdministradorServicos
    {
        private readonly DbContexto _contexto;
        public AdministradorServico(DbContexto contexto) =>_contexto = contexto;
        public Administrador? BuscarPorId(int id)  
        {
            return _contexto.Administradores.Where(v=> v.Id == id).FirstOrDefault();
        }
        public Administrador? Login(LoginDTOs loginDTOs)
        {
            var adm = _contexto.Administradores.Where(a => a.Email == loginDTOs.Email && a.Senha == loginDTOs.Senha).FirstOrDefault();
            return adm;
        }
        Administrador IAdministradorServicos.Incluir(Administrador administrador)
        {
            _contexto.Administradores.Add(administrador);
            _contexto.SaveChanges();

            return administrador;
        }
        List<Administrador> IAdministradorServicos.Todos(int? pagina)
        {
            var query = _contexto.Administradores.AsQueryable();
            int itenPorPagina = 10;

            if(pagina !=  null)
                query = query.Skip(((int)pagina - 1) *itenPorPagina).Take(itenPorPagina);

            return query.ToList();
        }


    }
}