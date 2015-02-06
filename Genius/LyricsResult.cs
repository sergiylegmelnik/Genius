using System.Xml.Serialization;

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
}