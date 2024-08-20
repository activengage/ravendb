using System.Threading.Tasks;
using Raven35.Abstractions.Data;

namespace Raven35.Abstractions.Smuggler
{
    public interface ISmugglerApi<TIn, out TOptions, TOut>
        where TIn : ConnectionStringOptions, new()
        where TOptions : SmugglerOptions<TIn>
    {
        TOptions Options { get; }

        Task<TOut> ExportData(SmugglerExportOptions<TIn> exportOptions);

        Task ImportData(SmugglerImportOptions<TIn> importOptions);

        Task Between(SmugglerBetweenOptions<TIn> betweenOptions);
    }
}
