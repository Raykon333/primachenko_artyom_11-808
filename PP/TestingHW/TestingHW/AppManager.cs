using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestingHW
{
    public class AppManager
    {
        private static ThreadLocal<AppManager> app = new ThreadLocal<AppManager>();

        public IWebDriver driver { get; protected set; }
        public IDictionary<string, object> vars { get; private set; }
        protected IJavaScriptExecutor js;

        public NavigationHelper Navigation { get; private set; }
        public LoginHelper Auth { get; private set; }
        public PostHelper Poster { get; private set; }

        private AppManager()
        {
            driver = new FirefoxDriver();
            driver.Manage().Window.Maximize();
            Navigation = new NavigationHelper(this, Settings.BaseURL);
            Auth = new LoginHelper(this);
            Poster = new PostHelper(this);
        }

        public void Stop()
        {
            driver.Quit();
        }

        public static AppManager GetInstance()
        {
            if (!app.IsValueCreated)
            {
                AppManager newInstance = new AppManager();
                newInstance.Navigation.GoToHomepage();
                app.Value = newInstance;
            }
            return app.Value;
        }

        ~AppManager()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                //ignore
            }
        }

    }
}
