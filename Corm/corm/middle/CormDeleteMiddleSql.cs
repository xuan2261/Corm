using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using CORM.attrs;
using CORM.utils;

namespace CORM
{
    public class CormDeleteMiddleSql<T> where T : new()
    {
        private CormTable<T> _cormTable;
        private string sqlBuff;
        private string tableName;
        // 缓存该类型的列名，避免经常反射
        private Dictionary<string, PropertyInfo> PropertyMap;

        private T whereObj;
        private bool deleteAllFlag = false;
        
        public CormDeleteMiddleSql(CormTable<T> cormTable)
        {
            this._cormTable = cormTable;
            this.tableName = _cormTable._tableName;
            this.PropertyMap = _cormTable.PropertyMap;
        }

        public CormDeleteMiddleSql<T> Where(T obj)
        {
            this.whereObj = obj;
            return this;
        }

        public CormDeleteMiddleSql<T> All()
        {
            deleteAllFlag = true;
            return this;
        }

        public int Commit()
        {
            return Commit(null);
        }
        
        /*
         * DELETE FROM [dbo].[Product]
         */
        public int Commit(CormTransaction transaction)
        {
            int resDeleteSize = -1;
            sqlBuff += "DELETE FROM " + this.tableName + " ";
            // 既没有使用 All() 方法也没有使用 Where() 方法，抛出异常提示用户
            if (whereObj == null && !deleteAllFlag)
            {
                throw new CormException("DELETE 需要指定删除范围，请使用 All() 方法或者 Where() 方法");
            }
            else
            {
                if (whereObj == null && deleteAllFlag)
                {
                    // All 条件
                    sqlBuff = sqlBuff + ";";
                }
                else if (whereObj != null && !deleteAllFlag)
                {
                    // Query 条件
                    var whereQuery = GetWhereQuery(whereObj);
                    if (whereQuery.Trim().Equals(""))
                    {
                        throw new CormException("DELETE 没有指定具体的 Where 条件");
                    }
                    else
                    {
                        sqlBuff = sqlBuff + whereQuery;
                    }
                }
                else
                {
                    throw new CormException("DELETE 时候 All() 和 Where() 不能同时使用");
                }
            }
            this._cormTable.SqlLog(sqlBuff);            
            List<SqlParameter> paramList = new List<SqlParameter>(); 
            var properties = typeof(T).GetProperties();
            if (sqlBuff.Contains("WHERE "))
            {
                foreach (string key in PropertyMap.Keys)
                {
                    if (sqlBuff.Contains("@"+key))
                    {
                        // 证明预编译语句里面有这个属性
                        var value = PropertyMap[key].GetValue(this.whereObj);
                        Column attr = PropertyMap[key].GetCustomAttributes(typeof(Column), true)[0] as Column;
                        var param = new SqlParameter("@"+attr.Name, attr.DbType, attr.Size);
                        param.Value = value;
                        paramList.Add(param);
                    }
                }
            }

            if (transaction != null)
            {
                resDeleteSize = transaction.AddSql(sqlBuff, paramList).ExecuteNonQuery();
            }
            else
            {
                using (SqlConnection conn = this._cormTable._corm.NewConnection())
                {
                    SqlCommand sqlCommand = new SqlCommand(sqlBuff, conn);
                    foreach (SqlParameter param in paramList)
                    {
                        sqlCommand.Parameters.Add(param);
                    }
                    resDeleteSize = sqlCommand.ExecuteNonQuery();
                }
            }
            if (resDeleteSize < 0)
            {
                throw new CormException(" DELETE 操作，受影响操作函数 < 0，请检查是否有错误");
            }
            return resDeleteSize;
        }
        
        
        private string GetWhereQuery(T obj)
        {
            if (obj == null){
                return "";
            }
            var resWhereQuery = "";
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                // 如果这个属性存在的话
                if (value != null)
                {
                    // 从注解厘米拿到具体的字段名称，拼接
                    var objAttrs = property.GetCustomAttributes(typeof(Column), true);
                    if (objAttrs.Length > 0)
                    {
                        Column attr = objAttrs[0] as Column;
                        if (attr != null)
                        {
                            resWhereQuery += " " + attr.Name + "=@" + attr.Name + " and";
                        }
                    }
                }
            }
            if (resWhereQuery.EndsWith("and"))
            {
                resWhereQuery = "WHERE "+resWhereQuery.Substring(0, resWhereQuery.Length - 3);
            }
            return resWhereQuery;
        }
    }
}