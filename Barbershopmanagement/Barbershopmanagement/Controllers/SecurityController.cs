﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using BarbershopManagement.Helpers;
using Barbershopmanagement.Models;

namespace BarbershopManagement.Controllers
{
    public class SecurityController : Controller
    {

        //Login

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {

            BarbershopManagementEntities db = new BarbershopManagementEntities();
            var taiKhoan = db.USERS.SingleOrDefault(m => m.TENDANGNHAP == username && m.PASSWORD == password);
            if (taiKhoan != null)
            {
                TempData["ErrorLogin"] = null;
                Session["userid"] = taiKhoan.USERID;
                Session["userhoten"] = taiKhoan.HOTEN;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["ErrorLogin"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
                return RedirectToAction("Login");
            }
        }

        //Register

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(string username, string password, string confirmpassword, string hoten, string sodienthoai, string email, string diachi)
        {

            if (username.Length > 0 && password == confirmpassword && password.Length > 4 && hoten.Length > 0 && sodienthoai.Length == 10 && email != null)
            {
                BarbershopManagementEntities db = new BarbershopManagementEntities();
                var findUser = db.USERS.FirstOrDefault(m => m.TENDANGNHAP == username);

                if (findUser == null)
                {
                    USER newUser = new USER();
                    newUser.TENDANGNHAP = username;
                    newUser.PASSWORD = password;
                    newUser.HOTEN = hoten;
                    newUser.SODIENTHOAI = sodienthoai;
                    newUser.EMAIL = email;
                    newUser.DIACHI = diachi;
                    newUser.ROLE = "khachhang";
                    db.USERS.Add(newUser);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception ex) { }
                    TempData["Register"] = "Tạo tài khoản thành công!";
                    return RedirectToAction("Login");
                }
                else
                {
                    TempData["ErrorRegister"] = "Tên đăng nhập đã tồn tại!";
                    return RedirectToAction("Register");
                }
            }
            else
            {
                TempData["ErrorRegister"] = "Nhập thông tin không chính xác!";
                return RedirectToAction("Register");
            }
        }


        //ForgotPassword

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string username, string email)
        {

            BarbershopManagementEntities db = new BarbershopManagementEntities();
            var taiKhoan = db.USERS.FirstOrDefault(m => m.TENDANGNHAP == username && m.EMAIL == email);
            if (taiKhoan != null)
            {
                EmailService sendEmail = new EmailService();
                sendEmail.SendEmail(taiKhoan.EMAIL, "GỬI LẠI MẬT KHẨU", "Mật khẩu của bạn là: " + taiKhoan.PASSWORD);
                TempData["ForgotPassword"] = "Mật khẩu đã được gửi qua email của bạn";
                return RedirectToAction("Login");
            }
            else
            {
                TempData["ErrorForgotPassword"] = "Tài khoản không tồn tại!";
                return RedirectToAction("ForgotPassword");
            }
        }


        //LogOut
        public ActionResult LogOut()
        {
            Session["userid"] = null;
            Session["userhoten"] = null;
            return RedirectToAction("Login");
        }
    }
}