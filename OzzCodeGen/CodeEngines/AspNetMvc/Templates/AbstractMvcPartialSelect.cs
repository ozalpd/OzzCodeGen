using OzzCodeGen.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public abstract class AbstractMvcPartialSelect : AbstractMvcView
    {
        public AbstractMvcPartialSelect(AspNetMvcEntitySetting entity, AspNetMvcPropertySetting fKeyProperty)
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
                return ForeignProperties.FirstOrDefault().DependentPropertyName;
            }
        }

        public string GetRelatedEntityDisplayMemberName()
        {
            if (RelatedEntity == null)
            {
                return Entity.Name;
            }
            else
            {
                return RelatedEntity.EntityDefinition.DisplayMember;
            }
        }
    }
}
