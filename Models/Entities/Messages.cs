using System;

namespace CondoHub.Models.Entities
{
    public enum TipoAviso
    {
        Manutencao,
        Evento,
        Comunicado,
        Aviso
    }

    public class Messages
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensagem { get; set; } = string.Empty;

        public string UsuarioId { get; set; } // FK para ApplicationUser

        public DateTime DataPublicacao { get; set; }
        public string? ImagemPath { get; set; }
        public TipoAviso Tipo { get; set; }

        public bool Lido { get; set; } = false; // agora é bool
    }
}