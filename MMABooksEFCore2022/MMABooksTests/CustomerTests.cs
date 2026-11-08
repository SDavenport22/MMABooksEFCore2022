using System.Collections.Generic;
using System.Linq;
using System;

using NUnit.Framework;
using MMABooksEFClasses.MarisModels;
using Microsoft.EntityFrameworkCore;

namespace MMABooksTests
{
    [TestFixture]
    public class CustomerTests
    {
        MMABooksContext dbContext;
        Customer? c;
        List<Customer>? customers;

        [SetUp]
        public void Setup()
        {
            dbContext = new MMABooksContext();
            dbContext.Database.ExecuteSqlRaw("call usp_testingResetData()");
        }

        [Test]
        public void GetAllTest()
        {
            customers = dbContext.Customers
                .OrderBy(c => c.Name)
                .ToList();

            Assert.AreEqual(696, customers.Count);

            PrintAll(customers);
        }

        [Test]
        public void GetByPrimaryKeyTest()
        {
            c = dbContext.Customers.Find(20);

            Assert.IsNotNull(c);
            Assert.AreEqual(20, c.CustomerId);

            Console.WriteLine(c);
        }

        [Test]
        public void GetUsingWhere()
        {
            // get a list of all of the customers who live in OR

            customers = dbContext.Customers
                .Where(c => c.StateCode == "OR")
                .OrderBy(c => c.Name)
                .ToList();

            Assert.AreEqual(5, customers.Count);

            PrintAll(customers);
        }

        [Test]
        public void GetWithInvoicesTest()
        {
            // get the customer whose id is 20 and all of the invoices

            c = dbContext.Customers
                .Include("Invoices")
                .Where(c => c.CustomerId == 20)
                .SingleOrDefault();

            Assert.IsNotNull(c);

            Console.WriteLine(c);

            foreach (Invoices i in c.Invoices)
            {
                Console.WriteLine(i);
            }
        }

        [Test]
        public void GetWithJoinTest()
        {
            // get a list of objects that include the customer id, name, statecode and statename

            var customers = dbContext.Customers.Join(
                dbContext.States,
                c => c.StateCode,
                s => s.StateCode,
                (c, s) => new
                {
                    c.CustomerId,
                    c.Name,
                    c.StateCode,
                    s.StateName
                })
                .OrderBy(r => r.StateName)
                .ToList();

            Assert.AreEqual(696, customers.Count);

            foreach (var c in customers)
            {
                Console.WriteLine(c);
            }
        }

        [Test]
        public void DeleteTest()
        {
            c = dbContext.Customers.Find(20);

            Assert.IsNotNull(c);

            dbContext.Customers.Remove(c);

            dbContext.SaveChanges();

            Assert.IsNull(dbContext.Customers.Find(20));
        }

        [Test]
        public void CreateTest()
        {
            Customer customer = new Customer
            {
                Name = "Test Customer",
                Address = "123 Main St",
                City = "Klamath Falls",
                StateCode = "OR",
                ZipCode = "97601"
            };

            dbContext.Customers.Add(customer);

            dbContext.SaveChanges();

            Customer? addedCustomer =
                dbContext.Customers.Find(customer.CustomerId);

            Assert.IsNotNull(addedCustomer);
            Assert.AreEqual("Test Customer", addedCustomer.Name);
        }

        [Test]
        public void UpdateTest()
        {
            c = dbContext.Customers.Find(20);

            Assert.IsNotNull(c);

            c.Name = "Updated Customer";

            dbContext.SaveChanges();

            Customer? updatedCustomer =
                dbContext.Customers.Find(20);

            Assert.AreEqual(
                "Updated Customer",
                updatedCustomer.Name);
        }

        public void PrintAll(List<Customer> customers)
        {
            foreach (Customer c in customers)
            {
                Console.WriteLine(c);
            }
        }
    }
}