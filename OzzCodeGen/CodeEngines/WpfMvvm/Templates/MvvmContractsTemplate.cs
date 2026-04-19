namespace OzzCodeGen.CodeEngines.WpfMvvm.Templates;

public class MvvmContractsTemplate : AbstractTemplate
{
    private readonly WpfMvvmCodeEngine _codeEngine;

    public MvvmContractsTemplate(WpfMvvmCodeEngine codeEngine)
    {
        _codeEngine = codeEngine;
    }

    public override string GetDefaultFileName()
    {
        return "IMvvmContracts.cs";
    }

    public override string TransformText()
    {
        return "using System.Threading.Tasks;\n"
             + "using System.Windows.Input;\n\n"
             + $"namespace {_codeEngine.MvvmContractsNamespaceName};\n\n"
             + "public interface IViewModel\n"
             + "{\n"
             + "    Task InitializeAsync(object parameter = null);\n"
             + "}\n\n"
             + "public interface IAsyncCommand : ICommand\n"
             + "{\n"
             + "    Task ExecuteAsync(object parameter = null);\n"
             + "}\n\n"
             + "public interface INavigationService\n"
             + "{\n"
             + "    Task NavigateToAsync<TViewModel>(object parameter = null) where TViewModel : IViewModel;\n"
             + "    Task GoBackAsync();\n"
             + "}\n";
    }
}
