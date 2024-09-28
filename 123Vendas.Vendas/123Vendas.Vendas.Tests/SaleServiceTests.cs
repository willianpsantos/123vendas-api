using _123Vendas.Vendas.Data.Entities;
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


        private InsertOrUpdateSaleModel _CreateInsertOrUpdateModelWithProducts(int productsCount = 3)
        {            
            var utcNow = DateTimeOffset.UtcNow;
            var random = new Random();
            var possibleDiscounts = new int[] { 0, 5, 0, 10, 2, 0, 15, 0, 0, 25, 7 };

            return new InsertOrUpdateSaleModel
            {
                BranchId = Guid.NewGuid(),
                CompanyId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),                
                SalerId = Guid.NewGuid(),

                SaleCode = random.Next(50001).ToString(),
                SaleDate = utcNow,

                Products = Enumerable.Range(1, productsCount).Select(_ => new InsertUpdateOrDeleteSaleProductModel
                    {                        
                        ProductId = Guid.NewGuid(),

                        Discount = possibleDiscounts[random.Next(possibleDiscounts.Length - 1)],
                        Quantity = random.Next(10),
                        Amount = random.Next(100)                        
                    }
                ).ToHashSet()
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
                    new InsertUpdateOrDeleteSaleProductModel
                    {
                        Id = _.Id,
                        SaleId = _.SaleId,
                        ProductId = _.ProductId,    

                        Discount = _.Discount,
                        Quantity = _.Quantity,
                        Amount = _.Amount,

                        IncludedAt = _.IncludedAt,
                        UpdatedAt = _.UpdatedAt,
                        DeletedAt = _.DeletedAt
                    }
                )?.ToHashSet()
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

            Assert.That(affected, Is.GreaterThan(0));
            Assert.That(inserted, Is.Not.Empty);
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

            Assert.That(affected, Is.GreaterThan(0));

            var insertedModel = await service.GetByIdAsync(insertedId);

            if (insertedModel is null)
                Assert.Fail("Sale was not inserted");

            var newCompanyId = Guid.NewGuid();
            var newBranchId = Guid.NewGuid();
            var newSalerId = Guid.NewGuid();

            var oldCompanyId = insertedModel.CompanyId;
            var oldBranchId = insertedModel.BranchId;
            var oldSalerId = insertedModel.SalerId;

            insertedModel.CompanyId = newCompanyId;
            insertedModel.BranchId = newBranchId;
            insertedModel.SalerId = newSalerId;            

            var convertedInsertOrUpdateModel = _ConvertToInsertOrUpdateSaleModel(insertedModel);

            Assert.That(convertedInsertOrUpdateModel, Is.Not.Null);
            Assert.That(convertedInsertOrUpdateModel.Products, Is.Not.Null);
            Assert.That(convertedInsertOrUpdateModel.Products, Is.Not.Empty);

            convertedInsertOrUpdateModel!.Products!.ElementAt(2).IsDeleted = true;

            convertedInsertOrUpdateModel!.Products!.Add(new InsertUpdateOrDeleteSaleProductModel
            {
                ProductId = Guid.NewGuid(),
                Amount = 10,
                Quantity = 2,
                Discount = 0
            });

            var updated = service.Update(insertedId, convertedInsertOrUpdateModel, updatedBy);
            affected = await service.SaveChangesAsync();

            Assert.That(affected, Is.GreaterThan(0));
            Assert.That(updated, Is.Not.Null);
            Assert.That(updated.CompanyId, Is.EqualTo(newCompanyId));
            Assert.That(updated.BranchId, Is.EqualTo(newBranchId));
            Assert.That(updated.SalerId, Is.EqualTo(newSalerId));
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

            Assert.That(affected, Is.GreaterThanOrEqualTo(2));

            var inserted1 = await service.GetByIdAsync(insertedId1);
            var inserted2 = await service.GetByIdAsync(insertedId2);

            Assert.That(affected, Is.GreaterThanOrEqualTo(2));
            Assert.That(inserted1, Is.Not.Null);

            Assert.That(inserted1.Id, Is.Not.EqualTo(Guid.Empty));
            Assert.That(inserted1.CompanyId, Is.EqualTo(entity1.CompanyId));
            Assert.That(inserted1.BranchId, Is.EqualTo(entity1.BranchId));
            Assert.That(inserted1.CustomerId, Is.EqualTo(entity1.CustomerId));
            Assert.That(inserted1.SaleCode, Is.EqualTo(entity1.SaleCode));
            Assert.That(inserted1.SalerId, Is.EqualTo(entity1.SalerId));
            Assert.That(inserted1.IncludedAt.GetValueOrDefault().Date, Is.EqualTo(DateTimeOffset.UtcNow.Date));

            Assert.That(inserted1.Products, Is.Not.Null);
            Assert.That(inserted1.Products, Is.All.Not.Null);

            Assert.That(inserted1.Products.All(p => p.SaleId == inserted1.Id), Is.True);
            Assert.That(inserted1.Total == inserted1.Products.Sum(p => p.Total), Is.True);


            Assert.That(inserted2, Is.Not.Null);
            Assert.That(inserted2.Id == Guid.Empty, Is.False);
            Assert.That(inserted2.CompanyId == entity2.CompanyId, Is.True);
            Assert.That(inserted2.BranchId == entity2.BranchId, Is.True);
            Assert.That(inserted2.CustomerId == entity2.CustomerId, Is.True);
            Assert.That(inserted2.SaleCode == entity2.SaleCode, Is.True);
            Assert.That(inserted2.SalerId == entity2.SalerId, Is.True);
            Assert.That(inserted2.IncludedAt.GetValueOrDefault().Date == DateTimeOffset.UtcNow.Date, Is.True);

            Assert.That(inserted2.Products, Is.Not.Null);
            Assert.That(inserted2.Products, Is.All.No.Null);

            Assert.That(inserted2.Products.All(p => p.SaleId == inserted2.Id), Is.True);
            Assert.That(inserted2.Total, Is.EqualTo(inserted2.Products.Sum(p => p.Total)));
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

            Assert.That(affected, Is.GreaterThanOrEqualTo(2));

            var sales = await service.GetAsync();

            Assert.That(sales, Is.Not.Null);
            Assert.That(sales, Is.Not.Empty);
            Assert.That(sales.All(_ => _.Products.Count() > 0), Is.True);
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
            Assert.That(sales.All(_ => _.Products.Count() > 0), Is.True);
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

            Assert.That(affected, Is.GreaterThanOrEqualTo(10));

            var result = await service.GetPaginatedAsync(1, 5);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Data, Is.Not.Null);
            Assert.That(result.Data, Is.Not.Empty);
            Assert.That(result.Data.Count, Is.EqualTo(5));

            Assert.That(result.Count, Is.EqualTo(10));
            Assert.That(result.PageNumber, Is.EqualTo(1));
            Assert.That(result.PageSize, Is.EqualTo(5));
        }
    }
}
