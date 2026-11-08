using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class ProductTests
    {
        Products? p;
        List<Products>? products;
        MMABooksContext dbContext;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            products = dbContext.Products
                .OrderBy(p => p.ProductCode)
                .ToList();

            Assert.AreEqual(16, products.Count);

            PrintAll(products);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            p = dbContext.Products.Find("A4CS");

            Assert.IsNotNull(p);
            Assert.AreEqual("A4CS", p.ProductCode);

            Console.WriteLine(p);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the products that have a unit price of 56.50

            products = dbContext.Products
                .Where(p => p.UnitPrice == 56.50m)
                .OrderBy(p => p.ProductCode)
                .ToList();

            Assert.IsTrue(products.Count > 0);

            PrintAll(products);
        }

        [Test]
        public void GetWithCalculatedFieldTest()
        {
            var products = dbContext.Products
                .Select(p => new
                {
                    p.ProductCode,
                    p.UnitPrice,
                    p.OnHandQuantity,
                    Value = p.UnitPrice * p.OnHandQuantity
                })
                .OrderBy(p => p.ProductCode)
                .ToList();

            Assert.AreEqual(16, products.Count);

            foreach (var p in products)
            {
                Console.WriteLine(p);
            }
        }

        [Test]
        public void DeleteTest()
        {
            Products product = new Products
            {
                ProductCode = "DELETE",
                Description = "Delete Me",
                UnitPrice = 10.00m,
                OnHandQuantity = 10
            };

            dbContext.Products.Add(product);
            dbContext.SaveChanges();

            Products? p = dbContext.Products.Find("DELETE");

            Assert.IsNotNull(p);

            dbContext.Products.Remove(p);
            dbContext.SaveChanges();

            Assert.IsNull(
                dbContext.Products.Find("DELETE"));
        }

        [Test]
        public void CreateTest()
        {
            Products product = new Products
            {
                ProductCode = "TEST",
                Description = "Test Product",
                UnitPrice = 10.99m,
                OnHandQuantity = 100
            };

            dbContext.Products.Add(product);

            dbContext.SaveChanges();

            Products? addedProduct =
                dbContext.Products.Find("TEST");

            Assert.IsNotNull(addedProduct);
            Assert.AreEqual(
                "Test Product",
                addedProduct.Description);
        }

        [Test]
        public void UpdateTest()
        {
            p = dbContext.Products.Find("A4CS");

            Assert.IsNotNull(p);

            p.Description = "Updated Product";

            dbContext.SaveChanges();

            Products? updatedProduct =
                dbContext.Products.Find("A4CS");

            Assert.AreEqual(
                "Updated Product",
                updatedProduct.Description);
        }

        public void PrintAll(List<Products> products)
        {
            foreach (Products p in products)
            {
                Console.WriteLine(p);
            }
        }
    }
}