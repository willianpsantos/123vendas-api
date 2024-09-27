using _123Vendas.Vendas.DependencyInjection;
using _123Vendas.Vendas.Domain.Interfaces.Services;
using _123Vendas.Vendas.Domain.Models;
using _123Vendas.Vendas.Domain.Queries;
using NUnit.Framework.Legacy;

namespace _123Vendas.Vendas.Tests
{
    public class SaleServiceTests
    {
        private ManualDependencyInjection _manualDependencyInjection;

        [SetUp]
        public void Setup()
        {
            var builder = new ManualDependencyInjectionBuilder();

            _manualDependencyInjection = builder.BuildConfiguration(false).Build(services =>
            {
                services
                    .AddDomainInMemoryDbContext()
                    .AddDomainRepositories()
                    .AddDomainQueryToExpressionAdapters()
                    .AddDomainServices();
            });
        }


        private InsertOrUpdateSaleModel _CreateInsertOrUpdateModelWithProducts()
        {
            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;
            var random = new Random();

            return new InsertOrUpdateSaleModel
            {
                BranchId = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                SaleCode = random.Next().ToString(),
                SaleDate = utcNow,
                SalerId = Guid.NewGuid(),

                Products = new HashSet<InsertOrUpdateSaleProductModel>
                {
                    new InsertOrUpdateSaleProductModel
                    {                        
                        ProductId = Guid.NewGuid(),
                        Discount = 5,
                        Quantity = random.Next(),
                        Amount = random.Next()                        
                    },

                    new InsertOrUpdateSaleProductModel
                    {
                        ProductId = Guid.NewGuid(),
                        Discount = 5,
                        Quantity = random.Next(),
                        Amount = random.Next()
                    }
                }
            };
        }

        private InsertOrUpdateSaleModel _ConvertToInsertOrUpdateSaleModel(SaleModel model) =>
            new InsertOrUpdateSaleModel
            {
                BranchId = model.BranchId,
                CompanyId = model.CompanyId,
                CustomerId = model.CustomerId,
                SaleCode = model.SaleCode,
                SaleDate = model.SaleDate,
                SalerId = model.SalerId,

                Products = model.Products?.Select(_ => 
                    new InsertOrUpdateSaleProductModel
                    {
                        Id = _.Id,
                        ProductId = _.ProductId,    
                        Discount = _.Discount,
                        Quantity = _.Quantity,
                        Amount = _.Amount
                    }
                )?.ToArray() ?? Enumerable.Empty<InsertOrUpdateSaleProductModel>()
            };

        [Test]
        public async Task Should_Insert_Sale_And_Generate_NewId()
        {
            var service = _manualDependencyInjection.GetService<ISaleService>();

            if (service is null)
                Assert.Fail("Service not created!");

            var includedBy = Guid.NewGuid();
            var model = _CreateInsertOrUpdateModelWithProducts();
            var inserted = await service.InsertAsync(model, includedBy);
            var affected = await service.SaveChangesAsync();

            ClassicAssert.IsTrue(affected > 0);
            ClassicAssert.IsNotNull(inserted);
            Assert.That(inserted, Is.Not.EqualTo(Guid.Empty));
        }

