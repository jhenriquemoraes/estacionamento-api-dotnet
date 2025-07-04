using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Dominio.Enuns;

namespace MinimalAPI.Dominio.ModelViews
{
    public record AdministradorModelView
    {
        public int Id { get; set; } = default;
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
        public string Perfil { get; set; } = default!;
    }
}