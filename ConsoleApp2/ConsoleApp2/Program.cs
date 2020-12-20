using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Linq;
/*1. Из лабораторной №5 выберите класс с наследованием и/или
композицией/агрегацией для сериализации. Выполните
сериализацию/десериализацию объекта используя
a. бинарный,
b. SOAP,
c. JSON,
d. XML формат.
2. Создайте коллекцию (массив) объектов и выполните
сериализацию/десериализацию.
3. Используя XPath напишите два селектора для вашего XML документа.
4. Используя Linq to XML (или Linq to JSON) создайте новый xml (json) -
документ и напишите несколько запросов.*/

namespace lab14
{
    [Serializable]
    public abstract partial class Exam
    {
        public string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        public int mark;
        public int Mark
        {
            get { return mark; }
            set { mark = value; }
        }
        public Exam(string Name, int Mark)
        {
            this.Name = Name;
            this.Mark = Mark;
        }
        public Exam()
        {

        }
    }
    [Serializable]
    public class FinalExam : Exam
    {
        public string Specialty { get; set; }
        public FinalExam()
        {

        }
        public FinalExam(string Name, int Mark, string Specialty) : base(Name, Mark)
        {
            this.Specialty = Specialty;
        }
    }
    class Program
    {

        static void Main(string[] args)
        {
            FinalExam ex1 = new FinalExam("Ksenia", 8, "DAIVI");
            FinalExam ex2 = new FinalExam("Karina", 6, "POIT");
            FinalExam ex3 = new FinalExam("Petr", 3, "ISIT");
            FinalExam[] exams = new FinalExam[] { ex1, ex2, ex3 };


            BinaryFormatter formatter = new BinaryFormatter();//бинарная сериализация
            using (FileStream fs = new FileStream("ex.dat", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, ex1);
                Console.WriteLine("Сериализовали объект");
            }
            using (FileStream fs = new FileStream("ex.dat", FileMode.OpenOrCreate))
            {
                FinalExam newpex1 = (FinalExam)formatter.Deserialize(fs);
                Console.WriteLine("Объект десериализован");

                Console.WriteLine($"{newpex1.ToString()} ");
            }

            SoapFormatter soap = new SoapFormatter();//Soap сериализация
            using (FileStream fs = new FileStream("ex.soap", FileMode.OpenOrCreate))
            {
                soap.Serialize(fs, ex2);
                Console.WriteLine("Объект сериализован (SOAP)");
            }
            using (FileStream fs = new FileStream("ex.soap", FileMode.OpenOrCreate))
            {
                FinalExam newpex2 = (FinalExam)soap.Deserialize(fs);
                Console.WriteLine("Объект десериализован(SOAP)");
                Console.WriteLine($"{newpex2.ToString()} ");
            }


            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(FinalExam));//Json сериализация
            using (FileStream fs = new FileStream("ex.json", FileMode.OpenOrCreate))
            {
                js.WriteObject(fs, ex3);
                Console.WriteLine("Объект сериализован (JSON)");
            }
            using (FileStream fs = new FileStream("ex.json", FileMode.OpenOrCreate))
            {
                FinalExam newpex3 = (FinalExam)js.ReadObject(fs);
                Console.WriteLine("Объект десериализован(Json)");
                Console.WriteLine($"{newpex3.ToString()} ");
            }


            XmlSerializer xml = new XmlSerializer(typeof(FinalExam));//XML сериализация
            using (FileStream fs = new FileStream("ex.xml", FileMode.OpenOrCreate))
            {
                xml.Serialize(fs, ex3);
                Console.WriteLine("Объект сериализован (XML)");
            }
            using (FileStream fs = new FileStream("ex.xml", FileMode.OpenOrCreate))
            {
                FinalExam newpex4 = (FinalExam)xml.Deserialize(fs);
                Console.WriteLine("Объект десериализован(XML)");
                Console.WriteLine($"{newpex4.ToString()} ");
            }



            XmlSerializer xmlarr = new XmlSerializer(typeof(FinalExam[]));//сериализация массива
            using (FileStream fs = new FileStream("exArr.xml", FileMode.OpenOrCreate))
            {
                xmlarr.Serialize(fs, exams);
                Console.WriteLine("Объект сериализован (XML)");
            }

            using (FileStream fs = new FileStream("exArr.xml", FileMode.OpenOrCreate))
            {
                FinalExam[] newpeople = (FinalExam[])xmlarr.Deserialize(fs);
                Console.WriteLine("Объект десериализован(XML)");
                Console.WriteLine($"{newpeople.ToString()} ");
            }


            XmlDocument xDoc = new XmlDocument();
            xDoc.Load("11.xml");
            XmlElement xRoot = xDoc.DocumentElement;
            XmlNodeList childnodes = xRoot.SelectNodes("*");//1 селектор
            foreach (XmlNode n in childnodes)
                Console.WriteLine(n.OuterXml);

            XmlNodeList students = xRoot.SelectNodes("//student/age");//2 селектор
            foreach (XmlNode s in students)
                Console.WriteLine(s.OuterXml);


            XDocument xdoc = new XDocument();//новый xml и запросы
            XElement teacher1 = new XElement("teacher");
            XAttribute teacherName1 = new XAttribute("name", "Татьяна Александровна");
            XElement unic1 = new XElement("university", "БГТУ");
            XElement age1 = new XElement("age", "40");
            teacher1.Add(teacherName1);
            teacher1.Add(unic1);
            teacher1.Add(age1);
            XElement teacher2 = new XElement("teacher");
            XAttribute teacherName2 = new XAttribute("name", "Елизавета Николаевна");
            XElement unic2 = new XElement("university", "БГУИР");
            XElement age2 = new XElement("age", "20");
            teacher2.Add(teacherName2);
            teacher2.Add(unic2);
            teacher2.Add(age2);
            XElement teachers = new XElement("teachers");
            teachers.Add(teacher1);
            teachers.Add(teacher2);
            xdoc.Add(teachers);
            xdoc.Save("teachers.xml");
            var tch = from xe in xdoc.Element("teachers").Elements("teacher")//1 запрос
                       where Convert.ToInt32(xe.Element("age").Value) <= 48
                       select xe;
            foreach (var s in tch)
                Console.WriteLine($"{s}");

            var items = from xs in xdoc.Element("teachers").Elements("teacher")//2 запрос
                        where xs.Attribute("name").Value == "Елизавета Николаевна"
                        select xs;
            foreach (var n in items)
                Console.WriteLine($"{n}");
        }
    }
}