using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Models.Dashboard
{
    // Localização: Models/ViewModels/DashBoardData.cs

public class DashBoardData
{
    // --- PROPRIEDADES DE STATUS (CORRIGIDAS PARA GESTOR/MORADOR) ---
    
    // Corrigido o erro CS1061. O Controller precisa dessas propriedades.
    public int AvisosPublicados { get; set; } 
    public int ManutencoesPendentes { get; set; }
    public int UnidadesAtivas { get; set; }
    public int MoradoresAtivos { get; set; }
    
    // Propriedades usadas para o Morador
    public int PagamentosEmDia { get; set; }
    public int AvisosLidos { get; set; }
    public int AcessosMes { get; set; }
    public int ReservasAtivas { get; set; }

    // --- PROPRIEDADES DA VIEW (CORRIGIDAS PARA COMPATIBILIDADE) ---

    // A VIEW USA 'PagamentosDia' e 'TotalPagamentosDia'
    // O Controller deve preencher estes campos para o card de "Pagamentos em Dia" funcionar.
    public int PagamentosDia { get; set; }
    public int TotalPagamentosDia { get; set; }
    public int TotalAvisos { get; set; } 

    // Propriedades de Ocupação da VIEW
    public int UnidadesOcupadas { get; set; }
    public int TotalUnidades { get; set; }
    
    // Corrigido o erro CS0200. Adicionado 'set;' para permitir atribuição no Controller.
    public int PercentualOcupacao { get; set; } 
    
    public int EngajamentoPercentual { get; set; }
    
    // Propriedade para o gráfico de 7 dias
    public int[] PagamentosUltimos7Dias { get; set; } = Array.Empty<int>();

    // --- Outras propriedades que você tinha (se houver) devem ser mantidas aqui ---
}
}
