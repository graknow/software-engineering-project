namespace MealShareDotNet.Core.Repositories;

public interface ITransactableRepository
{
    void Commit();
    void Rollback();
}
