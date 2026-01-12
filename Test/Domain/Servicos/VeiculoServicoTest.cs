using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalAPI.Dominio.Entidade;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.Infraestrutura.Db;

namespace TestesProjeto
{
    [TestClass]
    public class VeiculoServicoTest
    {
        private DbContexto CriarContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<DbContexto>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            
            var inMemoryConfiguration = new ConfigurationBuilder().Build();

            return new DbContexto(options, inMemoryConfiguration);
        }

        [TestMethod]
        public void Deve_Incluir_Veiculo()
        {
            using var contexto = CriarContextoInMemory();
            IVeiculoServico servico = new VeiculoServico(contexto);

            var veiculo = new Veiculo { Nome = "Civic", Marca = "Honda", Ano = 2020 };
            servico.Incluir(veiculo);

            var salvo = servico.BuscaPorId(veiculo.Id);
            Assert.IsNotNull(salvo);
            Assert.AreEqual("Civic", salvo.Nome);
        }

        [TestMethod]
        public void Deve_Buscar_Veiculo_Por_Id()
        {
            using var contexto = CriarContextoInMemory();
            contexto.Veiculos.Add(new Veiculo { Id = 1, Nome = "Corolla", Marca = "Toyota", Ano = 2021 });
            contexto.SaveChanges();

            IVeiculoServico servico = new VeiculoServico(contexto);
            var resultado = servico.BuscaPorId(1);

            Assert.IsNotNull(resultado);
            Assert.AreEqual("Corolla", resultado.Nome);
        }

        [TestMethod]
        public void Deve_Atualizar_Veiculo()
        {
            using var contexto = CriarContextoInMemory();
            var veiculo = new Veiculo { Id = 1, Nome = "Fusca", Marca = "VW", Ano = 1980 };
            contexto.Veiculos.Add(veiculo);
            contexto.SaveChanges();

            IVeiculoServico servico = new VeiculoServico(contexto);
            veiculo.Nome = "Fusca Turbo";
            servico.Atualizar(veiculo);

            var atualizado = servico.BuscaPorId(1);
            Assert.AreEqual("Fusca Turbo", atualizado!.Nome);
        }

        [TestMethod]
        public void Deve_Apagar_Veiculo()
        {
            using var contexto = CriarContextoInMemory();
            var veiculo = new Veiculo { Id = 1, Nome = "Gol", Marca = "VW", Ano = 2005 };
            contexto.Veiculos.Add(veiculo);
            contexto.SaveChanges();

            IVeiculoServico servico = new VeiculoServico(contexto);
            servico.Apagar(veiculo);

            var apagado = servico.BuscaPorId(1);
            Assert.IsNull(apagado);
        }

        [TestMethod]
        public void Deve_Listar_Veiculos_Paginados()
        {
            using var contexto = CriarContextoInMemory();
            for (int i = 1; i <= 15; i++)
            {
                contexto.Veiculos.Add(new Veiculo { Nome = $"Carro{i}", Marca = "Genérica", Ano = 2000 + i });
            }
            contexto.SaveChanges();

            IVeiculoServico servico = new VeiculoServico(contexto);
            var pagina1 = servico.Todos(1);
            var pagina2 = servico.Todos(2);

            Assert.AreEqual(10, pagina1.Count);
            Assert.AreEqual(5, pagina2.Count);
        }

        [TestMethod]
        public void Deve_Filtrar_Por_Nome()
        {
            using var contexto = CriarContextoInMemory();
            contexto.Veiculos.Add(new Veiculo { Nome = "Uno", Marca = "Fiat", Ano = 2001 });
            contexto.Veiculos.Add(new Veiculo { Nome = "Palio", Marca = "Fiat", Ano = 2005 });
            contexto.SaveChanges();

            IVeiculoServico servico = new VeiculoServico(contexto);
            var filtrado = servico.Todos(1, nome: "uno");

            Assert.AreEqual(1, filtrado.Count);
            Assert.AreEqual("Uno", filtrado[0].Nome);
        }
    }
}