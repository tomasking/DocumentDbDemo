using System;

namespace DocumentDbDemo.Tests
{
    using System.Threading.Tasks;
    using Dtos;
    using Listeners;
    using Repository;
    using Shouldly;
    using Xunit;

    public class ReceiveProductTests
    {
        private ProductListener _productListener;
        private ProductRepository _productRepository;

        public ReceiveProductTests()
        {
            _productRepository = new ProductRepository();
            _productListener = new ProductListener(_productRepository);
        }

        [Fact]
        public async Task WhenAProductIsReceivedItGetsSavedInDocumentDb()
        {
            var productDto = new ProductDto {Id = "123", Name = "Apron"};

            await _productListener.ProductReceived(productDto);

            //Check in DocDb
            var productInDb = await _productRepository.Read(productDto.Id);
            productInDb.ShouldNotBeNull();

            productInDb.Name.ShouldBe(productDto.Name);
        }
    }
}
