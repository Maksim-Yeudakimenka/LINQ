// Copyright © Microsoft Corporation.  All Rights Reserved.
// This code released under the terms of the 
// Microsoft Public License (MS-PL, http://opensource.org/licenses/ms-pl.html.)
//
//Copyright (C) Microsoft Corporation.  All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;
using SampleSupport;
using Task.Data;

// Version Mad01

namespace SampleQueries
{
    [Title("LINQ Module")]
    [Prefix("Linq")]
    public class LinqSamples : SampleHarness
    {

        private DataSource dataSource = new DataSource();

        [Category("Homework")]
        [Title("Task 1")]
        [Description("Customers with orders' total > X")]
        public void Linq1()
        {
            // Выдайте список всех клиентов, чей суммарный оборот (сумма всех заказов) превосходит некоторую величину X.
            // Продемонстрируйте выполнение запроса с различными X (подумайте, можно ли обойтись без копирования запроса несколько раз)

            Func<decimal, Func<Customer, bool>> minTotal =
                value => (customer => customer.Orders.Sum(order => order.Total) > value);

            var activeCustomers = dataSource.Customers.Where(minTotal(0M));
            var bigCustomers = dataSource.Customers.Where(minTotal(50000M));
            var hugeCustomers = dataSource.Customers.Where(minTotal(100000M));

            foreach (var customer in hugeCustomers)
            {
                ObjectDumper.Write(customer);
            }
        }

        [Category("Homework")]
        [Title("Task 2")]
        [Description("Suppliers from the same country and city as a customer")]
        public void Linq2()
        {
            // Для каждого клиента составьте список поставщиков, находящихся в той же стране и том же городе.
            // Сделайте задания с использованием группировки и без.

            var grouping =
                from customer in dataSource.Customers
                group customer by new { Customer = customer, customer.Country, customer.City }
                into customerGroup
                select new
                {
                    customerGroup.Key.Customer,
                    Suppliers =
                        from supplier in dataSource.Suppliers
                        where supplier.Country == customerGroup.Key.Country && supplier.City == customerGroup.Key.City
                        select supplier
                };

            var noGrouping = dataSource.Customers.ToDictionary(
                customer => customer,
                customer => dataSource.Suppliers
                    .Where(supplier => supplier.Country == customer.Country && supplier.City == customer.City));

            foreach (var item in noGrouping)
            {
                ObjectDumper.Write(item.Key);
                foreach (var supplier in item.Value)
                {
                    ObjectDumper.Write(supplier);
                }
            }
        }

        [Category("Homework")]
        [Title("Task 3")]
        [Description("Customers with order's total > X")]
        public void Linq3()
        {
            // Найдите всех клиентов, у которых были заказы, превосходящие по сумме величину X

            var minTotal = 300M;

            var customers = dataSource.Customers
                .Where(customer => customer.Orders
                    .Any(order => order.Total > minTotal));

            foreach (var customer in customers)
            {
                ObjectDumper.Write(customer);
            }
        }

        [Category("Homework")]
        [Title("Task 4")]
        [Description("Customers' start date")]
        public void Linq4()
        {
            // Выдайте список клиентов с указанием, начиная с какого месяца какого года они стали клиентами
            // (принять за таковые месяц и год самого первого заказа)

            var customers =
                from customer in dataSource.Customers
                where customer.Orders.Any()
                let firstOrder = customer.Orders.Min(order => order.OrderDate)
                select new { customer.CustomerID, customer.CompanyName, firstOrder.Month, firstOrder.Year };

            foreach (var customer in customers)
            {
                ObjectDumper.Write(customer);
            }
        }

        [Category("Homework")]
        [Title("Task 5")]
        [Description("Customers' start date, ordered")]
        public void Linq5()
        {
            // Сделайте предыдущее задание, но выдайте список отсортированным по году, месяцу,
            // оборотам клиента (от максимального к минимальному) и имени клиента

            var customers =
                from customer in dataSource.Customers
                where customer.Orders.Any()
                let firstOrder = customer.Orders.Min(order => order.OrderDate)
                select new
                {
                    customer.CustomerID,
                    customer.CompanyName,
                    Total = customer.Orders.Sum(order => order.Total),
                    firstOrder.Month,
                    firstOrder.Year
                }
                into customerInfo
                orderby customerInfo.Year, customerInfo.Month, customerInfo.Total descending, customerInfo.CompanyName
                select customerInfo;

            foreach (var customer in customers)
            {
                ObjectDumper.Write(customer);
            }
        }

        [Category("Homework")]
        [Title("Task 6")]
        [Description("Customers having empty data fields")]
        public void Linq6()
        {
            // Укажите всех клиентов, у которых указан нецифровой почтовый код или не заполнен регион 
            // или в телефоне не указан код оператора (для простоты считаем, что это равнозначно «нет круглых скобочек в начале»)

            var customers =
                from customer in dataSource.Customers
                where (!string.IsNullOrEmpty(customer.PostalCode) && customer.PostalCode.Any(c => !char.IsDigit(c))) ||
                      string.IsNullOrEmpty(customer.Region) ||
                      !customer.Phone.StartsWith("(")
                select customer;

            foreach (var customer in customers)
            {
                ObjectDumper.Write(customer);
            }
        }
    }
}