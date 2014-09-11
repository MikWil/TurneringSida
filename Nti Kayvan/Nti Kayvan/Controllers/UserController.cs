using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Nti_Kayvan.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult LogIn()
        {
            return View();

        }
        [HttpPost]
        public ActionResult LogIn(Models.UserModel user)
        {

            if (ModelState.IsValid)
            {
                if (IsValid(user.Email,user.Password))
                {
                    FormsAuthentication.SetAuthCookie(user.Email,false);
                    return RedirectToAction("Home", "Home");
                }
                else
                {
                    ModelState.AddModelError("","Login Data is incorrect.");
                }
            }



            return View(user);
        }

        [HttpGet]
        public ActionResult Registration()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Registration(Models.UserModel user)
        {
            using (var db = new MainDbEntities())
            {
                
            
            if (ModelState.IsValid)
            {
                var crypto = new SimpleCrypto.PBKDF2();
                var encrpPass = crypto.Compute(user.Password);
                var sysUser = db.SystemUsers.Create();

                sysUser.Emal = user.Email;
                sysUser.Password = encrpPass;
                sysUser.PasswordSalt = crypto.Salt;
                sysUser.UserId = Guid.NewGuid();

                db.SystemUsers.Add(sysUser);

                db.SaveChanges();

                return RedirectToAction("Home", "Home");

            }


                }
            return View(user);

        }
        public ActionResult LogOut()
        {


            FormsAuthentication.SignOut();
            return RedirectToAction("Home", "Home");


        }
        private bool IsValid(string email,string password)
        {

            var crypto = new SimpleCrypto.PBKDF2();
            var isValid = false;
            using (var db = new MainDbEntities())
            {
                var user = db.SystemUsers.FirstOrDefault(u => u.Emal == email);

                if (user != null)
                {
                    if (user.Password == crypto.Compute(password,user.PasswordSalt))
                    {
                        isValid = true;
                    }
                }
            }


            return isValid;

        }

    }
}