        [Test]
        public async Task Should_Update_Sale_With_Given_Information()
        {
            var service = _manualDependencyInjection.GetService<ISaleService>();

            if (service is null)
                Assert.Fail("Service not created!");

            var updatedBy = Guid.NewGuid();
            var model = _CreateInsertOrUpdateModelWithProducts();            
            var insertedId = await service.InsertAsync(model, updatedBy);
            var affected = await service.SaveChangesAsync();

            ClassicAssert.IsTrue(affected > 0);

            var insertedModel = await service.GetByIdAsync(insertedId);

            if (insertedModel is null)
                Assert.Fail("Sale was not inserted");

            var newCompanyId = Guid.NewGuid();
            var newBranchId = Guid.NewGuid();
            var newSalerId = Guid.NewGuid();
            var newTotal = 150;

            var oldCompanyId = insertedModel.CompanyId;
            var oldBranchId = insertedModel.BranchId;
            var oldSalerId = insertedModel.SalerId;
            var oldTotal = insertedModel.Total;

            insertedModel.CompanyId = newCompanyId;
            insertedModel.BranchId = newBranchId;
            insertedModel.SalerId = newSalerId;
            insertedModel.Total = newTotal;

            var convertedInsertOrUpdateModel = _ConvertToInsertOrUpdateSaleModel(insertedModel);

            var updateted = service.Update(insertedId, convertedInsertOrUpdateModel, updatedBy);
            affected = await service.SaveChangesAsync();

            ClassicAssert.IsTrue(affected > 0);
            ClassicAssert.IsNotNull(updateted);
            ClassicAssert.IsTrue(updateted.CompanyId == newCompanyId);
            ClassicAssert.IsTrue(updateted.BranchId == newBranchId);
            ClassicAssert.IsTrue(updateted.SalerId == newSalerId);
            ClassicAssert.IsTrue(updateted.Total == newTotal);
        }

