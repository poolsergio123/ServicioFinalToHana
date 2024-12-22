namespace ServicioFinal.Modelo
{
    public class HistorialModel
    {
        public string Id { get; set; }
        public string NombreArchivo1 { get; set; }
        public string NombreArchivo2 { get; set; }
        public byte[] Contenido { get; set; }
        public string ContenidoMostrar { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaActualizacion { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public int contador { get; set; }
    }
}
