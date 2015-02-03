using Grikwa.Models;
using Microsoft.Web.Helpers;
using System;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Drawing.Imaging;
using System.Net;
using System.Threading.Tasks;
using System.Globalization;
using System.Diagnostics;

namespace Grikwa.Controllers
{
    [Authorize]
    public class NoticeBoardController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        //
        // GET: /NoticeBoard/
        public async Task<ActionResult> Index(int id = 1)
        {
            // set pagination filter url
            PaginationModel.FilterURL = Url.Action("Index", "NoticeBoard") + "?id=";

            // get current institution if set
            if (Session != null && Session["currentInstitution"] != null)
            {
                // get institution name
                var name = (string)Session["currentInstitution"];

                // get all products in this institution
                var ps = from p in db.Products
                         where p.User.Institution.abbreviation.Equals(name) && p.ProductIntention==ProductIntention.SELL && p.Visible == true
                         select new CatalogProductModel()
                         {
                             ProductID = p.ProductID,
                             Name = p.Name,
                             UserID = p.UserID,
                             UserName = p.User.UserName,
                             UserFullName = p.User.TitleID + " " + p.User.Intials + " " + p.User.Surname,
                             Price = p.Price,
                             ShortDescription = p.ShortDescription,
                             ProductStatus = p.ProductStatus,
                             ProductIntention = p.ProductIntention,
                             DatePosted = p.DatePosted
                         };
                var ps2 = from p in db.Products
                          where p.User.Institution.abbreviation.Equals(name) && p.ProductIntention == ProductIntention.NOTIFY && p.Visible == true
                         select new CatalogProductModel()
                         {
                             ProductID = p.ProductID,
                             Name = p.Name,
                             UserID = p.UserID,
                             UserName = p.User.UserName,
                             UserFullName = p.User.TitleID + " " + p.User.Intials + " " + p.User.Surname,
                             Price = p.Price,
                             ShortDescription = p.ShortDescription,
                             ProductStatus = p.ProductStatus,
                             ProductIntention = p.ProductIntention,
                             DatePosted = p.DatePosted
                         };

                // setup pagination
                PaginationModel.TotalItems = await ps.CountAsync();
                var pageIndex = PaginationModel.GoToPage(id) - 1;

                SetFilters();
                var unpendingPosters = await ps2.OrderByDescending(p => p.DatePosted).Skip(pageIndex * PaginationModel.PosterPageSize).Take(PaginationModel.PosterPageSize).ToListAsync();
                var n = unpendingPosters.Count;
                var unpendingProducts = await ps.OrderByDescending(p => p.DatePosted).Skip(pageIndex * (PaginationModel.PageSize - n)).Take(PaginationModel.PageSize - n).ToListAsync();
                unpendingPosters.AddRange(unpendingProducts);
                return View(await SetPendingProducts(unpendingPosters));
            }

            // get all products at the default institution ("UCT")
            var products = from p in db.Products
                           where p.User.Institution.abbreviation.Equals("UCT") && p.Visible == true
                           select new CatalogProductModel()
                           {
                               ProductID = p.ProductID,
                               Name = p.Name,
                               UserID = p.UserID,
                               UserName = p.User.UserName,
                               UserFullName = p.User.TitleID + " " + p.User.Intials + " " + p.User.Surname,
                               Price = p.Price,
                               ShortDescription = p.ShortDescription,
                               ProductStatus = p.ProductStatus,
                               DatePosted = p.DatePosted
                           };

            // setup pagination
            PaginationModel.TotalItems = await products.CountAsync();
            var pageIndex2 = PaginationModel.GoToPage(id) - 1;
            SetFilters();
            Session.Add("currentInstitution", "UCT");

            var unpendingProducts2 = await products.OrderByDescending(p => p.DatePosted).Skip(pageIndex2 * PaginationModel.PageSize).Take(PaginationModel.PageSize).ToListAsync();

            return View(await SetPendingProducts(unpendingProducts2));
        }

        private async Task<List<CatalogProductModel>> SetPendingProducts(List<CatalogProductModel> products)
        {
            // check for pending products
            //if (Request.IsAuthenticated)
            //{
            //    var userName = User.Identity.Name;
            //    var pendingProducts = await (from p in db.ConversationRoomProducts
            //                                 where !(p.Product.User.UserName.Equals(userName))
            //                                 && (p.ConversationRoom.User1.UserName.Equals(userName) || p.ConversationRoom.User2.UserName.Equals(userName))
            //                                 select p.ProductID).ToListAsync();

            //    foreach (var product in products)
            //    {
            //        if (pendingProducts.Contains(product.ProductID))
            //        {
            //            product.ProductStatus = ProductStatus.REQUESTED;
            //        }
            //    }
            //}

            return products;
        }

