using System.ComponentModel.Composition;

namespace Raven35.Database.Plugins
{
    [InheritedExport]
    public interface ISilverlightRequestedAware
    {
        void SilverlightWasRequested(DocumentDatabase database);
    }
}
