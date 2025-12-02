using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ControleAcesso.Models.ViewModels
{
   public class DashBoardData
    {
        public string PeriodoSelecionado { get; set; }
        public int PagamentosDia { get; set; }
        public int TotalPagamentosDia { get; set; }
        public int AvisosLidos { get; set; }
        public int TotalAvisos { get; set; }
        public int AcessosMes { get; set; }
        public int ReservasAtivas { get; set; }
        public int[] PagamentosUltimos7Dias { get; set; }
        public int UnidadesOcupadas { get; set; }
        public int TotalUnidades { get; set; }
        public int PercentualOcupacao => (int)((double)UnidadesOcupadas / TotalUnidades * 100);
        public int EngajamentoPercentual { get; set; }
    }
}