using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OzzCodeGen.Definitions;
using OzzCodeGen.UI;
using System.IO;

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
                    _modelDialog.ModelTemplates = ModelTemplates;
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


        public List<ModelTemplate> ModelTemplates
        {
            set { _modelTemplates = value; }
            get
            {
                if (_modelTemplates == null)
                {
                    _modelTemplates = GetModelTemplates();

                }
                return _modelTemplates;
            }
        }
        private List<ModelTemplate> _modelTemplates;

        private List<ModelTemplate> GetModelTemplates()
        {
            var templates = new List<ModelTemplate>();
            if (Directory.Exists(DefaultsFolder))
            {
                AppendTemplates(templates, DefaultsFolder);
            }
            return templates;
        }

        private void AppendTemplates(List<ModelTemplate> templates, string directory)
        {
            var files = Directory.GetFiles(directory, "*.OzzGen");
            foreach (var item in files)
            {
                templates.Add(new ModelTemplate(item));
            }
            var dirs = Directory.GetDirectories(directory);
            foreach (var item in dirs)
            {
                AppendTemplates(templates, item);
            }
        }
    }
}
