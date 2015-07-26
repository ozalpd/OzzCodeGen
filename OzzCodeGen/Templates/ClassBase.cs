using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OzzCodeGen.Utilities;

namespace OzzCodeGen.Templates
{
    public abstract class ClassBase : AbstractTemplate
    {
        /// <summary>
        /// Name of the class
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Namespace name of the class
        /// </summary>
        public string NamespaceName { get; set; }

        /// <summary>
        /// Name of base type
        /// </summary>
        public string BaseClass { get; set; }

        /// <summary>
        /// Namespace names in using statements
        /// </summary>
        public List<string> UsingNamespaces
        {
            get
            {
                if (usingNamespaces == null)
                {
                    usingNamespaces = new List<string>();
                    var defaults = DefaultUsingNamespaceList();
                    foreach (string n in defaults)
                    {
                        usingNamespaces.Add(n);
                    }
                }
                return usingNamespaces;
            }
            set { usingNamespaces = value; }
        }
        List<string> usingNamespaces;

        public abstract List<string> DefaultUsingNamespaceList();

        /// <summary>
        /// Implemented interface names
        /// </summary>
        public List<string> Interfaces
        {
            get
            {
                if (interfaces == null) interfaces = new List<string>();
                return interfaces;
            }
            set { Interfaces = value; }
        }
        List<string> interfaces;

        public string Implements()
        {
            if (string.IsNullOrEmpty(NamespaceName) && Interfaces.Count == 0)
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrEmpty(BaseClass))
            {
                sb.Append(" : ");
                sb.Append(BaseClass);
            }

            foreach (var interfaceName in Interfaces)
            {
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                else
                {
                    sb.Append(" : ");
                }
            }
            return sb.ToString();
        }
    }
}
