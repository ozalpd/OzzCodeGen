using OzzCodeGen.CodeEngines.CsModelClass;
using OzzCodeGen.CodeEngines.Metadata.Templates;
using OzzCodeGen.CodeEngines.Metadata.UI;
using OzzCodeGen.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Serialization;

namespace OzzCodeGen.CodeEngines.Metadata;

[XmlInclude(typeof(MetadataEntitySetting))]
public class MetadataCodeEngine : BaseModelClassCodeEngine
{
    public override string EngineId { get { return EngineTypes.MetadataCodeEngineId; } }

    [XmlIgnore]
    [JsonIgnore]
    public static string DefaultFileName { get { return "MetadataCodeEngine.settings"; } }

    public override string ProjectTypeName { get { return "Metadata class generator"; } }

    public override string GetDefaultFileName()
    {
        return DefaultFileName;
    }

    public override string GetDefaultTargetFolder()
    {
        return "Metadata";
    }

    public override List<string> GetTemplateList()
    {
        return new List<string> { metadataClass };
    }
    private const string metadataClass = "Metadata Class";

    protected override BaseEntitySetting CreateEntitySetting()
    {
        return new MetadataEntitySetting();
    }

    protected override BaseModelClassPropertySetting CreatePropertySetting()
    {
        return new MetadataPropertySetting();
    }


    [XmlIgnore]
    [JsonIgnore]
    public List<MetadataEntitySetting> Entities
    {
        get
        {
            if (Project == null || string.IsNullOrEmpty(Project.SearchString))
            {
                return _entities;
            }
            else
            {
                var result = _entities
                    .Where(e => e.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase) ||
                        e.Properties.Where(p => p.Name.StartsWith(Project.SearchString, StringComparison.InvariantCultureIgnoreCase)).Any());
                return result.ToList();
            }
        }
        set
        {
            if (_entities == value) return;
            _entities = value;
            RaisePropertyChanged("Entities");
        }
    }
    private List<MetadataEntitySetting> _entities;

    protected override void OnEntitySettingsChanged()
    {
        var entities = new List<MetadataEntitySetting>();
        if (EntitySettings != null)
        {
            foreach (MetadataEntitySetting item in EntitySettings)
            {
                entities.Add(item);
            }
        }
        Entities = entities;
    }

    public bool SeperateMetaDataClass
    {
        get { return _seperateMetaDataClass ?? true; }
        set
        {
            _seperateMetaDataClass = value;
            RaisePropertyChanged("SeperateMetaDataClass");
        }
    }
    private bool? _seperateMetaDataClass;


    protected bool RenderTemplate(MetadataEntitySetting entitySettings)
    {
        if (entitySettings == null)
            return false;

        var template = new MetadataClassTemplate(entitySettings);
        var fileName = Path.Combine(TargetDirectory, template.GetDefaultFileName());
        bool allWritten = true;

        if (GenerateForDTO)
        {
            var dtoTemplate = new MetadataClassTemplate(entitySettings, true);
            var dtoFileName = Path.Combine(TargetDirectory, dtoTemplate.GetDefaultFileName());
            allWritten = dtoTemplate.WriteToFile(dtoFileName, OverwriteExisting || entitySettings.OverwriteExisting);
        }
        return template.WriteToFile(fileName, OverwriteExisting || entitySettings.OverwriteExisting) & allWritten;
    }

    public override bool RenderSelectedTemplate()
    {
        if (!string.IsNullOrEmpty(Project.SearchString))
        {
            Project.SearchString = string.Empty;
        }

        if (RenderAllEntities)
        {
            bool allWritten = true;
            foreach (MetadataEntitySetting setting in EntitySettings.Where(e => e.Exclude == false))
            {
                allWritten = RenderTemplate(setting) & allWritten;
            }
            return allWritten;
        }
        else
        {
            return RenderTemplate(GetSelectedEntity());
        }
    }

    public override bool RenderAllTemplates()
    {
        throw new NotImplementedException("There is only one template in this code engine!");
    }

    protected override UserControl GetUiControl()
    {
        if (_engineUI == null)
        {
            _engineUI = SetNewUI();
        }
        return _engineUI;
    }
    MetadataEngineUI _engineUI = null;

    private MetadataEngineUI SetNewUI()
    {
        var control = new MetadataEngineUI()
        {
            CodeEngine = this
        };
        MenuItem mnuExclude = UiTools.CreateMenuItem("Exclude", "Exclude selected property");
        MenuItem mnuSetUiHint = UiTools
            .CreateMenuItem("Set UIHint to Same", "Set UIHint to same named properties in other entities");
        MenuItem mnuClearReq = UiTools.CreateMenuItem("Clear Required Settings from Entity", "Clear all Required Settings in this entity");
        MenuItem mnuClearAllReq = UiTools.CreateMenuItem("Clear Required Settings from All Entities", "Clear all Required Settings in all entities");

        mnuExclude.Click += ExcludeProperty;
        mnuClearReq.Click += ClearReq;
        mnuClearAllReq.Click += ClearAllReq;
        mnuSetUiHint.Click += SetUIHintToOthers;

        control.grdEntitySettings.ContextMenu.Items.Add(mnuClearReq);
        control.grdEntitySettings.ContextMenu.Items.Add(mnuClearAllReq);
        control.grdPropertySettings.ContextMenu.Items.Add(mnuExclude);
        control.grdPropertySettings.ContextMenu.Items.Add(mnuSetUiHint);

        return control;
    }

    private void ClearReq(object o, RoutedEventArgs ea)
    {
        var entity = GetSelectedEntity();
        ClearReq(entity);
    }

    private void ClearAllReq(object o, RoutedEventArgs ea)
    {
        foreach (var entity in Entities)
        {
            ClearReq(entity);
        }
    }

    private static void ClearReq(MetadataEntitySetting entity)
    {
        if (entity == null) return;
        foreach (var property in entity.Properties)
        {
            property.Required = string.Empty;
        }
    }

    private void ExcludeProperty(object o, RoutedEventArgs ea)
    {
        var property = GetSelectedProperty();
        if (property == null) return;
        property.Exclude = true;
    }

    private void SetUIHintToOthers(object o, RoutedEventArgs ea)
    {
        var selected = GetSelectedProperty();
        if (selected == null) return;

        foreach (var entity in Entities)
        {
            var property = entity.Properties.FirstOrDefault(p => p.Name.Equals(selected.Name));
            if (property != null)
            {
                property.UIHint = selected.UIHint;
            }
        }
    }

    private MetadataPropertySetting GetSelectedProperty()
    {
        if (_engineUI.grdPropertySettings.SelectedItem == null) return null;
        return (MetadataPropertySetting)_engineUI.grdPropertySettings.SelectedItem;
    }

    protected MetadataEntitySetting GetSelectedEntity()
    {
        if (_engineUI.grdEntitySettings.SelectedItem == null) return null;
        return (MetadataEntitySetting)_engineUI.grdEntitySettings.SelectedItem;
    }


    /// <summary>
    /// Reads a project settings file and creates a ProjectSettings instance
    /// </summary>
    /// <param name="fileName">An XML file's path that contains project settings</param>
    /// <returns></returns>
    public static MetadataCodeEngine OpenFile(string fileName)
    {
        MetadataCodeEngine instance = GetInstanceFromFile(
           fileName,
           typeof(MetadataCodeEngine)) as MetadataCodeEngine;

        return instance;
    }
}
