using System;
using System.Collections.Generic;
using System.Text;

namespace ModernizacionPersonas.Entities
{
    [Flags]
    public enum CotizacionState
    {
        Open = 1000,
        InProgress = 1100,
        Created = 1101,
        OnDatosTomador = 1102,
        OnInformacionNegocio = 1103,
        OnIntermediarios = 1104,
        OnGruposAsegurados = 1105,
        OnSiniestralidad = 1106,
        OnResumen = 1107,
        OnFichaTecnica = 1108,
        OnSlipConfiguration = 1109,
        Validated = 1110,
        PendingAuthorization = 1111,
        Lookover = 1112, // en revision por usuario cotizador
        ApprovedAuthorization = 1113,
        RefusedAuthorization = 1114,
        OnSlip = 1115,
        Resumed = 1200,
        Sent = 1300,
        Accepted = 1400,
        RejectedByClient = 1500,
        RejectedByCompany = 1600,
        Closed = 1700,
        Expired = 1800,
        Issued = 1900, // expedida
        ExpeditionRequest = 1901, // solicitud expedición
    }
}
