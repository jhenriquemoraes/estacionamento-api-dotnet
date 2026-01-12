using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Dominio.Entidade;
using MinimalAPI.Dominio.Enuns;
using MinimalAPI.Dominio.ModelViews;
using Test.Domain.Entidades;
using Test.Helpers;
using Test.Mocks;
using TestesProjeto;

namespace Test.Requests
{
    [TestClass]
    public class AdministradorTestRequest
    {

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            Setup.ClassInit(context);
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            Setup.ClassCleanup();
        }

        [TestMethod]
        public async Task Deve_Autenticar_Administrador_Com_Credenciais_Corretas()
        {
            // Arrange
            var loginDTO = new LoginDTOs
            {
                Email = "administrador@teste.com",
                Senha = "123456"
            };

            // Act
            var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "application/json");
            var response = await Setup.client.PostAsync("/administradores/login", content);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Falha na requisição: {await response.Content.ReadAsStringAsync()}");

            var result = await response.Content.ReadAsStreamAsync();
            var admLogado = JsonSerializer.Deserialize<AdministradorLogado>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var adm = admLogado!;

            Assert.IsNotNull(adm.Email);
            Assert.IsNotNull(adm.Perfil);
            Assert.IsNotNull(adm.Token);

            Setup.Token = adm.Token;

        }

        [TestMethod]
        public async Task Deve_Retornar_Administrador_pelo_Id()
        {
            //Arrange
            Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Setup.Token);

            //Act
            var response = await Setup.client.GetAsync("/administradores/1");

            //Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Falha na requisição: {await response.Content.ReadAsStringAsync()}");

            var result = await response.Content.ReadAsStringAsync();
            var admin = JsonSerializer.Deserialize<Administrador>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admin);

            Assert.AreEqual(1, admin.Id);
            Assert.AreEqual("administrador@teste.com", admin.Email);
            Assert.AreEqual("Adm", admin.Perfil);

        }

        [TestMethod]
        public async Task Deve_Criar_Outro_Administrador()
        {
            //Arrange
            Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Setup.Token);
            var emailUnico = $"teste{Guid.NewGuid().ToString().Substring(0, 5)}@teste.com";


            var admin = new AdministradorDTO
            {
                Email = emailUnico,
                Senha = "teste",
                Perfil = Perfil.Editor
            };

            //Act
            var content = new StringContent(JsonSerializer.Serialize(admin), Encoding.UTF8, "application/json");
            var response = await Setup.client.PostAsync("/administradores/cadastrar", content);

            //Assert

            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Corpo: {await response.Content.ReadAsStringAsync()}");

            Assert.IsTrue(response.IsSuccessStatusCode, $"Falha na requisição: {await response.Content.ReadAsStringAsync()}");


            var result = await response.Content.ReadAsStringAsync();
            var admiCriado = JsonSerializer.Deserialize<Administrador>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admiCriado);
            Assert.AreEqual(emailUnico, admiCriado.Email);

        }

        [TestMethod]
        public async Task Deve_Listar_Administradores_Cadastrados()
        {
            //Arramge
            Setup.client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Setup.Token);

            // Act
            var response = await Setup.client.GetAsync("/administradores");

            //Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"Falha na requisição: {await response.Content.ReadAsStringAsync()}");

            var result = await response.Content.ReadAsStringAsync();

            var admins = JsonSerializer.Deserialize<List<Administrador>>(result, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Assert.IsNotNull(admins);
            Assert.IsTrue(admins.Count > 0);

            var primeiro = admins[0];

            Assert.IsNotNull(string.IsNullOrWhiteSpace(primeiro.Email));
            Assert.IsNotNull(string.IsNullOrWhiteSpace(primeiro.Perfil));

        }
    }
}
