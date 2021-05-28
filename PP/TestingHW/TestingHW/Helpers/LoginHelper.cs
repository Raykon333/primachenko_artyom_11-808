using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestingHW
{
    public class LoginHelper : HelperBase
    {
        public LoginHelper(AppManager manager) : base(manager) { }

        public void Login(AccountData accountData)
        {
            if (IsLoggedIn())
                return;
            driver.Navigate().GoToUrl("https://reddit.com/login");
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(By.Id("loginUsername")));
                element.Click();
                element.SendKeys(accountData.Username);
            }
            driver.FindElement(By.Id("loginPassword")).Click();
            driver.FindElement(By.Id("loginPassword")).SendKeys(accountData.Password);
            driver.FindElement(By.CssSelector(".mode-auth .Step__content")).Click();
            driver.FindElement(By.CssSelector(".m-full-width")).SendKeys(Keys.Enter);
            Thread.Sleep(2000);
            //driver.SwitchTo().DefaultContent();
        }

        public void Logout()
        {
            if (!IsLoggedIn())
                return;
            manager.Navigation.GoToHomepage();
            driver.FindElement(By.CssSelector(".\\_50RxI-5rW1xzwoC42vhzM > path")).Click();
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                    .ElementToBeClickable(By.CssSelector(".\\_1YWXCINvcuU7nk0ED-bta8:nth-child(13) > .vzhy90YD0qH7ZDJi7xMGw")));
                element.Click();
            }
            Thread.Sleep(5000);
        }

        public string GetUsername()
        {
            manager.Navigation.GoToHomepage();
            return driver.FindElement(By.ClassName("_2BMnTatQ5gjKGK5OWROgaG")).Text;
        }

        public bool IsLoggedIn()
        {
            manager.Navigation.GoToHomepage();
            IWebElement loginButton = null;
            try
            {
                loginButton = driver.FindElement(By.ClassName("_3Wg53T10KuuPmyWOMWsY2F"));
            }
            catch (Exception ex) { }
            return loginButton == null || !loginButton.Displayed;
        }

        public bool IsLoggedIn(string username)
        {
            manager.Navigation.GoToHomepage();
            string _username = null;
            try
            {
                _username = driver.FindElement(By.ClassName("_2BMnTatQ5gjKGK5OWROgaG")).Text;
            }
            catch (Exception ex) { }
            return username == _username;
        }

        public bool InvalidDataWasSent()
        {
            IWebElement element = null;
            try
            {
                element = driver.FindElement(By.ClassName("m-invalid"));
            }
            catch (Exception ex) { }
            return element != null;
        }
    }
}
