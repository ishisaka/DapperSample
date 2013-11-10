// ============================================================================
// DapperSample - GetData - Program.cs
// 
// Last update	:2013-11-10  Tadahiro Ishisaka
// Origin		:2013-11-04 
// ============================================================================
// 
// License:
// 
//    Copyright 2013 Tadahiro Ishisaka
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//        http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// 
// --------------------------------------------------------------------------
// 

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace GetData
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string connecttionString =
                @"Data Source=(localdb)\Projects;Initial Catalog=DappaerSample;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False";
            using (var con = new SqlConnection(connecttionString))
            {
                int ret;
                con.Open();
                //データ追加
                Console.WriteLine("単純なインサート");
                ret = con.Execute("INSERT Log(TimeStamp, Description) VALUES(@tm, @desc)",
                    new {tm = DateTime.Now, desc = "Description"});
                Console.WriteLine("{0}行追加しました。", ret);
                //データ抽出
                Console.WriteLine("SELECT");
                var logs = con.Query<Log>("SELECT Id, TimeStamp, Description FROM Log");
                logs.ToList().ForEach(l => Console.WriteLine("{0}, {1}, {2}", l.Id, l.TimeStamp, l.Description));
                //ストアードプロシージャの実行
                Console.WriteLine("ストアードプロシージャの実行");
                var logs2 = con.Query<Log>("GetLogs", new {param1 = DateTime.Now.AddHours(1.0)},
                    commandType: CommandType.StoredProcedure);
                logs2.ToList().ForEach(l => Console.WriteLine("{0}, {1}, {2}", l.Id, l.TimeStamp, l.Description));
                //事前に作られたオブジェクトを使ってインサートする。
                Console.WriteLine("事前に作られたオブジェクトを使ってインサートする。");
                var log = new Log {TimeStamp = DateTime.Now, Description = "はろー"};
                //POCOのインスタンスを使ってINSERTなどをする場合には、名前解決できるようにQuery側の引き数名をクラスのプロパティ名とあわせておく
                ret = con.Execute("INSERT Log(TimeStamp, Description) VALUES(@TimeStamp, @Description)", log);
                Console.WriteLine("{0}行処理をしました。", ret);
                //リストを使った複数データのインサート。アップデートも同じやり方
                Console.WriteLine("List<T>での複数行のインサート");
                var logs3 = new List<Log>
                {
                    new Log {TimeStamp = DateTime.Now, Description = "はろー1"},
                    new Log {TimeStamp = DateTime.Now, Description = "はろー2"},
                    new Log {TimeStamp = DateTime.Now, Description = "はろー3"}
                };
                ret = con.Execute("INSERT Log(TimeStamp, Description) VALUES(@TimeStamp, @Description)", logs3);
                Console.WriteLine("{0}行処理をしました。", ret);
                //これは匿名型を使ってこう書いてしまっても良い
                Console.WriteLine("匿名型の配列を使っての複数行のインサート");
                ret = con.Execute("INSERT Log(TimeStamp, Description) VALUES(@TimeStamp, @Description)",
                    new[]
                    {
                        new {TimeStamp = DateTime.Now, Description = "はろー11"},
                        new {TimeStamp = DateTime.Now, Description = "はろー12"},
                        new {TimeStamp = DateTime.Now, Description = "はろー13"}
                    });
                Console.WriteLine("{0}行処理をしました。", ret);
                //Update
                Console.WriteLine("更新処理");
                con.Execute("INSERT Log(TimeStamp, Description) VALUES(@tm, @desc)",
                    new {tm = new DateTime(2013, 01, 01), desc = "アップデート前"});
                var log4 =
                    con.Query<Log>("SELECT Id, TimeStamp, Description FROM Log WHERE TimeStamp = @tm",
                        new {tm = new DateTime(2013, 01, 01)}).First();

                log4.Description = "アップデート後";
                //Updateの実行
                ret = con.Execute("UPDATE Log SET Description = @Description WHERE Id = @Id", log4);
                Console.WriteLine("{0}行処理をしました。", ret);
                logs = con.Query<Log>("SELECT Id, TimeStamp, Description FROM Log ORDER BY TimeStamp");
                logs.ToList().ForEach(l => Console.WriteLine("{0}, {1}, {2}", l.Id, l.TimeStamp, l.Description));

                //Delete
                Console.WriteLine("削除処理");
                ret = con.Execute("DELETE Log WHERE Id = @Id", log4);
                Console.WriteLine("{0}行処理をしました。", ret);
                logs = con.Query<Log>("SELECT Id, TimeStamp, Description FROM Log ORDER BY TimeStamp");
                logs.ToList().ForEach(l => Console.WriteLine("{0}, {1}, {2}", l.Id, l.TimeStamp, l.Description));


                Console.WriteLine("テーブルのデータを削除するにはEnterキーを押して下さい。");
                Console.ReadLine();
                ret = con.Execute("TRUNCATE TABLE Log");
                Console.WriteLine("戻り値 {0}", ret);

                Console.WriteLine("終了するにはEnterキーを押して下さい。");
                Console.ReadLine();
            }
        }
    }

    public class Log
    {
        public Guid Id { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Description { get; set; }
    }
}