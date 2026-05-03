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
                                               .OrderBy(p => p.Name)
                                               .ToList();
    }

    public IEnumerable<WpfMvvmEntitySetting> GetForeignLookupEntities(bool isForEdit = false)
    {
        var complexproperties = Properties.Where(p => p.PropertyDefinition is ComplexProperty);
        var complexTypeNames = complexproperties.Select(p => p.GetTypeName(getReturnType: true))
                                                .Distinct()
                                                .ToList();
        if (isForEdit)
        {
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

            return CodeEngine.EntitySettings.OfType<WpfMvvmEntitySetting>()
                                            .Where(e => e.GenModeLookupService > FileGenerationMode.DoNotGenerate 
                                                     && complexTypeNamesForEdit.Contains(e.EntityDefinition.Name))
                                            .OrderBy(e => e.Name)
                                            .ToList();
        }

        return CodeEngine.EntitySettings.OfType<WpfMvvmEntitySetting>()
                                        .Where(e => e.GenModeLookupService > FileGenerationMode.DoNotGenerate
                                                 && complexTypeNames.Contains(e.EntityDefinition.Name))
                                        .OrderBy(e => e.Name)
                                        .ToList();
    }
}
