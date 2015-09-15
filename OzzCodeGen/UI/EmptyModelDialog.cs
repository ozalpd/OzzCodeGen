using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OzzCodeGen.Definitions;
using OzzCodeGen.Providers;
using System.IO;
using OzzCodeGen.CodeEngines.Localization;
using OzzCodeGen.CodeEngines;

namespace OzzCodeGen.UI
{
    public partial class EmptyModelDialog : Form
    {
        public EmptyModelDialog()
        {
            InitializeComponent();
        }

        public string ModelSource { get; set; }
        public EmptyModel Provider { get; set; }

        public EntityDefinition GetEntityDefinition()
        {
            var entity = Provider.GetDefaultEntityDefinition();
            entity.Name = txtDefaultEntityName.Text.Trim();



            return entity;
        }

        public DataModel GetDataModel()
        {
            var model = new DataModel();
            if (chkCreateDefaultEntity.Checked)
            {
                var entity = Provider.GetDefaultEntityDefinition();
                entity.Name = txtDefaultEntityName.Text.Trim();
                entity.NamespaceName = txtNamespace.Text.Trim();
                model.Add(entity);
            }

            return model;
        }

        public CodeGenProject GetProject()
        {
            //TODO: Put a ComboBox (eg cboProjectTemplateFiles) in UI
            //      and use selected ProjectTemplateFile instead of EmptyModel.ProjectTemplateFile
            var templatePath = Path.Combine(Provider.DefaultsFolder, EmptyModel.ProjectTemplateFile);
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
                                    .Replace(' ', '.')
                                    .Replace('-', '.');
            }
        }

        private void chkCreateDefaultEntity_CheckedChanged(object sender, EventArgs e)
        {
            txtDefaultEntityName.Enabled = chkCreateDefaultEntity.Checked;
        }
    }
}
