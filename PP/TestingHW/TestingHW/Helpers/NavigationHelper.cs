using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingHW
{
    public class NavigationHelper : HelperBase
    {
        private string baseURL;

        public NavigationHelper(AppManager manager, string _baseURL) : base(manager)
        {
            baseURL = _baseURL;
        }

        public void GoToHomepage()
        {
            var s = driver.Url;
            if (!driver.Url.EndsWith("reddit.com/"))
                driver.Navigate().GoToUrl(baseURL);
        }

        public void GoToProfile(string username)
        {
            driver.Navigate().GoToUrl("https://www.reddit.com/user/" + username);
        }

        public void GoToOwnProfile()
        {
            GoToHomepage();
            driver.Navigate().GoToUrl("https://www.reddit.com/user/" + manager.Auth.GetUsername());
        }
    }
}
