﻿using System;
using System.Collections;
using System.Collections.Generic;
using FlixOne.Web.Common;
using FlixOne.Web.Models;
using FlixOne.Web.Persistence;
using Microsoft.AspNetCore.Mvc;

namespace FlixOne.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IInventoryRepository _repository;

        public ProductController(IInventoryRepository inventoryRepository) => _repository = inventoryRepository;

        public IActionResult Index([FromQuery]Sort sort, string searchTerm)
        {
            ViewData["cSort"] = sort.Order;
            ViewData["cSort"] = (SortOrder)ViewData["cSort"] == SortOrder.A ? SortOrder.D : sort.Order;
            var products = _repository.GetProducts(sort, searchTerm);
            return View(products.ToProductvm());
        }

        public IActionResult Details(Guid id) => View(_repository.GetProduct(id).ToProductvm());

        public IActionResult Create() => View();
        public IActionResult Report()
        {
            var mango = _repository.GetProduct(new Guid("09C2599E-652A-4807-A0F8-390A146F459B"));
            var apple = _repository.GetProduct(new Guid("7AF8C5C2-FA98-42A0-B4E0-6D6A22FC3D52"));
            var orange = _repository.GetProduct(new Guid("E2A8D6B3-A1F9-46DD-90BD-7F797E5C3986"));
            var model = new List<MessageViewModel>();
            //provider
            ProductRecorder productProvider = new ProductRecorder();
            //observer1
            ProductReporter productObserver1 = new ProductReporter(nameof(mango));
            //observer2
            ProductReporter productObserver2 = new ProductReporter(nameof(apple));
            //observer3
            ProductReporter productObserver3 = new ProductReporter(nameof(orange));

            //subssribe
            productObserver1.Subscribe(productProvider);
            productObserver2.Subscribe(productProvider);
            productObserver3.Subscribe(productProvider);

            //Report and Unsubscribe
            productProvider.Record(mango);
            model.AddRange(productObserver1.Reporter);
            productObserver1.Unsubscribe();
            productProvider.Record(apple);
            model.AddRange(productObserver2.Reporter);
            productObserver2.Unsubscribe();
            productProvider.Record(orange);
            model.AddRange(productObserver3.Reporter);
            productObserver3.Unsubscribe();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([FromBody] Product product)
        {
            try
            {
                _repository.AddProduct(product);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Edit(Guid id) => View(_repository.GetProduct(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Guid id, [FromBody] Product product)
        {
            try
            {
                _repository.UpdateProduct(product);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Delete(Guid id) => View(_repository.GetProduct(id));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(Guid id, [FromBody] Product product)
        {
            try
            {
                _repository.RemoveProduct(product);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}