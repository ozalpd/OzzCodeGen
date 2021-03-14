using System.ComponentModel;

namespace OzzCodeGen.CodeEngines.Storage
{
    public enum StoredProcGeneration
    {
        [Description("No Stored Procedure")]
        NoStoredProcedure = 0,
        [Description("CUD (Create, Update, Delete)")]
        CUD = 100,
        [Description("InsertOrUpdate(returns simple result) and CUD")]
        InsertOrUpdateAndCUD = 200,
        [Description("InsertOrUpdate(returns row) and CUD")]
        InsertOrUpdateWithRowResult = 300,
        [Description("InsertOrUpdate(returns row with result) and CUD")]
        InsertOrUpdateWithComplexResult = 400
    }
}
