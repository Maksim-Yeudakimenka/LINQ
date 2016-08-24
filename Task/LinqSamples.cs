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
    }
}