using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Linq;

namespace WebDriverApp
{
    class Program
    {
        static void Main(string[] args)
        {
            IWebDriver driver;

            using(driver = new ChromeDriver())
            {
                driver.Navigate().GoToUrl("https://www.mangareader.net/naruto/1");
                WaitUntilReady(driver);
                Console.WriteLine("Page Loaded");


                var exec = (IJavaScriptExecutor)driver;

                var res = exec.ExecuteScript("return JSON.stringify(document['mj']['im']);").ToString();

                var images = JArray.Parse(res).Select(x => new { Page = x["p"].ToString(), Url = x["u"].ToString() });

                foreach (var img in images)
                {
                    Console.WriteLine($"Page #{img.Page}: {img.Url}");
                }

                                
                Console.ReadLine();
            }

            static void WaitUntilReady(IWebDriver driver)
            {
                new WebDriverWait(driver, new TimeSpan(0, 0, 30)).Until(
                    d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            }
        }
    }
}
