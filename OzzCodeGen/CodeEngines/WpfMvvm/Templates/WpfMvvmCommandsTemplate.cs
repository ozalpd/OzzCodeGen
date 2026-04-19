namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates;

public class WpfMvvmCommandsTemplate : AbstractTemplate
{
    private readonly WpfMvvmEntitySetting _entitySetting;
    private readonly WpfMvvmCodeEngine _codeEngine;

    public WpfMvvmCommandsTemplate(WpfMvvmEntitySetting entitySetting)
    {
        _entitySetting = entitySetting;
        _codeEngine = (WpfMvvmCodeEngine)entitySetting.CodeEngine;
    }

    public override string GetDefaultFileName()
    {
        return $"{_entitySetting.Name}Commands.cs";
    }

    public override string TransformText()
    {
        return $"using {_codeEngine.MvvmNamespaceName};\n"
             + "using System;\n"
             + "using System.Threading.Tasks;\n\n"
             + $"namespace {_codeEngine.CommandNamespaceName};\n\n"
             + $"public sealed class {_entitySetting.Name}Commands\n"
             + "{\n"
             + "    public AsyncRelayCommand SaveCommand { get; }\n"
             + "    public RelayCommand CancelCommand { get; }\n\n"
             + $"    public {_entitySetting.Name}Commands(Func<Task> onSaveAsync, Action onCancel, Func<bool> canSave = null)\n"
             + "    {\n"
             + "        SaveCommand = new AsyncRelayCommand(onSaveAsync, canSave);\n"
             + "        CancelCommand = new RelayCommand(onCancel);\n"
             + "    }\n"
             + "}\n";
    }
}
