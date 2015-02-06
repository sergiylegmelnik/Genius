using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Genius
{
    public class LyricsResult
    {
        [XmlElement(ElementName = "artist")]
        public string Artist { get; set; }

        [XmlElement(ElementName = "song")]
        public string SongName { get; set; }

        [XmlElement(ElementName = "lyrics")]
        public string Lyrics { get; set; }

        [XmlElement(ElementName = "url")]
        public string Url { get; set; }
    }

    public class UrlUtility
    {
        public static string ConstructQueryString(NameValueCollection Params)
        {
            return string.Join("&", (from string name in Params
                                     select String.Concat(name, "=", HttpUtility.UrlEncode(Params[name])))
                                     .ToArray());
        }

        public static string ObjectToParams(object parameters)
        {
            Type t = parameters.GetType();
            var nvc = new NameValueCollection();
            foreach (var p in t.GetProperties())
            {
                var name = p.Name;
                var value = p.GetValue(parameters, null).ToString();
                nvc.Add(name, value);
            }

            var result = ConstructQueryString(nvc);
            return result;
        }
    }

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
