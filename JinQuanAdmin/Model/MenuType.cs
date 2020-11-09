using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JinQuanAdmin.Model
{
    public enum MenuType
    {
        /// <summary>
        /// 展品展示
        /// </summary>
        [Description("展品展示")]
        produceList = 1,
        /// <summary>
        /// 公司新闻
        /// </summary>
        [Description("公司新闻")]
        news_list = 2,

        /// <summary>
        /// 供求信息
        /// </summary>
        [Description("公司新闻")]
        WholesaleList = 3

    }

    public static class EnumExt
    {
        public static string GetDescription(this Enum enumValue)
        {
            string value = enumValue.ToString();
            System.Reflection.FieldInfo field = enumValue.GetType().GetField(value);
            object[] objs = field.GetCustomAttributes(typeof(DescriptionAttribute), false);    //获取描述属性
            if (objs.Length == 0)    //当描述属性没有时，直接返回名称
                return value;
            DescriptionAttribute descriptionAttribute = (DescriptionAttribute)objs[0];
            return descriptionAttribute.Description;
        }


    }
}
