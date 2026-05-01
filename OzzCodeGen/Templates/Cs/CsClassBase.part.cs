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

        /// <summary>
        /// Returns the C# accessibility modifier and partial keyword for the current type based on its visibility.
        /// </summary>
        /// <returns>A string containing either "public partial" if the type is public, or "internal partial"
        /// if the type is internal.</returns>
        public string GetAccessibility()
        {
            return IsPublic ? "public partial" : "internal partial";
        }


        protected string GetFolderToNamespace(string folder)
        {
            if (string.IsNullOrWhiteSpace(folder))
                return string.Empty;
            return folder.Replace("/", ".").Replace("\\", ".");
        }

        /// <summary>
        /// The generated class or interface will have public accessibility when IsPublic is true
        /// and will have internal accessibility when IsPublic is false.
        /// </summary>
        public bool IsPublic { get; set; } = true;
    }
}
