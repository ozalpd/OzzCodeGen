using OzzCodeGen.CodeEngines.Mvvm;
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
}
