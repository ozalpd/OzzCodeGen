using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Definitions;
using OzzCodeGen.UI;

namespace OzzCodeGen.Providers
{
    public class EmptyModel : IModelProvider
    {
        public string ProviderId
        {
            get { return "Model_Provider_Empty"; }
        }

        public override string ToString()
        {
            return "Empty Model Provider";
        }

        public DataModel GetDataModel(string modelSource)
        {
            return ModelDialog.GetDataModel();
        }

        public CodeGenProject GenerateProject(string modelSource)
        {
            var project = ModelDialog.GetProject();
            project.ModelProvider = this;
            return project;
        }

        public DataModel RefreshDataModel(string modelSource, DataModel model, bool cleanRemovedItems)
        {
            throw new NotImplementedException();
        }

        public bool CanRefresh
        {
            get { return false; }
        }

        public CodeGenProject Project { get; set; }

        protected EmptyModelDialog ModelDialog
        {
            get
            {
                if (_modelDialog == null)
                {
                    _modelDialog = new EmptyModelDialog();
                    _modelDialog.Provider = this;
                    _modelDialog.ModelSource = "EmptyModel";
                }
                return _modelDialog;
            }
        }
        EmptyModelDialog _modelDialog;

        public string SelectSource()
        {
            var result = ModelDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                return ModelDialog.ModelSource;
            }
            else
            {
                return string.Empty;
            }
        }

        public string DefaultsFolder { get; set; }
        public static string ProjectTemplateFile = "DefaultEmptyProvider.OzzGen";

        public EntityDefinition GetDefaultEntityDefinition()
        {
            var entity = EntityDefinition.CreateDefaultEntityDefinition();

            var title = BaseProperty.CreatePropertyDefinition("string", "Title");
            ((StringProperty)title).MaxLenght = 50;
            entity.Properties.Add(title);

            entity.Properties.Add(BaseProperty.CreatePropertyDefinition("int", "ModifyNr", true));
            entity.Properties.Add(BaseProperty.CreatePropertyDefinition("DateTime", "ModifyDate", true));
            entity.Properties.Add(BaseProperty.CreatePropertyDefinition("DateTime", "CreateDate", true));

            var modifierIp = BaseProperty.CreatePropertyDefinition("string", "ModifierIp");
            ((StringProperty)modifierIp).MaxLenght = 50;
            modifierIp.IsServerComputed = true;
            entity.Properties.Add(modifierIp);

            var creatorIp = BaseProperty.CreatePropertyDefinition("string", "CreatorIp");
            ((StringProperty)creatorIp).MaxLenght = 50;
            creatorIp.IsServerComputed = true;
            entity.Properties.Add(creatorIp);

            entity.Abstract = true;
            return entity;
        }
    }
}
