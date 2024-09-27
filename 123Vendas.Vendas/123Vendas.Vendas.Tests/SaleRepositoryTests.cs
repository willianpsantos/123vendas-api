using _123Vendas.Vendas.Data.Entities;
using _123Vendas.Vendas.DependencyInjection;
using _123Vendas.Vendas.Domain.Interfaces.Base;
using NUnit.Framework.Legacy;

namespace _123Vendas.Vendas.Tests
{
    public class SaleRepositoryTests
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
                    .AddDomainRepositories();
            });
        }

        private Sale _CreateEntityWithoutProducts()
        {
            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;

            var entity = new Sale
            {
                BranchId = Guid.NewGuid(),
                Canceled = false,
                CompanyId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                IsDeleted = false,
                SaleCode = "123456",
                SaleDate = utcNow,
                SalerId = Guid.NewGuid(),
                Total = 100
            };

            return entity;
        }

        private Sale _CreateEntityWithProducts()
        {
            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;
            var random = new Random();

            return new Sale
            {
                BranchId = Guid.NewGuid(),
                Canceled = false,
                CompanyId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                IsDeleted = false,
                SaleCode = random.Next().ToString(),
                SaleDate = utcNow,
                SalerId = Guid.NewGuid(),
                Total = 100,

                Products = new HashSet<SaleProduct>
                {
                    new SaleProduct
                    {
                        Amount = new _123Vendas.Vendas.Data.ValueObjects.ProductAmount(random.Next(), random.Next(), 5),
                        ProductId = Guid.NewGuid()
                    },

                    new SaleProduct
                    {
                        Amount = new _123Vendas.Vendas.Data.ValueObjects.ProductAmount(random.Next(), random.Next(), 0),
                        ProductId = Guid.NewGuid()
                    }
                }
            };
        }

        [Test]
        public async Task Should_Insert_Sale_And_Generate_NewId()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var entity = _CreateEntityWithoutProducts();
            var inserted = await repository.InsertAndSaveChangesAsync(entity, entity.IncludedBy.Value);

            ClassicAssert.IsNotNull(inserted);
            ClassicAssert.IsFalse(inserted.Id == Guid.Empty);
            ClassicAssert.IsTrue(inserted.CompanyId == entity.CompanyId);
            ClassicAssert.IsTrue(inserted.BranchId == entity.BranchId);
            ClassicAssert.IsTrue(inserted.CustomerId == entity.CustomerId);
            ClassicAssert.IsTrue(inserted.SaleCode == entity.SaleCode);
            ClassicAssert.IsTrue(inserted.SalerId == entity.SalerId);
            ClassicAssert.IsTrue(inserted.IncludedAt.GetValueOrDefault().Date == DateTimeOffset.UtcNow.Date);
        }

        [Test]
        public async Task Should_Update_Sale_With_Given_Information()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var entity = _CreateEntityWithoutProducts();
            var updatedBy = Guid.NewGuid();

            var inserted = await repository.InsertAndSaveChangesAsync(entity, entity.IncludedBy.Value);

            var newCompanyId = Guid.NewGuid();
            var newBranchId = Guid.NewGuid();
            var newSalerId = Guid.NewGuid();
            var newTotal = 150;

            var oldCompanyId = inserted.CompanyId;
            var oldBranchId = inserted.BranchId;
            var oldSalerId = inserted.SalerId;
            var oldTotal = inserted.Total;

            inserted.CompanyId = newCompanyId;
            inserted.BranchId = newBranchId;
            inserted.SalerId = newSalerId;
            inserted.Total = newTotal;

            var updateted = await repository.UpdateAndSaveChangesAsync(inserted, updatedBy);

            ClassicAssert.IsNotNull(updateted);
            ClassicAssert.IsTrue(updateted.CompanyId == newCompanyId);
            ClassicAssert.IsTrue(updateted.BranchId == newBranchId);
            ClassicAssert.IsTrue(updateted.SalerId == newSalerId);
            ClassicAssert.IsTrue(updateted.Total == newTotal);
            ClassicAssert.IsNotNull(updateted.UpdatedAt);
            ClassicAssert.IsTrue(updateted.UpdatedAt?.Date == DateTimeOffset.UtcNow.Date);
            ClassicAssert.IsNotNull(updateted.UpdatedBy);
            ClassicAssert.IsTrue(updateted.UpdatedBy == updatedBy);
        }

        [Test]
        public async Task Should_Insert_Sale_With_Products_And_CommitAll_AtEnd()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var utcNow = DateTimeOffset.UtcNow;
            var entity = _CreateEntityWithProducts();
            var inserted = await repository.InsertAndSaveChangesAsync(entity, entity.IncludedBy.Value);

            ClassicAssert.IsNotNull(inserted);
            ClassicAssert.IsFalse(inserted.Id == Guid.Empty);
            ClassicAssert.IsTrue(inserted.CompanyId == entity.CompanyId);
            ClassicAssert.IsTrue(inserted.BranchId == entity.BranchId);
            ClassicAssert.IsTrue(inserted.CustomerId == entity.CustomerId);
            ClassicAssert.IsTrue(inserted.SaleCode == entity.SaleCode);
            ClassicAssert.IsTrue(inserted.SalerId == entity.SalerId);
            ClassicAssert.IsTrue(inserted.IncludedAt.GetValueOrDefault().Date == DateTimeOffset.UtcNow.Date);

            ClassicAssert.IsNotNull(inserted.Products);
            CollectionAssert.IsNotEmpty(inserted.Products);
            CollectionAssert.AllItemsAreNotNull(inserted.Products);

            ClassicAssert.IsTrue(inserted.Products.All(p => !p.IsDeleted && p.SaleId == inserted.Id));
            ClassicAssert.IsTrue(inserted.Total == inserted.Products.Where(p => !p.IsDeleted).Sum(p => p.Amount?.Total ?? 0));
        }

        [Test]
        public async Task Should_Insert_Many_Sales_With_Products_And_CommitAll_AtEnd()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;

            var entity1 = _CreateEntityWithProducts();
            var entity2 = _CreateEntityWithProducts();

            var inserted1 = await repository.InsertAsync(entity1, insertedBy);
            var inserted2 = await repository.InsertAsync(entity2, insertedBy);

            var affected = await repository.SaveChangesAsync();

            ClassicAssert.IsTrue(affected >= 2);

            ClassicAssert.IsNotNull(inserted1);
            ClassicAssert.IsFalse(inserted1.Id == Guid.Empty);
            ClassicAssert.IsTrue(inserted1.CompanyId == entity1.CompanyId);
            ClassicAssert.IsTrue(inserted1.BranchId == entity1.BranchId);
            ClassicAssert.IsTrue(inserted1.CustomerId == entity1.CustomerId);
            ClassicAssert.IsTrue(inserted1.SaleCode == entity1.SaleCode);
            ClassicAssert.IsTrue(inserted1.SalerId == entity1.SalerId);
            ClassicAssert.IsTrue(inserted1.IncludedAt.GetValueOrDefault().Date == DateTimeOffset.UtcNow.Date);

            ClassicAssert.IsNotNull(inserted1.Products);
            CollectionAssert.IsNotEmpty(inserted1.Products);
            CollectionAssert.AllItemsAreNotNull(inserted1.Products);

            ClassicAssert.IsTrue(inserted1.Products.All(p => !p.IsDeleted && p.SaleId == inserted1.Id));
            ClassicAssert.IsTrue(inserted1.Total == inserted1.Products.Where(p => !p.IsDeleted).Sum(p => p.Amount?.Total ?? 0));


            ClassicAssert.IsNotNull(inserted2);
            ClassicAssert.IsFalse(inserted2.Id == Guid.Empty);
            ClassicAssert.IsTrue(inserted2.CompanyId == entity2.CompanyId);
            ClassicAssert.IsTrue(inserted2.BranchId == entity2.BranchId);
            ClassicAssert.IsTrue(inserted2.CustomerId == entity2.CustomerId);
            ClassicAssert.IsTrue(inserted2.SaleCode == entity2.SaleCode);
            ClassicAssert.IsTrue(inserted2.SalerId == entity2.SalerId);
            ClassicAssert.IsTrue(inserted2.IncludedAt.GetValueOrDefault().Date == DateTimeOffset.UtcNow.Date);

            ClassicAssert.IsNotNull(inserted2.Products);
            CollectionAssert.IsNotEmpty(inserted2.Products);
            CollectionAssert.AllItemsAreNotNull(inserted2.Products);

            ClassicAssert.IsTrue(inserted2.Products.All(p => !p.IsDeleted && p.SaleId == inserted2.Id));
            ClassicAssert.IsTrue(inserted2.Total == inserted2.Products.Where(p => !p.IsDeleted).Sum(p => p.Amount?.Total ?? 0));
        }

        [Test]
        public async Task Should_Get_All_Sales_When_Pass_Null_As_Query()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;

            var entity1 = _CreateEntityWithProducts();
            var entity2 = _CreateEntityWithProducts();

            var inserted1 = await repository.InsertAsync(entity1, insertedBy);
            var inserted2 = await repository.InsertAsync(entity2, insertedBy);
            var affected = await repository.SaveChangesAsync();

            ClassicAssert.IsTrue(affected >= 2);

            var inserteds = new Sale[] { inserted1, inserted2 };
            var count = 0;

            await foreach (var item in repository.GetAsync())
            {
                if (!inserteds.Any(_ => _.Id == item.Id))
                {
                    Assert.Fail("Got record different from inserted sales");
                    continue;
                }

                count++;
            }

            ClassicAssert.AreEqual(inserteds.Length, count);
        }

        [Test]
        public async Task Should_Get_AtLeast_OneSale_When_Pass_A_Query()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;

            var entity1 = _CreateEntityWithProducts();
            var entity2 = _CreateEntityWithProducts();
            var entity3 = _CreateEntityWithProducts();

            var inserted1 = await repository.InsertAsync(entity1, insertedBy);
            var inserted2 = await repository.InsertAsync(entity2, insertedBy);
            var inserted3 = await repository.InsertAsync(entity3, insertedBy);
            var affected = await repository.SaveChangesAsync();

            ClassicAssert.IsTrue(affected >= 2);

            var inserteds = new Sale[] { inserted1, inserted2, inserted3 };
            var count = 0;

            await foreach (var item in repository.GetAsync(_ => _.SaleCode == entity2.SaleCode))
            {
                if (!inserteds.Any(_ => _.Id == item.Id))
                {
                    Assert.Fail("Got record different from inserted sales");
                    continue;
                }

                count++;
            }

            ClassicAssert.IsTrue(count >= 1);
        }

        [Test]
        public async Task Should_Get_All_Sales_When_Pass_Null_as_Query_With_Pagination()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;
            var inserteds = new HashSet<Sale>();
            
            for(var i = 0; i < 10; i++)
            {
                var entity = _CreateEntityWithProducts();
                var inserted = await repository.InsertAsync(entity, insertedBy);
                inserteds.Add(inserted);
            }

            var affected = await repository.SaveChangesAsync();

            ClassicAssert.IsTrue(affected >= 2);

            var insertedCount = await repository.CountAsync(null);
            var count = 0;

            await foreach (var item in repository.GetAsync(null, 1, 5))
                count++;

            ClassicAssert.IsTrue(count == 5 && count < insertedCount);
        }
    }
}