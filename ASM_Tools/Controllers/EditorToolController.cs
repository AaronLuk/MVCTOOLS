﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ASM_Tools.DAL;
using ASM_Tools.Models;
using System.IO;

namespace ASM_Tools.Controllers
{
    public class EditorToolController : Controller
    {
        private ToolContext db = new ToolContext();
        // GET: Editor
        public ActionResult Index(string searchString)
        {

            ViewBag.LinkText = "Editor";
            var Tools = from t in db.Tools
                        select t;
            if (!String.IsNullOrEmpty(searchString))
            {
                Tools = Tools.Where(t => t.Title.Contains(searchString));
            }
            return View(Tools.ToList());
        }

        public ActionResult GetEmployees(int id)
        {
            
            Tool tool = db.Tools.Find(id);
            ViewBag.LinkText = "Editor";

            var Results = from e in db.Employees
                          select new
                          {
                              //e.ID,
                              //e.FirstMidName,
                              //e.LastName,
                              employee = e,
                              Checked = ((from te in db.ToolToEmployees
                                          where (te.ToolID == id) & (te.EmployeeID == e.ID)
                                          select te).Count() > 0),
                              Role = ((from te in db.ToolToEmployees
                                       where (te.ToolID == id) & (te.EmployeeID == e.ID)
                                       select te.Role).FirstOrDefault())
                          };

            var MyViewmodel = new ToolViewModel();

            MyViewmodel.tool = tool;

            var MyCheckBoxList = new List<CheckBoxToolViewModel>();

            foreach (var item in Results)
            {
                MyCheckBoxList.Add(new CheckBoxToolViewModel { Employee = item.employee, Checked = item.Checked, Role = item.Role });
            }

            MyViewmodel.Employees = MyCheckBoxList;

            return PartialView("PeekEmployees", MyViewmodel);
        }

        // GET: Editor/Details/5
        public ActionResult Details(int? id)
        {
            ViewBag.LinkText = "Editor";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }
        public JsonResult IsToolIDExist(int ToolID)
        {
            var validateID = db.Tools.FirstOrDefault(x => x.ToolID == ToolID);
            if (validateID != null)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        // GET: Editor/Create
        public ActionResult Create()
        {
            ViewBag.LinkText = "Editor";
            return View();
        }

        // POST: Editor/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ToolID,Title,Description,Tag,team,CoverImagePath,GalleryPath,DocumentationPath,InstallationPath,VideoPath,CoverImageFile,DocumentationFiles,InstallationFiles,VideoFiles,GalleryFiles")] Tool tool)
        {

            string root = Server.MapPath("~/Assets/Tools/" + tool.ToolID); // ~/Assets/products/software1
            string coverFolder = root + "\\cover folder";
            string documentationFolder = root + "\\documentation";
            string galleryFolder = root + "\\gallery";
            string installationFolder = root + "\\installation";
            string videoFolder = root + "\\video";


            //cover photo
            string imageName = Path.GetFileNameWithoutExtension(tool.CoverImageFile.FileName);  //image
            string imageExtension = Path.GetExtension(tool.CoverImageFile.FileName);  //.jpg
            string coverFullName = imageName + imageExtension;  //image.jpg

            tool.CoverImagePath = "~/Assets/Tools/" + tool.ToolID + "/cover folder/" + coverFullName;  //  ~/Assets/products/software1/cover image/image.jpg
            coverFullName = Path.Combine(Server.MapPath("~/Assets/Tools/" + tool.ToolID + "/cover folder/" + coverFullName));


            tool.GalleryPath = "~/Assets/Tools/" + tool.ToolID + "/gallery/";  //  ~/Assets/products/software1/gallery/
            tool.DocumentationPath = "~/Assets/Tools/" + tool.ToolID + "/documentation/";
            tool.InstallationPath = "~/Assets/Tools/" + tool.ToolID + "/Installation/";
            tool.VideoPath = "~/Assets/Tools/" + tool.ToolID + "/Video/";

            if (ModelState.IsValid && !Directory.Exists(root))
            {
                //Create folders
                Directory.CreateDirectory(root);
                Directory.CreateDirectory(documentationFolder);
                Directory.CreateDirectory(coverFolder);
                Directory.CreateDirectory(galleryFolder);
                Directory.CreateDirectory(installationFolder);
                Directory.CreateDirectory(videoFolder);

                //save files to respective folders
                //if(tool.DocumentationFile != null)
                //{
                //    tool.DocumentationFile.SaveAs(documentationFullName);
                //}
                //if(tool.VideoFile != null)
                //{
                //    tool.VideoFile.SaveAs(videoFullName);
                //}
                //if(tool.InstallationFile != null)
                //{
                //    tool.InstallationFile.SaveAs(installationFullName);
                //}
                tool.CoverImageFile.SaveAs(coverFullName);

                foreach (HttpPostedFileBase file in tool.GalleryFiles)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        string galleryFileName = Path.GetFileNameWithoutExtension(file.FileName);   //image
                        string galleryFileExtention = Path.GetExtension(file.FileName);  //.jpg
                        string galleryFileFullName = galleryFileName + galleryFileExtention;

                        galleryFileFullName = Path.Combine(Server.MapPath("~/Assets/Tools/" + tool.ToolID + "/gallery/" + galleryFileFullName)); ;  //image.jpg
                        //Save file to server folder  
                        file.SaveAs(galleryFileFullName);
                        //assigning file uploaded status to ViewBag for showing message to user.  
                        
                    }

                }

                foreach (HttpPostedFileBase file in tool.DocumentationFiles)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        string documentFileName = Path.GetFileNameWithoutExtension(file.FileName);   //image
                        string documentFileExtention = Path.GetExtension(file.FileName);  //.jpg
                        string documentFileFullName = documentFileName + documentFileExtention;

                        documentFileFullName = Path.Combine(Server.MapPath("~/Assets/Tools/" + tool.ToolID + "/documentation/" + documentFileFullName)); ;  //image.jpg
                        //Save file to server folder  
                        file.SaveAs(documentFileFullName);
                        //assigning file uploaded status to ViewBag for showing message to user.  

                    }

                }

