namespace DocumentDbDemo.Repository
{
    using System;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using GenericRepo;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Pocos;

    public class ProductRepository : IProductRepository
    {
        private const string EndpointUri = "https://tomasdemo.documents.azure.com:443/";
        private const string PrimaryKey = "e3SNuaxwX5l5pKOSSeyzW9pRa3kfSS6fvIfS2NmYZF8IjBxWDbrgymzuIosNyEgaV58Zb6QlJChDtXYHGCoGlg==";
        private string _databaseId = "ProductDb";
        private readonly Repository<ProductPoco> _repository;

        public ProductRepository()
        {
            var client = new DocumentClient(new Uri(EndpointUri), PrimaryKey);
            _repository = new Repository<ProductPoco>(client, _databaseId);
        }

        public async Task<ProductPoco> Read(string id)
        {
            return await _repository.Get(id);
        }

        public async Task Upsert(ProductPoco product)
        {
            product.LastModifed = DateTime.UtcNow;

            var existing = await _repository.Get(product.Id);
            if (existing == null)
            {
                await _repository.Create(product);
            }
            else
            {
                await _repository.Update(product);
            }
        }
    }
}