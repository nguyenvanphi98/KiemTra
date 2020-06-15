using System;
using Microsoft.AspNetCore.Mvc;
using BookStore.Data;
using Microsoft.AspNetCore.Http;
using BookStore.Models;

namespace BookStore.Controllers
{
    public class AccountController : Controller
    {
        private AcountService _acountService;

        public AccountController(AcountService acountService)
        {
            _acountService = acountService;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (!(string.IsNullOrEmpty(HttpContext.Session.GetString("User"))))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Login(Account User)
        {
            var user = _acountService.GetAccount(User);
            if (user != null)
            {
                HttpContext.Session.SetString("User", user.UserName);
                HttpContext.Session.SetString("Role", user.FullName);   

                return RedirectToAction("Index","Home");
            }
            ViewBag.Message = "Tài khoản hoặc mật khẩu không đúng";
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("User");
            HttpContext.Session.Remove("Role");
            
            return RedirectToAction("Index", "Home");
        }
        [HttpGet]
        public IActionResult Register()
        {
            if (!(string.IsNullOrEmpty(HttpContext.Session.GetString("User"))))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
        [HttpPost]
        public IActionResult Register(Account User,string RepeatPassword)
        {
            if (User != null)
            {
                if (User.FullName != null && User.UserName != null && User.Password != null && RepeatPassword != null)
                {
                    if (User.Password.Trim() != RepeatPassword.Trim())
                    {
                        ViewBag.ErrorMessage = "Mật Khẩu không trùng khớp";
                        return View();
                    }

                    if (_acountService.CheckAccount(User.UserName))
                    {
                        ViewBag.ErrorMessage = "Tài khoản này đã tồn tại";
                        return View();
                    }

                    _acountService.InsertAccount(User);
                    ViewBag.ErrorMessage = "Đăng ký thành công";
                    return View();

                }
            }
            ViewBag.ErrorMessage = "Vui lòng nhập thông tin";
            return View();
        }
    }
}