using OzzCodeGen.CodeEngines.Mvvm;
using OzzCodeGen.Definitions;
using System.Collections.Generic;
using System.Linq;

namespace OzzCodeGen.CodeEngines.WpfMvvm;

public class WpfMvvmEntitySetting : BaseMvvmEntitySetting<WpfMvvmPropertySetting>
{
    public override AbstractEntitySetting<WpfMvvmPropertySetting> GetBaseEntitySetting()
    {
        if (string.IsNullOrEmpty(EntityDefinition.BaseTypeName))
            return null;

        return (CodeEngine as WpfMvvmCodeEngine)?
            .Entities
            .FirstOrDefault(e => e.EntityDefinition.Name.Equals(EntityDefinition.BaseTypeName));
    }

    public List<WpfMvvmPropertySetting> GetPreselectProperties()
    {
        return GetInheritedIncludedProperties().Where(p => p.IsPreselectedInCreate)
                                               .OrderBy(e => e.Name)
                                               .ToList();
    }

    public IEnumerable<WpfMvvmEntitySetting> GetForeignLookupEntities(bool isForEdit = false)
    {
        if (isForEdit && _foreignLookupForEdit != null)
            return _foreignLookupForEdit;

        if (!isForEdit && _foreignLookupEntities != null)
            return _foreignLookupEntities;

        var complexproperties = Properties.Where(p => p.PropertyDefinition is ComplexProperty);
        var complexTypeNames = complexproperties.Select(p => p.GetTypeName(getReturnType: true))
                                                .Distinct()
                                                .ToList();
        var complexTypeNamesForEdit = new List<string>();
        foreach (var item in complexproperties)
        {
            string fkeyName = item.GetForeignKeyName();
            var fkeyProp = Properties.FirstOrDefault(p => p.Name.Equals(fkeyName));
            string typeName = fkeyProp != null && !fkeyProp.IsImmutable ? item.GetTypeName(getReturnType: true) : null;
            if (typeName != null && !complexTypeNamesForEdit.Contains(typeName))
            {
                complexTypeNamesForEdit.Add(typeName);
            }
        }


        _foreignLookupEntities = CodeEngine.EntitySettings.OfType<WpfMvvmEntitySetting>()
                                           .Where(e => e.GenerateLookupService
                                                    && complexTypeNames.Contains(e.EntityDefinition.Name))
                                           .OrderBy(e => e.Name)
                                           .ToList();
        _foreignLookupForEdit = CodeEngine.EntitySettings.OfType<WpfMvvmEntitySetting>()
                                          .Where(e => e.GenerateLookupService
                                                   && complexTypeNamesForEdit.Contains(e.EntityDefinition.Name))
                                          .OrderBy(e => e.Name)
                                          .ToList();

        return _foreignLookupEntities;
    }
    private List<WpfMvvmEntitySetting> _foreignLookupEntities;
    private List<WpfMvvmEntitySetting> _foreignLookupForEdit;
}
