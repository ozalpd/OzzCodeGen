using System.Collections.Generic;

namespace OzzCodeGen.Templates.Cs
{
    public abstract partial class CsClassBase : ClassBase
    {
        /// <summary>
        /// Returns a list of name spaces those are assumed to used in most class types.
        /// Override this to change list..
        /// </summary>
        /// <returns></returns>
        public override List<string> DefaultUsingNamespaceList()
        {
            return new List<string>()
            {
                "System",
                "System.Linq",
                "System.Collections",
                "System.Collections.Generic"
            };
        }

        protected string GetFolderToNamespace(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
                return string.Empty;
            return folder.Replace("/", ".").Replace("\\", ".");
        }
    }
}
