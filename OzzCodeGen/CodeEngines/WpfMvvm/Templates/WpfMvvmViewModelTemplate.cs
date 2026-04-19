using System.Linq;
using System.Text;

namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates;

public class WpfMvvmViewModelTemplate : AbstractTemplate
{
    private readonly WpfMvvmEntitySetting _entitySetting;
    private readonly WpfMvvmCodeEngine _codeEngine;
    private readonly bool _isEdit;

    public WpfMvvmViewModelTemplate(WpfMvvmEntitySetting entitySetting, bool isEdit)
    {
        _entitySetting = entitySetting;
        _codeEngine = (WpfMvvmCodeEngine)entitySetting.CodeEngine;
        _isEdit = isEdit;
    }

    public override string GetDefaultFileName()
    {
        return $"{_entitySetting.Name}{(_isEdit ? "Edit" : "Create")}ViewModel.cs";
    }

    public override string TransformText()
    {
        var props = _entitySetting
            .GetInheritedIncludedProperties()
            .OfType<WpfMvvmPropertySetting>()
            .Where(p => p.IncludeInViewModel)
            .ToList();

        var fields = new StringBuilder();
        var properties = new StringBuilder();

        foreach (var prop in props)
        {
            fields.AppendLine($"    private {prop.GetTypeName()} _{CamelCase(prop.Name)};");
            fields.AppendLine();

            properties.AppendLine($"    public {prop.GetTypeName()} {prop.Name}");
            properties.AppendLine("    {");
            properties.AppendLine($"        get => _{CamelCase(prop.Name)};");
            properties.AppendLine($"        set => SetProperty(ref _{CamelCase(prop.Name)}, value);");
            properties.AppendLine("    }");
            properties.AppendLine();
        }

        var className = $"{_entitySetting.Name}{(_isEdit ? "Edit" : "Create")}ViewModel";

        return $"using {_codeEngine.MvvmNamespaceName};\n"
             + $"using {_codeEngine.CommandNamespaceName};\n"
             + "using System.Threading.Tasks;\n\n"
             + $"namespace {_codeEngine.ViewModelNamespaceName};\n\n"
             + $"public partial class {className} : ViewModelBase\n"
             + "{\n"
             + fields
             + $"    public {_entitySetting.Name}Commands Commands {{ get; }}\n\n"
             + $"    public {className}()\n"
             + "    {\n"
             + $"        Commands = new {_entitySetting.Name}Commands(SaveAsync, Cancel, CanSave);\n"
             + "    }\n\n"
             + properties
             + (_isEdit
                ? "    protected override Task OnInitializeAsync(object parameter)\n"
                  + "    {\n"
                  + "        return LoadAsync(parameter);\n"
                  + "    }\n\n"
                  + "    protected virtual Task LoadAsync(object parameter)\n"
                  + "    {\n"
                  + "        return Task.CompletedTask;\n"
                  + "    }\n\n"
                : string.Empty)
             + "    protected virtual bool CanSave()\n"
             + "    {\n"
             + "        return true;\n"
             + "    }\n\n"
             + "    protected virtual Task SaveAsync()\n"
             + "    {\n"
             + "        return Task.CompletedTask;\n"
             + "    }\n\n"
             + "    protected virtual void Cancel()\n"
             + "    {\n"
             + "    }\n"
             + "}\n";
    }
}
