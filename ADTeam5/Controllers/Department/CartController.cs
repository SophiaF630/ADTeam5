using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ADTeam5.Helper;
using ADTeam5.Models;

namespace ADTeam5.Controllers.Department
{
   
    public class CartController : Controller
    {
        private readonly SSISTeam5Context _context;

        public CartController(SSISTeam5Context context)
        {
            _context = context;
        }


        public IActionResult Index()

        {
            List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
            // ViewBag.total = cart.Sum(item => item.Quantity);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Buy([Bind("ItemName,Quantity")] Item item)
        {
            //ProductModel productModel = new ProductModel();
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                Item i = new Item();
                i.ItemName = item.ItemName;
                i.Quantity = item.Quantity;

                List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
                cart.Add(i);
                SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }
            //else
            //{
            //    List<Item> cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            //    int index = isExist(id);
            //    if (index != -1)
            //    {
            //        cart[index].Quantity++;
            //    }
            //    else
            //    {
            //        cart.Add(new Item { Product = productModel.find(id), Quantity = 1 });
            //    }
            //    SessionHelper.SetObjectAsJson(HttpContext.Session, "cart", cart);
            //}
            return View("Index");
        }
    }
}