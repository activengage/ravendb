using System.ComponentModel.Composition.Primitives;

namespace Raven35.Database.Plugins.Catalogs
{
    public class BuiltinFilteringCatalog : FilteredCatalog
    {
        public BuiltinFilteringCatalog(ComposablePartCatalog catalogToFilter) : base(catalogToFilter)
        {
        }

        protected override bool IsMatch(ComposablePartDefinition composablePartDefinition)
        {
            var element = composablePartDefinition as ICompositionElement;
            if (element == null)
                return true;
            return element.DisplayName.StartsWith("Raven35.Database") == false;
        }
    }
}	
