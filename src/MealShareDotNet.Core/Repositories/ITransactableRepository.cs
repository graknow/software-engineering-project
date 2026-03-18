namespace MealShareDotNet.Core.Repositories;

public interface ITransactableRepository
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}
