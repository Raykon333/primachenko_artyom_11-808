using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestingHW
{
    public class AuthBase : TestBase
    {
        [SetUp]
        public new void SetupTest()
        {
            app = AppManager.GetInstance();
            app.Auth.Login(new AccountData(Settings.Login, Settings.Password));
        }
    }
}
