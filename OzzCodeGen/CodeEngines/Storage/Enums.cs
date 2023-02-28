using System.ComponentModel;

namespace OzzCodeGen.CodeEngines.Storage
{
    public enum StoredProcGeneration
    {
        /// <summary>
        /// No stored procedure will be generated
        /// </summary>
        [Description("No Stored Procedure")]
        NoStoredProcedure = 0,
        /// <summary>
        /// Generates stored procedures for Create, Update and Delete operations
        /// </summary>
        [Description("CUD (Create, Update, Delete)")]
        CUD = 100,
        /// <summary>
        /// Generates an InsertOrUpdate stored procedure one which retuns an Id result
        /// </summary>
        [Description("InsertOrUpdate(returns row with result) wo CUD")]
        InsertOrUpdateOnly = 150,
        /// <summary>
        /// Generates CUD stored procedures and an InsertOrUpdate one which retuns an int result
        /// </summary>
        [Description("InsertOrUpdate(returns simple result) and CUD")]
        InsertOrUpdateAndCUD = 200,
        /// <summary>
        /// Generates CUD stored procedures and an InsertOrUpdate one which retuns entire row
        /// </summary>
        [Description("InsertOrUpdate(returns row) and CUD")]
        InsertOrUpdateWithRowResult = 300,
        /// <summary>
        /// Generates CUD stored procedures and an InsertOrUpdate one which retuns entire row and int result
        /// </summary>
        [Description("InsertOrUpdate(returns row with result) and CUD")]
        InsertOrUpdateWithComplexResult = 400
    }
}
