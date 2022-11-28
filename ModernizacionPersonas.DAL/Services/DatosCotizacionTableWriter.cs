using ModernizacionPersonas.DAL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ModernizacionPersonas.DAL.Services
{
    public class DatosCotizacionTableWriter : IDatosCotizacionWriter
    {
        const string SP_NAME = "PMP.USP_TB_Cotizacion";

        private async Task<string> GetNumeroCotizacionAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    //CommandText = "select MAX(CAST(VC_numero_cotizacion as INT)) from PMP.TB_Cotizacion",
                    CommandText = "select MAX(IN_cod_cotizacion) from PMP.TB_Cotizacion",
                    Connection = conn
                };
                try
                {
                    var max = 0;
                    var response = await cmd.ExecuteScalarAsync();
                    if (response != DBNull.Value)
                    {
                        max = int.Parse(response.ToString());
                        return string.Format("{0:D10}", max + 1);
                    }

                    return string.Format("{0:D10}", max + 1);
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: GetNumeroCotizacion", ex);
                }
            }
        }

        private async Task<string> GetNumeroCotizacionNuevaAsync()
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.Text,
                    CommandText = "select MAX(CAST(VC_numero_cotizacion as INT)) from PMP.TB_Cotizacion",
                    //CommandText = "select MAX(IN_cod_cotizacion) from PMP.TB_Cotizacion",
                    Connection = conn
                };
                try
                {
                    var max = 0;
                    var response = await cmd.ExecuteScalarAsync();
                    if (response != DBNull.Value)
                    {
                        max = int.Parse(response.ToString());
                        return string.Format("{0:D10}", max + 1);
                    }

                    return string.Format("{0:D10}", max + 1);
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: GetNumeroCotizacion", ex);
                }
            }
        }

        public async Task<DbActionResponse> CrearCotizacionAsync(InicializarCotizacionArgs args)
        {
            using (var conn = ConnectionFactory.Default())
            {
                try
                {
                    var numeroCotizacion = await GetNumeroCotizacionNuevaAsync();

                    var cmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = SP_NAME
                    };

                    cmd.Parameters.Add("@VAR_IN_cod_ramo", SqlDbType.Int).Value = args.CodigoRamo;
                    cmd.Parameters.Add("@VAR_IN_cod_subramo", SqlDbType.Int).Value = args.CodigoSubRamo;
                    cmd.Parameters.Add("@VAR_VC_numero_cotizacion", SqlDbType.VarChar).Value = numeroCotizacion;
                    cmd.Parameters.Add("@VAR_IN_cod_agencia", SqlDbType.Int).Value = args.CodigoSucursal;
                    cmd.Parameters.Add("@VAR_IN_cod_zona", SqlDbType.Int).Value = args.CodigoZona;
                    cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.Int).Value = args.UserId;
                    cmd.Parameters.Add("@VAR_IN_cod_estado_cotizacion", SqlDbType.Int).Value = CotizacionState.Created;
                    cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                    cmd.Connection = conn;

                    var resp = await cmd.ExecuteScalarAsync();
                    var codigoCotizacion = resp.ToString();
                    return new DbActionResponse
                    {
                        CodigoCotizacion = int.Parse(codigoCotizacion),
                        Version = 1,
                        NumeroCotizacion = numeroCotizacion
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: CrearCotizacionAsync", ex);
                }
            }
        }

        public async Task UpdateCotizacionTomadorAsync(int codigoCotizacion, int codigoTomador)
        {

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_tomador", SqlDbType.Int).Value = codigoTomador;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 4;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteNonQueryAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: InsertarDatosTomadorAsync", ex);
                }
            }
        }

        public async Task InsertarUsuarioNotificadoAsync(int codigoCotizacion, int version, string codigoUsuario)
        {

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_VC_cod_usuario_notificado", SqlDbType.VarChar).Value = codigoUsuario;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 15;
                cmd.Connection = conn;
                try
                {
                    await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: InsertarDatosTomadorAsync", ex);
                }
            }
        }

        public async Task InsertarDatosIntermediarioAsync(Intermediario model, int codigoTomador)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "PMP.USP_TB_Intermediario"
                };

                cmd.Parameters.Add("@VAR_IN_cod_intermediario", SqlDbType.Int).Value = model.Codigo;
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = model.CodigoCotizacion;
                cmd.Parameters.Add("@VAR_NM_participacion_intermediario", SqlDbType.Int).Value = model.Participacion;
                cmd.Parameters.Add("@VAR_IN_clave_intermediario", SqlDbType.VarChar).Value = model.Clave;
                cmd.Parameters.Add("@VAR_VC_nombre_intermediario", SqlDbType.VarChar).Value = model.PrimerNombre;
                cmd.Parameters.Add("@VAR_IN_cod_tipo_documento", SqlDbType.VarChar).Value = model.CodigoTipoDocumento;
                cmd.Parameters.Add("@VAR_IN_numero_documento", SqlDbType.VarChar).Value = model.NumeroDocumento;
                cmd.Parameters.Add("@VAR_IN_cod_estado_intermediario", SqlDbType.Int).Value = model.CodigoEstado;

                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 3;
                cmd.Connection = conn;
                try
                {
                    var res = await cmd.ExecuteScalarAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: InsertarDatosIntermediarioAsync", ex);
                }
            }
        }

        public async Task<DbActionResponse> CopiarCotizacionAsync(int userId, int cotizacion, int version, string username)
        {
            using (var conn = ConnectionFactory.Default())
            {
                try
                {
                    var numeroCotizacion = await GetNumeroCotizacionNuevaAsync();
                    var cmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = SP_NAME
                    };

                    cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = cotizacion;
                    cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@VAR_VC_cod_usuario", SqlDbType.VarChar).Value = username;
                    cmd.Parameters.Add("@VAR_VC_numero_cotizacion", SqlDbType.VarChar).Value = numeroCotizacion;
                    cmd.Parameters.Add("@VAR_IN_version_copia", SqlDbType.Int).Value = version;
                    cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 7;
                    cmd.Connection = conn;
                    var res = await cmd.ExecuteScalarAsync();
                    var codigoCotizacion = int.Parse(res.ToString());

                    return new DbActionResponse
                    {
                        CodigoCotizacion = codigoCotizacion,
                        Version = 1,
                        CodigoEstadoCotizacion = (int)CotizacionState.Created,
                        NumeroCotizacion = numeroCotizacion
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: CopiarCotizacionAsync", ex);
                }
            }
        }

        public async Task<DbActionResponse> CreateVersionCotizacionAsync(int userId, int cotizacion, int versionCopia, bool copia)
        {
            using (var conn = ConnectionFactory.Default())
            {
                try
                {
                    var numeroCotizacion = await GetNumeroCotizacionAsync();
                    var cmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = SP_NAME
                    };

                    cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = cotizacion;
                    cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@VAR_IN_version_copia", SqlDbType.Int).Value = versionCopia;
                    cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 7;
                    cmd.Connection = conn;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var codigoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            codigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "codigoCotizacion");
                            version = SqlReaderUtilities.SafeGet<int>(reader, "versionCotizacion");
                        }

                        return new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            NumeroCotizacion = numeroCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: CopiarCotizacionAsync", ex);
                }
            }
        }

        public async Task<DbActionResponse> CreateVersionAltCotizacionAsync(int userId, int cotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                try
                {
                    var numeroCotizacion = await GetNumeroCotizacionAsync();
                    var cmd = new SqlCommand
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandText = SP_NAME
                    };

                    cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = cotizacion;
                    cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.Int).Value = userId;
                    cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 71;
                    cmd.Connection = conn;
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var codigoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            codigoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "codigoCotizacion");
                            version = SqlReaderUtilities.SafeGet<int>(reader, "versionCotizacion");
                        }

                        return new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            NumeroCotizacion = numeroCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: CopiarCotizacionAsync", ex);
                }
            }
        }

        public async Task<DbActionResponse> UpdateLastAuthorCotizacionAsync(int codigoCotizacion, int lastAuthor)
        {
            // TODO 
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.Int).Value = lastAuthor;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 17;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        return new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            CodigoEstadoCotizacion = estadoCotizacion,
                            NumeroCotizacion = numeroCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: ConfirmCotizacionAsync", ex);
                }
            }
        }

        public async Task<DbActionResponse> UpdateModifiedFlagCotizacionAsync(int codigoCotizacion, bool modificado)
        {
            // TODO 
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_modificado", SqlDbType.Int).Value = modificado ? 1 : 0;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 17;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        return new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            CodigoEstadoCotizacion = estadoCotizacion,
                            NumeroCotizacion = numeroCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: ConfirmCotizacionAsync", ex);
                }
            }
        }

        public async Task<DbActionResponse> UpdateEnvioSlipCotizacionAsync(int codigoCotizacion)
        {
            // TODO 
            using (var conn = ConnectionFactory.Default())
            {
                var fechaHoy = DateTime.Now;
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_DT_fecha_envio", SqlDbType.DateTime).Value = fechaHoy;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 24;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        return new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            CodigoEstadoCotizacion = estadoCotizacion,
                            NumeroCotizacion = numeroCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: ConfirmCotizacionAsync", ex);
                }
            }
        }


        public async Task<DbActionResponse> ConfirmCotizacionAsync(int codigoCotizacion, int codigoCausal, int userId, ConfirmCotizacionAction action)
        {
            // TODO 
            var codigoEstado = action == ConfirmCotizacionAction.Accepted ? CotizacionState.Accepted
                : action == ConfirmCotizacionAction.RejectedByClient ? CotizacionState.RejectedByClient
                : action == ConfirmCotizacionAction.RejectedByCompany ? CotizacionState.RejectedByCompany
                : CotizacionState.Closed;

            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_causal", SqlDbType.Int).Value = codigoCausal;
                cmd.Parameters.Add("@VAR_IN_cod_estado_cotizacion", SqlDbType.Int).Value = codigoEstado;
                cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@VAR_IN_cierre", SqlDbType.Int).Value = 1;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 17;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        return new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            CodigoEstadoCotizacion = estadoCotizacion,
                            NumeroCotizacion = numeroCotizacion
                        };
                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: ConfirmCotizacionAsync", ex);
                }
            }
        }

        public async Task<DbActionResponse> CambiarEstadoAsync(int codigoCotizacion, CotizacionState codigoEstado)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_estado_cotizacion", SqlDbType.Int).Value = codigoEstado;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 14;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        return new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            CodigoEstadoCotizacion = estadoCotizacion,
                            NumeroCotizacion = numeroCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosInformacionNegocioTableWriter :: CambiarEstadoAsync", ex);
                }
            }
        }

        public async Task<DbActionResponse> CloseCotizacionAsyn(int codigoCotizacion)
        {
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };
                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 21;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        return new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            CodigoEstadoCotizacion = estadoCotizacion,
                            NumeroCotizacion = numeroCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosInformacionNegocioTableWriter :: CambiarEstadoAsync", ex);
                }
            }
        }

        public async Task<DbActionResponse> ContinueCotizacion(int codigoCotizacion, int userId, string comments)
        {
            // TODO review transaction
            using (var conn = ConnectionFactory.Default())
            {
                var cmd = new SqlCommand
                {
                    CommandType = CommandType.StoredProcedure,
                    CommandText = SP_NAME
                };

                cmd.Parameters.Add("@VAR_IN_cod_cotizacion", SqlDbType.Int).Value = codigoCotizacion;
                cmd.Parameters.Add("@VAR_IN_cod_usuario", SqlDbType.Int).Value = userId;
                cmd.Parameters.Add("@Transaccion", SqlDbType.Int).Value = 17;
                cmd.Connection = conn;
                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        var numeroCotizacion = "";
                        var estadoCotizacion = 0;
                        var version = 0;
                        while (await reader.ReadAsync())
                        {
                            version = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_version");
                            numeroCotizacion = SqlReaderUtilities.SafeGet<string>(reader, "VC_numero_cotizacion");
                            estadoCotizacion = SqlReaderUtilities.SafeGet<int>(reader, "IN_cod_estado_cotizacion");
                        }

                        return new DbActionResponse
                        {
                            CodigoCotizacion = codigoCotizacion,
                            Version = version,
                            CodigoEstadoCotizacion = estadoCotizacion,
                            NumeroCotizacion = numeroCotizacion
                        };
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("DatosCotizacionTableWriter :: ContinueCotizacion", ex);
                }
            }
        }
    }
}
