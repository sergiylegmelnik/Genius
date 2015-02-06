using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Genius.Common;
using Genius.Lyrics.Wikia.models;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;

namespace Genius.Lyrics.Wikia
{
    public class WikiaCrawler : ICrawler
    {
        private readonly IWebDriver _driver;
        private readonly HttpClient _client;

        public WikiaCrawler()
        {
            _driver = new FirefoxDriver();
            _client = new HttpClient();
        }

        public WikiaCrawler(IWebDriver driver, HttpClient client)
        {
            _driver = driver;
            _client = client;
        }

        public async Common.Lyrics GrabLyrics(object parameters)
        {

            var task = client.GetAsync("http://lyrics.wikia.com/api.php?func=getSong&fmt=xml&" + parameters);
            return await task.ContinueWith((Task<HttpResponseMessage> taskwithresponse) =>
            {
                var response = taskwithresponse.Result;
                var jsonString = response.Content.ReadAsStringAsync();
                jsonString.Wait();
                var serializer = new XmlSerializer(typeof(LyricsResult));
                var model = (LyricsResult)serializer.Deserialize(new StringReader(jsonString.Result));

                if (model != null && model.Url != null)
                {
                    _driver.Navigate().GoToUrl(model.Url);

                    IWebElement linkElement = _driver.FindElement(By.CssSelector(".lyricbox"));

                    Console.WriteLine(linkElement.Text);
                }

            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_driver != null) _driver.Quit();
            }
        }
    }
}