        public async Task<ActionResult> Category(string category, int page = 1)
        {
            // bad request
            if (category == null)
            {
                return RedirectToAction("Index", "NoticeBoard");
            }

            // get current institution
            string name = GetCurrentInstitution();

            // set pagination returnURL
            PaginationModel.FilterURL = Url.Action("Category", "NoticeBoard") + "?category=" + category + "&page=";

            // get catalog product in a specific category
            var products = from p in db.ProductCategories
                           where p.Product.User.Institution.abbreviation.Equals(name) && p.Category.Code.Equals(category) && p.Product.Visible == true
                           select new CatalogProductModel()
                           {
                               ProductID = p.Product.ProductID,
                               Name = p.Product.Name,
                               UserID = p.Product.UserID,
                               UserName = p.Product.User.UserName,
                               UserFullName = p.Product.User.TitleID + " " + p.Product.User.Intials + " " + p.Product.User.Surname,
                               Price = p.Product.Price,
                               ShortDescription = p.Product.ShortDescription,
                               ProductStatus = p.Product.ProductStatus,
                               DatePosted = p.Product.DatePosted,
                               Offers = (from cp in db.ConversationRoomProducts
                                         where cp.ProductID == p.ProductID
                                         select cp).Count()
                           };

            PaginationModel.TotalItems = await products.CountAsync();
            var pageIndex = PaginationModel.GoToPage(page) - 1;

            // set filters
            SetFilters();

            var unpendingProducts = await products.OrderByDescending(p => p.DatePosted).Skip(pageIndex * PaginationModel.PageSize).Take(PaginationModel.PageSize).ToListAsync();
            return View("Index", await SetPendingProducts(unpendingProducts));
        }

        private string GetCurrentInstitution()
        {
            string name = "UCT";
            if (Session != null && Session["currentInstitution"] != null)
            {
                name = (string)Session["currentInstitution"];
            }
            else
            {
                Session.Add("currentInstitution", name);
            }

            return name;
        }

        private void SetFilters()
        {
            if (Session != null && Session["categories"] == null)
            {
                Session.Add("categories", (from c in db.Categories
                                           select c).ToList());
            }
        }

        public async Task<ActionResult> Search(string query, int page = 1)
        {

            // bad request
            if (query == null || query.Length == 0)
            {
                return RedirectToAction("Index", "NoticeBoard");
            }

            // set pagination returnURL
            PaginationModel.FilterURL = Url.Action("Search", "NoticeBoard") + "?query=" + query + "&page=";

            // current institution
            string name = GetCurrentInstitution();

            // get all products that match the serach query
            var ps = from p in db.Products
                     where p.User.Institution.abbreviation.Equals(name) && p.Visible == true && 
                           (p.Name.Contains(query) ||
                            p.ShortDescription.Contains(query) ||
                            p.LongDescription.Contains(query) ||
                            p.KeyWords.Contains(query))
                     select new CatalogProductModel()
                     {
                         ProductID = p.ProductID,
                         Name = p.Name,
                         UserID = p.UserID,
                         UserName = p.User.UserName,
                         UserFullName = p.User.TitleID + " " + p.User.Intials + " " + p.User.Surname,
                         Price = p.Price,
                         ShortDescription = p.ShortDescription,
                         ProductStatus = p.ProductStatus,
                         DatePosted = p.DatePosted,
                         Offers = (from cp in db.ConversationRoomProducts
                                   where cp.ProductID == p.ProductID
                                   select cp).Count()
                     };

            // setup pagination
            PaginationModel.TotalItems = await ps.CountAsync();
            var pageIndex = PaginationModel.GoToPage(page) - 1;
            SetFilters();

            // get unpending products
            var products = await ps.OrderByDescending(p => p.DatePosted).Skip(pageIndex * PaginationModel.PageSize).Take(PaginationModel.PageSize).ToListAsync();

            return View("Index", await SetPendingProducts(products));
        }

        public async Task<ActionResult> BusinessCard(string id, int page = 1)
        {
            // bad request
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // get pagination
            PaginationModel.FilterURL = Url.Action("BusinessCard", "NoticeBoard") + "?id=" + id + "&page=";

            var businessCard = from u in db.Users
                               where u.Id.Equals(id)
                               select new BusinessCardModel()
                               {
                                   Id = u.Id,
                                   Name = u.TitleID + " " + u.Intials + " " + u.Surname,
                                   Qualification = "(" + u.Qualification.Code + ") " + u.Qualification.FullName,
                                   Institution = u.Institution.Name,
                                   HasPicture = u.Picture == null ? false : true
                               };

            var count = await businessCard.CountAsync();
            if (count < 1)
            {
                return HttpNotFound("The Business Card request was not found");
            }

            // get user of first business card
            var user = await businessCard.FirstAsync();

            // get all the user's products
            var products = from p in db.Products
                           where p.User.Id.Equals(id) && p.User.Institution.Name == user.Institution && p.Visible == true
                           select new CatalogProductModel()
                            {
                                ProductID = p.ProductID,
                                Name = p.Name,
                                UserID = p.UserID,
                                UserName = p.User.UserName,
                                UserFullName = p.User.TitleID + " " + p.User.Intials + " " + p.User.Surname,
                                Price = p.Price,
                                ShortDescription = p.ShortDescription,
                                ProductStatus = p.ProductStatus,
                                DatePosted = p.DatePosted,
                                Offers = (from cp in db.ConversationRoomProducts
                                          where cp.ProductID == p.ProductID
                                          select cp).Count()
                            };

            // setup pagination
            PaginationModel.TotalItems = await products.CountAsync();
            var pageIndex = PaginationModel.GoToPage(page) - 1;

            // get unpending user's products
            var unpendingProducts = await products.OrderByDescending(p => p.DatePosted).Skip(pageIndex * PaginationModel.PageSize).Take(PaginationModel.PageSize).ToListAsync();
            user.Products = await SetPendingProducts(unpendingProducts);

            return View(user);
        }

