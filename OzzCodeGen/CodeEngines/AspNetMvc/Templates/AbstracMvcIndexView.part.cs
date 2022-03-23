using OzzCodeGen.Definitions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OzzCodeGen.CodeEngines.AspNetMvc.Templates
{
    public abstract partial class AbstracMvcIndexView : AbstractMvcView
    {
        public AbstracMvcIndexView(AspNetMvcEntitySetting entity) : base(entity) { }

        public override string GetDefaultFileName()
        {
            return "Index.cshtml";
        }

        public string ModalDialogPartial
        {
            get
            {
                return Path.GetFileNameWithoutExtension(MvcPartialModalDialog.DefaultFileName);
            }
        }

        protected override List<string> DefaultUsingNamespaceList()
        {
            var list = base.DefaultUsingNamespaceList();
            //if we need controller's namespace
            //var controller = new MvcController(Entity);
            //list.Add(controller.NamespaceName);
            return list;
        }

        public bool HasModalInputForm
        {
            get { return Entity.CreatePartialView || Entity.EditPartialView; }
        }

        public bool HasModalPartialForm
        {
            get { return Entity.DetailsPartialView || HasModalInputForm; }
        }

        protected string GetSharedViewsDir(string filePath)
        {
            return Path.Combine(Path.GetDirectoryName(Path.GetDirectoryName(filePath)), "Shared");
        }

        public override bool WriteToFile(string filePath, bool overwriteExisting)
        {
            string sharedViewsDir = GetSharedViewsDir(filePath);

            string modalFilePath = Path.Combine(sharedViewsDir, MvcPartialModalDialog.DefaultFileName);
            if (!File.Exists(modalFilePath))
            {
                var modalTemplate = new MvcPartialModalDialog(Entity);
                modalTemplate.WriteToFile(modalFilePath, false);
            }

            string pagerFilePath = Path.Combine(sharedViewsDir, MvcPartialPager.DefaultFileName);
            if (!File.Exists(pagerFilePath))
            {
                var pagerTemplate = new MvcPartialPager(Entity);
                pagerTemplate.WriteToFile(pagerFilePath, false);
            }

            string scriptFilePath = Path.Combine(sharedViewsDir, MvcPartialIndexScripts.DefaultFileName);
            if (!File.Exists(scriptFilePath))
            {
                var scriptTemplate = new MvcPartialIndexScripts(Entity);
                scriptTemplate.WriteToFile(scriptFilePath, false);
            }

            string searchboxPath = Path.Combine(sharedViewsDir, MvcPartialSearchBox.DefaultFileName);
            if (!File.Exists(searchboxPath))
            {
                var searchboxTempl = new MvcPartialSearchBox(Entity);
                searchboxTempl.WriteToFile(searchboxPath, false);
            }

            var snippetTempl = new MvcViewSnippets(Entity);
            snippetTempl.WriteToFile(snippetTempl.GetDefaultFilePath(), true);

            var fkeys = Entity
                        .GetInheritedIncludedProperties()
                        .Where(e => (e.InCreateView | e.InEditView) & e.IsForeignKey());

            foreach (var item in Entity.GetInheritedComplexProperties())
            {
                var complex = (ComplexProperty)item.PropertyDefinition;
                var fkey = fkeys.FirstOrDefault(f => f.Name.Equals(complex.DependentPropertyName));
                var fEntity = Entity.CodeEngine.Entities.FirstOrDefault(e => e.Name.Equals(complex.TypeName));
                if (fEntity != null && fkey != null)
                {
                    var autocompl = new MvcJQueryAutoComplete(fEntity, fkey);
                    autocompl.RelatedEntity = Entity;
                    autocompl.WriteToFile(autocompl.GetDefaultFilePath(), true);
                }
            }
            foreach (var fkey in fkeys)
            {
                var relateds = Entity.CodeEngine
                                .Entities
                                .Where(e => e.Properties.Any(p => p.PropertyDefinition.TypeName.Equals(Entity.Name)));
                foreach (var item in relateds)
                {
                    var selectTempl = new MvcJQuerySelectList(Entity, fkey);
                    selectTempl.RelatedEntity = item;
                    selectTempl.WriteToFile(selectTempl.GetDefaultFilePath(), true);
                }

                if (relateds.Count() == 0)
                {
                    var selectTempl = new MvcJQuerySelectList(Entity, fkey);
                    selectTempl.WriteToFile(selectTempl.GetDefaultFilePath(), true);
                }
            }

            var displayTempl = new MvcDisplayTemplate(Entity);
            displayTempl.WriteToFile(displayTempl.GetDefaultFilePath(), false);

            return base.WriteToFile(filePath, overwriteExisting);
        }
    }
}
