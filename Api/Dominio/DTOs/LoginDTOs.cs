using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MinimalAPI.Dominio.DTOs
{
    public class LoginDTOs
    {
        public string Email { get; set; } = default!;
        public string Senha { get; set; } = default!;
    }
}