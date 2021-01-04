using Model.Dao;
using Model.EF;
using OfficeOpenXml;
using OnlineShop.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using System.Data.OleDb;
using System.Data;
using System.IO;
using System.Data.Entity.Validation;

namespace OnlineShop.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        // GET: Admin/Product
        public ActionResult Index(string searchString, int page = 1, int pageSize = 10)
        {
            var dao = new ProductDao();
            var model = dao.ListAllPaging(searchString, page, pageSize);
            ViewBag.SearchString = searchString;
            return View(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public ActionResult Create(Product model)
        {
            if (ModelState.IsValid)
            {
                var session = (UserLogin)Session[CommonConstants.USER_SESSION];
                model.CreatedBy = session.UserName;
                new ProductDao().Create(model);
                return RedirectToAction("Index");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Edit(long id)
        {
            var dao = new ProductDao();
            var product = dao.GetByID(id);
            return View(product);
        }
        [HttpPost]
        public ActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                var session = (UserLogin)Session[CommonConstants.USER_SESSION];
                model.CreatedBy = session.UserName;
                var dao = new ProductDao();
                var result = dao.Edit(model);
                if (result == 1)
                {
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    ModelState.AddModelError("", "Sua Product khong thanh cong!!");
                }
                //new ProductDao().Edit(model);
                //return RedirectToAction("Index");
            }
            return View();
        }
        [HttpDelete]
        public ActionResult Delete(int id)
        {
            new ProductDao().Delete(id);

            return RedirectToAction("Index");
        }
        [HttpPost]
        public FileResult ExportToExcel()
        {
            DataTable dt = new DataTable("Grid");
            dt.Columns.AddRange(new DataColumn[11] { new DataColumn("STT"),
                                                     new DataColumn("Name"),
                                                     new DataColumn("Code"),
                                                     new DataColumn("MetaTitle"),
                                                     new DataColumn("Price"),
                                                     new DataColumn("PromotionPrice"),
                                                     new DataColumn("Quantity"),
                                                     new DataColumn("CategoryID"),
                                                     new DataColumn("CreatedDate"),
                                                     new DataColumn("CreatedBy"),
                                                     new DataColumn("Status")});
            OnlineShopDbContext db = new OnlineShopDbContext();
            var insuranceCertificate = from InsuranceCertificate in db.Products select InsuranceCertificate;

            foreach (var insurance in insuranceCertificate)
            {
                dt.Rows.Add(insurance.ID, insurance.Name, insurance.Code, insurance.MetaTitle, insurance.Price,
                    insurance.PromotionPrice, insurance.Quantity, insurance.CategoryID,
                    insurance.CreatedDate, insurance.CreatedBy, insurance.Status);
            }

            using (XLWorkbook wb = new XLWorkbook()) //Install ClosedXml from Nuget for XLWorkbook  
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream()) //using System.IO;  
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ExcelFile.xlsx");
                }
            }
        }
        // Import Data From Excel To SQL Server Database Using MVC
        [HttpPost]
        public ActionResult Upload(FormCollection formCollection)
        {
            var productsList = new List<Product>();
            if (Request != null)
            {
                HttpPostedFileBase file = Request.Files["UploadedFile"];
                if ((file != null) && (file.ContentLength > 0) && !string.IsNullOrEmpty(file.FileName))
                {
                    string fileName = file.FileName;
                    string fileContentType = file.ContentType;
                    byte[] fileBytes = new byte[file.ContentLength];
                    var data = file.InputStream.Read(fileBytes, 0, Convert.ToInt32(file.ContentLength));
                    using (var package = new ExcelPackage(file.InputStream))
                    {
                        var currentSheet = package.Workbook.Worksheets;
                        var workSheet = currentSheet.First();
                        var noOfCol = workSheet.Dimension.End.Column;
                        var noOfRow = workSheet.Dimension.End.Row;
                        for (int rowIterator = 2; rowIterator <= noOfRow; rowIterator++)
                        {
                            var product = new Product();
                            product.Name = Convert.ToString(workSheet.Cells[rowIterator, 1].Value);
                            product.Code = Convert.ToString(workSheet.Cells[rowIterator, 2].Value);
                            product.MetaTitle = Convert.ToString(workSheet.Cells[rowIterator, 3].Value);
                            product.Price = Convert.ToDecimal(workSheet.Cells[rowIterator, 4].Value);
                            product.PromotionPrice = Convert.ToDecimal(workSheet.Cells[rowIterator, 5].Value);
                            product.Quantity = Convert.ToInt32(workSheet.Cells[rowIterator, 6].Value);
                            product.CategoryID = Convert.ToInt32(workSheet.Cells[rowIterator, 7].Value);
                            //product.CreatedDate = Convert.ToDateTime(workSheet.Cells[rowIterator, 9].Value);
                            product.CreatedBy = Convert.ToString(workSheet.Cells[rowIterator, 8].Value);
                            //product.Status = Convert.ToBoolean(workSheet.Cells[rowIterator, 11].Value);
                            productsList.Add(product);
                        }
                    }
                }
            }
            try
            {
                using (OnlineShopDbContext db = new OnlineShopDbContext())
                {
                    foreach (var item in productsList)
                    {
                        db.Products.Add(item);
                    }
                    db.SaveChanges();
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
            }
            return Redirect("Index");
        }
    }
}