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
    [Route("cart")]
    public class CartController : Controller
    {
        [Route("index")]
        public IActionResult Index()
        {
            var cart = SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart");
            ViewBag.cart = cart;
           // ViewBag.total = cart.Sum(item => item.Quantity);
            return View();
        }

        [Route("buy")]
        public IActionResult Buy(string itemname, int quantity)
        {
            //ProductModel productModel = new ProductModel();
            if (SessionHelper.GetObjectFromJson<List<Item>>(HttpContext.Session, "cart") == null)
            {
                List<Item> cart = new List<Item>();
                Item i = new Item();
                i.ItemName = itemname;
                i.Quantity = quantity;
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
            return RedirectToAction("Index");
        }
    }
}