using ModernizacionPersonas.BLL.Entities;
using ModernizacionPersonas.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ModernizacionPersonas.BLL.Contracts
{
    public interface ITextosSlipDataProvider
    {
        Task<IEnumerable<TextoSlipViewModel>> GetTextosSlipAsync();
        Task<ActionResponseBase> UpdateTextoSlipAsync(TextoSlip model);
        Task<ActionResponseBase> CreateTextoSlipAsync(TextoSlip model);
    }
}
