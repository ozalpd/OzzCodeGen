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

namespace OzzCodeGen.UI
{
    public partial class EmptyModelDialog : Form
    {
        public EmptyModelDialog()
        {
            InitializeComponent();
        }

        public string ModelSource { get; set; }
        public IModelProvider Provider { get; set; }

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
            var project = new CodeGenProject()
            {
                Name = txtProjectName.Text.Trim(),
                NamespaceName = txtNamespace.Text.Trim(),
                ModelProviderId = Provider.ProviderId,
                ModelSource = ModelSource,
                DataModel = GetDataModel()
            };

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
