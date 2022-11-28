//------------------------------------------------------------------------------
// <generado automáticamente>
//     Este código fue generado por una herramienta.
//     //
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </generado automáticamente>
//------------------------------------------------------------------------------

namespace ParametrizacionServiceReference
{
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Sucursal", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Sucursal : object
    {
        
        private int CodigoSucursalField;
        
        private string DireccionField;
        
        private string NombreSucursalField;
        
        private string TelefonoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoSucursal
        {
            get
            {
                return this.CodigoSucursalField;
            }
            set
            {
                this.CodigoSucursalField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Direccion
        {
            get
            {
                return this.DireccionField;
            }
            set
            {
                this.DireccionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreSucursal
        {
            get
            {
                return this.NombreSucursalField;
            }
            set
            {
                this.NombreSucursalField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Telefono
        {
            get
            {
                return this.TelefonoField;
            }
            set
            {
                this.TelefonoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="MacroRamo", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class MacroRamo : object
    {
        
        private int CodigoMacroRamoField;
        
        private string NombreMacroRamoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoMacroRamo
        {
            get
            {
                return this.CodigoMacroRamoField;
            }
            set
            {
                this.CodigoMacroRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreMacroRamo
        {
            get
            {
                return this.NombreMacroRamoField;
            }
            set
            {
                this.NombreMacroRamoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Ramo", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Ramo : object
    {
        
        private int CodigoRamoField;
        
        private string NombreAbreviadoField;
        
        private string NombreRamoField;
        
        private string NombreReducidoField;
        
        private bool SnIvaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoRamo
        {
            get
            {
                return this.CodigoRamoField;
            }
            set
            {
                this.CodigoRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreAbreviado
        {
            get
            {
                return this.NombreAbreviadoField;
            }
            set
            {
                this.NombreAbreviadoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreRamo
        {
            get
            {
                return this.NombreRamoField;
            }
            set
            {
                this.NombreRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreReducido
        {
            get
            {
                return this.NombreReducidoField;
            }
            set
            {
                this.NombreReducidoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool SnIva
        {
            get
            {
                return this.SnIvaField;
            }
            set
            {
                this.SnIvaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoPoliza", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoPoliza : object
    {
        
        private int CodigoRamoField;
        
        private int CodigoTipoPolizaField;
        
        private string NombreTipoPolizaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoRamo
        {
            get
            {
                return this.CodigoRamoField;
            }
            set
            {
                this.CodigoRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoPoliza
        {
            get
            {
                return this.CodigoTipoPolizaField;
            }
            set
            {
                this.CodigoTipoPolizaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoPoliza
        {
            get
            {
                return this.NombreTipoPolizaField;
            }
            set
            {
                this.NombreTipoPolizaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="SubRamo", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class SubRamo : object
    {
        
        private int CodigoRamoField;
        
        private int CodigoSubRamoField;
        
        private string NombreAbreviadoField;
        
        private string NombreRamoField;
        
        private string NombreReducidoField;
        
        private string NombreSubRamoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoRamo
        {
            get
            {
                return this.CodigoRamoField;
            }
            set
            {
                this.CodigoRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoSubRamo
        {
            get
            {
                return this.CodigoSubRamoField;
            }
            set
            {
                this.CodigoSubRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreAbreviado
        {
            get
            {
                return this.NombreAbreviadoField;
            }
            set
            {
                this.NombreAbreviadoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreRamo
        {
            get
            {
                return this.NombreRamoField;
            }
            set
            {
                this.NombreRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreReducido
        {
            get
            {
                return this.NombreReducidoField;
            }
            set
            {
                this.NombreReducidoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreSubRamo
        {
            get
            {
                return this.NombreSubRamoField;
            }
            set
            {
                this.NombreSubRamoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Deducible", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Deducible : object
    {
        
        private int CodigoAplicaDeducibleField;
        
        private int CodigoAplicaDeducibleMaximoField;
        
        private int CodigoAplicaDeducibleMinimoField;
        
        private int CodigoDeducibleField;
        
        private int CodigoRamoField;
        
        private int CodigoTipoPorcentajeField;
        
        private int CodigoUnidadDeducibleField;
        
        private int CodigoUnidadMaximaField;
        
        private int CodigoUnidadMaximaMinimoField;
        
        private decimal ImporteMaximoField;
        
        private decimal ImporteMinimoField;
        
        private decimal ImporteValorField;
        
        private string NombreDeducibleField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoAplicaDeducible
        {
            get
            {
                return this.CodigoAplicaDeducibleField;
            }
            set
            {
                this.CodigoAplicaDeducibleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoAplicaDeducibleMaximo
        {
            get
            {
                return this.CodigoAplicaDeducibleMaximoField;
            }
            set
            {
                this.CodigoAplicaDeducibleMaximoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoAplicaDeducibleMinimo
        {
            get
            {
                return this.CodigoAplicaDeducibleMinimoField;
            }
            set
            {
                this.CodigoAplicaDeducibleMinimoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoDeducible
        {
            get
            {
                return this.CodigoDeducibleField;
            }
            set
            {
                this.CodigoDeducibleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoRamo
        {
            get
            {
                return this.CodigoRamoField;
            }
            set
            {
                this.CodigoRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoPorcentaje
        {
            get
            {
                return this.CodigoTipoPorcentajeField;
            }
            set
            {
                this.CodigoTipoPorcentajeField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoUnidadDeducible
        {
            get
            {
                return this.CodigoUnidadDeducibleField;
            }
            set
            {
                this.CodigoUnidadDeducibleField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoUnidadMaxima
        {
            get
            {
                return this.CodigoUnidadMaximaField;
            }
            set
            {
                this.CodigoUnidadMaximaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoUnidadMaximaMinimo
        {
            get
            {
                return this.CodigoUnidadMaximaMinimoField;
            }
            set
            {
                this.CodigoUnidadMaximaMinimoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal ImporteMaximo
        {
            get
            {
                return this.ImporteMaximoField;
            }
            set
            {
                this.ImporteMaximoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal ImporteMinimo
        {
            get
            {
                return this.ImporteMinimoField;
            }
            set
            {
                this.ImporteMinimoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal ImporteValor
        {
            get
            {
                return this.ImporteValorField;
            }
            set
            {
                this.ImporteValorField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreDeducible
        {
            get
            {
                return this.NombreDeducibleField;
            }
            set
            {
                this.NombreDeducibleField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Pais", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Pais : object
    {
        
        private int CodigoPaisField;
        
        private int CodigoRuntField;
        
        private string NombrePaisField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoPais
        {
            get
            {
                return this.CodigoPaisField;
            }
            set
            {
                this.CodigoPaisField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoRunt
        {
            get
            {
                return this.CodigoRuntField;
            }
            set
            {
                this.CodigoRuntField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombrePais
        {
            get
            {
                return this.NombrePaisField;
            }
            set
            {
                this.NombrePaisField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Departamento", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Departamento : object
    {
        
        private int CodigoDepartamentoField;
        
        private int CodigoPaisField;
        
        private string NombreDepartamentoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoDepartamento
        {
            get
            {
                return this.CodigoDepartamentoField;
            }
            set
            {
                this.CodigoDepartamentoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoPais
        {
            get
            {
                return this.CodigoPaisField;
            }
            set
            {
                this.CodigoPaisField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreDepartamento
        {
            get
            {
                return this.NombreDepartamentoField;
            }
            set
            {
                this.NombreDepartamentoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Municipio", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Municipio : object
    {
        
        private int CodigoDepartamentoField;
        
        private long CodigoDivipolaField;
        
        private int CodigoMunicipioField;
        
        private int CodigoPaisField;
        
        private string NombreMunicipioField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoDepartamento
        {
            get
            {
                return this.CodigoDepartamentoField;
            }
            set
            {
                this.CodigoDepartamentoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long CodigoDivipola
        {
            get
            {
                return this.CodigoDivipolaField;
            }
            set
            {
                this.CodigoDivipolaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoMunicipio
        {
            get
            {
                return this.CodigoMunicipioField;
            }
            set
            {
                this.CodigoMunicipioField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoPais
        {
            get
            {
                return this.CodigoPaisField;
            }
            set
            {
                this.CodigoPaisField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreMunicipio
        {
            get
            {
                return this.NombreMunicipioField;
            }
            set
            {
                this.NombreMunicipioField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoDocumento", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoDocumento : object
    {
        
        private int CodigoTipoDocumentoField;
        
        private string NombreReducidoField;
        
        private string NombreTipoDocumentoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoDocumento
        {
            get
            {
                return this.CodigoTipoDocumentoField;
            }
            set
            {
                this.CodigoTipoDocumentoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreReducido
        {
            get
            {
                return this.NombreReducidoField;
            }
            set
            {
                this.NombreReducidoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoDocumento
        {
            get
            {
                return this.NombreTipoDocumentoField;
            }
            set
            {
                this.NombreTipoDocumentoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoTelefono", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoTelefono : object
    {
        
        private int CodigoTipoTelefonoField;
        
        private string NombreTipoTelefonoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoTelefono
        {
            get
            {
                return this.CodigoTipoTelefonoField;
            }
            set
            {
                this.CodigoTipoTelefonoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoTelefono
        {
            get
            {
                return this.NombreTipoTelefonoField;
            }
            set
            {
                this.NombreTipoTelefonoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoDireccion", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoDireccion : object
    {
        
        private int CodigoTipoDireccionField;
        
        private string NombreTipoDireccionField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoDireccion
        {
            get
            {
                return this.CodigoTipoDireccionField;
            }
            set
            {
                this.CodigoTipoDireccionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoDireccion
        {
            get
            {
                return this.NombreTipoDireccionField;
            }
            set
            {
                this.NombreTipoDireccionField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoAgente", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    [System.Runtime.Serialization.KnownTypeAttribute(typeof(ParametrizacionServiceReference.Agente))]
    public partial class TipoAgente : object
    {
        
        private int CodigoTipoAgenteField;
        
        private string NombreTipoAgenteField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoAgente
        {
            get
            {
                return this.CodigoTipoAgenteField;
            }
            set
            {
                this.CodigoTipoAgenteField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoAgente
        {
            get
            {
                return this.NombreTipoAgenteField;
            }
            set
            {
                this.NombreTipoAgenteField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Agente", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Agente : ParametrizacionServiceReference.TipoAgente
    {
        
        private int CodigoAgenteField;
        
        private string NombreAgenteField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoAgente
        {
            get
            {
                return this.CodigoAgenteField;
            }
            set
            {
                this.CodigoAgenteField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreAgente
        {
            get
            {
                return this.NombreAgenteField;
            }
            set
            {
                this.NombreAgenteField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Moneda", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Moneda : object
    {
        
        private string AbreviaturaMonedaField;
        
        private int CodigoMonedaField;
        
        private string NombreMonedaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string AbreviaturaMoneda
        {
            get
            {
                return this.AbreviaturaMonedaField;
            }
            set
            {
                this.AbreviaturaMonedaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoMoneda
        {
            get
            {
                return this.CodigoMonedaField;
            }
            set
            {
                this.CodigoMonedaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreMoneda
        {
            get
            {
                return this.NombreMonedaField;
            }
            set
            {
                this.NombreMonedaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoMovimiento", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoMovimiento : object
    {
        
        private int CodigoTipoMovimientoField;
        
        private string NombreTipoMovimientoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoMovimiento
        {
            get
            {
                return this.CodigoTipoMovimientoField;
            }
            set
            {
                this.CodigoTipoMovimientoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoMovimiento
        {
            get
            {
                return this.NombreTipoMovimientoField;
            }
            set
            {
                this.NombreTipoMovimientoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GrupoEndoso", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class GrupoEndoso : object
    {
        
        private int CodigoGrupoEndosoField;
        
        private string NombreGrupoEndosoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoGrupoEndoso
        {
            get
            {
                return this.CodigoGrupoEndosoField;
            }
            set
            {
                this.CodigoGrupoEndosoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreGrupoEndoso
        {
            get
            {
                return this.NombreGrupoEndosoField;
            }
            set
            {
                this.NombreGrupoEndosoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoEndoso", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoEndoso : object
    {
        
        private int CodigoGrupoEndosoField;
        
        private int CodigoTipoEndosoField;
        
        private string NombreTipoEndosoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoGrupoEndoso
        {
            get
            {
                return this.CodigoGrupoEndosoField;
            }
            set
            {
                this.CodigoGrupoEndosoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoEndoso
        {
            get
            {
                return this.CodigoTipoEndosoField;
            }
            set
            {
                this.CodigoTipoEndosoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoEndoso
        {
            get
            {
                return this.NombreTipoEndosoField;
            }
            set
            {
                this.NombreTipoEndosoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Convenio", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Convenio : object
    {
        
        private int CodigoConvenioField;
        
        private string NombreConvenioField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoConvenio
        {
            get
            {
                return this.CodigoConvenioField;
            }
            set
            {
                this.CodigoConvenioField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreConvenio
        {
            get
            {
                return this.NombreConvenioField;
            }
            set
            {
                this.NombreConvenioField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PuntoVenta", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class PuntoVenta : object
    {
        
        private int CodigoPuntoVentaField;
        
        private string DireccionField;
        
        private string NombrePuntoVentaField;
        
        private string TelefonoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoPuntoVenta
        {
            get
            {
                return this.CodigoPuntoVentaField;
            }
            set
            {
                this.CodigoPuntoVentaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Direccion
        {
            get
            {
                return this.DireccionField;
            }
            set
            {
                this.DireccionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombrePuntoVenta
        {
            get
            {
                return this.NombrePuntoVentaField;
            }
            set
            {
                this.NombrePuntoVentaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Telefono
        {
            get
            {
                return this.TelefonoField;
            }
            set
            {
                this.TelefonoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="OficinaConvenio", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class OficinaConvenio : object
    {
        
        private int CodigoConvenioField;
        
        private int CodigoOficinaField;
        
        private int CodigoPuntoVentaField;
        
        private string NombreOficinaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoConvenio
        {
            get
            {
                return this.CodigoConvenioField;
            }
            set
            {
                this.CodigoConvenioField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoOficina
        {
            get
            {
                return this.CodigoOficinaField;
            }
            set
            {
                this.CodigoOficinaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoPuntoVenta
        {
            get
            {
                return this.CodigoPuntoVentaField;
            }
            set
            {
                this.CodigoPuntoVentaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreOficina
        {
            get
            {
                return this.NombreOficinaField;
            }
            set
            {
                this.NombreOficinaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Usuario", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Usuario : object
    {
        
        private int CodigoAgenteField;
        
        private int CodigoGrupoPerfilField;
        
        private int CodigoGrupoUsuarioField;
        
        private int CodigoPuntoVentaField;
        
        private int CodigoRamoField;
        
        private int CodigoSectorField;
        
        private int CodigoSucursalField;
        
        private int CodigoTipoAgenteField;
        
        private string CodigoUsuarioField;
        
        private string CorreoField;
        
        private string DescripcionPerfilField;
        
        private string FechaVencimientoPwdField;
        
        private decimal LimiteOperacionField;
        
        private string NombreUsuarioField;
        
        private string RutaImpresionField;
        
        private int SnActivoField;
        
        private int SnExternoField;
        
        private int SnPerfilField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoAgente
        {
            get
            {
                return this.CodigoAgenteField;
            }
            set
            {
                this.CodigoAgenteField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoGrupoPerfil
        {
            get
            {
                return this.CodigoGrupoPerfilField;
            }
            set
            {
                this.CodigoGrupoPerfilField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoGrupoUsuario
        {
            get
            {
                return this.CodigoGrupoUsuarioField;
            }
            set
            {
                this.CodigoGrupoUsuarioField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoPuntoVenta
        {
            get
            {
                return this.CodigoPuntoVentaField;
            }
            set
            {
                this.CodigoPuntoVentaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoRamo
        {
            get
            {
                return this.CodigoRamoField;
            }
            set
            {
                this.CodigoRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoSector
        {
            get
            {
                return this.CodigoSectorField;
            }
            set
            {
                this.CodigoSectorField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoSucursal
        {
            get
            {
                return this.CodigoSucursalField;
            }
            set
            {
                this.CodigoSucursalField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoAgente
        {
            get
            {
                return this.CodigoTipoAgenteField;
            }
            set
            {
                this.CodigoTipoAgenteField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string CodigoUsuario
        {
            get
            {
                return this.CodigoUsuarioField;
            }
            set
            {
                this.CodigoUsuarioField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Correo
        {
            get
            {
                return this.CorreoField;
            }
            set
            {
                this.CorreoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DescripcionPerfil
        {
            get
            {
                return this.DescripcionPerfilField;
            }
            set
            {
                this.DescripcionPerfilField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string FechaVencimientoPwd
        {
            get
            {
                return this.FechaVencimientoPwdField;
            }
            set
            {
                this.FechaVencimientoPwdField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal LimiteOperacion
        {
            get
            {
                return this.LimiteOperacionField;
            }
            set
            {
                this.LimiteOperacionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreUsuario
        {
            get
            {
                return this.NombreUsuarioField;
            }
            set
            {
                this.NombreUsuarioField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string RutaImpresion
        {
            get
            {
                return this.RutaImpresionField;
            }
            set
            {
                this.RutaImpresionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SnActivo
        {
            get
            {
                return this.SnActivoField;
            }
            set
            {
                this.SnActivoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SnExterno
        {
            get
            {
                return this.SnExternoField;
            }
            set
            {
                this.SnExternoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SnPerfil
        {
            get
            {
                return this.SnPerfilField;
            }
            set
            {
                this.SnPerfilField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ActividadEconomica", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class ActividadEconomica : object
    {
        
        private int CodigoActividadEconomicaField;
        
        private string NombreActividadEconomicaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoActividadEconomica
        {
            get
            {
                return this.CodigoActividadEconomicaField;
            }
            set
            {
                this.CodigoActividadEconomicaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreActividadEconomica
        {
            get
            {
                return this.NombreActividadEconomicaField;
            }
            set
            {
                this.NombreActividadEconomicaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoAsegurado", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoAsegurado : object
    {
        
        private int CodigoTipoAseguradoField;
        
        private string NombreTipoAseguradoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoAsegurado
        {
            get
            {
                return this.CodigoTipoAseguradoField;
            }
            set
            {
                this.CodigoTipoAseguradoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoAsegurado
        {
            get
            {
                return this.NombreTipoAseguradoField;
            }
            set
            {
                this.NombreTipoAseguradoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CategoriaAsegurado", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class CategoriaAsegurado : object
    {
        
        private int CodigoCategoriaAseguradoField;
        
        private string NombreCategoriaAseguradoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoCategoriaAsegurado
        {
            get
            {
                return this.CodigoCategoriaAseguradoField;
            }
            set
            {
                this.CodigoCategoriaAseguradoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreCategoriaAsegurado
        {
            get
            {
                return this.NombreCategoriaAseguradoField;
            }
            set
            {
                this.NombreCategoriaAseguradoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoCliente", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoCliente : object
    {
        
        private int CodigoTipoClienteField;
        
        private string NombreTipoClienteField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoCliente
        {
            get
            {
                return this.CodigoTipoClienteField;
            }
            set
            {
                this.CodigoTipoClienteField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoCliente
        {
            get
            {
                return this.NombreTipoClienteField;
            }
            set
            {
                this.NombreTipoClienteField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="GiroNegocio", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class GiroNegocio : object
    {
        
        private int CodigoGiroNegocioField;
        
        private int CodigoRamoField;
        
        private string DescripcionGiroNegocioField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoGiroNegocio
        {
            get
            {
                return this.CodigoGiroNegocioField;
            }
            set
            {
                this.CodigoGiroNegocioField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoRamo
        {
            get
            {
                return this.CodigoRamoField;
            }
            set
            {
                this.CodigoRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DescripcionGiroNegocio
        {
            get
            {
                return this.DescripcionGiroNegocioField;
            }
            set
            {
                this.DescripcionGiroNegocioField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Ocupacion", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Ocupacion : object
    {
        
        private int CodigoOcupacionField;
        
        private string NombreOcupacionField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoOcupacion
        {
            get
            {
                return this.CodigoOcupacionField;
            }
            set
            {
                this.CodigoOcupacionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreOcupacion
        {
            get
            {
                return this.NombreOcupacionField;
            }
            set
            {
                this.NombreOcupacionField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Agrupador", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Agrupador : object
    {
        
        private int CodigoAseguradoField;
        
        private int CodigoGrupoField;
        
        private int CodigoSucursalField;
        
        private string NombreGrupoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoAsegurado
        {
            get
            {
                return this.CodigoAseguradoField;
            }
            set
            {
                this.CodigoAseguradoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoGrupo
        {
            get
            {
                return this.CodigoGrupoField;
            }
            set
            {
                this.CodigoGrupoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoSucursal
        {
            get
            {
                return this.CodigoSucursalField;
            }
            set
            {
                this.CodigoSucursalField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreGrupo
        {
            get
            {
                return this.NombreGrupoField;
            }
            set
            {
                this.NombreGrupoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoNegocio", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoNegocio : object
    {
        
        private int CodigoTipoNegocioField;
        
        private string DescripcionTipoNegocioField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoNegocio
        {
            get
            {
                return this.CodigoTipoNegocioField;
            }
            set
            {
                this.CodigoTipoNegocioField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string DescripcionTipoNegocio
        {
            get
            {
                return this.DescripcionTipoNegocioField;
            }
            set
            {
                this.DescripcionTipoNegocioField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Parentesco", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Parentesco : object
    {
        
        private int CodigoParentescoField;
        
        private string NombreParentescoField;
        
        private int OrdenField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoParentesco
        {
            get
            {
                return this.CodigoParentescoField;
            }
            set
            {
                this.CodigoParentescoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreParentesco
        {
            get
            {
                return this.NombreParentescoField;
            }
            set
            {
                this.NombreParentescoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int Orden
        {
            get
            {
                return this.OrdenField;
            }
            set
            {
                this.OrdenField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="EstadoCivil", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class EstadoCivil : object
    {
        
        private int CodigoEstadoCivilField;
        
        private string NombreEstadoCivilField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoEstadoCivil
        {
            get
            {
                return this.CodigoEstadoCivilField;
            }
            set
            {
                this.CodigoEstadoCivilField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreEstadoCivil
        {
            get
            {
                return this.NombreEstadoCivilField;
            }
            set
            {
                this.NombreEstadoCivilField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoTasa", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoTasa : object
    {
        
        private int CodigoTipoTasaField;
        
        private string NombreTipoTasaField;
        
        private long NumeroDivisorField;
        
        private string SimboloField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoTasa
        {
            get
            {
                return this.CodigoTipoTasaField;
            }
            set
            {
                this.CodigoTipoTasaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoTasa
        {
            get
            {
                return this.NombreTipoTasaField;
            }
            set
            {
                this.NombreTipoTasaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public long NumeroDivisor
        {
            get
            {
                return this.NumeroDivisorField;
            }
            set
            {
                this.NumeroDivisorField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Simbolo
        {
            get
            {
                return this.SimboloField;
            }
            set
            {
                this.SimboloField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoVigencia", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoVigencia : object
    {
        
        private int CantidadMesesField;
        
        private int CodigoTipoVigenciaField;
        
        private string NombreTipoVigenciaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CantidadMeses
        {
            get
            {
                return this.CantidadMesesField;
            }
            set
            {
                this.CantidadMesesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoVigencia
        {
            get
            {
                return this.CodigoTipoVigenciaField;
            }
            set
            {
                this.CodigoTipoVigenciaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreTipoVigencia
        {
            get
            {
                return this.NombreTipoVigenciaField;
            }
            set
            {
                this.NombreTipoVigenciaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PeriodoFacturacion", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class PeriodoFacturacion : object
    {
        
        private int CantidadMesesField;
        
        private int CodigoPeriodoField;
        
        private string NombrePeriodoField;
        
        private int NumeroDivisorField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CantidadMeses
        {
            get
            {
                return this.CantidadMesesField;
            }
            set
            {
                this.CantidadMesesField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoPeriodo
        {
            get
            {
                return this.CodigoPeriodoField;
            }
            set
            {
                this.CodigoPeriodoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombrePeriodo
        {
            get
            {
                return this.NombrePeriodoField;
            }
            set
            {
                this.NombrePeriodoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumeroDivisor
        {
            get
            {
                return this.NumeroDivisorField;
            }
            set
            {
                this.NumeroDivisorField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CalculoFacturacion", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class CalculoFacturacion : object
    {
        
        private int CodigoCalculoField;
        
        private string NombreCalculoField;
        
        private int SnHabilitadoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoCalculo
        {
            get
            {
                return this.CodigoCalculoField;
            }
            set
            {
                this.CodigoCalculoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreCalculo
        {
            get
            {
                return this.NombreCalculoField;
            }
            set
            {
                this.NombreCalculoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SnHabilitado
        {
            get
            {
                return this.SnHabilitadoField;
            }
            set
            {
                this.SnHabilitadoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Coaseguradora", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Coaseguradora : object
    {
        
        private int CodigoCoaseguradoraField;
        
        private string NombreCoaseguradoraField;
        
        private string NombreReducidoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoCoaseguradora
        {
            get
            {
                return this.CodigoCoaseguradoraField;
            }
            set
            {
                this.CodigoCoaseguradoraField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreCoaseguradora
        {
            get
            {
                return this.NombreCoaseguradoraField;
            }
            set
            {
                this.NombreCoaseguradoraField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreReducido
        {
            get
            {
                return this.NombreReducidoField;
            }
            set
            {
                this.NombreReducidoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Cargo", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Cargo : object
    {
        
        private int CodigoCargoField;
        
        private string NombreCargoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoCargo
        {
            get
            {
                return this.CodigoCargoField;
            }
            set
            {
                this.CodigoCargoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreCargo
        {
            get
            {
                return this.NombreCargoField;
            }
            set
            {
                this.NombreCargoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Area", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Area : object
    {
        
        private int CodigoAreaField;
        
        private string NombreAreaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoArea
        {
            get
            {
                return this.CodigoAreaField;
            }
            set
            {
                this.CodigoAreaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreArea
        {
            get
            {
                return this.NombreAreaField;
            }
            set
            {
                this.NombreAreaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="InformacionFinanciera", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class InformacionFinanciera : object
    {
        
        private int CodigoConceptoField;
        
        private int CodigoTipoDatoField;
        
        private string NombreConceptoField;
        
        private string OperadorField;
        
        private decimal ValorField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoConcepto
        {
            get
            {
                return this.CodigoConceptoField;
            }
            set
            {
                this.CodigoConceptoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoDato
        {
            get
            {
                return this.CodigoTipoDatoField;
            }
            set
            {
                this.CodigoTipoDatoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreConcepto
        {
            get
            {
                return this.NombreConceptoField;
            }
            set
            {
                this.NombreConceptoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Operador
        {
            get
            {
                return this.OperadorField;
            }
            set
            {
                this.OperadorField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal Valor
        {
            get
            {
                return this.ValorField;
            }
            set
            {
                this.ValorField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Conducto", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Conducto : object
    {
        
        private int CodigoConductoField;
        
        private int CodigoTipoConductoField;
        
        private string NombreConductoField;
        
        private int SiNoContadoField;
        
        private int SiNoEmisionField;
        
        private int SiNoFinanciacionField;
        
        private int SiNoFraccionamientoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoConducto
        {
            get
            {
                return this.CodigoConductoField;
            }
            set
            {
                this.CodigoConductoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoConducto
        {
            get
            {
                return this.CodigoTipoConductoField;
            }
            set
            {
                this.CodigoTipoConductoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreConducto
        {
            get
            {
                return this.NombreConductoField;
            }
            set
            {
                this.NombreConductoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SiNoContado
        {
            get
            {
                return this.SiNoContadoField;
            }
            set
            {
                this.SiNoContadoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SiNoEmision
        {
            get
            {
                return this.SiNoEmisionField;
            }
            set
            {
                this.SiNoEmisionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SiNoFinanciacion
        {
            get
            {
                return this.SiNoFinanciacionField;
            }
            set
            {
                this.SiNoFinanciacionField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SiNoFraccionamiento
        {
            get
            {
                return this.SiNoFraccionamientoField;
            }
            set
            {
                this.SiNoFraccionamientoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ConductoPlan", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class ConductoPlan : object
    {
        
        private int CodigoConductoField;
        
        private int CodigoPlanPagoField;
        
        private string NombreConductoField;
        
        private string NombrePlanPagoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoConducto
        {
            get
            {
                return this.CodigoConductoField;
            }
            set
            {
                this.CodigoConductoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoPlanPago
        {
            get
            {
                return this.CodigoPlanPagoField;
            }
            set
            {
                this.CodigoPlanPagoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreConducto
        {
            get
            {
                return this.NombreConductoField;
            }
            set
            {
                this.NombreConductoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombrePlanPago
        {
            get
            {
                return this.NombrePlanPagoField;
            }
            set
            {
                this.NombrePlanPagoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="PlanPago", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class PlanPago : object
    {
        
        private int CodigoPlanPagoField;
        
        private string NombrePlanPagoField;
        
        private int SiNoFinanciacionField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoPlanPago
        {
            get
            {
                return this.CodigoPlanPagoField;
            }
            set
            {
                this.CodigoPlanPagoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombrePlanPago
        {
            get
            {
                return this.NombrePlanPagoField;
            }
            set
            {
                this.NombrePlanPagoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int SiNoFinanciacion
        {
            get
            {
                return this.SiNoFinanciacionField;
            }
            set
            {
                this.SiNoFinanciacionField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="FormaPago", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class FormaPago : object
    {
        
        private int CodigoFormaPagoField;
        
        private string NombreFormaPagoField;
        
        private int NumeroDivisorField;
        
        private int NumeroMesesField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoFormaPago
        {
            get
            {
                return this.CodigoFormaPagoField;
            }
            set
            {
                this.CodigoFormaPagoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreFormaPago
        {
            get
            {
                return this.NombreFormaPagoField;
            }
            set
            {
                this.NombreFormaPagoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumeroDivisor
        {
            get
            {
                return this.NumeroDivisorField;
            }
            set
            {
                this.NumeroDivisorField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumeroMeses
        {
            get
            {
                return this.NumeroMesesField;
            }
            set
            {
                this.NumeroMesesField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="UsuarioSoligespro", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class UsuarioSoligespro : object
    {
        
        private string AreaField;
        
        private string CargoField;
        
        private int NumeroAreaField;
        
        private int NumeroCargoField;
        
        private int NumeroZonaField;
        
        private string UsuarioField;
        
        private string ZonaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Area
        {
            get
            {
                return this.AreaField;
            }
            set
            {
                this.AreaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Cargo
        {
            get
            {
                return this.CargoField;
            }
            set
            {
                this.CargoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumeroArea
        {
            get
            {
                return this.NumeroAreaField;
            }
            set
            {
                this.NumeroAreaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumeroCargo
        {
            get
            {
                return this.NumeroCargoField;
            }
            set
            {
                this.NumeroCargoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int NumeroZona
        {
            get
            {
                return this.NumeroZonaField;
            }
            set
            {
                this.NumeroZonaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Usuario
        {
            get
            {
                return this.UsuarioField;
            }
            set
            {
                this.UsuarioField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Zona
        {
            get
            {
                return this.ZonaField;
            }
            set
            {
                this.ZonaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="ConductoPago", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class ConductoPago : object
    {
        
        private int CodigoConductoField;
        
        private string ConductoField;
        
        private string IdConductoField;
        
        private string NumeroCuentaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoConducto
        {
            get
            {
                return this.CodigoConductoField;
            }
            set
            {
                this.CodigoConductoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Conducto
        {
            get
            {
                return this.ConductoField;
            }
            set
            {
                this.ConductoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string IdConducto
        {
            get
            {
                return this.IdConductoField;
            }
            set
            {
                this.IdConductoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NumeroCuenta
        {
            get
            {
                return this.NumeroCuentaField;
            }
            set
            {
                this.NumeroCuentaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Banco", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class Banco : object
    {
        
        private int CodigoBancoField;
        
        private string NombreBancoField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoBanco
        {
            get
            {
                return this.CodigoBancoField;
            }
            set
            {
                this.CodigoBancoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string NombreBanco
        {
            get
            {
                return this.NombreBancoField;
            }
            set
            {
                this.NombreBancoField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="TipoEmpresa", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class TipoEmpresa : object
    {
        
        private int CodigoTipoEmpresaField;
        
        private string TipoDeEmpresaField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoTipoEmpresa
        {
            get
            {
                return this.CodigoTipoEmpresaField;
            }
            set
            {
                this.CodigoTipoEmpresaField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string TipoDeEmpresa
        {
            get
            {
                return this.TipoDeEmpresaField;
            }
            set
            {
                this.TipoDeEmpresaField = value;
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.Runtime.Serialization.DataContractAttribute(Name="RamoImpuesto", Namespace="http://schemas.datacontract.org/2004/07/Tablas_Parametrizacion.Entidades")]
    public partial class RamoImpuesto : object
    {
        
        private int CodigoRamoField;
        
        private decimal PjeIVAField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int CodigoRamo
        {
            get
            {
                return this.CodigoRamoField;
            }
            set
            {
                this.CodigoRamoField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public decimal PjeIVA
        {
            get
            {
                return this.PjeIVAField;
            }
            set
            {
                this.PjeIVAField = value;
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ParametrizacionServiceReference.IService1")]
    public interface IService1
    {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerSucursales", ReplyAction="http://tempuri.org/IService1/TraerSucursalesResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Sucursal[]> TraerSucursalesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerSucursal", ReplyAction="http://tempuri.org/IService1/TraerSucursalResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Sucursal> TraerSucursalAsync(int CodigoSucursal);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerSucursalesxAgente", ReplyAction="http://tempuri.org/IService1/TraerSucursalesxAgenteResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Sucursal[]> TraerSucursalesxAgenteAsync(int CodigoTipoAgente, int CodigoAgente);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerSucursalesxUsuario", ReplyAction="http://tempuri.org/IService1/TraerSucursalesxUsuarioResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Sucursal[]> TraerSucursalesxUsuarioAsync(string CodigoUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerMacroRamos", ReplyAction="http://tempuri.org/IService1/TraerMacroRamosResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.MacroRamo[]> TraerMacroRamosAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerMacroRamo", ReplyAction="http://tempuri.org/IService1/TraerMacroRamoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.MacroRamo> TraerMacroRamoAsync(int CodigoMacroRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerMacroRamoxRamo", ReplyAction="http://tempuri.org/IService1/TraerMacroRamoxRamoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.MacroRamo> TraerMacroRamoxRamoAsync(int CodigoMacroRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerRamos", ReplyAction="http://tempuri.org/IService1/TraerRamosResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo[]> TraerRamosAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerRamosxMacroRamo", ReplyAction="http://tempuri.org/IService1/TraerRamosxMacroRamoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo[]> TraerRamosxMacroRamoAsync(int CodigoMacroRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerRamo", ReplyAction="http://tempuri.org/IService1/TraerRamoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo> TraerRamoAsync(int CodigoRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerRamosxSucursal", ReplyAction="http://tempuri.org/IService1/TraerRamosxSucursalResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo[]> TraerRamosxSucursalAsync(int CodigoSucursal);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerRamoxSucursal", ReplyAction="http://tempuri.org/IService1/TraerRamoxSucursalResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo> TraerRamoxSucursalAsync(int CodigoSucursal, int CodigoRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoPoliza", ReplyAction="http://tempuri.org/IService1/TraerTipoPolizaResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoPoliza[]> TraerTipoPolizaAsync(int CodigoRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerSubRamosxRamo", ReplyAction="http://tempuri.org/IService1/TraerSubRamosxRamoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.SubRamo[]> TraerSubRamosxRamoAsync(int CodigoRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerSubRamo", ReplyAction="http://tempuri.org/IService1/TraerSubRamoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.SubRamo> TraerSubRamoAsync(int CodigoRamo, int CodigoSubRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerDeducible", ReplyAction="http://tempuri.org/IService1/TraerDeducibleResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Deducible[]> TraerDeducibleAsync(int CodigoRamo, int CodigoSubRamo, int CodigoTarifa);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerPaises", ReplyAction="http://tempuri.org/IService1/TraerPaisesResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Pais[]> TraerPaisesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerDepartamentos", ReplyAction="http://tempuri.org/IService1/TraerDepartamentosResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Departamento[]> TraerDepartamentosAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerDepartamento", ReplyAction="http://tempuri.org/IService1/TraerDepartamentoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Departamento> TraerDepartamentoAsync(int CodigoDepartamento);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerMunicipiosxDepartamento", ReplyAction="http://tempuri.org/IService1/TraerMunicipiosxDepartamentoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Municipio[]> TraerMunicipiosxDepartamentoAsync(int CodigoDepartamento);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerMunicipio", ReplyAction="http://tempuri.org/IService1/TraerMunicipioResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Municipio> TraerMunicipioAsync(int CodigoDepartamento, int CodigoMunicipio);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoDocumento", ReplyAction="http://tempuri.org/IService1/TraerTipoDocumentoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoDocumento[]> TraerTipoDocumentoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoTelefono", ReplyAction="http://tempuri.org/IService1/TraerTipoTelefonoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoTelefono[]> TraerTipoTelefonoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoTelefonoxCodigo", ReplyAction="http://tempuri.org/IService1/TraerTipoTelefonoxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoTelefono> TraerTipoTelefonoxCodigoAsync(int CodigoTipoTelefono);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoDireccion", ReplyAction="http://tempuri.org/IService1/TraerTipoDireccionResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoDireccion[]> TraerTipoDireccionAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoDireccionxCodigo", ReplyAction="http://tempuri.org/IService1/TraerTipoDireccionxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoDireccion> TraerTipoDireccionxCodigoAsync(int CodigoTipoDireccion);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoAgente", ReplyAction="http://tempuri.org/IService1/TraerTipoAgenteResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoAgente[]> TraerTipoAgenteAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoAgentexCodigo", ReplyAction="http://tempuri.org/IService1/TraerTipoAgentexCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoAgente> TraerTipoAgentexCodigoAsync(int CodigoTipoAgente);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerMoneda", ReplyAction="http://tempuri.org/IService1/TraerMonedaResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Moneda[]> TraerMonedaAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoMovimiento", ReplyAction="http://tempuri.org/IService1/TraerTipoMovimientoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoMovimiento[]> TraerTipoMovimientoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerGrupoEndoso", ReplyAction="http://tempuri.org/IService1/TraerGrupoEndosoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.GrupoEndoso[]> TraerGrupoEndosoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoEndoso", ReplyAction="http://tempuri.org/IService1/TraerTipoEndosoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoEndoso[]> TraerTipoEndosoAsync(int CodigoGrupoEndoso);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoEndosoxCodigo", ReplyAction="http://tempuri.org/IService1/TraerTipoEndosoxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoEndoso> TraerTipoEndosoxCodigoAsync(int CodigoGrupoEndoso, int CodigoTipoEndoso);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerConvenios", ReplyAction="http://tempuri.org/IService1/TraerConveniosResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Convenio[]> TraerConveniosAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerConvenioxCodigo", ReplyAction="http://tempuri.org/IService1/TraerConvenioxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Convenio> TraerConvenioxCodigoAsync(int CodigoConvenio);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerPuntoVenta", ReplyAction="http://tempuri.org/IService1/TraerPuntoVentaResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta[]> TraerPuntoVentaAsync(int CodigoSucursal);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerPuntoVentaxCodigo", ReplyAction="http://tempuri.org/IService1/TraerPuntoVentaxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta> TraerPuntoVentaxCodigoAsync(int CodigoSucursal, int CodigoPuntoVenta);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerPuntoVentaxUsuario", ReplyAction="http://tempuri.org/IService1/TraerPuntoVentaxUsuarioResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta[]> TraerPuntoVentaxUsuarioAsync(string CodigoUsuario, int CodigoSucursal);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerPuntoVentaxSucursalxUsuario", ReplyAction="http://tempuri.org/IService1/TraerPuntoVentaxSucursalxUsuarioResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta[]> TraerPuntoVentaxSucursalxUsuarioAsync(int CodigoSucursal, string CodigoUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerPuntoVentaxAgente", ReplyAction="http://tempuri.org/IService1/TraerPuntoVentaxAgenteResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta[]> TraerPuntoVentaxAgenteAsync(int CodigoTipoAgente, int CodigoAgente);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerOficinasConvenio", ReplyAction="http://tempuri.org/IService1/TraerOficinasConvenioResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.OficinaConvenio[]> TraerOficinasConvenioAsync(int CodigoConvenio);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerOficinasConvenioxCodigo", ReplyAction="http://tempuri.org/IService1/TraerOficinasConvenioxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.OficinaConvenio> TraerOficinasConvenioxCodigoAsync(int CodigoConvenio, int CodigoPuntoVenta);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerUsuarioxCodigo", ReplyAction="http://tempuri.org/IService1/TraerUsuarioxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Usuario> TraerUsuarioxCodigoAsync(string CodigoUsuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerUsuarioxSucursal", ReplyAction="http://tempuri.org/IService1/TraerUsuarioxSucursalResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Usuario[]> TraerUsuarioxSucursalAsync(int CodigoSucursal);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerUsuarioxIntermediario", ReplyAction="http://tempuri.org/IService1/TraerUsuarioxIntermediarioResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Usuario[]> TraerUsuarioxIntermediarioAsync(int CodigoTipoAgente, int CodigoAgente);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerActividadEconomica", ReplyAction="http://tempuri.org/IService1/TraerActividadEconomicaResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.ActividadEconomica[]> TraerActividadEconomicaAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoAsegurado", ReplyAction="http://tempuri.org/IService1/TraerTipoAseguradoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoAsegurado[]> TraerTipoAseguradoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerCategoriaAsegurado", ReplyAction="http://tempuri.org/IService1/TraerCategoriaAseguradoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.CategoriaAsegurado[]> TraerCategoriaAseguradoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoCliente", ReplyAction="http://tempuri.org/IService1/TraerTipoClienteResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoCliente[]> TraerTipoClienteAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerGiroNegocio", ReplyAction="http://tempuri.org/IService1/TraerGiroNegocioResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.GiroNegocio[]> TraerGiroNegocioAsync(int CodigoRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerGiroNegocioxCodigo", ReplyAction="http://tempuri.org/IService1/TraerGiroNegocioxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.GiroNegocio> TraerGiroNegocioxCodigoAsync(int CodigoRamo, int CodigoGiroNegocio);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerOcupaciones", ReplyAction="http://tempuri.org/IService1/TraerOcupacionesResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Ocupacion[]> TraerOcupacionesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerAgrupadores", ReplyAction="http://tempuri.org/IService1/TraerAgrupadoresResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Agrupador[]> TraerAgrupadoresAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerAgrupadoresxNombre", ReplyAction="http://tempuri.org/IService1/TraerAgrupadoresxNombreResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Agrupador[]> TraerAgrupadoresxNombreAsync(string NombreAgrupador);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerAgrupadoresxSucursal_Nombre", ReplyAction="http://tempuri.org/IService1/TraerAgrupadoresxSucursal_NombreResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Agrupador[]> TraerAgrupadoresxSucursal_NombreAsync(int CodigoSucursal, string NombreAgrupador);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerAgrupadorxCodigo", ReplyAction="http://tempuri.org/IService1/TraerAgrupadorxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Agrupador> TraerAgrupadorxCodigoAsync(int CodigoAgrupador);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoNegocio", ReplyAction="http://tempuri.org/IService1/TraerTipoNegocioResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoNegocio[]> TraerTipoNegocioAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerAgentes", ReplyAction="http://tempuri.org/IService1/TraerAgentesResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Agente[]> TraerAgentesAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerAgentexCodigo", ReplyAction="http://tempuri.org/IService1/TraerAgentexCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Agente> TraerAgentexCodigoAsync(int CodigoTipoAgente, int CodigoAgente);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/ConsultarTipoAgente", ReplyAction="http://tempuri.org/IService1/ConsultarTipoAgenteResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Agente[]> ConsultarTipoAgenteAsync(int CodigoAgente);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerParentesco", ReplyAction="http://tempuri.org/IService1/TraerParentescoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Parentesco[]> TraerParentescoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerEstadoCivil", ReplyAction="http://tempuri.org/IService1/TraerEstadoCivilResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.EstadoCivil[]> TraerEstadoCivilAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoTasa", ReplyAction="http://tempuri.org/IService1/TraerTipoTasaResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoTasa[]> TraerTipoTasaAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoTasaxCodigo", ReplyAction="http://tempuri.org/IService1/TraerTipoTasaxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoTasa> TraerTipoTasaxCodigoAsync(int CodigoTipoTasa);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoVigencia", ReplyAction="http://tempuri.org/IService1/TraerTipoVigenciaResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoVigencia[]> TraerTipoVigenciaAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerTipoVigenciaxCodigo", ReplyAction="http://tempuri.org/IService1/TraerTipoVigenciaxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoVigencia> TraerTipoVigenciaxCodigoAsync(int CodigoTipoVigencia);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerPeriodoFacturacion", ReplyAction="http://tempuri.org/IService1/TraerPeriodoFacturacionResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.PeriodoFacturacion[]> TraerPeriodoFacturacionAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerPeriodoFacturacionxCodigo", ReplyAction="http://tempuri.org/IService1/TraerPeriodoFacturacionxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.PeriodoFacturacion> TraerPeriodoFacturacionxCodigoAsync(int CodigoPeriodoFacturacion);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerCalculoFacturacion", ReplyAction="http://tempuri.org/IService1/TraerCalculoFacturacionResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.CalculoFacturacion[]> TraerCalculoFacturacionAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerCoaseguradoras", ReplyAction="http://tempuri.org/IService1/TraerCoaseguradorasResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Coaseguradora[]> TraerCoaseguradorasAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerCoaseguradoraxCodigo", ReplyAction="http://tempuri.org/IService1/TraerCoaseguradoraxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Coaseguradora> TraerCoaseguradoraxCodigoAsync(int CodigoCoaseguradora);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerCargos", ReplyAction="http://tempuri.org/IService1/TraerCargosResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Cargo[]> TraerCargosAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerCargoxCodigo", ReplyAction="http://tempuri.org/IService1/TraerCargoxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Cargo> TraerCargoxCodigoAsync(int CodigoCargo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerAreas", ReplyAction="http://tempuri.org/IService1/TraerAreasResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Area[]> TraerAreasAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerAreaxCodigo", ReplyAction="http://tempuri.org/IService1/TraerAreaxCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Area> TraerAreaxCodigoAsync(int CodigoArea);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerInformacionFinanciera", ReplyAction="http://tempuri.org/IService1/TraerInformacionFinancieraResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.InformacionFinanciera[]> TraerInformacionFinancieraAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerConducto", ReplyAction="http://tempuri.org/IService1/TraerConductoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Conducto[]> TraerConductoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerConductoPlan", ReplyAction="http://tempuri.org/IService1/TraerConductoPlanResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.ConductoPlan[]> TraerConductoPlanAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerConductoPlanxPlan", ReplyAction="http://tempuri.org/IService1/TraerConductoPlanxPlanResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.ConductoPlan[]> TraerConductoPlanxPlanAsync(int CodigoPlan);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerConductoPlanxConducto", ReplyAction="http://tempuri.org/IService1/TraerConductoPlanxConductoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.ConductoPlan[]> TraerConductoPlanxConductoAsync(int CodigoConducto);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerPlanPago", ReplyAction="http://tempuri.org/IService1/TraerPlanPagoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.PlanPago[]> TraerPlanPagoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerFormaPago", ReplyAction="http://tempuri.org/IService1/TraerFormaPagoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.FormaPago[]> TraerFormaPagoAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/UsuarioSoligesproXCodigo", ReplyAction="http://tempuri.org/IService1/UsuarioSoligesproXCodigoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.UsuarioSoligespro> UsuarioSoligesproXCodigoAsync(string Usuario);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/ConsultarConductoPago", ReplyAction="http://tempuri.org/IService1/ConsultarConductoPagoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.ConductoPago[]> ConsultarConductoPagoAsync(int CodigoAsegurado);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/ConsultarPlanpago", ReplyAction="http://tempuri.org/IService1/ConsultarPlanpagoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.PlanPago[]> ConsultarPlanpagoAsync(int CodigoRamo, int CodigoAsegurado, int CodigoTipoPoliza, int CodigoConducto);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/TraerBancos", ReplyAction="http://tempuri.org/IService1/TraerBancosResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.Banco[]> TraerBancosAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/ConsultarTipoEmpresa", ReplyAction="http://tempuri.org/IService1/ConsultarTipoEmpresaResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoEmpresa[]> ConsultarTipoEmpresaAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/ConsultarRamoImpuestoXRamo", ReplyAction="http://tempuri.org/IService1/ConsultarRamoImpuestoXRamoResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.RamoImpuesto> ConsultarRamoImpuestoXRamoAsync(int pCodigoRamo);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IService1/ConsultarRamosImpuestos", ReplyAction="http://tempuri.org/IService1/ConsultarRamosImpuestosResponse")]
        System.Threading.Tasks.Task<ParametrizacionServiceReference.RamoImpuesto[]> ConsultarRamosImpuestosAsync();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public interface IService1Channel : ParametrizacionServiceReference.IService1, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("dotnet-svcutil", "1.0.0.1")]
    public partial class Service1Client : System.ServiceModel.ClientBase<ParametrizacionServiceReference.IService1>, ParametrizacionServiceReference.IService1
    {
        
    /// <summary>
    /// Implemente este método parcial para configurar el punto de conexión de servicio.
    /// </summary>
    /// <param name="serviceEndpoint">El punto de conexión para configurar</param>
    /// <param name="clientCredentials">Credenciales de cliente</param>
    static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);
        
        public Service1Client() : 
                base(Service1Client.GetDefaultBinding(), Service1Client.GetDefaultEndpointAddress())
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IService1.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public Service1Client(EndpointConfiguration endpointConfiguration) : 
                base(Service1Client.GetBindingForEndpoint(endpointConfiguration), Service1Client.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public Service1Client(EndpointConfiguration endpointConfiguration, string remoteAddress) : 
                base(Service1Client.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public Service1Client(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(Service1Client.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }
        
        public Service1Client(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress)
        {
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Sucursal[]> TraerSucursalesAsync()
        {
            return base.Channel.TraerSucursalesAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Sucursal> TraerSucursalAsync(int CodigoSucursal)
        {
            return base.Channel.TraerSucursalAsync(CodigoSucursal);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Sucursal[]> TraerSucursalesxAgenteAsync(int CodigoTipoAgente, int CodigoAgente)
        {
            return base.Channel.TraerSucursalesxAgenteAsync(CodigoTipoAgente, CodigoAgente);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Sucursal[]> TraerSucursalesxUsuarioAsync(string CodigoUsuario)
        {
            return base.Channel.TraerSucursalesxUsuarioAsync(CodigoUsuario);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.MacroRamo[]> TraerMacroRamosAsync()
        {
            return base.Channel.TraerMacroRamosAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.MacroRamo> TraerMacroRamoAsync(int CodigoMacroRamo)
        {
            return base.Channel.TraerMacroRamoAsync(CodigoMacroRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.MacroRamo> TraerMacroRamoxRamoAsync(int CodigoMacroRamo)
        {
            return base.Channel.TraerMacroRamoxRamoAsync(CodigoMacroRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo[]> TraerRamosAsync()
        {
            return base.Channel.TraerRamosAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo[]> TraerRamosxMacroRamoAsync(int CodigoMacroRamo)
        {
            return base.Channel.TraerRamosxMacroRamoAsync(CodigoMacroRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo> TraerRamoAsync(int CodigoRamo)
        {
            return base.Channel.TraerRamoAsync(CodigoRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo[]> TraerRamosxSucursalAsync(int CodigoSucursal)
        {
            return base.Channel.TraerRamosxSucursalAsync(CodigoSucursal);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Ramo> TraerRamoxSucursalAsync(int CodigoSucursal, int CodigoRamo)
        {
            return base.Channel.TraerRamoxSucursalAsync(CodigoSucursal, CodigoRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoPoliza[]> TraerTipoPolizaAsync(int CodigoRamo)
        {
            return base.Channel.TraerTipoPolizaAsync(CodigoRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.SubRamo[]> TraerSubRamosxRamoAsync(int CodigoRamo)
        {
            return base.Channel.TraerSubRamosxRamoAsync(CodigoRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.SubRamo> TraerSubRamoAsync(int CodigoRamo, int CodigoSubRamo)
        {
            return base.Channel.TraerSubRamoAsync(CodigoRamo, CodigoSubRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Deducible[]> TraerDeducibleAsync(int CodigoRamo, int CodigoSubRamo, int CodigoTarifa)
        {
            return base.Channel.TraerDeducibleAsync(CodigoRamo, CodigoSubRamo, CodigoTarifa);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Pais[]> TraerPaisesAsync()
        {
            return base.Channel.TraerPaisesAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Departamento[]> TraerDepartamentosAsync()
        {
            return base.Channel.TraerDepartamentosAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Departamento> TraerDepartamentoAsync(int CodigoDepartamento)
        {
            return base.Channel.TraerDepartamentoAsync(CodigoDepartamento);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Municipio[]> TraerMunicipiosxDepartamentoAsync(int CodigoDepartamento)
        {
            return base.Channel.TraerMunicipiosxDepartamentoAsync(CodigoDepartamento);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Municipio> TraerMunicipioAsync(int CodigoDepartamento, int CodigoMunicipio)
        {
            return base.Channel.TraerMunicipioAsync(CodigoDepartamento, CodigoMunicipio);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoDocumento[]> TraerTipoDocumentoAsync()
        {
            return base.Channel.TraerTipoDocumentoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoTelefono[]> TraerTipoTelefonoAsync()
        {
            return base.Channel.TraerTipoTelefonoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoTelefono> TraerTipoTelefonoxCodigoAsync(int CodigoTipoTelefono)
        {
            return base.Channel.TraerTipoTelefonoxCodigoAsync(CodigoTipoTelefono);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoDireccion[]> TraerTipoDireccionAsync()
        {
            return base.Channel.TraerTipoDireccionAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoDireccion> TraerTipoDireccionxCodigoAsync(int CodigoTipoDireccion)
        {
            return base.Channel.TraerTipoDireccionxCodigoAsync(CodigoTipoDireccion);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoAgente[]> TraerTipoAgenteAsync()
        {
            return base.Channel.TraerTipoAgenteAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoAgente> TraerTipoAgentexCodigoAsync(int CodigoTipoAgente)
        {
            return base.Channel.TraerTipoAgentexCodigoAsync(CodigoTipoAgente);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Moneda[]> TraerMonedaAsync()
        {
            return base.Channel.TraerMonedaAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoMovimiento[]> TraerTipoMovimientoAsync()
        {
            return base.Channel.TraerTipoMovimientoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.GrupoEndoso[]> TraerGrupoEndosoAsync()
        {
            return base.Channel.TraerGrupoEndosoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoEndoso[]> TraerTipoEndosoAsync(int CodigoGrupoEndoso)
        {
            return base.Channel.TraerTipoEndosoAsync(CodigoGrupoEndoso);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoEndoso> TraerTipoEndosoxCodigoAsync(int CodigoGrupoEndoso, int CodigoTipoEndoso)
        {
            return base.Channel.TraerTipoEndosoxCodigoAsync(CodigoGrupoEndoso, CodigoTipoEndoso);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Convenio[]> TraerConveniosAsync()
        {
            return base.Channel.TraerConveniosAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Convenio> TraerConvenioxCodigoAsync(int CodigoConvenio)
        {
            return base.Channel.TraerConvenioxCodigoAsync(CodigoConvenio);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta[]> TraerPuntoVentaAsync(int CodigoSucursal)
        {
            return base.Channel.TraerPuntoVentaAsync(CodigoSucursal);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta> TraerPuntoVentaxCodigoAsync(int CodigoSucursal, int CodigoPuntoVenta)
        {
            return base.Channel.TraerPuntoVentaxCodigoAsync(CodigoSucursal, CodigoPuntoVenta);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta[]> TraerPuntoVentaxUsuarioAsync(string CodigoUsuario, int CodigoSucursal)
        {
            return base.Channel.TraerPuntoVentaxUsuarioAsync(CodigoUsuario, CodigoSucursal);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta[]> TraerPuntoVentaxSucursalxUsuarioAsync(int CodigoSucursal, string CodigoUsuario)
        {
            return base.Channel.TraerPuntoVentaxSucursalxUsuarioAsync(CodigoSucursal, CodigoUsuario);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.PuntoVenta[]> TraerPuntoVentaxAgenteAsync(int CodigoTipoAgente, int CodigoAgente)
        {
            return base.Channel.TraerPuntoVentaxAgenteAsync(CodigoTipoAgente, CodigoAgente);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.OficinaConvenio[]> TraerOficinasConvenioAsync(int CodigoConvenio)
        {
            return base.Channel.TraerOficinasConvenioAsync(CodigoConvenio);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.OficinaConvenio> TraerOficinasConvenioxCodigoAsync(int CodigoConvenio, int CodigoPuntoVenta)
        {
            return base.Channel.TraerOficinasConvenioxCodigoAsync(CodigoConvenio, CodigoPuntoVenta);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Usuario> TraerUsuarioxCodigoAsync(string CodigoUsuario)
        {
            return base.Channel.TraerUsuarioxCodigoAsync(CodigoUsuario);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Usuario[]> TraerUsuarioxSucursalAsync(int CodigoSucursal)
        {
            return base.Channel.TraerUsuarioxSucursalAsync(CodigoSucursal);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Usuario[]> TraerUsuarioxIntermediarioAsync(int CodigoTipoAgente, int CodigoAgente)
        {
            return base.Channel.TraerUsuarioxIntermediarioAsync(CodigoTipoAgente, CodigoAgente);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.ActividadEconomica[]> TraerActividadEconomicaAsync()
        {
            return base.Channel.TraerActividadEconomicaAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoAsegurado[]> TraerTipoAseguradoAsync()
        {
            return base.Channel.TraerTipoAseguradoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.CategoriaAsegurado[]> TraerCategoriaAseguradoAsync()
        {
            return base.Channel.TraerCategoriaAseguradoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoCliente[]> TraerTipoClienteAsync()
        {
            return base.Channel.TraerTipoClienteAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.GiroNegocio[]> TraerGiroNegocioAsync(int CodigoRamo)
        {
            return base.Channel.TraerGiroNegocioAsync(CodigoRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.GiroNegocio> TraerGiroNegocioxCodigoAsync(int CodigoRamo, int CodigoGiroNegocio)
        {
            return base.Channel.TraerGiroNegocioxCodigoAsync(CodigoRamo, CodigoGiroNegocio);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Ocupacion[]> TraerOcupacionesAsync()
        {
            return base.Channel.TraerOcupacionesAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Agrupador[]> TraerAgrupadoresAsync()
        {
            return base.Channel.TraerAgrupadoresAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Agrupador[]> TraerAgrupadoresxNombreAsync(string NombreAgrupador)
        {
            return base.Channel.TraerAgrupadoresxNombreAsync(NombreAgrupador);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Agrupador[]> TraerAgrupadoresxSucursal_NombreAsync(int CodigoSucursal, string NombreAgrupador)
        {
            return base.Channel.TraerAgrupadoresxSucursal_NombreAsync(CodigoSucursal, NombreAgrupador);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Agrupador> TraerAgrupadorxCodigoAsync(int CodigoAgrupador)
        {
            return base.Channel.TraerAgrupadorxCodigoAsync(CodigoAgrupador);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoNegocio[]> TraerTipoNegocioAsync()
        {
            return base.Channel.TraerTipoNegocioAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Agente[]> TraerAgentesAsync()
        {
            return base.Channel.TraerAgentesAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Agente> TraerAgentexCodigoAsync(int CodigoTipoAgente, int CodigoAgente)
        {
            return base.Channel.TraerAgentexCodigoAsync(CodigoTipoAgente, CodigoAgente);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Agente[]> ConsultarTipoAgenteAsync(int CodigoAgente)
        {
            return base.Channel.ConsultarTipoAgenteAsync(CodigoAgente);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Parentesco[]> TraerParentescoAsync()
        {
            return base.Channel.TraerParentescoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.EstadoCivil[]> TraerEstadoCivilAsync()
        {
            return base.Channel.TraerEstadoCivilAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoTasa[]> TraerTipoTasaAsync()
        {
            return base.Channel.TraerTipoTasaAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoTasa> TraerTipoTasaxCodigoAsync(int CodigoTipoTasa)
        {
            return base.Channel.TraerTipoTasaxCodigoAsync(CodigoTipoTasa);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoVigencia[]> TraerTipoVigenciaAsync()
        {
            return base.Channel.TraerTipoVigenciaAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoVigencia> TraerTipoVigenciaxCodigoAsync(int CodigoTipoVigencia)
        {
            return base.Channel.TraerTipoVigenciaxCodigoAsync(CodigoTipoVigencia);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.PeriodoFacturacion[]> TraerPeriodoFacturacionAsync()
        {
            return base.Channel.TraerPeriodoFacturacionAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.PeriodoFacturacion> TraerPeriodoFacturacionxCodigoAsync(int CodigoPeriodoFacturacion)
        {
            return base.Channel.TraerPeriodoFacturacionxCodigoAsync(CodigoPeriodoFacturacion);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.CalculoFacturacion[]> TraerCalculoFacturacionAsync()
        {
            return base.Channel.TraerCalculoFacturacionAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Coaseguradora[]> TraerCoaseguradorasAsync()
        {
            return base.Channel.TraerCoaseguradorasAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Coaseguradora> TraerCoaseguradoraxCodigoAsync(int CodigoCoaseguradora)
        {
            return base.Channel.TraerCoaseguradoraxCodigoAsync(CodigoCoaseguradora);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Cargo[]> TraerCargosAsync()
        {
            return base.Channel.TraerCargosAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Cargo> TraerCargoxCodigoAsync(int CodigoCargo)
        {
            return base.Channel.TraerCargoxCodigoAsync(CodigoCargo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Area[]> TraerAreasAsync()
        {
            return base.Channel.TraerAreasAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Area> TraerAreaxCodigoAsync(int CodigoArea)
        {
            return base.Channel.TraerAreaxCodigoAsync(CodigoArea);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.InformacionFinanciera[]> TraerInformacionFinancieraAsync()
        {
            return base.Channel.TraerInformacionFinancieraAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Conducto[]> TraerConductoAsync()
        {
            return base.Channel.TraerConductoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.ConductoPlan[]> TraerConductoPlanAsync()
        {
            return base.Channel.TraerConductoPlanAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.ConductoPlan[]> TraerConductoPlanxPlanAsync(int CodigoPlan)
        {
            return base.Channel.TraerConductoPlanxPlanAsync(CodigoPlan);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.ConductoPlan[]> TraerConductoPlanxConductoAsync(int CodigoConducto)
        {
            return base.Channel.TraerConductoPlanxConductoAsync(CodigoConducto);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.PlanPago[]> TraerPlanPagoAsync()
        {
            return base.Channel.TraerPlanPagoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.FormaPago[]> TraerFormaPagoAsync()
        {
            return base.Channel.TraerFormaPagoAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.UsuarioSoligespro> UsuarioSoligesproXCodigoAsync(string Usuario)
        {
            return base.Channel.UsuarioSoligesproXCodigoAsync(Usuario);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.ConductoPago[]> ConsultarConductoPagoAsync(int CodigoAsegurado)
        {
            return base.Channel.ConsultarConductoPagoAsync(CodigoAsegurado);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.PlanPago[]> ConsultarPlanpagoAsync(int CodigoRamo, int CodigoAsegurado, int CodigoTipoPoliza, int CodigoConducto)
        {
            return base.Channel.ConsultarPlanpagoAsync(CodigoRamo, CodigoAsegurado, CodigoTipoPoliza, CodigoConducto);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.Banco[]> TraerBancosAsync()
        {
            return base.Channel.TraerBancosAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.TipoEmpresa[]> ConsultarTipoEmpresaAsync()
        {
            return base.Channel.ConsultarTipoEmpresaAsync();
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.RamoImpuesto> ConsultarRamoImpuestoXRamoAsync(int pCodigoRamo)
        {
            return base.Channel.ConsultarRamoImpuestoXRamoAsync(pCodigoRamo);
        }
        
        public System.Threading.Tasks.Task<ParametrizacionServiceReference.RamoImpuesto[]> ConsultarRamosImpuestosAsync()
        {
            return base.Channel.ConsultarRamosImpuestosAsync();
        }
        
        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }
        
        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }
        
        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IService1))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            throw new System.InvalidOperationException(string.Format("No se pudo encontrar un punto de conexión con el nombre \"{0}\".", endpointConfiguration));
        }
        
        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IService1))
            {
                return new System.ServiceModel.EndpointAddress("http://192.1.2.13:8088/WCFTablasParametrizacion/Service1.svc");
            }
            throw new System.InvalidOperationException(string.Format("No se pudo encontrar un punto de conexión con el nombre \"{0}\".", endpointConfiguration));
        }
        
        private static System.ServiceModel.Channels.Binding GetDefaultBinding()
        {
            return Service1Client.GetBindingForEndpoint(EndpointConfiguration.BasicHttpBinding_IService1);
        }
        
        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress()
        {
            return Service1Client.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_IService1);
        }
        
        public enum EndpointConfiguration
        {
            
            BasicHttpBinding_IService1,
        }
    }
}
