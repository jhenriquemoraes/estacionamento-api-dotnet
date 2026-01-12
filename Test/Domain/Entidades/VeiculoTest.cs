using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MinimalAPI.Dominio.Entidade;

namespace Test.Domain.Entidades
{
    [TestClass]
    public class VeiculoTest
    {

        [TestMethod]
        public void TestarGetSetVeiculo()
        {
            var veiculo = new Veiculo();

            veiculo.Id = 1;
            veiculo.Nome = "teste";
            veiculo.Marca = "teste 2";
            veiculo.Ano = 9999;

            Assert.AreEqual(1, veiculo.Id);
            Assert.AreEqual("teste", veiculo.Nome);
            Assert.AreEqual("teste 2", veiculo.Marca);
            Assert.AreEqual(9999, veiculo.Ano);

        }
    }
}