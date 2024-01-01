using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OneSolution_API.Models.Utils
{
    public class DinhDang
    {
        public string Dinhdangkho(double number)
        {
            return string.Format("#,###,###.###", number);
        }

        public string DinhdangdongiaVND(double number)
        {
            return string.Format("#,###,###.###", number);
        }
        public double Dinhdangso(double number)
        {
            double kq = number;
            string heso = "1";

            for (int i = 1; i <= 4; i++)
            {
                heso += "0";
            }

            double _heso = double.Parse(heso);
            // Console.WriteLine("*** HE SO LA: " + _heso);
            kq = Math.Round(number * _heso) / _heso;
            return kq;
        }
        public double Dinhdangso(double number, int sothapphan)
        {
            double kq = number;
            string heso = "1";

            for (int i = 1; i <= sothapphan; i++)
            {
                heso += "0";
            }

            double _heso = double.Parse(heso);
            // Console.WriteLine("*** HE SO LA: " + _heso);
            kq = Math.Round(number * _heso) / _heso;
            return kq;
        }
        public double Dinhdangsoluong(double number)
        {
            double kq = number;
            int sothapphan = 3;
            string heso = "1";

            for (int i = 1; i <= sothapphan; i++)
            {
                heso += "0";
            }

            double _heso = double.Parse(heso);
            // Console.WriteLine("*** HE SO LA: " + _heso);
            kq = Math.Round(number * _heso) / _heso;
            return kq;
        }
        public double Dinhdang_thanhtien_nguyente(double number)
        {
            double kq = number;
            int sothapphan = 4;
            string heso = "1";

            for (int i = 1; i <= sothapphan; i++)
            {
                heso += "0";
            }

            double _heso = double.Parse(heso);
            // Console.WriteLine("*** HE SO LA: " + _heso);
            kq = Math.Round(number * _heso) / _heso;
            return kq;
        }
        public double Dinhdang_dongia_VND(double number)
        {
            double kq = number;
            int sothapphan = 3;
            string heso = "1";

            for (int i = 1; i <= sothapphan; i++)
            {
                heso += "0";
            }

            double _heso = double.Parse(heso);
            // Console.WriteLine("*** HE SO LA: " + _heso);
            kq = Math.Round(number * _heso) / _heso;
            return kq;
        }
        public string Dinhdanglamtron(double number)
        {
            double so = Dinhdangso(number, 2);
            long roundedNumber = (long)Math.Round(so);
            string formattedNumber = roundedNumber.ToString("#,###,###");
            return formattedNumber;
        }
        public double Round_Number(double number)
        {
            double so = Dinhdangso(number, 2);
            return Math.Round(so);
        }
        public double Dinhdangso_xuat(double number, int sothapphan)
        {
            double kq = number;

            string heso = "1";
            for (int i = 1; i <= sothapphan; i++)
                heso += "0";

            double _heso = double.Parse(heso);
            kq = Math.Round(number * _heso) / _heso;
            return kq;
        }
        public double Dinhdang_dongia_nguyente(double number)
        {
            double kq = number;
            int sothapphan = 8;
            string heso = "1";
            for (int i = 1; i <= sothapphan; i++)
            {
                heso += "0";
            }

            double _heso = double.Parse(heso);

            // Console.WriteLine("*** HE SO LA: " + _heso);
            kq = Math.Round(number * _heso) / _heso;
            return kq;
        }
    }
}