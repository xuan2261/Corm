using System.Collections.Generic;
using System.Data.SqlClient;
using CORM.attrs;
using CORM.utils;

namespace CORM
{
    public class CormTable<T> where T : new()
    {
        public Corm _corm { get; }
        public string _tableName { get; }
        public List<string> ColumnNameTemp { get; }
        
        public CormTable(Corm corm)
        {
            this._corm = corm;
            var properties = typeof(T).GetProperties();
            // 自动分析 TableName
            var tableAttributes = typeof(T).GetCustomAttributes(typeof(Table), true);
            if (tableAttributes.Length > 0)
            {
                Table tableAttr = tableAttributes[0] as Table;
                if (tableAttr != null && tableAttr.TableName != null && !tableAttr.TableName.Trim().Equals(""))
                {
                    this._tableName = tableAttr.TableName;
                }
            }
            if (this._tableName == null || this._tableName.Trim().Equals(""))
            {
                throw new CormException("Entity 类 " + typeof(T).Name + "没有指定 TableName 属性，" +
                                        "请使用 [CormTable(TableName=\"xxx\")] 或在CormTable 构造函数中指定");
   
            }
            this.ColumnNameTemp = new List<string>();
            foreach (var property in properties)
            {
                var objAttrs = property.GetCustomAttributes(typeof(Column), true);
                if (objAttrs.Length > 0)
                {
                    Column attr = objAttrs[0] as Column;
                    if (attr != null)
                    {
                        this.ColumnNameTemp.Add(attr.Name);
                    }
                }
            }
        }
        
        public CormTable(Corm corm ,string tableName)
        {
            this._corm = corm;
            this._tableName = tableName;
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var objAttrs = property.GetCustomAttributes(typeof(Column), true);
                if (objAttrs.Length > 0)
                {
                    Column attr = objAttrs[0] as Column;
                    if (attr != null)
                    {
                        this.ColumnNameTemp.Add(attr.Name);
                    }
                }
            }
        }

        // Select 查询
        public CormSelectMiddleSql<T> Find()
        {
            return new CormSelectMiddleSql<T>(this);
        }

        // Inset 插入
        public CormInsertMiddleSql<T> Insert()
        {
            return new CormInsertMiddleSql<T>(this);
        }
        
        // Update 更新
        public CormUpdateMiddleSql<T> Update()
        {
            return new CormUpdateMiddleSql<T>(this);
        }
        
        // Delete 删除操作
        public CormDeleteMiddleSql<T> Delete()
        {
            return new CormDeleteMiddleSql<T>(this);
        }

        public CormTransaction BeginTransaction()
        {
            return new CormTransaction(this._corm);
        }
        
        public SqlCommand SqlCommand(string sql)
        {
            return new SqlCommand(sql, this._corm._sqlConnection);
        }

        public void SqlLog(string sql)
        {
            _corm.LogUtils.SqlPrint(sql);
        }
        
    }
    
    
}