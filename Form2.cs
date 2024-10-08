﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AccountingApp
{
    public partial class Form2 : Form
    {
        public static string formname = "Form2";
        //建立分類
        Categories categories = new Categories();
        Records records = new Records();

        string user = "";
        string input = "";
        public Form2()
        {
            InitializeComponent();
        }

        public Form2(string user)
        {
            InitializeComponent();
            this.user = user + ".txt";
        }


        private void Form2_Load(object sender, EventArgs e)
        {
            this.Text = "歡迎來到簡易記帳 ! ";
            //將按鈕定錨，縮放視窗大小按鈕不會固定不動(按鈕跑掉)
            /*label1.Anchor = AnchorStyles.Left;
            label2.Anchor = AnchorStyles.Right;
            label3.Anchor = AnchorStyles.None;
            lblpay.Anchor = AnchorStyles.Left;
            lblincome.Anchor = AnchorStyles.Right;
            lblbalance.Anchor = AnchorStyles.None;*/
            btnAdd.Anchor = AnchorStyles.Bottom;
            btnexit.Anchor = AnchorStyles.None;
            btnview.Anchor = AnchorStyles.None;
            if (!File.Exists(user))     //若文件不存在
            {
                //即創建user文件
                File.Create(user).Close();
            }
            StreamReader sr = new StreamReader(user);       //讀取文件資料
            while ((input = sr.ReadLine()) != null)         //讀至資料結束
            {
                //將recoed所讀得的資料分開，並加入到record這個列表
                records._records.Add(new Record(input.Split(' ')[0], input.Split(' ')[1], int.Parse(input.Split(' ')[2]), input.Split(' ')[3]));
            }
            sr.Close(); 
        }

        private void Form2_Resize(object sender, EventArgs e)
        {
            //設定背景圖片
            this.BackgroundImage = AccountingApp.Properties.Resources.Form2_bgc;
            this.BackgroundImageLayout = ImageLayout.Stretch;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {

            Form3 f = new Form3(user,ref records);  //產生Form3的物件，才可以使用它所提供的Method
            f.ShowDialog(this);                     //設定Form3為Form2的上層，並開啟Form3視窗。
        }

        private void btnexit_Click(object sender, EventArgs e)
        {
            //按下離開鈕後會結束應用程式
            DialogResult choose;
            string mes = "真的要離開嗎";
            choose = MessageBox.Show(mes, "離開", MessageBoxButtons.OKCancel);
            if (choose == DialogResult.Cancel) {
                MessageBox.Show(records.View());
            }
            else
            {
                StreamWriter sw = new StreamWriter(user);
                foreach(Record r in records._records)
                {
                    sw.WriteLine(r._category+" "+r._description+" "+r._amount+" " + r._time);
                }
                sw.Close();
                this.Close();
            }
        }

        private void btnview_Click(object sender, EventArgs e)
        {
            Form4 f = new Form4(user,ref records);      //產生Form3的物件，才可以使用它所提供的Method
            f.ShowDialog(this);                         //設定Form3為Form2的上層，並開啟Form3視窗。
        }
    }

    //建立record此class以記錄
    public class Record
    {
        public string _category { get; }            //記錄類別
        public string _description { get; }         //記錄描述
        public int _amount { get; }                 //記錄金額

        public string _time { get; }                //記錄時間

        //接收所輸入的資料
        public Record(string category, string description, int amount,string time)
        {
            _category = category;
            _description = description;
            _amount = amount;
            _time = time;
        }
    }

    public class Records
    {
        private int _initial_money;
        public List<Record> _records;

        public Records()
        {
            Initialize();
        }

        public void Initialize()
        {
            //初始金額為0
            int initial_money = 0;
            List<Record> records = new List<Record>();

            try
            {
                using (StreamReader sr = new StreamReader("records.txt"))
                {
                    //若文件存在
                    Console.WriteLine("Welcome back!");
                    initial_money = int.Parse(sr.ReadLine());
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        records.Add(new Record(line.Split(' ')[0], line.Split(' ')[1], int.Parse(line.Split(' ')[2]), line.Split(' ')[3]));
                    }
                }
            }
            //若文件不存在
            catch (FileNotFoundException)
            {
                Console.WriteLine("No previous records found.");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error occurred while reading records: " + e.Message);
            }

            if (initial_money == 0)
            {
                Console.Write("How much money do you have? ");
                string input_money = Console.ReadLine();
                if (int.TryParse(input_money, out initial_money) == false)
                {
                    Console.WriteLine("Invalid value for money. Set to 0 by default.");
                }
            }

            _initial_money = initial_money;
            _records = records;
        }

        
        //建立view此方法
        public string View()
        {
            string result = "";
            result+="Category       \tDescription          \tAmount \n";
            result+="=========================================== \n";

            int total_money = _initial_money;
            foreach (Record record in _records)         //讀取各records內資料
            {
                total_money += record._amount;
                result+=$"{record._category,-15}\t{record._description,-20}\t{record._amount}\n";
            }

            result += "===========================================\n";
            result += $"Now you have {total_money} dollars.\n";

            return result;
        }

        //檢視功能的方法
        public string ViewInTextbox()
        {
            string result = "";

            result += "Index\tCategory       \tDescription          \tAmount   \tTime "+Environment.NewLine;
            result += "======================================================= "+Environment.NewLine;

            int total_money = _initial_money;
            int index = 0;
            foreach (Record record in _records)
            {
                total_money += record._amount;
                result += $"{index,-5}\t{record._category,-15}\t{record._description,-20}\t{record._amount}\t{record._time}" + Environment.NewLine;
                index++;
            }

            result += "==============================================" + Environment.NewLine;
            result += $"Now you have {total_money} dollars." + Environment.NewLine;

            return result;
        }

        //建立listbox內的刪除功能
        public void Delete(int delete_record)
        {
            if (delete_record.GetType() == typeof(int) && delete_record >= 0 && delete_record < _records.Count)
            {
                _records.RemoveAt(delete_record);
            }
            else
            {
                Console.WriteLine("Invalid format. Fail to delete a record.");
            }
        }

        //將分類有對到的資料加到字串並回傳做為總表，使textbox接收完字串可以正確顯示
        public string FindInTextBox(string target_category, Categories categories)
        {
            string result = "";
            result += $"Here's your expense and income records under category '{target_category}' :" + Environment.NewLine;
            result += "Index\tCategory       \tDescription          \tAmount   \tTime" + Environment.NewLine;
          
            int total_amount = 0;
            int index = 0;
            foreach (Record record in _records)
            {
                if (record._category == target_category)
                {
                    total_amount += record._amount;
                    result += $"{index,-5}\t{record._category,-15}\t{record._description,-20}\t{record._amount,-5}\t{record._time}" + Environment.NewLine;
                }
                index++;
            }

            result += "==============================================" + Environment.NewLine;
            result += $"Now you have {total_amount} dollars." + Environment.NewLine;

            return result;
        }

        //將指定分類有對到的資料加到字串並回傳，使listbox接收完字串可以正確顯示
        public List<string> FindInListBox(string target_category, Categories categories)
        {
            if (target_category.Substring(0,1) == "-")
            {
                target_category =  target_category.TrimStart('-');
                Console.WriteLine("remove");
            }
            //如果'target_category'是"食物"、"交通"、"娛樂"、"生活"或"收入"為大類別，用findsubcategory()將結果存入find
            List<string> find = new List<string>();
            if (target_category == "食物"|| target_category == "交通" || target_category == "娛樂" || 
                target_category == "生活" || target_category == "收入" )
            {
               find = categories.FindSubcategories(target_category);
            }
            //儲存匹配的記錄
            List <string> result = new List<string>();
            int total_amount = 0;           //累計總金額
            int index = 0;                  //記錄索引
            foreach (Record record in _records)
            {
                //若搜尋全，要查所有記錄的類別
                if(target_category == "全")
                {
                    total_amount += record._amount;
                    result.Add($"{index,-5}\t{record._category,-15}\t{record._description,-20}\t{record._amount, -5}\t{record._time}");
                }
                //若搜詢指定類別，記錄到result並累計金額
                else if (record._category == target_category)
                {
                    total_amount += record._amount;
                    result.Add($"{index,-5}\t{record._category,-15}\t{record._description,-20}\t{record._amount, -5}\t{record._time}");
                }
                else
                {
                    if(find.Contains(record._category))
                    {
                        total_amount += record._amount;
                        result.Add($"{index,-5}\t{record._category,-15}\t{record._description,-20}\t{record._amount,-5}\t{record._time}");
                    }
                }
                index++;
            }

            /*
             List <string> result = new List<string>();
            int total_amount = 0;
            int index = 0;
            foreach (Record record in _records)
            {
                if(target_category == "全")
                {
                    total_amount += record._amount;
                    result.Add($"{index,-5}\t{record._category,-15}\t{record._description,-20}\t{record._amount, -5}\t{record._time}");
                }
                else if (record._category == target_category)
                {
                    total_amount += record._amount;
                    result.Add($"{index,-5}\t{record._category,-15}\t{record._description,-20}\t{record._amount, -5}\t{record._time}");
                }
                index++;
            }
             */


            return result;
        }


        
    }

    //創建categories以記錄標籤
    public class Categories
    {
        private List<string> _categories;

        public Categories()
        {
            _categories = new List<string>
        {
            "支出"
            ,"食物","早餐", "午餐", "晚餐", "點心", "飲料", "宵夜","其他（食）"
            ,"交通","汽油","公車","捷運","火車","其他（交）"
            ,"娛樂","電影","衣著","旅遊","其他（娛）"
            ,"生活", "房租", "電信", "水電", "其他（生）"
            ,"收入"
            ," ","薪水", "獎金", "其他(收)"
        };
        }
        //將小分類規劃至大分類，可將其分至大類別中
        public List<string> FindSubcategories(string target_category)
        {
            List<string> find_categories = new List<string>();

            if (target_category == "支出")
            {
                for (int i = 1; i < 24; i++)
                {
                    find_categories.Add(_categories[i]);
                }
            }
            if (target_category == "食物")
            {
                for (int i = 2; i < 9; i++)
                {
                    find_categories.Add(_categories[i]);
                }
            }
            if (target_category == "交通")
            {
                for (int i = 10; i < 15; i++)
                {
                    find_categories.Add(_categories[i]);
                }
            }
            if (target_category == "娛樂")
            {
                for (int i = 16; i < 20; i++)
                {
                    find_categories.Add(_categories[i]);
                }
            }
            if (target_category == "生活")
            {
                for (int i = 21; i < 25; i++)
                {
                    find_categories.Add(_categories[i]);
                }
            }
            if (target_category == "收入")
            {
                for (int i = 27; i < 30; i++)
                {
                    find_categories.Add(_categories[i]);
                }
            }
            return find_categories;
        }
        
    }
    
}
