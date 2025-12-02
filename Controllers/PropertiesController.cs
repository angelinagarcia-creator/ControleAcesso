using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Collections.Generic;
using CondoHub.Data;
using CondoHub.Models.Entities;

namespace CondoHub.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PropertiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "GESTOR, SINDICO, MORADOR")]
        public async Task<IActionResult> Index(string searchString, string status)
        {
            var query = _context.Properties
                .Include(i => i.Proprietario)
                .AsQueryable();

            // FILTRO BUSCA
            if (!string.IsNullOrEmpty(searchString))
            {
                query = query.Where(i =>
                    i.Numero.Contains(searchString) ||
                    (i.Proprietario != null && i.Proprietario.Nome.Contains(searchString)));
            }

            // FILTRO STATUS
            if (!string.IsNullOrEmpty(status))
            {
                var statusEnum = Enum.Parse<PropertyType>(status);
                query = query.Where(i => i.Type == statusEnum);
            }

            // RESULTADO DOS IMÓVEIS
            var imoveis = await query
                .OrderByDescending(p => p.Id)
                .ToListAsync();

            var propertyIds = imoveis.Select(i => i.Id).ToList();

            // --------------------------
            // DONOS VIA CONTRATOS
            // --------------------------
            var contractOwnersList = _context.Contracts
                .Where(c => c.PropertyId != null && propertyIds.Contains(c.PropertyId.Value))
                .AsEnumerable() // traz para memória antes do OR
                .Where(c => !string.IsNullOrEmpty(c.ProprietarioNome) || !string.IsNullOrEmpty(c.LocatarioNome))
                .ToList();

            var contractOwners = contractOwnersList
                .GroupBy(c => c.PropertyId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => !string.IsNullOrEmpty(x.ProprietarioNome) ? x.ProprietarioNome : x.LocatarioNome)
                          .FirstOrDefault()
                );

            var ownersDict = new Dictionary<int, string>();

            foreach (var owner in contractOwners)
                ownersDict[owner.Key] = owner.Value!;

            foreach (var p in imoveis)
            {
                if (p.Proprietario != null && !string.IsNullOrWhiteSpace(p.Proprietario.Nome))
                {
                    ownersDict[p.Id] = p.Proprietario.Nome;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(p.ProprietarioNome))
                {
                    ownersDict[p.Id] = p.ProprietarioNome;
                    continue;
                }

                if (!string.IsNullOrWhiteSpace(p.ProprietarioId))
                {
                    var user = _context.Users.Find(p.ProprietarioId);
                    if (user != null)
                    {
                        ownersDict[p.Id] = user.Nome;
                        continue;
                    }
                }
            }

            ViewBag.PropertyOwners = ownersDict;

            // --------------------------
            // TIPO DE CONTRATO
            // --------------------------
            var contractTypeList = _context.Contracts
                .Where(c => c.PropertyId != null && propertyIds.Contains(c.PropertyId.Value))
                .ToList();

            var contractTypeDict = contractTypeList
                .GroupBy(c => c.PropertyId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.Type).FirstOrDefault()
                );

            var contractTypeLabels = new Dictionary<int, string>();

            foreach (var kv in contractTypeDict)
            {
                var label = kv.Value switch
                {
                    ContractType.LocatarioProprietario => "Locatário/Proprietário",
                    ContractType.Condomino => "Condomínio",
                    ContractType.Funcionario => "Funcionário",
                    _ => kv.Value.ToString()
                };
                contractTypeLabels[kv.Key] = label;
            }

            foreach (var p in imoveis)
            {
                if (!string.IsNullOrWhiteSpace(p.TipoImovel))
                    contractTypeLabels[p.Id] = p.TipoImovel;
                else if (!contractTypeLabels.ContainsKey(p.Id))
                    contractTypeLabels[p.Id] = "—";
            }

            ViewBag.PropertyContractTypes = contractTypeLabels;

            return View(imoveis);
        }
    }
}
