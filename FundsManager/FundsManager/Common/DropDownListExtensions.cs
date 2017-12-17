using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// 下拉框扩展
    /// mailto:xiaotuni@gmail.com
    /// 我的BLOG http://blog.csdn.net/xiaotuni
    /// </summary>
    public static class DropDownListExtensions
    {
        static string MvcResources_HtmlHelper_MissingSelectData = "Missing Select Data";
        static string MvcResources_HtmlHelper_WrongSelectDataType = "Wrong Select Data Type";
        static string MvcResources_Common_NullOrEmpty = "Null Or Empty";

        /// <summary>
        /// 创建一个DropDownList控件
        /// </summary>
        /// <typeparam name="TModel">当前数据</typeparam>
        /// <typeparam name="TProperty">属性</typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression">lambda 表达式</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="selectList">DropDownList里的数据</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression, object defaultValue, IEnumerable<SelectListItem> selectList)
        {
            return DropDownListFor(htmlHelper, expression, defaultValue, selectList, null, null);
        }
        /// <summary>
        /// 创建一个DropDownList控件
        /// </summary>
        /// <typeparam name="TModel">当前数据</typeparam>
        /// <typeparam name="TProperty">属性</typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression">lambda 表达式</param>
        /// <param name="defaultValue">默认值</param>
        /// <param name="selectList">DropDownList里的数据</param>
        /// <param name="optionLabel">标签名称</param>
        /// <param name="htmlAttributes">DorpDownList属性</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object defaultValue, IEnumerable<SelectListItem> selectList, string optionLabel, IDictionary<string, object> htmlAttributes)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            string name = ExpressionHelper.GetExpressionText(expression);
            return SelectInternal(htmlHelper, optionLabel, name, selectList, defaultValue, false, htmlAttributes);
        }

        /// <summary>
        /// 获取选中项数据
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        static IEnumerable<SelectListItem> GetSelectData(this HtmlHelper htmlHelper, string name)
        {
            object o = null;
            if (htmlHelper.ViewData != null)
            {
                o = htmlHelper.ViewData.Eval(name);
            }
            if (o == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources_HtmlHelper_MissingSelectData,
                        name,
                        "IEnumerable<SelectListItem>"));
            }
            IEnumerable<SelectListItem> selectList = o as IEnumerable<SelectListItem>;
            if (selectList == null)
            {
                throw new InvalidOperationException(
                    String.Format(
                        CultureInfo.CurrentCulture,
                        MvcResources_HtmlHelper_WrongSelectDataType,
                        name,
                        o.GetType().FullName,
                        "IEnumerable<SelectListItem>"));
            }
            return selectList;
        }

        static object GetModelStateValue(HtmlHelper htmlHelper, string key, Type destinationType)
        {
            ModelState modelState;
            if (htmlHelper.ViewContext.ViewData.ModelState.TryGetValue(key, out modelState))
            {
                if (modelState.Value != null)
                {
                    return modelState.Value.ConvertTo(destinationType, null /* culture */);
                }
            }
            return null;
        }

        static string ListItemToOption(SelectListItem item)
        {
            TagBuilder builder = new TagBuilder("option")
            {
                InnerHtml = HttpUtility.HtmlEncode(item.Text)
            };
            if (item.Value != null)
            {
                builder.Attributes["value"] = item.Value;
            }
            if (item.Selected)
            {
                builder.Attributes["selected"] = "selected";
            }
            return builder.ToString(TagRenderMode.Normal);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="optionLabel"></param>
        /// <param name="name"></param>
        /// <param name="selectList"></param>
        /// <param name="selectedValue"></param>
        /// <param name="allowMultiple"></param>
        /// <param name="htmlAttributes"></param>
        /// <returns></returns>
        static MvcHtmlString SelectInternal(this HtmlHelper htmlHelper, string optionLabel, string name, IEnumerable<SelectListItem> selectList, object selectedValue, bool allowMultiple, IDictionary<string, object> htmlAttributes)
        {
            string fullName = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(name);
            if (String.IsNullOrEmpty(fullName))
            {
                throw new ArgumentException(MvcResources_Common_NullOrEmpty, "name");
            }

            bool usedViewData = false;

            if (selectList == null)
            {
                selectList = htmlHelper.GetSelectData(fullName);
                usedViewData = true;
            }


            object defaultValue = selectedValue != null ? selectedValue : (allowMultiple) ? GetModelStateValue(htmlHelper, fullName, typeof(string[])) : GetModelStateValue(htmlHelper, fullName, typeof(string));

            if (!usedViewData)
            {
                if (defaultValue == null)
                {
                    defaultValue = htmlHelper.ViewData.Eval(fullName);
                }
            }

            if (defaultValue != null)
            {
                IEnumerable defaultValues = (allowMultiple) ? defaultValue as IEnumerable : new[] { defaultValue };
                IEnumerable<string> values = from object value in defaultValues select Convert.ToString(value, CultureInfo.CurrentCulture);
                HashSet<string> selectedValues = new HashSet<string>(values, StringComparer.OrdinalIgnoreCase);
                List<SelectListItem> newSelectList = new List<SelectListItem>();

                foreach (SelectListItem item in selectList)
                {
                    item.Selected = (item.Value != null) ? selectedValues.Contains(item.Value) : selectedValues.Contains(item.Text);
                    newSelectList.Add(item);
                }
                selectList = newSelectList;
            }

            StringBuilder listItemBuilder = new StringBuilder();

            if (optionLabel != null)
            {
                listItemBuilder.AppendLine(ListItemToOption(new SelectListItem() { Text = optionLabel, Value = String.Empty, Selected = false }));
            }

            foreach (SelectListItem item in selectList)
            {
                listItemBuilder.AppendLine(ListItemToOption(item));
            }

            TagBuilder tagBuilder = new TagBuilder("select")
            {
                InnerHtml = listItemBuilder.ToString()
            };
            tagBuilder.MergeAttributes(htmlAttributes);
            tagBuilder.MergeAttribute("name", fullName, true /* replaceExisting */);
            tagBuilder.GenerateId(fullName);
            if (allowMultiple)
            {
                tagBuilder.MergeAttribute("multiple", "multiple");
            }

            ModelState modelState;
            if (htmlHelper.ViewData.ModelState.TryGetValue(fullName, out modelState))
            {
                if (modelState.Errors.Count > 0)
                {
                    tagBuilder.AddCssClass(HtmlHelper.ValidationInputCssClassName);
                }
            }

            tagBuilder.MergeAttributes(htmlHelper.GetUnobtrusiveValidationAttributes(name));

            return MvcHtmlString.Create(tagBuilder.ToString(TagRenderMode.Normal));
        }
    }
}