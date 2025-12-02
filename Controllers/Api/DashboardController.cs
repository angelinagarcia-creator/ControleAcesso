using System;
using System.Linq;
using System.Security.Claims;
using ControleAcesso.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using CondoHub.Data;
using CondoHub.Models.Entities;

namespace ControleAcesso.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var dados = new DashBoardData();

            // --- CÁLCULOS GERAIS PARA TODOS OS PERFIS ---
            int totalUnidades = _context.Properties.Count();
            int unidadesOcupadas = _context.Properties.Count(p => p.Ocupada);
            int totalAvisos = _context.Messages.Count();
            int totalUsers = _context.Users.Count();

            // Engajamento: usuários com login nos últimos 30 dias
            int engajamentoPercentual = totalUsers > 0 
                ? (int)((decimal)_context.Users.Count(u => u.LastLogin.HasValue && u.LastLogin.Value > DateTime.Now.AddDays(-30)) / totalUsers * 100)
                : 0;

            // Preenche propriedades gerais
            dados.TotalUnidades = totalUnidades;
            dados.UnidadesOcupadas = unidadesOcupadas;
            dados.PercentualOcupacao = totalUnidades > 0 ? (unidadesOcupadas * 100 / totalUnidades) : 0;
            dados.TotalAvisos = totalAvisos;
            dados.EngajamentoPercentual = engajamentoPercentual;

            // --- LÓGICA DE PERFIL ---
            if (User.IsInRole("Sindico") || User.IsInRole("Gestor"))
            {
                dados.UnidadesAtivas = totalUnidades;
                dados.MoradoresAtivos = totalUsers;
                dados.AvisosPublicados = totalAvisos;
                dados.ManutencoesPendentes = _context.ChecklistItems.Count(x => !x.Concluido);

                // Pagamentos em dia no mês atual
                dados.PagamentosDia = _context.Payments.Count(p =>
                    p.Pago && p.DataVencimento.Month == DateTime.Now.Month && p.DataVencimento.Year == DateTime.Now.Year);
                dados.TotalPagamentosDia = totalUnidades;
            }
            else if (User.IsInRole("Morador"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!string.IsNullOrEmpty(userId))
                {
                    dados.PagamentosEmDia = _context.Payments.Count(x => x.UserId == userId && x.Pago);
                    dados.TotalPagamentosDia = _context.Payments.Count(x => x.UserId == userId);
                    dados.AvisosLidos = _context.Messages.Count(x => x.UsuarioId == userId && x.Lido);

                    dados.AcessosMes = _context.Accesses.Count(x =>
                        x.UserId == userId &&
                        x.DataHoraAcesso.Month == DateTime.Now.Month &&
                        x.DataHoraAcesso.Year == DateTime.Now.Year);

                    dados.ReservasAtivas = _context.Contracts.Count(x => x.UserId == userId && x.Status == "Ativo");
                }
            }

            // Dados para gráfico (mock inicial)
            dados.PagamentosUltimos7Dias = new int[] { 0, 0, 0, 0, 0, 0, 0 };

            return View(dados);
        }

        [HttpGet]
        public IActionResult ObterDadosMoradores()
        {
            // Contagem por perfil
            var dadosMoradores = new
            {
                proprietarios = _context.Users.Count(u => u.Role == UserRole.Gestor || u.Role == UserRole.Sindico),
                moradores = _context.Users.Count(u => u.Role == UserRole.Morador),

                // Contagem por bloco
                blocoA = _context.Properties.Count(p => p.Bloco == "A" && p.Ocupada),
                blocoB = _context.Properties.Count(p => p.Bloco == "B" && p.Ocupada),
                blocoC = _context.Properties.Count(p => p.Bloco == "C" && p.Ocupada),
                blocoD = _context.Properties.Count(p => p.Bloco == "D" && p.Ocupada)
            };
            return Ok(dadosMoradores);
        }
    }
}
