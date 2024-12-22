using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServicioFinal.Modelo;
using Sap.Data.Hana;
using System.Data;

namespace ServicioFinal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistorialController : ControllerBase
    {
        private readonly string _connectionString;

        public HistorialController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        [HttpGet("{nombrearchivo2}")]
        public IActionResult Buscar(string nombrearchivo2)
        {
            try
            {
                HistorialModel obj = new HistorialModel();

                using (HanaConnection connection = new HanaConnection(_connectionString))
                {
                    connection.Open();
                    string ssql = "SP_LMA_BUSCAR_FILE";
                    using (HanaCommand cmd = new HanaCommand(ssql, connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_LMA_NOMBREARCHIVO2", nombrearchivo2);
                        using (HanaDataReader rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                obj = new HistorialModel();
                                obj.Id = rd.IsDBNull(rd.GetOrdinal("Code")) ? "" : rd.GetString(rd.GetOrdinal("Code"));
                                obj.NombreArchivo1 = rd.IsDBNull(rd.GetOrdinal("U_LMA_NOMBREARCHIVO1")) ? "" : rd.GetString(rd.GetOrdinal("U_LMA_NOMBREARCHIVO1"));
                                obj.NombreArchivo2 = rd.IsDBNull(rd.GetOrdinal("U_LMA_NOMBREARCHIVO2")) ? "" : rd.GetString(rd.GetOrdinal("U_LMA_NOMBREARCHIVO2"));
                                if (!string.IsNullOrEmpty(rd.GetString(rd.GetOrdinal("U_LMA_CONTENIDO"))))
                                {
                                    obj.ContenidoMostrar = rd.GetString(rd.GetOrdinal("U_LMA_CONTENIDO"));
                                }
                                else
                                {
                                    obj.Contenido = null;
                                }
                                obj.FechaCreacion = rd.IsDBNull(rd.GetOrdinal("U_LMA_FECHACREACION")) ? DateTime.Now : rd.GetDateTime(rd.GetOrdinal("U_LMA_FECHACREACION"));
                                obj.FechaActualizacion = rd.IsDBNull(rd.GetOrdinal("U_LMA_FECHAACTUALIZACION")) ? DateTime.Now : rd.GetDateTime(rd.GetOrdinal("U_LMA_FECHAACTUALIZACION"));
                                obj.FechaEnvio = rd.IsDBNull(rd.GetOrdinal("U_LMA_FECHAENVIO")) ? DateTime.Now : rd.GetDateTime(rd.GetOrdinal("U_LMA_FECHAENVIO"));
                                //if (obj.contador >0)
                                //{
                                //    res = 1;
                                //    return res;
                                //}

                            }
                        }
                    }
                }
                System.IO.File.AppendAllText(@"E\Escritorio\logs\logs.txt", $"Respuesta al intentar guardar datos en hanna: Ok\n");
                return Ok(obj);
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"E\Escritorio\logs\logs.txt", $"Respuesta al intentar guardar datos en hanna: {ex.Message}\n");
                return StatusCode(500, "Ocurrió un error al obtener los productos.");
            }
        }

        [HttpPost]

        public IActionResult Grabar([FromBody] HistorialModel obj)
        {
            try
            {
                using (HanaConnection connection = new HanaConnection(_connectionString))
                {
                    connection.Open();
                    string ssql = "SP_LMA_INSERT_FILE";
                    using (HanaCommand cmd = new HanaCommand(ssql, connection))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("_LMA_NOMBREARCHIVO1", obj.NombreArchivo1);
                        cmd.Parameters.AddWithValue("_LMA_NOMBREARCHIVO2", obj.NombreArchivo2);
                        cmd.Parameters.AddWithValue("_LMA_CONTENIDO", obj.Contenido);
                        cmd.Parameters.AddWithValue("_LMA_FECHACREACION", obj.FechaCreacion);
                        cmd.Parameters.AddWithValue("_LMA_FECHAACTUALIZACION", obj.FechaActualizacion);
                        cmd.Parameters.AddWithValue("_LMA_FECHAENVIO", obj.FechaEnvio);
                        string res = cmd.ExecuteNonQuery().ToString();
                        System.IO.File.AppendAllText(@"E\Escritorio\logs\logs.txt", $"Respuesta al intentar guardar datos en hanna: {res}\n");
                        return Ok(res);
                    }
                }

            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"E\Escritorio\logs\logs.txt", $"Respuesta al intentar guardar datos en hanna: {ex.Message}\n");
                return StatusCode(500, "Ocurrió un error al grabar el producto.");
            }
        }


    }
}
