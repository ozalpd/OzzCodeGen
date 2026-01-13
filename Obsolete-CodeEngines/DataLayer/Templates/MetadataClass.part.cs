using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.CodeEngines.DataLayer.Templates
{
    public partial class MetadataClass : BaseDalTemplate
    {
        protected List<string> GetPropertyAttributes(DalPropertySetting property)
        {
            List<string> attributes = new List<string>();
            var entity = (DalEntitySetting)property.EntitySetting;
            if (property.PropertyDefinition is SimpleProperty)
            {
                SimpleProperty definition = (SimpleProperty)property.PropertyDefinition;
                if (definition.IsKey) attributes.Add("[Key]");
            }
            if (property.PropertyDefinition is StringProperty && ((StringProperty)property.PropertyDefinition).MaxLenght > 0)
            {
                StringProperty definition = (StringProperty)property.PropertyDefinition;

                if (entity.CodeEngine.ResxEngine != null)
                {
                    attributes.Add(string.Format("[StringLength({0}, ErrorMessageResourceType = typeof(Resources.AppMessageString), ErrorMessageResourceName = \"MaxLeght\")]",
                        definition.MaxLenght));
                }
                else
                {
                    attributes.Add(string.Format("[StringLength({0})]", definition.MaxLenght));
                }
            }

            if (!string.IsNullOrEmpty(property.UIHint))
                attributes.Add("[UIHint(\"" + property.UIHint + "\")]");

            if (!string.IsNullOrEmpty(property.RegularExpression))
                attributes.Add("[RegularExpression(" + property.RegularExpression + ")]");

            if (!string.IsNullOrEmpty(property.Range))
                attributes.Add("[Range(" + property.Range + ")]");

            if (!string.IsNullOrEmpty(property.Required))
                attributes.Add("[Required(" + property.Required + ")]");

            if (!string.IsNullOrEmpty(property.DataType))
                attributes.Add("[DataType(DataType." + property.DataType + ")]");

            string displayAttrib = entity.CodeEngine.UseDisplayName ? property.GetDisplayAttrib() : string.Empty;
            if (!string.IsNullOrEmpty(displayAttrib))
            {
                attributes.Add(displayAttrib);
            }
            return attributes;
        }

        public override string GetDefaultFileName()
        {
            return EntitySetting.Name + ".meta.cs";
        }

    }
}