        [Test]
        public async Task Should_Insert_Sale_With_Products_And_CommitAll_AtEnd()
        {
            var service = _manualDependencyInjection.GetService<ISaleService>();

            if (service is null)
                Assert.Fail("Service not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;
            var model = _CreateInsertOrUpdateModelWithProducts();

            var insertedId = await service.InsertAsync(model, insertedBy);
            var affected = await service.SaveChangesAsync();
            
            ClassicAssert.IsTrue(affected > 0);
            ClassicAssert.IsNotNull(insertedId);

            var inserted = await service.GetByIdAsync(insertedId);

            ClassicAssert.IsNotNull(inserted);            
            ClassicAssert.IsFalse(inserted.Id == Guid.Empty);
            ClassicAssert.IsTrue(inserted.CompanyId == model.CompanyId);
            ClassicAssert.IsTrue(inserted.BranchId == model.BranchId);
            ClassicAssert.IsTrue(inserted.CustomerId == model.CustomerId);
            ClassicAssert.IsTrue(inserted.SaleCode == model.SaleCode);
            ClassicAssert.IsTrue(inserted.SalerId == model.SalerId);

            ClassicAssert.IsNotNull(inserted.Products);
            Assert.That(inserted.Products, Is.Not.Empty);
            Assert.That(inserted.Products, Is.All.Not.Null);

            ClassicAssert.IsTrue(inserted.Products.All(p => p.SaleId == inserted.Id));
            ClassicAssert.IsTrue(inserted.Total == inserted.Products.Sum(p => p.Total));
        }

        [Test]
        public async Task Should_Insert_Many_Sales_With_Products_And_CommitAll_AtEnd()
        {
            var service = _manualDependencyInjection.GetService<ISaleService>();

            if (service is null)
                Assert.Fail("Service not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;

            var entity1 = _CreateInsertOrUpdateModelWithProducts();
            var entity2 = _CreateInsertOrUpdateModelWithProducts();

            var insertedId1 = await service.InsertAsync(entity1, insertedBy);
            var insertedId2 = await service.InsertAsync(entity2, insertedBy);
            var affected = await service.SaveChangesAsync();

            ClassicAssert.IsTrue(affected >= 2);

            var inserted1 = await service.GetByIdAsync(insertedId1);
            var inserted2 = await service.GetByIdAsync(insertedId2);

            ClassicAssert.IsNotNull(inserted1);
            ClassicAssert.IsNotNull(inserted2);

            ClassicAssert.IsFalse(inserted1.Id == Guid.Empty);
            ClassicAssert.IsTrue(inserted1.CompanyId == entity1.CompanyId);
            ClassicAssert.IsTrue(inserted1.BranchId == entity1.BranchId);
            ClassicAssert.IsTrue(inserted1.CustomerId == entity1.CustomerId);
            ClassicAssert.IsTrue(inserted1.SaleCode == entity1.SaleCode);
            ClassicAssert.IsTrue(inserted1.SalerId == entity1.SalerId);

            Assert.That(inserted1.Products, Is.Not.Null);
            Assert.That(inserted1.Products, Is.Not.Empty);
            Assert.That(inserted1.Products, Is.All.Not.Null);

            ClassicAssert.IsTrue(inserted1.Products.All(p => p.SaleId == inserted1.Id));
            ClassicAssert.IsTrue(inserted1.Total == inserted1.Products.Sum(p => p.Total));


            ClassicAssert.IsNotNull(inserted2);
            ClassicAssert.IsFalse(inserted2.Id == Guid.Empty);
            ClassicAssert.IsTrue(inserted2.CompanyId == entity2.CompanyId);
            ClassicAssert.IsTrue(inserted2.BranchId == entity2.BranchId);
            ClassicAssert.IsTrue(inserted2.CustomerId == entity2.CustomerId);
            ClassicAssert.IsTrue(inserted2.SaleCode == entity2.SaleCode);
            ClassicAssert.IsTrue(inserted2.SalerId == entity2.SalerId);

            ClassicAssert.IsNotNull(inserted2.Products);
            Assert.That(inserted2.Products, Is.Not.Null);
            Assert.That(inserted2.Products, Is.Not.Empty);
            Assert.That(inserted2.Products, Is.All.Not.Null);

            ClassicAssert.IsTrue(inserted2.Products.All(p => p.SaleId == inserted2.Id));
            ClassicAssert.IsTrue(inserted2.Total == inserted2.Products.Sum(p => p.Total));
        }

        [Test]
        public async Task Should_Get_All_Sales_When_Pass_Null_As_Query()
        {
            var service = _manualDependencyInjection.GetService<ISaleService>();

            if (service is null)
                Assert.Fail("Service not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;
            var entity1 = _CreateInsertOrUpdateModelWithProducts();
            var entity2 = _CreateInsertOrUpdateModelWithProducts();

            var inserted1 = await service.InsertAsync(entity1, insertedBy);
            var inserted2 = await service.InsertAsync(entity2, insertedBy);
            var affected = await service.SaveChangesAsync();

            ClassicAssert.IsTrue(affected >= 2);

            var sales = await service.GetAsync();

            Assert.That(sales, Is.Not.Null);
            Assert.That(sales, Is.Not.Empty);
            Assert.That(sales.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task Should_Get_AtLeast_OneSale_When_Pass_A_Query()
        {
            var service = _manualDependencyInjection.GetService<ISaleService>();

            if (service is null)
                Assert.Fail("Service not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;

            var model1 = _CreateInsertOrUpdateModelWithProducts();
            var model2 = _CreateInsertOrUpdateModelWithProducts();
            var model3 = _CreateInsertOrUpdateModelWithProducts();

            var inserted1 = await service.InsertAsync(model1, insertedBy);
            var inserted2 = await service.InsertAsync(model2, insertedBy);
            var inserted3 = await service.InsertAsync(model3, insertedBy);
            var affected = await service.SaveChangesAsync();

            ClassicAssert.IsTrue(affected >= 2);

            var sales = await service.GetAsync(new SaleQuery
            {
                SaleCode = model1.SaleCode
            });

            Assert.That(sales, Is.Not.Null);
            Assert.That(sales, Is.Not.Empty);
            Assert.That(sales.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task Should_Get_All_Sales_When_Pass_Null_as_Query_With_Pagination()
        {
            var service = _manualDependencyInjection.GetService<ISaleService>();

            if (service is null)
                Assert.Fail("Service not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;
            var inserteds = new HashSet<Guid>();

            for (var i = 0; i < 10; i++)
            {
                var entity = _CreateInsertOrUpdateModelWithProducts();
                var inserted = await service.InsertAsync(entity, insertedBy);
                inserteds.Add(inserted);
            }

            var affected = await service.SaveChangesAsync();

            ClassicAssert.IsTrue(affected >= 2);

            var insertedCount = await service.CountAsync(null);            

            var sales = await service.GetAsync(new SaleQuery
            {
                PageNumber = 1,
                PageSize = 5
            });

            Assert.That(sales, Is.Not.Null);
            Assert.That(sales, Is.Not.Empty);
            ClassicAssert.IsTrue(sales.Count() == 5 && sales.Count() < insertedCount);
        }
    }
}