        public ActionResult Institution(string name)
        {
            Session.Add("currentInstitution", name);
            SetFilters();

            return RedirectToAction("Index", "NoticeBoard");
        }

        public async Task<ActionResult> AdvertImage(int? id, int? sizeType)
        {
            if (id == null || sizeType == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            using (var db1 = new ApplicationDbContext())
            {

                // get the product
                var product = await db1.Products.FindAsync(id);

                if (product == null)
                {
                    return HttpNotFound("Image was not found"); // image was not found
                }

                byte[] imageBytes = null;
                if (sizeType == 1) // thumbnail
                {
                    imageBytes = product.ThumbnailImage;
                }
                else if (sizeType == 2) // big image
                {
                    imageBytes = product.FullSizeImage;
                }

                return File(imageBytes, "image/png");
            }
        }

        /// <summary>
        /// Get the MultiSelectList of categories
        /// </summary>
        /// <param name="selectedValues"></param>
        /// <returns></returns>
        private async Task<MultiSelectList> GetCategories(IEnumerable<int> selectedValues)
        {
            var categories = await db.Categories.ToListAsync();
            return new MultiSelectList(categories, "CategoryID", "Name", selectedValues);
        }

        public ActionResult PostAdvert()
        {
            return View();
        }

        public async Task<ActionResult> Notify()
        {
            ViewBag.Categories = await GetCategories(null);
            return View();
        }

        //
        // GET: /NoticeBoard/SellProduct
        public async Task<ActionResult> Sell()
        {
            ViewBag.Categories = await GetCategories(null);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Notify([Bind(Include = "PosterImage,Name,Categories,WebsiteLink,Email,PhoneNumber,Description,AcceptTerms,KeyWords")] NotifyPosterModel poster)
        {
            if (ModelState.IsValid)
            {
                if (poster.AcceptTerms)
                {
                    // get images
                    byte[] fullSizeImage = null;
                    byte[] thumbnailImage = null;
                    try
                    {
                        System.Drawing.Image originalImage = System.Drawing.Image.FromStream(poster.PosterImage.InputStream);
                        System.Drawing.Image.GetThumbnailImageAbort callback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallBack);
                        System.Drawing.Image fullImg = originalImage.GetThumbnailImage(500, 500, callback, IntPtr.Zero);
                        System.Drawing.Image thumbNailImg = originalImage.GetThumbnailImage(300, 300, callback, IntPtr.Zero);

                        // get large image
                        MemoryStream ms = new MemoryStream();
                        fullImg.Save(ms, ImageFormat.Png);
                        fullSizeImage = ms.ToArray();
                        ms.Dispose();

                        // get small image
                        MemoryStream ms2 = new MemoryStream();
                        thumbNailImg.Save(ms2, ImageFormat.Png);
                        thumbnailImage = ms2.ToArray();
                        ms2.Dispose();

                        // free memory
                        thumbNailImg.Dispose();
                        fullImg.Dispose();
                        originalImage.Dispose();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message, "Product Image Creation Failed During Sell.");
                    }

                    // check if user exist
                    var userInfo = db.Users.Where(u => u.UserName == User.Identity.Name);
                    var count = await userInfo.CountAsync();
                    if (count < 1)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Your Advert posting was unsuccessful because you are not logged in.");
                    }

                    // get user who is posting the ad
                    var user = await userInfo.FirstAsync();

                    // check if this poster already exists
                    var posterExist = (from p in db.Products
                                        where p.User.Id.Equals(user.Id) && p.Name.Equals(poster.Name)
                                        select p).Count() > 0;

                    if (posterExist)
                    {
                        ModelState.AddModelError("Name", "You already have a poster with this name. Type a different name.");
                    }
                    else
                    {

                        // set text info to be able to capitalize the poster name
                        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                        // create poster
                        Product newPoster = new Product()
                        {
                            Name = ti.ToTitleCase(poster.Name),
                            LongDescription = poster.Description,
                            ContactEmail = poster.Email,
                            ContactNumber = poster.PhoneNumber,
                            ProductStatus = ProductStatus.NEW,
                            DatePosted = DateTime.Now,
                            KeyWords = poster.KeyWords,
                            User = user,
                            WebsiteLink = poster.WebsiteLink,
                            ProductIntention = ProductIntention.NOTIFY,
                            ThumbnailImage = thumbnailImage,
                            FullSizeImage = fullSizeImage,
                            Visible = true
                        };
                        db.Products.Add(newPoster);

                        // save categories
                        foreach (var categoryID in poster.Categories)
                        {
                            var category = await (from c in db.Categories
                                                  where c.CategoryID == categoryID
                                                  select c).FirstAsync();
                            db.ProductCategories.Add(new ProductCategory() { Category = category, Product = newPoster });
                        }

                        // save changes
                        await db.SaveChangesAsync();

                        // get categories
                        SetFilters();

                        return RedirectToAction("Index", "NoticeBoard");
                    }
                }
                else
                {
                    ModelState.AddModelError("AcceptTerms", "please accept Terms and Conditions");
                }
            }