                foreach (HttpPostedFileBase file in tool.InstallationFiles)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        string installationFileName = Path.GetFileNameWithoutExtension(file.FileName);   //image
                        string installationExtention = Path.GetExtension(file.FileName);  //.jpg
                        string installationFileFullName = installationFileName + installationExtention;

                        installationFileFullName = Path.Combine(Server.MapPath("~/Assets/Tools/" + tool.ToolID + "/installation/" + installationFileFullName)); ;  //image.jpg
                        //Save file to server folder  
                        file.SaveAs(installationFileFullName);
                        //assigning file uploaded status to ViewBag for showing message to user.  

                    }

                }

                foreach (HttpPostedFileBase file in tool.InstallationFiles)
                {
                    //Checking file is available to save.  
                    if (file != null)
                    {
                        string videoFileName = Path.GetFileNameWithoutExtension(file.FileName);   //image
                        string videoExtention = Path.GetExtension(file.FileName);  //.jpg
                        string videoFileFullName = videoFileName + videoExtention;

                        videoFileFullName = Path.Combine(Server.MapPath("~/Assets/Tools/" + tool.ToolID + "/video/" + videoFileFullName)); ;  //image.jpg
                        //Save file to server folder  
                        file.SaveAs(videoFileFullName);
                        //assigning file uploaded status to ViewBag for showing message to user.  

                    }

                }

                db.Tools.Add(tool);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tool);
        }

        // GET: Editor/Edit/5
        public ActionResult Edit(int? id)
        {
            ViewBag.LinkText = "Editor";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.Tools.Find(id);

            TempData["CoverImagePath"] = tool.CoverImagePath;
            TempData["DocumentationPath"] = tool.DocumentationPath;
            TempData["InstallationPath"] = tool.InstallationPath;
            TempData["VideoPath"] = tool.VideoPath;
            TempData["GalleryPath"] = tool.GalleryPath;

            if (tool == null)
            {
                return HttpNotFound();
            }

            var Results = from e in db.Employees
                          select new
                          {
                              //e.ID,
                              //e.FirstMidName,
                              //e.LastName,
                              employee = e,
                              Checked = ((from te in db.ToolToEmployees
                                          where (te.ToolID == id) & (te.EmployeeID == e.ID)
                                          select te).Count() > 0),
                              Role = ((from te in db.ToolToEmployees
                                          where (te.ToolID == id) & (te.EmployeeID == e.ID)
                                          select te.Role).FirstOrDefault())
                          };

            var MyViewmodel = new ToolViewModel();

            MyViewmodel.tool = tool;

            var MyCheckBoxList = new List<CheckBoxToolViewModel>();

            foreach (var item in Results)
            {
                MyCheckBoxList.Add(new CheckBoxToolViewModel { Employee = item.employee, Checked = item.Checked, Role = item.Role });
            }

            MyViewmodel.Employees = MyCheckBoxList;

            return View(MyViewmodel);
        }


        // POST: Editor/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ToolViewModel toolView)
        {

            if (ModelState.IsValid)
            {

                var MyTool = db.Tools.Find(toolView.ID);

                //Edit cover image
                if (toolView.CoverImageFile != null)
                {
                    //cover photo
                    string imageName = Path.GetFileNameWithoutExtension(toolView.CoverImageFile.FileName);  //image
                    string imageExtension = Path.GetExtension(toolView.CoverImageFile.FileName);  //.jpg
                    string coverFullName = imageName + imageExtension;  //image.jpg

                    string oldCoverImage = Request.MapPath(TempData["CoverImagePath"].ToString());

                    MyTool.CoverImagePath = "~/Assets/Tools" + toolView.tool.ToolID + "/cover folder/" + coverFullName;  //  ~/Assets/products/software1/cover image/image.jpg
                    coverFullName = Path.Combine(Server.MapPath("C:/Users/Adoca/source/repos/ASM_Tools/Assets/Tools" + toolView.tool.ToolID + "/cover folder/" + coverFullName));

                    //Delete old cover image
                    if (System.IO.File.Exists(oldCoverImage))
                    {
                        System.IO.File.Delete(oldCoverImage);
                    }

                    MyTool.CoverImageFile = toolView.CoverImageFile;
                    MyTool.CoverImageFile.SaveAs(coverFullName);

                }
                //if no file chose keep the original
                else
                {
                    MyTool.CoverImagePath = TempData["CoverImagePath"].ToString();
                }

                foreach (var item in db.ToolToEmployees)
                {
                    if (item.ID == toolView.tool.ToolID)
                    {
                        db.Entry(item).State = EntityState.Deleted;
                    }
                }

                foreach (var item in toolView.Employees)
                {
                    if (item.Checked)
                    {
                        var role = item.Role;
                        var tee = from te in db.ToolToEmployees
                                  where te.ToolID == toolView.tool.ToolID && te.EmployeeID == item.Employee.ID
                                  select te;
                        if (tee.Count() == 0)
                        {
                            db.ToolToEmployees.Add(new ToolToEmployee() { ToolID = toolView.tool.ToolID, EmployeeID = item.Employee.ID, Role = item.Role });
                        }
                        else
                        {
                            tee.First().Role = role;
                        }
                    }
                    else
                    {
                        var tee = from te in db.ToolToEmployees
                                  where te.ToolID == toolView.tool.ToolID && te.EmployeeID == item.Employee.ID
                                  select te;
                        if (tee.Count() != 0)
                        {
                            db.ToolToEmployees.Remove(tee.First());
                        }
                    }
                }

                MyTool.Title = toolView.tool.Title;
                MyTool.Description = toolView.tool.Description;
                MyTool.Tag = toolView.tool.Tag;
                MyTool.team = toolView.tool.team;
                MyTool.GalleryPath = toolView.tool.GalleryPath;
                MyTool.DocumentationPath = toolView.tool.DocumentationPath;
                MyTool.InstallationPath = toolView.tool.InstallationPath;
                MyTool.VideoPath = toolView.tool.VideoPath;

                db.Entry(MyTool).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(toolView);
        }

        // GET: Editor/Delete/5
        public ActionResult Delete(int? id)
        {
            ViewBag.LinkText = "Editor";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tool tool = db.Tools.Find(id);
            if (tool == null)
            {
                return HttpNotFound();
            }
            return View(tool);
        }

        // POST: Editor/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tool tool = db.Tools.Find(id);
            string fullPath = Server.MapPath("~/Assets/Tools/" + tool.ToolID);
            if (Directory.Exists(fullPath))
            {
                Directory.Delete(fullPath, true);
            }
            db.Tools.Remove(tool);
            db.SaveChanges();
            return RedirectToAction("Index");
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        
        
        public ActionResult ViewTool(int ID, string type)
        {
            Tool tool = db.Tools.Find(ID);
            ViewBag.Type = type;
            return PartialView(tool);
        }


        [HttpPost]
        public ActionResult UploadFiles()
        {
            var data = System.Web.HttpContext.Current.Request.Params["id"];
            var type = System.Web.HttpContext.Current.Request.Params["type"];
            var id = Int32.Parse(data);
            //var id = data.get('id');
            Tool tool = db.Tools.Find(id);

            if (type.Equals("gallery", StringComparison.InvariantCultureIgnoreCase))
            {
                string path = Server.MapPath(tool.GalleryPath);
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    file.SaveAs(path + file.FileName);
                }
                return PartialView("GalleryEditor", tool);
            }
            else if (type.Equals("documentation", StringComparison.InvariantCultureIgnoreCase))
            {
                string path = Server.MapPath(tool.DocumentationPath);
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    file.SaveAs(path + file.FileName);
                }
                return PartialView("DocumentationEditor", tool);
            }
            else if (type.Equals("installation", StringComparison.InvariantCultureIgnoreCase))
            {
                string path = Server.MapPath(tool.InstallationPath);
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    file.SaveAs(path + file.FileName);
                }
                return PartialView("InstallationEditor", tool);
            }
            else
            {
                string path = Server.MapPath(tool.VideoPath);
                HttpFileCollectionBase files = Request.Files;
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    file.SaveAs(path + file.FileName);
                }
                return PartialView("VideoEditor", tool);
            }

        }    


        [HttpPost]
        public ActionResult DeleteFile()
        {
            var path = System.Web.HttpContext.Current.Request.Params["path"];
            var data = System.Web.HttpContext.Current.Request.Params["id"];
            var type = System.Web.HttpContext.Current.Request.Params["type"];
            
            int id = Int32.Parse(data);
            Tool tool = db.Tools.Find(id);
            if (System.IO.File.Exists(Server.MapPath(path)))
            {
                System.IO.File.Delete(Server.MapPath(path));
            }

            if(type.Equals("gallery", StringComparison.InvariantCultureIgnoreCase))
            {
                return PartialView("GalleryEditor", tool);
            }
            else if(type.Equals("documentation", StringComparison.InvariantCultureIgnoreCase))
            {
                return PartialView("DocumentationEditor", tool);
            }
            else if (type.Equals("installation", StringComparison.InvariantCultureIgnoreCase))
            {
                return PartialView("InstallationEditor", tool);
            }
            else
            {
                return PartialView("VideoEditor", tool);
            }
        }

        public FileResult GetPreview(string FileName)
        {
            //string ReportURL = filePath;
            byte[] FileBytes = System.IO.File.ReadAllBytes(Server.MapPath(FileName));
            return File(FileBytes, "application/pdf");
        }

        public string ConvertImgToURL(string sImgPath)
        {
            if (sImgPath == null || !System.IO.File.Exists(sImgPath))
            {
                return "";
            }
            byte[] imageByteData = System.IO.File.ReadAllBytes(sImgPath);
            string imageBase64Data = Convert.ToBase64String(imageByteData);
            string imageDataURL = string.Format("data:image/png;base64,{0}", imageBase64Data);
            return imageDataURL;
        }
    }
}
