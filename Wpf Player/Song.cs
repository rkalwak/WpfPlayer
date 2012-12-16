using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.IO;

namespace Wpf_Player
{

    public class Song
    {
        private int _number;
        private string _filename;
        private string _title;
        private string _artist;
        private string _album;
        private int _duration;
        private int _track;
        private int _year;
        private string _genre;
        private BitmapImage coverImage;
        public BitmapImage CoverImage
        {
            get { return coverImage; }
            set { coverImage = value; }
        }

        public Song()
        {

            _number = 0;
            _filename = "";
            _title = "";
            _artist = "";
            _album = "";
            _duration = 0;
            _track = 0;
            _year = 0;
            _genre = "";
        }
        public Song(string filename)
        {
            
            _filename = filename;
            GetTags();
        }
#region accesors
        public string FileName
        {
            set { _filename = value; }
            get { return _filename; }
        }
        public int Number
        {
            set { _number = value; }
            get { return _number; }
        }
        public string Title
        {
            set { _title = value; }
            get { return _title; }
        }
        public string Artist
        {
            set { _artist = value; }
            get { return _artist; }
        }
        public string Album
        {
            set { _album = value; }
            get { return _album; }
        }
        public int Duration
        {
            set { _duration = value; }
            get { return _duration; }
        }
        public int Track
        {
            set { _track = value; }
            get { return _track; }
        }
        public int Year
        {
            set { _year = value; }
            get { return _year; }

        }
        public string Genre
        {
            set { _genre = value; }
            get { return _genre; }
        }
# endregion
        private void GetTags()
        {
            try
            {
                TagLib.File mp3 = TagLib.File.Create(_filename);
                _artist = GetAllStringsFromArrary(mp3.Tag.Artists, ",");
                _title = mp3.Tag.Title;
                _album = mp3.Tag.Album;
                _duration = (int)mp3.Properties.Duration.TotalSeconds;
                _track = (int)mp3.Tag.Track;
                _year = (int)mp3.Tag.Year;
                _genre = GetAllStringsFromArrary(mp3.Tag.Genres, ",");
                if (mp3.Tag.Pictures.Length > 0)
                {
                    MemoryStream coverMemStream = new MemoryStream(mp3.Tag.Pictures[0].Data.Data);
                    coverImage=new BitmapImage();
                    coverImage.BeginInit();
                    coverImage.CacheOption = BitmapCacheOption.OnLoad;
                    coverImage.StreamSource = coverMemStream;
                    coverImage.EndInit();
                    coverMemStream.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private string GetAllStringsFromArrary(string[] strArray, string strDelimeter)
        {
            string strFinal = string.Empty;

            for (int i = 0; i < strArray.Length; i++)
            {
                strFinal += strArray[i];

                if (i != strArray.Length - 1)
                {
                    strFinal += strDelimeter;
                }
            }
            return strFinal;
        }

    }
}