            ViewBag.Categories = await GetCategories(poster.Categories);

            // If we got this far, something failed, redisplay form
            return View(poster);
        }

        // POST: /NoticeBoard/SellProduct?=
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Sell([Bind(Include = "ProductImage,Name,Categories,Email,PhoneNumber,ShortDescription,LongDescription,Price,AcceptTerms,KeyWords")] SellProductModel product)
        {
            if (ModelState.IsValid)
            {
                if (product.AcceptTerms)
                {
                    // get images
                    byte[] fullSizeImage = null;
                    byte[] thumbnailImage = null;
                    try
                    {
                        System.Drawing.Image originalImage = System.Drawing.Image.FromStream(product.ProductImage.InputStream);
                        System.Drawing.Image.GetThumbnailImageAbort callback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallBack);
                        System.Drawing.Image fullImg = originalImage.GetThumbnailImage(500, 500, callback, IntPtr.Zero);
                        System.Drawing.Image thumbNailImg = originalImage.GetThumbnailImage(300, 300, callback, IntPtr.Zero);

                        // get large image
                        MemoryStream ms = new MemoryStream();
                        fullImg.Save(ms, ImageFormat.Png);
                        fullSizeImage = ms.ToArray();
                        ms.Dispose();

                        // get small image
                        MemoryStream ms2 = new MemoryStream();
                        thumbNailImg.Save(ms2, ImageFormat.Png);
                        thumbnailImage = ms2.ToArray();
                        ms2.Dispose();

                        // free memory
                        thumbNailImg.Dispose();
                        fullImg.Dispose();
                        originalImage.Dispose();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message, "Product Image Creation Failed During Sell.");
                    }

                    // check if user exist
                    var userInfo = db.Users.Where(u => u.UserName == User.Identity.Name);
                    var count = await userInfo.CountAsync();
                    if (count < 1)
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.Forbidden, "Your Ad posting was unsuccessful because you are not logged in.");
                    }

                    // get user who is posting the ad
                    var user = await userInfo.FirstAsync();

                    // check if this product already exists
                    var productExist = (from p in db.Products
                                        where p.User.Id.Equals(user.Id) && p.Name.Equals(product.Name)
                                        select p).Count() > 0;

                    if (productExist)
                    {
                        ModelState.AddModelError("Name", "You already have a product with this name. Type a different name.");
                    }
                    else
                    {

                        // set text info to be able to capitalize the product name
                        TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                        // create product
                        Product newProduct = new Product()
                        {
                            Name = ti.ToTitleCase(product.Name),
                            ShortDescription = product.ShortDescription,
                            LongDescription = product.LongDescription,
                            ContactEmail = product.Email,
                            ContactNumber = product.PhoneNumber,
                            Price = product.Price,
                            ProductStatus = ProductStatus.NEW,
                            DatePosted = DateTime.Now,
                            ProductIntention = ProductIntention.SELL,
                            KeyWords = product.KeyWords,
                            User = user,
                            Visible = true,
                            ThumbnailImage = thumbnailImage,
                            FullSizeImage = fullSizeImage
                        };
                        db.Products.Add(newProduct);

                        // save categories
                        foreach (var categoryID in product.Categories)
                        {
                            var category = await (from c in db.Categories
                                                  where c.CategoryID == categoryID
                                                  select c).FirstAsync();
                            db.ProductCategories.Add(new ProductCategory() { Category = category, Product = newProduct });
                        }

                        // save changes
                        await db.SaveChangesAsync();

                        // get categories
                        SetFilters();

                        return RedirectToAction("Index", "NoticeBoard");
                    }
                }
                else
                {
                    ModelState.AddModelError("AcceptTerms", "please accept Terms and Conditions");
                }
            }

            ViewBag.Categories = await GetCategories(product.Categories);

            // If we got this far, something failed, redisplay form
            return View(product);
        }

        private bool ThumbnailCallBack()
        {
            return false;
        }

        //
        // GET: /NoticeBoard/BuyProduct
        //remove later
        public async Task<ActionResult> Get(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var customer = await (from u in db.Users
                                  where u.UserName.Equals(User.Identity.Name)
                                  select u).FirstAsync();

            var product = await db.Products.FindAsync(id);

            if (product == null || product.ProductStatus == ProductStatus.SOLD || product.Visible == false)
            {
                return HttpNotFound("The product you want to buy was just bought by someone else or does not exist anymore.");
            }
            else if (customer.InstitutionID != product.User.InstitutionID)
            {
                return View("Forbidden", new ForbiddenMessageModel()
                            {
                                Title = "Sale Request Not Allowed",
                                Message = "You can only enquire about or get items from the community you belong."
                            });
            }

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Get([Bind(Include = "ProductID,RequestMessage")] GetModel model)
        {

            if (ModelState.IsValid)
            {
                var product = db.Products.Find(model.ProductID);
                if (product == null || product.ProductStatus == ProductStatus.SOLD || product.Visible == false)
                {
                    return HttpNotFound("The product you want to buy was just bought by someone else or does not exist anymore.");
                }
                var customer = db.Users.First(x => x.UserName == User.Identity.Name);
                var supplierEmail = (string.IsNullOrEmpty(product.ContactEmail) || string.IsNullOrWhiteSpace(product.ContactEmail)) ? product.User.Email : product.ContactEmail;
                NotificationsHelper.SendSaleRequestEmail(supplierEmail, customer.Email, product.Name, model.RequestMessage);
                return RedirectToAction("SaleRequestSent", "NoticeBoard");
            }

            return RedirectToAction("Index", "NoticeBoard");
        }

        public ActionResult SaleRequestSent()
        {
            return View();
        }

        public ActionResult Chat()
        {
            var products = from p in db.Products
                           where p.User.UserName.Equals(User.Identity.Name) && p.ProductStatus != ProductStatus.SOLD && p.Visible == true
                           select new
                           {
                               ProductID = p.ProductID,
                               Name = p.Name
                           };

            SoldProductModel product = new SoldProductModel();
            product.Products = new SelectList(products.ToList(), "ProductID", "Name");
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SellProduct([Bind(Include = "ProductID,CustomerID")] SoldProductModel product)
        {

            if (ModelState.IsValid)
            {
                if (product.ProductID > 0)
                {
                    if (product.CustomerID.Equals("OutsideGrikwa")) // product was sold outside of grikwa
                    {
                        var confirmModel = await (from p in db.Products
                                                  where p.ProductID == product.ProductID
                                                  select new ConfirmProductSaleModel()
                                                  {
                                                      CustomerID = product.CustomerID,
                                                      ProductID = product.ProductID,
                                                      ProductName = p.Name,
                                                      FullName = "Someone Outside Grikwa"
                                                  }).FirstAsync();

                        return View("ConfirmSale", confirmModel);
                    }
                    else // product was sold inside grikwa
                    {
                        var confirmModel = await (from cr in db.ConversationRoomProducts
                                                  where cr.Product.ProductID == product.ProductID &&
                                                  (cr.ConversationRoom.User1.Id.Equals(product.CustomerID) || cr.ConversationRoom.User2.Id.Equals(product.CustomerID))
                                                  select new ConfirmProductSaleModel()
                                                  {
                                                      CustomerID = product.CustomerID,
                                                      ProductID = product.ProductID,
                                                      ProductName = cr.Product.Name,
                                                      FullName = (cr.ConversationRoom.User1.Id.Equals(product.CustomerID)) ?
                                                                 cr.ConversationRoom.User1.TitleID + " "
                                                                 + cr.ConversationRoom.User1.Intials + " "
                                                                 + cr.ConversationRoom.User1.Surname :
                                                                 cr.ConversationRoom.User2.TitleID + " "
                                                                 + cr.ConversationRoom.User2.Intials + " "
                                                                 + cr.ConversationRoom.User2.Surname
                                                  }).FirstAsync();

                        return View("ConfirmSale", confirmModel);
                    }
                }
                else
                {
                    ModelState.AddModelError("ProductID", "Please select the product to sell");
                }
            }

            var products = from p in db.Products
                           where p.User.UserName.Equals(User.Identity.Name) && p.ProductStatus != ProductStatus.SOLD
                           select new
                           {
                               ProductID = p.ProductID,
                               Name = p.Name
                           };
            product.ProductID = 0;
            product.Products = new SelectList(await products.ToListAsync(), "ProductID", "Name");
            return View("Chat", product);
        }

        // POST: /NoticeBoard/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaleConfirmed([Bind(Include = "CustomerID,ProductID")] ConfirmProductSaleModel model)
        {
            if (ModelState.IsValid)
            {
                // Get product been sold
                var product = await db.Products.FindAsync(model.ProductID);
                product.ProductStatus = ProductStatus.SOLD;
                db.Entry(product).State = EntityState.Modified;

                if (model.CustomerID.Equals("OutsideGrikwa")) // product was sold outside of grikwa
                {

                }
                else // product was sold inside grikwa
                {
                    // Get customer info
                    var user = db.Users.Find(model.CustomerID);

                    // create sale transaction
                    Sale sale = new Sale() { Customer = user, Product = product, SaleDate = DateTime.Now };
                    db.Sales.Add(sale);
                }

                // Broadcast sale to all pending users
                var userName = User.Identity.Name;
                var allPendingUsersMesages = from cp in db.ConversationRoomProducts
                                             where cp.Product.ProductID == model.ProductID
                                             select new
                                             {
                                                 ToUser = (cp.ConversationRoom.User1.UserName.Equals(userName)) ? cp.ConversationRoom.User2 : cp.ConversationRoom.User1,
                                                 FromUser = (cp.ConversationRoom.User1.UserName.Equals(userName)) ? cp.ConversationRoom.User1 : cp.ConversationRoom.User2,
                                                 MessageStatus = MessageStatus.UNREAD,
                                                 Time = DateTime.Now,
                                                 Text = "(Regarding product: " + cp.Product.Name + ") This product was just sold "
                                                        + ((cp.ConversationRoom.User1ID.Equals(model.CustomerID) || cp.ConversationRoom.User2ID.Equals(model.CustomerID))
                                                        ? "to you." : "to someone else.")
                                             };
                foreach (var message in allPendingUsersMesages)
                {
                    db.Conversations.Add(new Conversation()
                    {
                        FromUser = message.FromUser,
                        ToUser = message.ToUser,
                        MessageStatus = message.MessageStatus,
                        Text = message.Text,
                        Time = message.Time
                    });
                }

                // Delete all pending user negotiations
                var negotiations = db.ConversationRoomProducts.Include(c => c.ConversationRoom).Where(c => c.ProductID == model.ProductID);
                db.ConversationRoomProducts.RemoveRange(negotiations);

                // Save changes
                await db.SaveChangesAsync();

                // Go back to transaction page
                return RedirectToAction("Chat");
            }

            return RedirectToAction("Index");
        }

        public async Task<ActionResult> PendingUsers(int? productID)
        {

            // bad request
            if (productID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // get current username
            var userName = User.Identity.Name;

            // get pending users of this specific product
            var users = from cp in db.ConversationRoomProducts
                        where cp.Product.ProductID == productID
                        select new PendingUser()
                        {
                            UserID = (cp.ConversationRoom.User1.UserName.Equals(userName)) ? cp.ConversationRoom.User2ID : cp.ConversationRoom.User1ID,
                            FullName = (cp.ConversationRoom.User1.UserName.Equals(userName)) ?
                                     cp.ConversationRoom.User2.TitleID + " " + cp.ConversationRoom.User2.Intials + " " + cp.ConversationRoom.User2.Surname :
                                     cp.ConversationRoom.User1.TitleID + " " + cp.ConversationRoom.User1.Intials + " " + cp.ConversationRoom.User1.Surname
                        };

            return Json(await users.ToListAsync(), JsonRequestBehavior.AllowGet);
        }

        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the product details
            var product = await db.Products.FindAsync(id);

            // if not found
            if (product == null || product.Visible == false)
            {
                return HttpNotFound();
            }

            // Get detailed catalog product
            var detailedProduct = new CatalogProductModel()
            {
                ProductID = product.ProductID,
                Name = product.Name,
                UserID = product.UserID,
                UserName = product.User.UserName,
                UserFullName = product.User.TitleID + " " + product.User.Intials + " " + product.User.Surname,
                Price = product.Price,
                ShortDescription = product.ShortDescription,
                LongDescription = product.LongDescription,
                ProductStatus = product.ProductStatus,
                DatePosted = product.DatePosted,
                ProductIntention = product.ProductIntention,
                Email = product.ContactEmail,
                WebsiteLink = product.WebsiteLink,
                PhoneNumber = product.ContactNumber,
                Offers = (from cp in db.ConversationRoomProducts
                          where cp.ProductID == product.ProductID
                          select cp).Count()
            };

            // check if product was requested by this specific user
            if (Request.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var isPending = (from p in db.ConversationRoomProducts
                                 where !(p.Product.User.UserName.Equals(userName)) && p.ProductID == id
                                 && (p.ConversationRoom.User1.UserName.Equals(userName) || p.ConversationRoom.User2.UserName.Equals(userName))
                                 select p.ProductID).Count() > 0;

                if (isPending)
                {
                    detailedProduct.ProductStatus = ProductStatus.REQUESTED;
                }
            }

            return View(detailedProduct);
        }

        // GET: /NoticeBoard/Edit/5
        public async Task<ActionResult> EditProduct(int? id)
        {
            if (id == null) // error
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get edit products
            var product = from p in db.Products
                          where p.ProductID == id && p.User.UserName.Equals(User.Identity.Name)
                          select new EditProductModel()
                          {
                              KeyWords = p.KeyWords,
                              LongDescription = p.LongDescription,
                              Name = p.Name,
                              Email = p.ContactEmail,
                              PhoneNumber = p.ContactNumber,
                              Price = p.Price,
                              ProductID = p.ProductID,
                              ShortDescription = p.ShortDescription,
                              UserID = p.UserID,
                          };

            // check if edit product exists
            var count = await product.CountAsync();
            if (count < 1)
            {
                return HttpNotFound("The product you want to edit does not exist.");
            }

            // Get the product to edit
            var editProduct = await product.FirstAsync();

            // Get and set selected categories
            var selectedCat = await (from c in db.ProductCategories
                                     where c.ProductID == editProduct.ProductID
                                     select c.CategoryID).ToListAsync();

            ViewBag.Categories = await GetCategories(selectedCat);

            return View(editProduct);
        }

        // GET: /NoticeBoard/Edit/5
        public async Task<ActionResult> EditNotice(int? id)
        {
            if (id == null) // error
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get edit products
            var poster = from p in db.Products
                          where p.ProductID == id && p.User.UserName.Equals(User.Identity.Name)
                          select new EditPosterModel()
                          {
                              KeyWords = p.KeyWords,
                              Description = p.LongDescription,
                              Name = p.Name,
                              PosterID = p.ProductID,
                              Email = p.ContactEmail,
                              PhoneNumber = p.ContactNumber,
                              WebsiteLink = p.WebsiteLink,
                              UserID = p.UserID,
                          };

            // check if edit product exists
            var count = await poster.CountAsync();
            if (count < 1)
            {
                return HttpNotFound("The product you want to edit does not exist.");
            }

            // Get the product to edit
            var editPoster = await poster.FirstAsync();

            // Get and set selected categories
            var selectedCat = await (from c in db.ProductCategories
                                     where c.ProductID == editPoster.PosterID
                                     select c.CategoryID).ToListAsync();

            ViewBag.Categories = await GetCategories(selectedCat);

            return View(editPoster);
        }

        // POST: /NoticeBoard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditProduct([Bind(Include = "ProductImage,ProductID,UserID,Name,Email,PhoneNumber,ShortDescription,Categories,LongDescription,Price,KeyWords")] EditProductModel product)
        {
            if (ModelState.IsValid)
            {

                var editedProduct = await db.Products.FindAsync(product.ProductID);

                // fill in the product image
                if (product.ProductImage != null)
                {
                    // get images
                    byte[] fullSizeImage = null;
                    byte[] thumbnailImage = null;
                    try
                    {
                        System.Drawing.Image originalImage = System.Drawing.Image.FromStream(product.ProductImage.InputStream);
                        System.Drawing.Image.GetThumbnailImageAbort callback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallBack);
                        System.Drawing.Image fullImg = originalImage.GetThumbnailImage(500, 500, callback, IntPtr.Zero);
                        System.Drawing.Image thumbNailImg = originalImage.GetThumbnailImage(300, 300, callback, IntPtr.Zero);

                        // get large image
                        MemoryStream ms = new MemoryStream();
                        fullImg.Save(ms, ImageFormat.Png);
                        fullSizeImage = ms.ToArray();
                        ms.Dispose();

                        // get small image
                        MemoryStream ms2 = new MemoryStream();
                        thumbNailImg.Save(ms2, ImageFormat.Png);
                        thumbnailImage = ms2.ToArray();
                        ms2.Dispose();

                        // free memory
                        thumbNailImg.Dispose();
                        fullImg.Dispose();
                        originalImage.Dispose();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message, "Product Image Creation Failed During Edit.");
                    }

                    editedProduct.FullSizeImage = fullSizeImage;
                    editedProduct.ThumbnailImage = thumbnailImage;
                }

                // set text info to be able to capitalize the product name
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                // put other edits
                editedProduct.Name = ti.ToTitleCase(product.Name);
                editedProduct.ShortDescription = product.ShortDescription;
                editedProduct.ContactNumber = product.PhoneNumber;
                editedProduct.ContactEmail = product.Email;
                editedProduct.LongDescription = product.LongDescription;
                editedProduct.Price = product.Price;
                editedProduct.KeyWords = product.KeyWords;


                // Get old selected categories
                var oldSelectedCategories = await (from c in db.ProductCategories
                                                   where c.ProductID == product.ProductID
                                                   select c.CategoryID).ToListAsync();
                // determine categories to be deleted from the database
                var toBeDelectedCategories = product.Categories.Union(oldSelectedCategories).Except(product.Categories);
                // determine new categories to be added to the database
                var toBeAddedCategories = product.Categories.Union(oldSelectedCategories).Except(oldSelectedCategories);

                // add new product categories
                foreach (var categoryID in toBeAddedCategories)
                {
                    var category = await (from c in db.Categories
                                          where c.CategoryID == categoryID
                                          select c).FirstAsync();
                    db.ProductCategories.Add(new ProductCategory() { Category = category, Product = editedProduct });
                }
                // delete old product categories
                foreach (var categoryID in toBeDelectedCategories)
                {
                    var productCategory = await (from pc in db.ProductCategories
                                                 where pc.CategoryID == categoryID && pc.ProductID == editedProduct.ProductID
                                                 select pc).FirstAsync();
                    db.ProductCategories.Remove(productCategory);
                }

                db.Entry(editedProduct).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = await GetCategories(product.Categories);

            return View(product);
        }

        // POST: /NoticeBoard/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditNotice([Bind(Include = "PosterImage,PosterID,UserID,Name,Email,WebsiteLink,PhoneNumber,Categories,Description,KeyWords")] EditPosterModel poster)
        {
            if (ModelState.IsValid)
            {

                var editedposter = await db.Products.FindAsync(poster.PosterID);

                // fill in the poster image
                if (poster.PosterImage != null)
                {
                    // get images
                    byte[] fullSizeImage = null;
                    byte[] thumbnailImage = null;
                    try
                    {
                        System.Drawing.Image originalImage = System.Drawing.Image.FromStream(poster.PosterImage.InputStream);
                        System.Drawing.Image.GetThumbnailImageAbort callback = new System.Drawing.Image.GetThumbnailImageAbort(ThumbnailCallBack);
                        System.Drawing.Image fullImg = originalImage.GetThumbnailImage(500, 500, callback, IntPtr.Zero);
                        System.Drawing.Image thumbNailImg = originalImage.GetThumbnailImage(300, 300, callback, IntPtr.Zero);

                        // get large image
                        MemoryStream ms = new MemoryStream();
                        fullImg.Save(ms, ImageFormat.Png);
                        fullSizeImage = ms.ToArray();
                        ms.Dispose();

                        // get small image
                        MemoryStream ms2 = new MemoryStream();
                        thumbNailImg.Save(ms2, ImageFormat.Png);
                        thumbnailImage = ms2.ToArray();
                        ms2.Dispose();

                        // free memory
                        thumbNailImg.Dispose();
                        fullImg.Dispose();
                        originalImage.Dispose();
                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(e.Message, "Poster Image Creation Failed During Edit.");
                    }

                    editedposter.FullSizeImage = fullSizeImage;
                    editedposter.ThumbnailImage = thumbnailImage;
                }

                // set text info to be able to capitalize the poster name
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                // put other edits
                editedposter.Name = ti.ToTitleCase(poster.Name);
                editedposter.ContactNumber = poster.PhoneNumber;
                editedposter.ContactEmail = poster.Email;
                editedposter.LongDescription = poster.Description;
                editedposter.WebsiteLink = poster.WebsiteLink;
                editedposter.KeyWords = poster.KeyWords;


                // Get old selected categories
                var oldSelectedCategories = await (from c in db.ProductCategories
                                                   where c.ProductID == poster.PosterID
                                                   select c.CategoryID).ToListAsync();
                // determine categories to be deleted from the database
                var toBeDelectedCategories = poster.Categories.Union(oldSelectedCategories).Except(poster.Categories);
                // determine new categories to be added to the database
                var toBeAddedCategories = poster.Categories.Union(oldSelectedCategories).Except(oldSelectedCategories);

                // add new poster categories
                foreach (var categoryID in toBeAddedCategories)
                {
                    var category = await (from c in db.Categories
                                          where c.CategoryID == categoryID
                                          select c).FirstAsync();
                    db.ProductCategories.Add(new ProductCategory() { Category = category, Product = editedposter });
                }
                // delete old poster categories
                foreach (var categoryID in toBeDelectedCategories)
                {
                    var posterCategory = await (from pc in db.ProductCategories
                                                where pc.CategoryID == categoryID && pc.ProductID == editedposter.ProductID
                                                 select pc).FirstAsync();
                    db.ProductCategories.Remove(posterCategory);
                }

                db.Entry(editedposter).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.Categories = await GetCategories(poster.Categories);

            return View(poster);
        }

        // GET: /NoticeBoard/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = await db.Products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            if (!product.User.UserName.Equals(User.Identity.Name))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(product);
        }

        // POST: /NoticeBoard/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Product product = await db.Products.FindAsync(id);
            db.Products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }
}