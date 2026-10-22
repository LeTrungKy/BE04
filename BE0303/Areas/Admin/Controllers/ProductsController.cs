using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BE0303.Models;
using BE0303.Models.ViewModel;
using PagedList;
using BE0303.Filters;

namespace BE0303.Areas.Admin.Controllers
{
    [AdminAuthorize]
    public class ProductsController : Controller
    {
        private MyStoreEntities db = new MyStoreEntities();

        // GET: Admin/Products

        public ActionResult Index(string searchTerm, decimal? minPrice, decimal? maxPrice, string sortOrder, int? page)
        {
            var model = new ProductSearchVM
            {
                SearchTerm = searchTerm,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortOrder = sortOrder,
                PageNumber = page ?? 1,
                PageSize = 6
            };

            var products = db.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model.SearchTerm = searchTerm;
                products = products.Where(p => p.ProductName.Contains(searchTerm)
                                            || p.ProductDescription.Contains(searchTerm)
                                            || p.Category.CategoryName.Contains(searchTerm));
            }

 
            if (minPrice.HasValue)
            {
                model.MinPrice = minPrice;
                products = products.Where(p => p.ProductPrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                model.MaxPrice = maxPrice;
                products = products.Where(p => p.ProductPrice <= maxPrice.Value);
            }

            model.SortOrder = sortOrder;
            switch (sortOrder)
            {
                case "name_asc":
                    products = products.OrderBy(p => p.ProductName);
                    break;
                case "name_desc":
                    products = products.OrderByDescending(p => p.ProductName);
                    break;
                case "price_asc":
                    products = products.OrderBy(p => p.ProductPrice);
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.ProductPrice);
                    break;
                default:
                    products = products.OrderBy(p => p.ProductID);
                    break;
            }

         
            model.PageNumber = page ?? 1;
            model.PageSize = 5;
            model.Products = products.ToPagedList(model.PageNumber, model.PageSize);

            return View(model);
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,UploadImg")] Product product)
        {
            ValidateImage(product);

            if (ModelState.IsValid)
            {
             
                if (string.IsNullOrEmpty(product.ProductImage))
                {
                    product.ProductImage = "~/Content/images/default_img.png";
                }

                SaveImage(product);
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
        
            ActionResult result = Details(id);
            ViewResult viewResult = result as ViewResult;
            if (viewResult != null)
            {
                Product product = (Product)viewResult.Model;
                ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            }
            return result;
        }

        // POST: Admin/Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,CategoryID,ProductName,ProductDescription,ProductPrice,ProductImage,UploadImg")] Product product)
        {
            ValidateImage(product);

            if (ModelState.IsValid)
            {
                SaveImage(product);
                db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            return Details(id);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        private void ValidateImage(Product product)
        {
            if (product.UploadImg != null && product.UploadImg.ContentLength > 0)
            {
                string ext = Path.GetExtension(product.UploadImg.FileName).ToLower();
                if (ext != ".png" && ext != ".jpg" && ext != ".jpeg" && ext != ".gif")
                {
                    ModelState.AddModelError("UploadImg", "Chỉ chấp nhận các định dạng ảnh PNG, JPG và GIF");
                }
            }
        }

        private void SaveImage(Product product)
        {
            if (product.UploadImg != null && product.UploadImg.ContentLength > 0)
            {
                string filename = Path.GetFileName(product.UploadImg.FileName);
                string savePath = "~/Content/images/";
                product.ProductImage = savePath + filename;
                product.UploadImg.SaveAs(Path.Combine(Server.MapPath(savePath), filename));
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}