using System;
using System.Linq;
using System.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRP.QServiceLib
{
    public class ShelfService : IShelfService, ICheckSettings
    {
        protected List<IShelfService> _services = new List<IShelfService>();

        public void Start()
        {
            Parallel.For(0, _services.Count, (i) =>
            {
                _services[i].Start();
            });

        }

        public void Stop()
        {
            Parallel.For(0, _services.Count, (i) =>
            {
                _services[i].Stop();
            });
        }

        public string CheckConnStr(string title, string name)
        {
            if (string.IsNullOrWhiteSpace(title))
            {
                return string.Format("Can't find ConnectionString for {0}! ", title);
            }

            var connStr = ConfigurationManager.ConnectionStrings[name];

            if (connStr == null || string.IsNullOrWhiteSpace(connStr.ConnectionString))
            {
                return string.Format("Can't find ConnectionString {0}! ", name);
            }

            return "";
        }

        public string CheckAppSetting(string title, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return string.Format("Can't find AppSetting with {0}! ", title);
            }

            if (!ConfigurationManager.AppSettings.AllKeys.Contains(name))
            {
                return string.Format("Can't find AppSetting {0}! ", name);
            }

            return "";
        }

        public bool CheckIfTrue(object value)
        {
            if (value == null) return false;

            string str = value.ToString().ToLower();

            string[] arr = new string[] { "true", "yes", "1" };

            return arr.Contains(str);
        }

        public TimeSpan GetTimeSpanFromString(string value)
        {
            var res = new TimeSpan();

            if (string.IsNullOrWhiteSpace(value)) return res;

            var arr = value.Split(',');

            try
            {
                res = new TimeSpan(int.Parse(arr[0]), int.Parse(arr[1]), int.Parse(arr[2]));
            }
            catch { }

            return res;
        }
    }
}