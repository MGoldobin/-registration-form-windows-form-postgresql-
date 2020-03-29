using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
using System.Security.Cryptography;

namespace форма_регистрации_БД
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string hash(string password)
        {
            //переводим строку в байт-массим  
            byte[] bytes = Encoding.Unicode.GetBytes(password);

            //создаем объект для получения средст шифрования  
            MD5CryptoServiceProvider CSP =
                new MD5CryptoServiceProvider();

            //вычисляем хеш-представление в байтах  
            byte[] byteHash = CSP.ComputeHash(bytes);

            string hash = string.Empty;

            //формируем одну цельную строку из массива  
            foreach (byte b in byteHash)
                hash += string.Format("{0:x2}", b);

            return hash;
        }
        string salt = "ILDB";

        private async void buttonReg_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxLogin.Text) && !string.IsNullOrWhiteSpace(textBoxLogin.Text) &&
                !string.IsNullOrEmpty(textBoxPass.Text) && !string.IsNullOrWhiteSpace(textBoxPass.Text))
            {
                string login = textBoxLogin.Text;
                string password = hash(textBoxPass.Text + salt);
                string ConnectionString = @"Host = localhost;Username = postgres;Password = Misha310; Database = postgres";
                var Connect = new NpgsqlConnection(ConnectionString);
                Connect.Open();
                NpgsqlCommand Command = new NpgsqlCommand("insert into users(login, password) values (@login, @password)", Connect);
                Command.Parameters.AddWithValue("login", login);
                Command.Parameters.AddWithValue("password", password);
                try
                {
                    await Command.ExecuteNonQueryAsync();
                    labelError.Text = "Пользоватьтель с логином " + login + " зарегистрирован";
                }
                catch
                {
                    labelError.Text = "Пользоватьтель с логином " + login + "  уже существует";
                }
                finally
                {
                    textBoxLogin.Text = "";
                    textBoxPass.Text = "";
                    Connect.Close();
                }
            }
            else labelError.Text = "Поля 'Login'  и  'Password' должны быть заполнены!";

        }

        private void buttonLogin_Click_1(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBoxLogin.Text) && !string.IsNullOrWhiteSpace(textBoxLogin.Text) &&
                  !string.IsNullOrEmpty(textBoxPass.Text) && !string.IsNullOrWhiteSpace(textBoxPass.Text))
            {
                string login = textBoxLogin.Text;
                string password = hash(textBoxPass.Text + salt);
                string ConnectionString = @"Host = localhost;Username = postgres;Password = Misha310; Database = postgres";
                NpgsqlConnection Connect = new NpgsqlConnection(ConnectionString);
                Connect.Open();
                var Command = new NpgsqlCommand("select password from users where login = @login", Connect);
                Command.Parameters.AddWithValue("login", login);
                var Reader = Command.ExecuteReader();
                if (Reader.Read() == true)
                {
                    if (Reader.GetString(0) == password)
                    {
                        labelError.Text = "Logged in";
                        textBoxLogin.Text = "";
                        textBoxPass.Text = "";
                    }
                    else
                    {
                        labelError.Text = "Incorrect password";
                        textBoxPass.Text = "";
                    }
                }
                textBoxLogin.Text = "";
                textBoxPass.Text = "";
                Connect.Close();
            } else labelError.Text = "Поля 'Login'  и  'Password' должны быть заполнены!";
        }
        
    }
}
