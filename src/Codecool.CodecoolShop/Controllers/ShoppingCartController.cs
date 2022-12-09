using Codecool.CodecoolShop.Models;
using Codecool.CodecoolShop.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Codecool.CodecoolShop.Controllers
{
    public class ShoppingCartController : Controller
    {

        public Product product = new Product();
        private readonly ShoppingCartRepository shoppingCartRepository;

        public List<Product> products= new List<Product>();



        public ShoppingCartController(ShoppingCartRepository shoppingCartRepository)
        {
            
           
            this.shoppingCartRepository = shoppingCartRepository;
         
        }


        [Route("/ShoppingCart")]
        public IActionResult Index()
        {
            ViewBag.id = HttpContext.Session.GetString("id");
            return View(shoppingCartRepository.GetAllProductsfromCart(ViewBag.id));
        }


        

        

        public IActionResult Checkout()
        {
            return View("Checkout", shoppingCartRepository.GetAllProductsfromCart(ViewBag.id = HttpContext.Session.GetString("id")));
        }


        public IActionResult DecreaseQuantity(Guid id)
        {
            shoppingCartRepository.DecreaseQuantity(id);
            return Redirect("/ShoppingCart");
        }

        public IActionResult IncreaseQuantity(Guid id)
        {
            shoppingCartRepository.IncreaseQuantity(id);
            return Redirect("/ShoppingCart");
        }


    }
}
