namespace DocumentDbDemo.Pocos
{
    using System;

    public class ProductPoco : Microsoft.Azure.Documents.Resource
    {
        public string Name { get; set; }

        public DateTime LastModifed { get; set; }
    }
}
