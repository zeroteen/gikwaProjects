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
    public class StoreController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();

        //
        // GET: /Store/
        [AllowAnonymous]
        public async Task<ActionResult> Index(int id = 1)
        {

            // set pagination filter url
            PaginationModel.FilterURL = Url.Action("Index", "Store") + "?id=";

            // get current institution if set
            if (Session != null && Session["currentInstitution"] != null)
            {
                // get institution name
                var name = (string)Session["currentInstitution"];

                // get all products in this institution
                var ps = from p in db.Products
                         where p.User.Institution.abbreviation.Equals(name)
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
                var pageIndex = PaginationModel.GoToPage(id) - 1;

                SetFilters();

                var unpendingProducts = await ps.OrderByDescending(p => p.DatePosted).Skip(pageIndex * PaginationModel.PageSize).Take(PaginationModel.PageSize).ToListAsync();

                return View(await SetPendingProducts(unpendingProducts));
            }

            // get all products at the default institution ("UCT")
            var products = from p in db.Products
                           where p.User.Institution.abbreviation.Equals("UCT")
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
            if (Request.IsAuthenticated)
            {
                var userName = User.Identity.Name;
                var pendingProducts = await (from p in db.ConversationRoomProducts
                                             where !(p.Product.User.UserName.Equals(userName))
                                             && (p.ConversationRoom.User1.UserName.Equals(userName) || p.ConversationRoom.User2.UserName.Equals(userName))
                                             select p.ProductID).ToListAsync();

                foreach (var product in products)
                {
                    if (pendingProducts.Contains(product.ProductID))
                    {
                        product.ProductStatus = ProductStatus.REQUESTED;
                    }
                }
            }

            return products;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Category(string category, int page = 1)
        {
            // bad request
            if (category == null)
            {
                return RedirectToAction("Index", "Store");
            }

            // get current institution
            string name = GetCurrentInstitution();

            // set pagination returnURL
            PaginationModel.FilterURL = Url.Action("Category", "Store") + "?category=" + category + "&page=";

            // get catalog product in a specific category
            var products = from p in db.ProductCategories
                           where p.Product.User.Institution.abbreviation.Equals(name) && p.Category.Code.Equals(category)
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

        [AllowAnonymous]
        public async Task<ActionResult> Search(string query, int page = 1)
        {

            // bad request
            if (query == null || query.Length == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            // set pagination returnURL
            PaginationModel.FilterURL = Url.Action("Search", "Store") + "?query=" + query + "&page=";

            // current institution
            string name = GetCurrentInstitution();

            // get all products that match the serach query
            var ps = from p in db.Products
                     where p.User.Institution.abbreviation.Equals(name) &&
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

        [AllowAnonymous]
        public async Task<ActionResult> BusinessCard(string id, int page = 1)
        {
            // bad request
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            // get pagination
            PaginationModel.FilterURL = Url.Action("BusinessCard", "Store") + "?id=" + id + "&page=";

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
                           where p.User.Id.Equals(id) && p.User.Institution.Name == user.Institution
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

        [AllowAnonymous]
        public ActionResult Institution(string name)
        {
            Session.Add("currentInstitution", name);
            SetFilters();

            return RedirectToAction("Index", "Store");
        }

        [AllowAnonymous]
        public async Task<ActionResult> ProductImage(int? id, int? sizeType)
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

        /// <summary>
        /// Get the SelectList of the number of sales request before sending a notification
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        private SelectList GetNumberOfRequests(int? selected)
        {
            List<SelectListItem> saleRequests = new List<SelectListItem>(){
                new SelectListItem(){Text="1 Buy Click",Value="1"},
                new SelectListItem(){Text="2 Buy Clicks",Value="2"},
                new SelectListItem(){Text="3 Buy Clicks",Value="3"},
                new SelectListItem(){Text="4 Buy Clicks",Value="4"},
                new SelectListItem(){Text="5 Buy Clicks",Value="5"}
            };

            return new SelectList(saleRequests, "Value", "Text", selected);
        }

        //
        // GET: /Store/SellProduct
        public async Task<ActionResult> Sell()
        {
            ViewBag.Categories = await GetCategories(null);
            ViewBag.NumberOfSaleRequests = GetNumberOfRequests(null);
            return View();
        }

        // POST: /Store/SellProduct?=
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Sell([Bind(Include = "ProductImage,Name,Categories,ShortDescription,LongDescription,Price,EmailNotification,NumberOfSaleRequests,AcceptTerms,KeyWords")] SellProductModel product)
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
                        System.Drawing.Image thumbNailImg = originalImage.GetThumbnailImage(150, 150, callback, IntPtr.Zero);

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

                        // rectify this
                        if (product.NumberOfSaleRequests < 1)
                        {
                            product.NumberOfSaleRequests = 1;
                        }

                        // create product
                        Product newProduct = new Product()
                        {
                            Name = ti.ToTitleCase(product.Name),
                            ShortDescription = product.ShortDescription,
                            LongDescription = product.LongDescription,
                            Price = product.Price,
                            ProductStatus = ProductStatus.NEW,
                            AcceptedTerms = product.AcceptTerms,
                            DatePosted = DateTime.Now,
                            EmailNotification = product.EmailNotification,
                            NumberOfSaleRequests = product.NumberOfSaleRequests,
                            KeyWords = product.KeyWords,
                            User = user,
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

                        return RedirectToAction("Index", "Store");
                    }
                }
                else
                {
                    ModelState.AddModelError("AcceptTerms", "please accept terms and conditions");
                }
            }

            ViewBag.Categories = await GetCategories(product.Categories);
            ViewBag.NumberOfSaleRequests = GetNumberOfRequests(null);

            // If we got this far, something failed, redisplay form
            return View(product);
        }

        private bool ThumbnailCallBack()
        {
            return false;
        }

        //
        // GET: /Store/BuyProduct
        //remove later
        public async Task<ActionResult> Buy(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var customer = await (from u in db.Users
                                  where u.UserName.Equals(User.Identity.Name)
                                  select u).FirstAsync();

            var product = await db.Products.FindAsync(id);

            if (product == null || product.ProductStatus == ProductStatus.SOLD)
            {
                return HttpNotFound("The product you want to buy was just bought by someone else or does not exist anymore.");
            }
            else if (customer.InstitutionID != product.User.InstitutionID)
            {
                return View("Forbidden", new ForbiddenMessageModel()
                            {
                                Title = "Sale Request Not Allowed",
                                Message = "You are not allowed to buy a product from a student from another institution."
                            });
            }

            return View(product);
        }

        public ActionResult SaleRequestSent()
        {
            return View();
        }

        public ActionResult Chat()
        {
            var products = from p in db.Products
                           where p.User.UserName.Equals(User.Identity.Name) && p.ProductStatus != ProductStatus.SOLD
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

        // POST: /Store/Delete/5
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

        [AllowAnonymous]
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            // Get the product details
            var product = await db.Products.FindAsync(id);

            // if not found
            if (product == null)
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

        // GET: /Store/Edit/5
        public async Task<ActionResult> Edit(int? id)
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
                              EmailNotification = p.EmailNotification,
                              KeyWords = p.KeyWords,
                              LongDescription = p.LongDescription,
                              Name = p.Name,
                              NumberOfSaleRequests = p.NumberOfSaleRequests,
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
            ViewBag.NumberOfSaleRequests = GetNumberOfRequests(editProduct.NumberOfSaleRequests);

            return View(editProduct);
        }

        // POST: /Store/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ProductImage,ProductID,UserID,Name,ShortDescription,Categories,LongDescription,Price,EmailNotification,NumberOfSaleRequests,KeyWords")] EditProductModel product)
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
                        System.Drawing.Image thumbNailImg = originalImage.GetThumbnailImage(150, 150, callback, IntPtr.Zero);

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

                // rectify this
                if (product.NumberOfSaleRequests < 1)
                {
                    product.NumberOfSaleRequests = 1;
                }

                // set text info to be able to capitalize the product name
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;

                // put other edits
                editedProduct.Name = ti.ToTitleCase(product.Name);
                editedProduct.ShortDescription = product.ShortDescription;
                editedProduct.LongDescription = product.LongDescription;
                editedProduct.Price = product.Price;
                editedProduct.EmailNotification = product.EmailNotification;
                editedProduct.NumberOfSaleRequests = product.NumberOfSaleRequests;
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
            ViewBag.NumberOfSaleRequests = GetNumberOfRequests(product.NumberOfSaleRequests);

            return View(product);
        }

        // GET: /Store/Delete/5
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

        // POST: /Store/Delete/5
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