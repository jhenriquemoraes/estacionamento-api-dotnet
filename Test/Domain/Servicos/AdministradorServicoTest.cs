using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.EntityFrameworkCore;
using MinimalAPI.Dominio.Entidade;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Infraestrutura.Db;
using Microsoft.Extensions.Configuration;

namespace TestesProjeto
{
    [TestClass]
    public class AdministradorServicoTest
    {
        private DbContexto CriarContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<DbContexto>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // Novo banco por teste
                .Options;

            var inMemoryConfiguration = new ConfigurationBuilder().Build();
            
            return new DbContexto(options, inMemoryConfiguration);
        }

        [TestMethod]
        public void Deve_Cadastrar_Administrador()
        {
            using var contexto = CriarContextoInMemory();
            IAdministradorServicos servico = new AdministradorServico(contexto);

            var adm = new Administrador
            {
                Email = "teste@teste.com",
                Senha = "123",
                Perfil = "Adm"
            };

            var salvo = servico.Incluir(adm);

            Assert.IsNotNull(salvo);
            Assert.AreEqual("teste@teste.com", salvo.Email);
        }

        [TestMethod]
        public void Deve_Buscar_Administrador_Por_Id()
        {
            using var contexto = CriarContextoInMemory();
            contexto.Administradores.Add(new Administrador { Id = 10, Email = "adm@a.com", Senha = "abc", Perfil = "Editor" });
            contexto.SaveChanges();

            IAdministradorServicos servico = new AdministradorServico(contexto);
            var resultado = servico.BuscarPorId(10);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("adm@a.com", resultado.Email);
        }

        [TestMethod]
        public void Deve_Retornar_Administrador_Se_Login_Correto()
        {
            using var contexto = CriarContextoInMemory();
            contexto.Administradores.Add(new Administrador { Email = "logado@adm.com", Senha = "senha123", Perfil = "Adm" });
            contexto.SaveChanges();

            IAdministradorServicos servico = new AdministradorServico(contexto);
            var loginDTO = new LoginDTOs
            {
                Email = "logado@adm.com",
                Senha = "senha123"
            };

            var resultado = servico.Login(loginDTO);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("logado@adm.com", resultado.Email);
        }

        [TestMethod]
        public void Nao_Deve_Autenticar_Se_Email_Ou_Senha_Estiverem_Incorretos()
        {
            using var contexto = CriarContextoInMemory();
            contexto.Administradores.Add(new Administrador { Email = "logado@adm.com", Senha = "senha123", Perfil = "Adm" });
            contexto.SaveChanges();

            IAdministradorServicos servico = new AdministradorServico(contexto);
            var loginDTO = new LoginDTOs
            {
                Email = "errado@adm.com",
                Senha = "senha123"
            };

            var resultado = servico.Login(loginDTO);

            Assert.IsNull(resultado);
        }

        [TestMethod]
        public void Deve_Listar_Administradores_Com_Paginacao()
        {
            using var contexto = CriarContextoInMemory();

            for (int i = 1; i <= 15; i++)
            {
                contexto.Administradores.Add(new Administrador
                {
                    Email = $"adm{i}@teste.com",
                    Senha = "123",
                    Perfil = "Editor"
                });
            }

            contexto.SaveChanges();

            IAdministradorServicos servico = new AdministradorServico(contexto);
            var pagina1 = servico.Todos(1); // Espera 10
            var pagina2 = servico.Todos(2); // Espera 5

            Assert.AreEqual(10, pagina1.Count);
            Assert.AreEqual(5, pagina2.Count);
        }
    }
}