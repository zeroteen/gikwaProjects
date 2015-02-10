using Grikwa.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Grikwa.Controllers
{
    public abstract class BaseController : Controller
    {
        public ApplicationDbContext db = new ApplicationDbContext();
        public string storageAccountName = ConfigurationManager.AppSettings["StorageAccountName"];
        public string storageAccountKey = ConfigurationManager.AppSettings["StorageAccountKey"];
    }
}