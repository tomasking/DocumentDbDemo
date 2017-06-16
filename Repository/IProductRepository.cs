namespace DocumentDbDemo.Repository
{
    using System.Threading.Tasks;
    using Pocos;

    public interface IProductRepository
    {
        Task<ProductPoco> Read(string id);
        Task Upsert(ProductPoco productPoco);
    }
}