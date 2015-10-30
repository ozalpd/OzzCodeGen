using OzzCodeGen.CodeEngines.AspNetMvc.Templates;
using System.Collections.Generic;
using System.IO;
using System.Linq;

//Render related methods here
namespace OzzCodeGen.CodeEngines.AspNetMvc
{
    public partial class AspNetMvcEngine
    {
        public bool RenderTemplate(string templateName, bool multiTemplates)
        {
            bool rendered = true;
            if (RenderAllEntities)
            {
                foreach (var entity in Entities.Where(e => !e.Exclude))
                {
                    rendered = RenderTemplate(entity, templateName, true) & rendered;
                }
            }
            else if (CurrentEntitySetting == null)
            {
                return false;
            }
            else
            {
                rendered = RenderTemplate((AspNetMvcEntitySetting)CurrentEntitySetting, templateName, multiTemplates) & rendered;
            }

            if (templateName.Equals(controllerClass))
            {
                rendered = RenderBaseController() & rendered;
            }

            return rendered;
        }

        public bool RenderTemplate(AspNetMvcEntitySetting entity, string templateName, bool multiTemplates)
        {
            bool notValid = (!entity.GenerateController && templateName.Equals(controllerClass)) ||
                            (!entity.IndexView && templateName.Equals(mvcIndexView)) ||
                            (!entity.DetailsView && templateName.Equals(mvcDetailsView)) ||
                            (!entity.CreateView && templateName.Equals(mvcCreateView)) ||
                            (!entity.EditView && templateName.Equals(mvcEditView));
            if (notValid)
            {
                if (!multiTemplates)
                {
                    System.Windows.Forms.MessageBox.Show(templateName + " is not valid.");
                }
                return false;
            }

            if (entity.Exclude &&
                System.Windows.Forms.MessageBox.Show(
                "Entity is excluded! Do you still want to generate it's file(s)?\r\nTemplate: " + templateName,
                "Entity is excluded!", System.Windows.Forms.MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.No)
            {
                return false;
            }
            var template = GetTemplateFile(entity, templateName);

            string fileName;
            if (template is MvcController)
            {
                fileName = ((MvcController)template).GetDefaultFilePath();
            }
            else if (template is AbstractMvcView)
            {
                fileName = ((AbstractMvcView)template).GetDefaultFilePath();
            }
            else
            {
                fileName = string.Empty;
            }

            return template.WriteToFile(fileName, OverwriteExisting || entity.OverwriteExisting);
        }

        public override bool RenderSelectedTemplate()
        {
            if (SelectedTemplate.Equals(mvcAllViews))
            {
                var templates = GetViewTemplates();
                return RenderTemplates(templates);
            }
            else
            {
                return RenderTemplate(SelectedTemplate, false);
            }
        }

        public override bool RenderAllTemplates()
        {
            var templates = GetViewTemplates();
            templates.Add(controllerClass);

            return RenderTemplates(templates);
        }

        private bool RenderTemplates(List<string> templates)
        {
            bool rendered = true;
            foreach (var template in templates)
            {
                rendered = RenderTemplate(template, true);
            }
            return rendered;
        }

        public bool RenderSecurityRoles()
        {
            var rolesTmpl = new MvcSecurityRoles(this);
            return rolesTmpl.WriteToFile(rolesTmpl.GetDefaultFilePath(), true);
        }

        public bool RenderBaseController()
        {
            var baseCtrlTmpl = new MvcBaseController(this);
            return baseCtrlTmpl.WriteToFile(baseCtrlTmpl.GetDefaultFilePath(), true);
        }

        private List<string> GetViewTemplates()
        {
            return new List<string>()
            {
                mvcIndexView,
                mvcDetailsView,
                mvcCreateView,
                mvcEditView,
            };
        }

        public override List<string> GetTemplateList()
        {
            return new List<string>()
            {
                controllerClass,
                mvcAllViews,
                mvcIndexView,
                mvcDetailsView,
                mvcCreateView,
                mvcEditView,
            };
        }
        private const string controllerClass = "Controller for ASP.NET MVC";
        private const string mvcAllViews = "All Views for ASP.NET MVC";
        private const string mvcIndexView = "Index View for ASP.NET MVC";
        private const string mvcDetailsView = "Details View for ASP.NET MVC";
        private const string mvcEditView = "Edit View for ASP.NET MVC";
        private const string mvcCreateView = "Create View for ASP.NET MVC";

        protected AbstractTemplate GetTemplateFile(AspNetMvcEntitySetting entity, string templateName)
        {
            AbstractTemplate tmp = null;
            switch (templateName)
            {
                case controllerClass:
                    tmp = new MvcController(entity);
                    break;

                case mvcIndexView:
                    tmp = new MvcIndexView(entity);
                    break;

                case mvcDetailsView:
                    tmp = new MvcDetailsView(entity);
                    break;

                case mvcEditView:
                    tmp = new MvcEditView(entity);
                    break;

                case mvcCreateView:
                    tmp = new MvcEditView(entity, true);
                    break;

                case mvcAllViews:
                    tmp = null;
                    break;

                default:
                    break;
            }

            return tmp;
        }


        public override string GetDefaultTargetFolder()
        {
            return Project != null ? Project.Name : "MVC Files";
        }
    }
}
