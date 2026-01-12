using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Microsoft.VisualBasic;
using MinimalAPI;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Dominio.Entidade;
using MinimalAPI.Dominio.Enuns;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.ModelViews;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.Infraestrutura.Db;

namespace Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            key = Configuration["Jwt"]
            ?? throw new InvalidOperationException("Chave JWT não configurada. Verifique o appsettings.json ou variáveis de ambiente.");
        }
        private string key;

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(Options =>
            {
                Options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            services.AddAuthorization();

            services.AddScoped<IAdministradorServicos, AdministradorServico>();
            services.AddScoped<IVeiculoServico, VeiculoServico>();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Insira o token JWT aqui"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                { new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                        new string[] {}
                    }
                });
            });

            services.AddDbContext<DbContexto>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                #region Home
                endpoints.MapGet("/", () => "Bem Vindo a Api de Estacionamenton!").AllowAnonymous().WithTags("Home");
                #endregion

                #region Adiministradores
                string GerarToeknJwt(Administrador administrador)
                {
                    if (string.IsNullOrEmpty(key)) return string.Empty;
                    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                    var claims = new List<Claim>()
                        {
                            new Claim("Email", administrador.Email),
                            new Claim("Perfil", administrador.Perfil),
                            new Claim(ClaimTypes.Role, administrador.Perfil),
                        };
                    var token = new JwtSecurityToken(
                        claims: claims,
                        expires: DateAndTime.Now.AddDays(1),
                        signingCredentials: credentials
                    );

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }
                ;
                endpoints.MapPost("/administradores/login", ([FromBody] LoginDTOs loginDTO, IAdministradorServicos administradorServicos) =>
                   {
                       var adm = administradorServicos.Login(loginDTO);
                       if (adm != null)
                       {
                           string token = GerarToeknJwt(adm);
                           return Results.Ok(new AdministradorLogado
                           {
                               Email = adm.Email,
                               Perfil = adm.Perfil,
                               Token = token

                           });
                       }
                       else
                           return Results.Unauthorized();
                   }).AllowAnonymous().WithTags("Administradores"); 

                endpoints.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServicos administradorServicos) =>
                   {
                       var admministrador = administradorServicos.BuscarPorId(id);

                       if (admministrador == null) return Results.NotFound();

                       return Results.Ok(new AdministradorModelView
                       {
                           Id = admministrador.Id,
                           Email = admministrador.Email,
                           Perfil = admministrador.Perfil
                       });
                   }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");

                endpoints.MapPost("/administradores/cadastrar", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServicos administradorServicos) =>
                   {
                       var validacao = new ErrosDeValidacao
                       {
                           Mensagens = new List<string>()
                       };

                       if (string.IsNullOrEmpty(administradorDTO.Email))
                           validacao.Mensagens.Add("Email não pode ser vazio");

                       if (string.IsNullOrEmpty(administradorDTO.Senha))
                           validacao.Mensagens.Add("Senha não pode ser vazia");

                       if (administradorDTO.Perfil == null)
                           validacao.Mensagens.Add("Perfil não pode ser vazio");

                       if (validacao.Mensagens.Count > 0)
                           return Results.BadRequest(validacao);


                       var administrador = new Administrador
                       {
                           Email = administradorDTO.Email,
                           Senha = administradorDTO.Senha,
                           Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString(),
                       };
                       administradorServicos.Incluir(administrador);

                       return Results.Created($"/Administrador/{administrador.Id}", new AdministradorModelView
                       {
                           Id = administrador.Id,
                           Email = administrador.Email,
                           Perfil = administrador.Perfil
                       });
                   }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");

                endpoints.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorServicos administradorServicos) =>
                   {
                       var adms = new List<AdministradorModelView>();
                       var administradores = administradorServicos.Todos(pagina);

                       foreach (var adm in administradores)
                       {
                           adms.Add(new AdministradorModelView
                           {
                               Id = adm.Id,
                               Email = adm.Email,
                               Perfil = adm.Perfil
                           });
                       }
                       return Results.Ok(adms);
                   }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Administradores");

                #endregion

                #region Veiculos
                ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
                {
                    var validacao = new ErrosDeValidacao
                    {
                        Mensagens = new List<string>()
                    };

                    if (string.IsNullOrEmpty(veiculoDTO.Nome))
                        validacao.Mensagens.Add("O nome não pode ser Vazio!");
                    if (string.IsNullOrEmpty(veiculoDTO.Marca))
                        validacao.Mensagens.Add("A Marca não pode ser Vazio!");
                    if (veiculoDTO.Ano < 1960)
                        validacao.Mensagens.Add("Veiculo muito antigo! O sistema so registra acima de 1960");
                    return validacao;
                }

                endpoints.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
                {

                    var validacao = validaDTO(veiculoDTO);

                    if (validacao.Mensagens.Count > 0)
                        return Results.BadRequest(validacao);

                    var veiculo = new Veiculo
                    {
                        Nome = veiculoDTO.Nome,
                        Marca = veiculoDTO.Marca,
                        Ano = veiculoDTO.Ano,
                    };
                    veiculoServico.Incluir(veiculo);

                    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm, Editor"}).WithTags("Veiculos");

                endpoints.MapGet("/veiculos", ([FromQuery] int pagina, IVeiculoServico veiculoServico) =>
                {
                    var veiculos = veiculoServico.Todos(pagina);
                    return Results.Ok(veiculos);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm, Editor"}).WithTags("Veiculos");

                endpoints.MapGet("/veiculos/{Id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
                {
                    var veiculo = veiculoServico.BuscaPorId(id);

                    if (veiculo == null) return Results.NotFound();

                    return Results.Ok(veiculo);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm, Editor"}).WithTags("Veiculos");

                endpoints.MapPut("/veiculos/{Id}", ([FromRoute] int id, VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
                {
                    var veiculo = veiculoServico.BuscaPorId(id);

                    if (veiculo == null) return Results.NotFound();

                    var validacao = validaDTO(veiculoDTO);

                    if (validacao.Mensagens.Count < 0) return Results.BadRequest(validacao);

                    veiculo.Nome = veiculoDTO.Nome;
                    veiculo.Marca = veiculoDTO.Marca;
                    veiculo.Ano = veiculoDTO.Ano;

                    veiculoServico.Atualizar(veiculo);

                    return Results.Ok(veiculo);
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).WithTags("Veiculos");

                endpoints.MapDelete("/veiculos/{Id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
                {
                    var veiculo = veiculoServico.BuscaPorId(id);

                    if (veiculo == null) return Results.NotFound();

                    veiculoServico.Apagar(veiculo);
                    return Results.NoContent();
                }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"}).WithTags("Veiculos");

                #endregion

            });
        }

    }
}