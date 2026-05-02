namespace OzzCodeGen.CodeEngines.CsDbRepository
{
    public enum MethodType
    {
        AnyByForeignKey,
        Create,
        DeleteByPKey,
        DeleteByUniqueIndex,
        GetAll,
        GetPaged,
        GetByPKey,
        GetByUnique,
        GetByForeignKey,
        UpdateEntity,
        UpdateSingleColumnById,
    }
}
