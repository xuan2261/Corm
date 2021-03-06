﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using CORM.utils;
using CORM.attrs;

namespace CORM
{
    [Table(TableName = "Student")]
    public class Student
    {
        [Column(Name = "Id", Size = 64, DbType = SqlDbType.VarChar, PrimaryKey = true)]
        public string Id { get; set; }
        [Column(Name = "studentName_", Size = 10,DbType = SqlDbType.VarChar)]
        public string studentName { get; set; }
        [Column(Name = "studentAge_", DbType = SqlDbType.Int)]
        public int? studentAge { get; set; }
        [Column(Name = "studentSex_", DbType = SqlDbType.VarChar, Size = 2)]
        public string studentSex { get; set; }
    }
    
    internal class Program
    {
        /*
         * 自定义一个回调类来完成 sql 打印
         */
        private class CustomSqlPrintCb : CormSqlPrintCB
        {
            public void SqlPrint(string sql)
            {
                sql = sql.Replace("\n", " ");
                sql = sql.Replace("  ", " ");
                Console.WriteLine(sql);
            }
        }

        private class TempStruct
        {
            [Column(Name = "name")]
            public string Name { get; set; }
            [Column(Name = "age")]
            public int? Age { get; set; }
        }
        
        public static void Main(string[] args)
        {
            var corm = new Corm.CormBuilder()
                .Server("server=127.0.0.1;database=corm;uid=TestAccount;pwd=TestAccount")
//                .SqlPrint(new CustomSqlPrintCb())    // 是否使用自定义的 Sql 打印方法
                .SyncTable(true)            // 是否自动创建表/自动同步表结构
                .Build();
            var studentTable = new CormTable<Student>(corm);

//            // 判断表是否存在，删除表，建表
//            if (studentTable.Exist())
//            {
//                studentTable.DropTable();
//                studentTable.CreateTable();
//            }
//            else
//            {
//                studentTable.CreateTable();
//            }
            
//            获取 Entity 类的信息
//            Console.WriteLine(CormUtils<Student>.GetTableName());
//            Console.WriteLine(CormUtils<Student>.GetProPropertyInfoMap().Count);

//            显示 DDL
//            Console.WriteLine(studentTable.DDL());
            

            List<Student> list;
                        
            /*
            // SELECT 查询全部数据
            list = studentTable.Find().Commit();
            Console.WriteLine(list.Count);
            
            
            // Order By ASC 排序
            list = studentTable.Find().OrderBy(new string[] {"studentAge_"}).Commit();
            Console.WriteLine(list.Count);
            // Order By DESC 排序
            list = studentTable.Find().OrderDescBy(new string[] {"studentAge_"}).Commit();
            Console.WriteLine(list.Count);
            

            // SELECT 按 where 条件查询
            var st = new Student();
            st.studentAge = 10;
            list = studentTable.Find().Where(st).Commit();
            // 自定义 Where 查询
            //SELECT * FROM Student WHERE studentName_ like '%me%' and studentAge_ > @StudentAge  ;
            list = studentTable.Find().WhereQuery("studentName_ like '%me%' and studentAge_ > @StudentAge", new[]
            {
                new SqlParameter("@StudentAge", 5),
            }).Commit();
            Console.WriteLine(list.Count);
            
            // SELECT 查询特定的属性
            list = studentTable.Find().Attributes(new[] {"studentName_"}).Commit();
            Console.WriteLine(list.Count);
            
            // SELECT 只查询前 n 条
            list = studentTable.Find().Top(1).Commit();
            Console.WriteLine(list.Count);
        
            // 直接返回 SqlDataReader
            // 并使用 SqlDataReaderParse 工具解析 reader
            var sqlTemp1 = @"SELECT 
                            studentName_ as name, 
                            studentAge_ as age 
                        FROM Student ";
            SqlDataReader reader = studentTable.Customize().SQL(sqlTemp1).CommitForReader();
            List<TempStruct> listTemp = SqlDataReaderParse<TempStruct>.parse(reader, true, true);
            Console.WriteLine(listTemp.Count);
            

            
            // SELECT 自定义查询语句
            list = studentTable.Find().Customize(
                "SELECT * FROM Student WHERE studentName_=@studentName_",
                new SqlParameter[]
                {
                    new SqlParameter("@studentName_", "test3"),
                }
            ).Commit();
            Console.WriteLine(list.Count);
            */

            /*
            // INSERT 插入，可插入 list 或者 单条数据，插入数据带有事务性质
            var insert1 = new Student
            {
                Id = "aaa",
                studentAge = 1, 
                studentName = "inset1",
            };
            var insert2 = new Student()
            {
                Id = "bbb",
                studentAge = 02,
                studentName = "inset2",
            };
            studentTable.Insert().Value(new List<Student>(){insert1,insert2}).Commit();
            studentTable.Insert().Value(insert1).Commit();
            */

            /*
            // UPDATE 更新，以 Where 作为过滤规则，以 Value 作为更新的值
            // 以下命令生成的 SqlCommand 语句为
            // UPDATE Student SET studentAge_=@studentAge_VALUE WHERE studentName_=@studentName_OLD ;
            // 相当于 Sql 
            // UPDATE Student SET studentAge_ = 20 WHERE studentName_ = 'testtest'
            studentTable.Update().Where(new Student()
            {
                studentName = "testtest",
            })
            .Value(new Student()
            {
                studentAge = 20,
            })
            .Commit();
            
            // 使用 WhereQuery 设置 UPDATE 条件
            // UPDATE Student SET studentSex_='男' WHERE studentName_ like '%name%' and studentAge_ > 2 ;
            studentTable.Update().WhereQuery("studentName_ like @NameQuery and studentAge_ > @StudentAge ", new []
            {
                new SqlParameter("NameQuery", "%name%"),
                new SqlParameter("StudentAge", 2), 
            }).Value( new Student(){studentSex = "男"} ).Commit();
            
            */

            /*
            // 判断是否存在特定条件的学生
            bool hasThisStudent = studentTable.Find().Where(new Student() {studentName = "inset2"}).CommitForHas();
            Console.WriteLine(hasThisStudent);
            */

            /*
            // Delete 删除操作
            // 删除该表全部数据
//            studentTable.Delete().All().Commit();
            // 删除所有 studentName 为 "testtest" , studentAge 为 20 的行
            studentTable.Delete().Where(new Student()
            {
                studentName = "testtest", 
                studentAge = 20, 
            }).Commit();
            
            // 使用 WhereQuery 设置 DELETE 条件
            // DELETE FROM Student  WHERE studentAge_ > 20 ; 
            studentTable.Delete().WhereQuery("studentAge_ > @StudentAge", new[]
            {
                new SqlParameter("StudentAge", 20),
            }).Commit();
            */

            /*
            // 事务操作示例
            using (CormTransaction transaction = studentTable.BeginTransaction())
            {
                try
                {
                    studentTable.Insert()
                        .Value(new Student() {studentName = "oldName"})
                        .Commit(transaction);
                    list = studentTable.Find()
                        .Where(new Student() {studentName = "oldName"})
                        .Commit(transaction);
                    studentTable.Update()
                        .Where(new Student() {studentName = list[0].studentName})
                        .Value(new Student() {studentName = "newName"})
                        .Commit(transaction);
                    var sql = @"SELECT * FROM student WHERE studentName_= @studentName_";
                    var list2 = studentTable.Customize()
                        .SQL(sql,new[] {new SqlParameter("studentName_", "newName"),})
                        .CommitForList(transaction);
                    Console.WriteLine(list2.Count);
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine("发生异常：" + e.Message + " ，插入失败，事务回滚");
                    transaction.Rollback();
                }
            }
            */

        }
    }
}