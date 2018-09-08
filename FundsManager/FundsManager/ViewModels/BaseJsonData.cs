namespace Lythen.ViewModels
{
    public class BaseJsonData
    {
        private int _state = 0;
        /// <summary>
        /// 状态
        /// </summary>
        public int state { get { return _state; } set { _state = value; } }
        /// <summary>
        /// 信息代码，分页数据时，state为1，则为查询总数
        /// </summary>
        public string msg_code { get; set; }
        /// <summary>
        /// 信息说明
        /// </summary>
        public string msg_text { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public object data { get; set; }
    }
}