using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestingHW
{
    public class PostHelper : HelperBase
    {
        public PostHelper(AppManager manager) : base(manager) { }

        public void MakeASelfpost(PostData postData)
        {
            manager.Navigation.GoToOwnProfile();
            driver.FindElement(By.CssSelector(".\\_2q1wcTx60QKM_bQ1Maev7b")).Click();
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                    .ElementToBeClickable(By.CssSelector(".PqYQ3WC15KaceZuKcFI02")));
                element.Click();
            }
            driver.FindElement(By.CssSelector(".PqYQ3WC15KaceZuKcFI02")).SendKeys(postData.Title);
            driver.FindElement(By.CssSelector(".notranslate")).Click();
            driver.FindElement(By.CssSelector(".notranslate")).SendKeys(postData.Content);
            driver.FindElement(By.CssSelector(".\\_18Bo5Wuo3tMV-RDB8-kh8Z")).Click();
        }

        public void EditThePost(string postId, string newContent)
        {

            driver.Navigate().GoToUrl("https://www.reddit.com/" + postId);
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                    .ElementToBeClickable(By.ClassName("_1EbinKu2t3KjaT2gR156Qp")));
                element.Click();
            }
            driver.FindElement(By.CssSelector(".\\_10K5i7NW6qcm-UoCtpB3aK:nth-child(3) > .pthKOcceozMuXLYrLlbL1")).Click();
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                var contentField = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                    .ElementToBeClickable(By.CssSelector(".\\_2baJGEALPiEMZpWB2iWQs7")))
                    .FindElement(By.ClassName("public-DraftEditor-content"));
                contentField.Click();
                contentField.SendKeys(Keys.Control + "a" + Keys.Delete);
                contentField.SendKeys(newContent);
            }
            driver.FindElement(By.CssSelector(".\\_1N8wF0OCvBu6gW4b4cpHhE")).Click();
        }

        public string CurrentPostTitle()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            var element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementIsVisible(By.CssSelector(".\\_29WrubtjAcKqzJSPdQqQ4h > ._eYtD2XCVieq6emjKBH3m")));
            return element.Text;
        }

        public string CurrentPostContent()
        {
            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            var element1 = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions
                .ElementIsVisible(By.ClassName("D3IL3FD0RFy_mkKLPwL4")));
            var element2 = element1.FindElement(By.CssSelector("._292iotee39Lmt0MkQZ2hPV"));
            return element2.FindElement(By.CssSelector("._1qeIAgB0cPwnLhDF9XSiJM")).Text;
        }

        public void DeleteLastPost()
        {
            manager.Navigation.GoToOwnProfile();
            var post = driver.FindElement(By.ClassName("_3KGXodqw9Ht3MoBpe8_gzB"));
            post.FindElement(By.ClassName("_2L8b_l8zFzAkWuMyZJ1_vg")).Click();
            driver.FindElement(By.CssSelector(".\\_10K5i7NW6qcm-UoCtpB3aK:nth-child(7) > .\\_2-cXnP74241WI7fpcpfPmg")).Click();
            //driver.FindElement(By.CssSelector(".\\_17UyTSs2atqnKg9dIq5ERg")).Click();
        }

        public bool DeletionAlertIsPresent()
        {
            var dialog = SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(By.ClassName("_2Bejocqb-InO8686E2ehf")).Invoke(driver);
            return dialog != null;
        }
    }
}
