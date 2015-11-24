using OzzCodeGen.Definitions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcJQuerySelectList
    {
        public MvcJQuerySelectList(AspNetMvcEntitySetting entity, AspNetMvcPropertySetting fKeyProperty)
            : base(entity)
        {
            CodeEngine = entity.CodeEngine;
            FKeyProperty = fKeyProperty;
        }

        public AspNetMvcEngine CodeEngine { get; private set; }
        public AspNetMvcPropertySetting FKeyProperty { get; private set; }

        public AspNetMvcEntitySetting RelatedEntity { get; set; }

        public IEnumerable<ComplexProperty> ForeignProperties
        {
            get
            {
                if (_foreignProperties == null && RelatedEntity != null)
                {
                    _foreignProperties = from p in RelatedEntity.Properties
                                         where p.PropertyDefinition.TypeName.Equals(Entity.Name) && p.PropertyDefinition is ComplexProperty
                                         select p.PropertyDefinition as ComplexProperty;

                            //.Properties
                            //.Where(p => p.PropertyDefinition.TypeName.Equals(Entity.Name) &&
                            //        p.PropertyDefinition is ComplexProperty);
                }
                return _foreignProperties;
            }
        }
        IEnumerable<ComplexProperty> _foreignProperties;

        public string GetRelatedEntityFKeyName()
        {
            if (ForeignProperties == null || ForeignProperties.Count() == 0)
            {
                return Entity.Name + "Id";
            }
            else
            {
                //TODO: Process other foreign keys
                return ForeignProperties.FirstOrDefault().DependentPropertyName;
            }
        }

        public override string GetDefaultFileName()
        {
            return "_Select" + Entity.Name + "By" + FKeyProperty.Name + ".cshtml";
        }

        public override string GetDefaultFilePath()
        {
            var folder = RelatedEntity == null ? "IHas" + Entity.Name : RelatedEntity.ControllerName;
            return Path.Combine(CodeEngine.SnippetsDir, folder, GetDefaultFileName());
        }
    }
}
