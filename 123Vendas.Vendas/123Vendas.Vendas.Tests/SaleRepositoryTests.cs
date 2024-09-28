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

        private Sale _CreateEntityWithProducts(int productsCount = 3)
        {
            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;
            var random = new Random();
            var possibleDiscounts = new int[] { 0, 5, 0, 10, 2, 0, 15, 0, 0, 25, 7 };

            return new Sale
            {
                BranchId = Guid.NewGuid(),                
                CompanyId = Guid.NewGuid(),
                CustomerId = Guid.NewGuid(),
                SalerId = Guid.NewGuid(),

                SaleCode = random.Next(50001).ToString(),
                SaleDate = utcNow,

                Canceled = false,
                IsDeleted = false,

                Products = Enumerable.Range(1, productsCount).Select(_ =>
                    new SaleProduct
                    {
                        ProductId = Guid.NewGuid(),

                        Amount = new Data.ValueObjects.ProductAmount(
                            random.Next(10), 
                            random.Next(100),
                            possibleDiscounts[random.Next(possibleDiscounts.Length - 1)]
                        )
                    }
                )
                .ToHashSet()
            };
        }

        [Test]
        public async Task Should_Insert_NewSale_And_Generate_NewId()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var includedBy = Guid.NewGuid();
            var entity = _CreateEntityWithProducts();
            var inserted = await repository.InsertAndSaveChangesAsync(entity, includedBy);

            Assert.That(inserted, Is.Not.Null);
            Assert.That(inserted.Id == Guid.Empty, Is.False);
            Assert.That(inserted.CompanyId == entity.CompanyId, Is.True);
            Assert.That(inserted.BranchId == entity.BranchId, Is.True);
            Assert.That(inserted.CustomerId == entity.CustomerId, Is.True);
            Assert.That(inserted.SaleCode == entity.SaleCode, Is.True);
            Assert.That(inserted.SalerId == entity.SalerId, Is.True);
            Assert.That(inserted.IncludedAt.GetValueOrDefault().Date == DateTimeOffset.UtcNow.Date, Is.True);

            Assert.That(inserted.Products, Is.Not.Null);
            Assert.That(inserted.Products, Is.Not.Empty);
            Assert.That(inserted.Products, Is.Not.All.Null);

            Assert.That(inserted.Products.All(p => !p.IsDeleted && p.SaleId == inserted.Id), Is.True);

            Assert.That(
                inserted.Total ==
                    inserted
                        .Products
                        .Where(p => !p.IsDeleted)
                        .Sum(p => p.Amount?.Total ?? 0),

                Is.True
            );
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

            Assert.That(inserted1.Products.All(p => !p.IsDeleted && p.SaleId == inserted1.Id), Is.True);
            Assert.That(inserted1.Total == inserted1.Products.Where(p => !p.IsDeleted).Sum(p => p.Amount?.Total ?? 0), Is.True);


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

            Assert.That(inserted2.Products.All(p => !p.IsDeleted && p.SaleId == inserted2.Id), Is.True);
            Assert.That(inserted2.Total == inserted2.Products.Where(p => !p.IsDeleted).Sum(p => p.Amount?.Total ?? 0), Is.True);
        }

        [Test]
        public async Task Should_Update_Sale_With_Given_Information()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var updatedBy = Guid.NewGuid();
            var entity = _CreateEntityWithProducts(3);
            var inserted = await repository.InsertAndSaveChangesAsync(entity, updatedBy);

            var newCompanyId = Guid.NewGuid();
            var newBranchId = Guid.NewGuid();
            var newSalerId = Guid.NewGuid();

            var oldCompanyId = inserted.CompanyId;
            var oldBranchId = inserted.BranchId;
            var oldSalerId = inserted.SalerId;
            var oldProducts = inserted.Products.ToArray();

            inserted.CompanyId = newCompanyId;
            inserted.BranchId = newBranchId;
            inserted.SalerId = newSalerId;

            inserted.Products.Add(new SaleProduct
            {
                ProductId = Guid.NewGuid(),
                Amount = new Data.ValueObjects.ProductAmount(2, 10, 0),
                Canceled = false
            });

            inserted.Products.ElementAt(2).IsDeleted = true;

            var updated = await repository.UpdateAndSaveChangesAsync(inserted, updatedBy);

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated.CompanyId == oldBranchId, Is.False);
            Assert.That(updated.BranchId == oldBranchId, Is.False);
            Assert.That(updated.SalerId == oldSalerId, Is.False);

            Assert.That(updated.Products, Is.Not.Null);
            Assert.That(updated.Products, Is.All.Not.Null);
            Assert.That(updated.Products.All(p => p.SaleId == updated.Id), Is.True);
            Assert.That(updated.Products.Where(p => p.IsDeleted).Count(), Is.EqualTo(1));
            Assert.That(updated.Products.Where(p => !p.IsDeleted).Count(), Is.GreaterThan(1));

            Assert.That(updated, Is.Not.Null);
            Assert.That(updated.CompanyId == newCompanyId, Is.True);
            Assert.That(updated.BranchId == newBranchId, Is.True);
            Assert.That(updated.SalerId == newSalerId, Is.True);

            Assert.That(updated.UpdatedAt, Is.Not.Null);
            Assert.That(updated.UpdatedAt?.Date == DateTimeOffset.UtcNow.Date, Is.True);
            Assert.That(updated.UpdatedBy, Is.Not.Null);
            Assert.That(updated.UpdatedBy == updatedBy, Is.True);
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

            Assert.That(affected, Is.GreaterThanOrEqualTo(2));

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

            Assert.That(inserteds.Length, Is.EqualTo(count));
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

            Assert.That(affected, Is.GreaterThanOrEqualTo(2));

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

            Assert.That(count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task Should_Get_Sales_When_Pass_Null_as_Query_With_Pagination()
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

            Assert.That(affected, Is.GreaterThanOrEqualTo(2));

            var insertedCount = await repository.CountAsync(null);

            Assert.That(insertedCount, Is.EqualTo(10));

            var count = 0;

            await foreach (var item in repository.GetAsync(null, 1, 5))
                count++;

            Assert.That(count, Is.EqualTo(5));
        }

        [Test]
        public async Task Should_Get_Sales_When_Pass_a_Query_With_Pagination()
        {
            var repository = _manualDependencyInjection.GetService<IRepository<Sale>>();

            if (repository is null)
                Assert.Fail("Repository not created!");

            var insertedBy = Guid.NewGuid();
            var utcNow = DateTimeOffset.UtcNow;
            var inserteds = new HashSet<Sale>();

            for (var i = 0; i < 10; i++)
            {
                var entity = _CreateEntityWithProducts();
                var inserted = await repository.InsertAsync(entity, insertedBy);
                inserteds.Add(inserted);
            }

            var affected = await repository.SaveChangesAsync();

            Assert.That(affected, Is.GreaterThanOrEqualTo(10));

            var thirdElement = inserteds.ElementAt(3);
            var insertedCount = await repository.CountAsync(x => x.SaleCode == thirdElement.SaleCode);

            Assert.That(insertedCount, Is.AtMost(10));

            var count = 0;

            await foreach (var item in repository.GetAsync(x => x.SaleCode == thirdElement.SaleCode, 1, 5))
                count++;

            Assert.That(count, Is.AtMost(5));
        }
    }
}