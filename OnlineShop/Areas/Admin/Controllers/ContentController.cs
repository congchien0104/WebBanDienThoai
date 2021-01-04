using Model.Dao;
using Model.EF;
using OnlineShop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class ContentController : BaseController
    {
        // GET: Admin/Content
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var dao = new ContentDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            setViewBag();
            return View();
        }
        [HttpGet]
        public ActionResult Edit(long id)
        {
            var dao = new ContentDao();
            var content = dao.GetByID(id);

            setViewBag(content.CategoryID);
            return View(content);
        }
        [HttpPost]
        public ActionResult Create(Content model)
        {
            if(ModelState.IsValid)
            {
                var session = (UserLogin)Session[CommonConstants.USER_SESSION];
                model.CreatedBy = session.UserName;
                new ContentDao().Create(model);
                return RedirectToAction("Index");
            }
            
            setViewBag();
            return View();
        }
        [HttpPost]
        public ActionResult Edit(Content model)
        {
            if (ModelState.IsValid)
            {
                var session = (UserLogin)Session[CommonConstants.USER_SESSION];
                model.CreatedBy = session.UserName;
                var dao = new ContentDao();
                var result = dao.Edit(model);
                if (result == 1)
                {
                    setViewBag(model.CategoryID);
                    SetAlert("Sua user thanh cong", "success");
                    return RedirectToAction("Index", "Content");
                }
                else
                {
                    ModelState.AddModelError("", "Update no success user");
                }
            }
            setViewBag(model.CategoryID);
            return View("Index");
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            new ContentDao().Delete(id);

            return RedirectToAction("Index");
        }
        public void setViewBag(long? selectedId = null)
        {
            var dao = new CategoryDao();
            ViewBag.CategoryID = new SelectList(dao.ListAll(), "ID", "Name", selectedId);
        }
    }
}