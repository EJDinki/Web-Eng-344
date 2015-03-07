﻿using Facebook;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WebPortal.Models;
using WebPortal.Controllers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity.Owin;
using Owin;
using System.Threading.Tasks;

namespace WebPortal.Controllers
{
    [RequireHttps]
    [Authorize]
    public class HomeController : Controller
    {
        // Commented out stuff is from Greg. Leaving it here in case it's needed again
        // Adding UserManager directly in this controller I *think* elimnated the need of AccountController -EJ
        //private AccountController ac = new AccountController();
        //private System.Threading.Tasks.Task<ActionResult> test;
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        [Authorize]
        //public ActionResult Index()
        public async Task<ActionResult> Index()
        {
            //return View(this.ac.FacebookFeed()); // Original Code
            //var claimsforUser = await ac.UserManager.GetClaimsAsync(User.Identity.GetUserId()); // Code copied from User Manager
            var claimsforUser = await UserManager.GetClaimsAsync(User.Identity.GetUserId()); // Copied UserManager stuff from account controller so we wouldn't need to use it
            var access_token = claimsforUser.FirstOrDefault(x => x.Type == "FacebookAccessToken").Value;
            var fb = new FacebookClient(access_token);
            fb.AppId = 1549521788629862 + "";
            fb.AppSecret = "3072d557ae33bd64013e58ed3dc44006";
            dynamic myInfo = fb.Get("/me/home");
            var friendsList = new List<FacebookFeedModel>();
            foreach (dynamic post in myInfo.data)
            {
                if (post.likes != null)
                {
                    friendsList.Add(new FacebookFeedModel()
                    {
                        Name = post.from.name,
                        Message = post.message,
                        Likes = post.likes.data.Count
                    });
                }//if
                else {
                    friendsList.Add(new FacebookFeedModel
                    {
                        Name = post.from.name,
                        Message = post.message,
                        Likes = 0
                    });
                }
            }

            return View(friendsList);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}