namespace HSEBank.DataAccess.Repositories.Abstractions;

/// <summary>
/// CRUD logic service.
/// </summary>
/// <typeparam name="TModel"></typeparam>
/// <typeparam name="TEditDto"></typeparam>
public interface IRepository<TModel, TEditDto> where TModel: class where TEditDto: class
{
    TModel Create(TModel model);
    TModel GetById(Guid id);
    bool Update(TEditDto editDto);
    bool Delete(Guid id);
    IEnumerable<TModel> GetAll();
    IEnumerable<TModel> GetByCondition(Func<TModel, bool> predicate);
    public bool Exists(Guid id);
}