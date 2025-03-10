namespace HSEBank.DataAccess.Repositories.Abstractions;

public interface IRepository<TModel, TEditDto> where TModel: class where TEditDto: class
{
    TModel Create(TModel model);
    TModel GetById(Guid id);
    bool Update(TEditDto editDto);
    bool Delete(Guid id);
    IEnumerable<TModel> GetAll();
    IEnumerable<TModel> GetByCondition(Func<TModel, bool> predicate);
}