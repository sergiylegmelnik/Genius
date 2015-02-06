using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Genius
{
    class Program
    {
        static void Main()
        {
            IWebDriver driver = new FirefoxDriver();
            var client = new HttpClient();

            var parameters = UrlUtility.ObjectToParams(
                new
                {
                    artist = "Breaking_Benjamin",
                    song = "Until_The_End"
                }
                );

            var task = client.GetAsync("http://lyrics.wikia.com/api.php?func=getSong&fmt=xml&" + parameters);
            task.ContinueWith((Task<HttpResponseMessage> taskwithresponse) =>
            {
                var response = taskwithresponse.Result;
                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();
                var serializer = new XmlSerializer(typeof(LyricsResult));
                var model = (LyricsResult)serializer.Deserialize(new StringReader(jsonString.Result));

                if (model != null && model.Url != null)
                {
                    driver.Navigate().GoToUrl(model.Url);

                    IWebElement linkElement = driver.FindElement(By.CssSelector(".lyricbox"));

                    Console.WriteLine(linkElement.Text);
                }

            });

            task.Wait();

            Console.ReadLine();
            driver.Quit();
        }
    }
}
