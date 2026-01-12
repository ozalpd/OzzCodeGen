using System;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;
using OzzCodeGen.Providers;
using System.IO;
using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.CodeEngines;
using System.Collections.Generic;
using OzzUtils;

namespace OzzCodeGen.UI
{
    public partial class EmptyModelDialog : Form
    {
        public EmptyModelDialog()
        {
            InitializeComponent();
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ModelSource { get; set; }
        
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public EmptyModel Provider { get; set; }

        public DataModel GetDataModel()
        {
            var model = new DataModel();
            return model;
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<ModelTemplate> ModelTemplates
        {
            get { return _modelTemplates; }
            set
            {
                _modelTemplates = value;
                cboTemplates.Items.AddRange(ModelTemplates.ToArray());
            }
        }
        private List<ModelTemplate> _modelTemplates;

        public ModelTemplate SelectedTemplate { get; private set; }

        public CodeGenProject GetProject()
        {
            var templatePath = SelectedTemplate != null ? SelectedTemplate.Path : string.Empty;
            CodeGenProject project = null;
            if (File.Exists(templatePath))
            {
                project = CodeGenProject.OpenFile(templatePath);
                if(project.CodeEngineList.Any(c=>c.Equals(EngineTypes.LocalizationResxGenId)))
                {
                    var localizationEngine = (ResxEngine)project.GetCodeEngine(EngineTypes.LocalizationResxGenId);
                    localizationEngine.OpenVocabularies();
                    localizationEngine.SaveWithVocabularies = true;
                }
            }
            else
            {
                project = new CodeGenProject()
                {
                    DataModel = GetDataModel()
                };
            }

            project.Name = txtProjectName.Text.Trim();
            project.NamespaceName = txtNamespace.Text.Trim();
            project.ModelProviderId = Provider.ProviderId;
            project.ModelSource = ModelSource;
            project.SavedFileName = string.Empty;
            foreach(var item in project.DataModel)
            {
                item.NamespaceName = project.NamespaceName;
            }

            return project;
        }

        private void txtProjectName_Leave(object sender, EventArgs e)
        {
            if (txtNamespace.Text.Trim().Length == 0)
            {
                txtNamespace.Text = txtProjectName.Text
                                    .Trim()
                                    .Replace('-', '.')
                                    .ToPascalCase();
            }
        }

        private void cboTemplates_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedTemplate = (ModelTemplate)cboTemplates.SelectedItem;
        }
    }
}
