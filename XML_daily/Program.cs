using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ReadXMLfromURL
{
    class XML_DailyConverter
    {
        // <summary>
        /// Необходимо реализовать конвертер из венгерские фориантов, в рубли
        // и из рублей в норвежские кроны, использя данные с сайта ЦБ:
        //   http://www.cbr.ru/scripts/XML_daily.asp
        // </summary>   

        static void Main(string[] args)
        { 
            //Словарь. Ключ - код валюты, значение - курс валюты
            Dictionary<string, decimal> codeToRate = new Dictionary<string, decimal>();

            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            XDocument xdoc = XDocument.Load("http://www.cbr.ru/scripts/XML_daily.asp");

            //добавление курсов валют в словарь

            var t = xdoc.Descendants("Valute")
                    .Where(v => v.Element("CharCode").Value == "HUF" || 
                                v.Element("CharCode").Value == "NOK");
           
            foreach (XElement node in t)
            {
                var charCode = node.Element("CharCode").Value;
                var rate     = GetRateByCharCode(node, charCode);
                codeToRate.Add(charCode, rate);
            }

            GetNokByHuf(codeToRate);
            Console.ReadLine();
        }
        
        //получение курса валюты по названию валюты
        public static  decimal GetRateByCharCode(XElement node, string charCode )
        {
            decimal value = decimal.Parse(node.Element("Value").Value);
            int nominal   = int.Parse(node.Element("Nominal").Value);
            decimal rate = value / nominal;

            return rate;
        }

        //конвертирование валют
        public static void GetNokByHuf(Dictionary<string, decimal> codeToRate)
        {
            Console.WriteLine("Введите количество венгенских фориантов:");
            decimal huf = decimal.Parse(Console.ReadLine());

            //конвертируем из венгерских форинтов в рубли
            decimal convertValute = huf * codeToRate["HUF"];
            Console.WriteLine("{0:N2} HUF = {1:N2} RUB", huf, convertValute);

            decimal rub = convertValute; // В рублях
            convertValute = rub / codeToRate["NOK"];
            Console.WriteLine("{0:N2} RUB = {1:N2} NOK\n", rub, convertValute);

            Console.WriteLine("Курсы валют по отношению к рублю:");
            Console.WriteLine("Венгерские форианты:\t" + codeToRate["HUF"]);
            Console.WriteLine("Норвежские кроны:\t"    + codeToRate["NOK"]);
        }
    }
}