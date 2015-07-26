using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OzzCodeGen.Definitions;

namespace OzzCodeGen.Providers
{
    public interface IModelProvider
    {
        string ProviderId { get; }
        
        /// <summary>
        /// Generates a DataModel from a data or model source
        /// </summary>
        /// <param name="modelSource">Source for DataModel</param>
        /// <returns>Generated DataModel instance</returns>
        DataModel GetDataModel(string modelSource);
        EntityDefinition GetDefaultEntityDefinition();
        CodeGenProject CreateProject(string modelSource);

        /// <summary>
        /// Refreshes a DataModel from source.
        /// </summary>
        /// <param name="modelSource">Source for DataModel</param>
        /// <param name="model">DataModel to be refreshed</param>
        /// <param name="cleanRemovedItems">If true items will be removed those are not found in the source.</param>
        DataModel RefreshDataModel(string modelSource, DataModel model, bool cleanRemovedItems);
        bool CanRefresh { get; }

        string SelectSource();
    }
}
