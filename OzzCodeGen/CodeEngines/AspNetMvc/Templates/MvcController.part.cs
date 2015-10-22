using OzzCodeGen.CodeEngines.Localization;
using OzzUtils;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public partial class MvcController : AbstractMvcController
    {
        public MvcController(AspNetMvcEntitySetting entity, bool customFile = false)
            : base(entity, customFile) { }

        public override string GetDefaultFileName()
        {
            if (CustomFile)
            {
                return Entity.ControllerName + "Controller.part.cs";
            }
            else
            {
                return Entity.ControllerName + "Controller.g.cs";
            }
        }

        public LocalizationEntitySetting ResxEntity
        {
            get
            {
                if (Resx != null && _resxEntity == null)
                {
                    _resxEntity = Resx
                                    .Entities
                                    .FirstOrDefault(e => e.Name.Equals(Entity.Name));
                }
                return _resxEntity;
            }
        }
        LocalizationEntitySetting _resxEntity;

        public string EntityResource
        {
            get
            {
                if (Resx != null)
                {
                    return Resx.SingleResx ? Resx.SingleResxFilename :
                        ResxEntity != null ? Resx.GetDefaultTargetFile(ResxEntity) : string.Empty;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public override bool WriteToFile(string filePath, bool overwriteExisting)
        {
            if (!CustomFile)
            {
                var customFile = new MvcController(Entity, true);
                string customPath = Path.Combine(Path.GetDirectoryName(filePath), customFile.GetDefaultFileName());
                customFile.WriteToFile(customPath, false);
            }
            return base.WriteToFile(filePath, overwriteExisting);
        }
    }
}
