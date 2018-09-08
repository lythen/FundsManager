using System;
namespace Lythen.ViewModels
{
    public class BaseViewModel
    {

    }
    public class BasePagerModel
    {
        private int _pagesize = 20;
        private int _pageindex = 1;
        private string _keyword = "";
        private int _amount = 0;
        private int _pages = 0;
        public int PageSize { get { return _pagesize; }set { this._pagesize = value; } }
        public int PageIndex { get { return _pageindex; } set { this._pageindex = value; } }
        public string KeyWord { get { return _keyword; } set { this._keyword = value; } }
        public int Amount { get { return _amount; } set { this._amount = value;this._pages = (int)Math.Ceiling((decimal)(value / _pagesize)); } }
        public int Pages { get { return _pages; } }
    }
}