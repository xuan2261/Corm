using System;

namespace CORM.attrs
{
    [AttributeUsage(AttributeTargets.Class ,
            AllowMultiple = false)]
    public class CormTable : System.Attribute
    {
        private string tableName;

        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }
    }
}