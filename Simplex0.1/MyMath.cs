using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Simplex0._1
{
    static class MyMath
    {
        static string M = Program.M;
        public static string PrintNumber(string Resm,string Resn)
        {
            if ((Resm == "1") || (Resm == "-1"))
                Resm = Resm.Replace("1", "");
            string Res;
            if ((Resm == "0") && (Resn == "0"))
                Res = "0";
            else if (Resm == "0")
                Res = Resn;
            else if (Resn == "0")
                Res = string.Format("{0}{1}", Resm, M);
            else
            {
                if (Resn[0] != '-')
                    Resn = "+" + Resn;
                Res = string.Format("{0}{1}{2}", Resm, M, Resn);
            }
            return Res;
        }
        public static void GetVarLike(string s, ref string m, ref string n)
        {
            if (!s.Contains(M))
            {
                m = "0";
                n = s;
            }
            else
            {
                // -xM - y
                string[] strlist = s.Split(M[0]);
                m = strlist[0];
                n = strlist[1];

                if (string.IsNullOrEmpty(m))
                    m = "1";
                else if (m == "-")
                    m = "-1";

                if (string.IsNullOrEmpty(n))
                    n = "0";
                else if (n[0] == '+')
                    n = n.Replace("+", "");
            }

            Debug.WriteLine(string.Format("GetVarLike2() Get: ({0}) Return: ({1}) and ({2})", s, m, n));
            
            return;
        }
        public static string Sub(string n1, string n2)
        {
            string Res = Add(n1, Mul("-1", n2));
            Debug.WriteLine(string.Format("Sub() Get: ({0}) and ({1}) Return : ({2})", n1, n2, Res));
            return Res;
        }
        public static string Add(string n1, string n2)
        {
            string Res;

            if ((n1.Contains(M)) || (n2.Contains(M)))
            {
                string n1m, n1n, n2m, n2n;
                n1m = n1n = n2m = n2n = "0";

                GetVarLike(n1, ref n1m, ref n1n);
                GetVarLike(n2, ref n2m, ref n2n);

                string Resm = Add(n1m, n2m);//===
                string Resn = Add(n1n, n2n);//===

                Res = PrintNumber(Resm, Resn);
            }

            else
            {
                double n11 = 1, n12 = 1, n21 = 1, n22 = 1;
                string[] strlist;
                if (n1.Contains("/"))
                {
                    strlist = n1.Split('/');
                    n11 = double.Parse(strlist[0]);
                    n12 = double.Parse(strlist[1]);
                }
                else
                {
                    n11 = double.Parse(n1);
                    n12 = 1;
                }

                if (n2.Contains("/"))
                {
                    strlist = n2.Split('/');
                    n21 = double.Parse(strlist[0]);
                    n22 = double.Parse(strlist[1]);
                }
                else
                {
                    n21 = double.Parse(n2);
                    n22 = 1;
                }
                Res = DivAndShrFun((n11 * n22) + (n12 * n21), (n12 * n22));
            }
            Debug.WriteLine(string.Format("Add2 Get: ({0}) and ({1}) Return: ({2})", n1, n2, Res));
            return Res;
        }
        public static string Min(string n1, string n2)
        {
            string Res = Sub(n1, n2);//n1-n2//===
            string m, n;
            m = n = "0";
            GetVarLike(Res,ref m,ref n);
            
            string tmpString = m;

            if (tmpString == "0")
                tmpString = n;

            double d1 = 1, d2 = 1;
            GetNumDenom(tmpString, ref d1, ref d2);
            decimal tmpDouble = (decimal)(d1 * d2);//double.Parse(Mul2(d1.ToString(), d2.ToString()));

            if (tmpDouble >= 0)
                Res = n2;
            else
                Res = n1;

            Debug.WriteLine(string.Format("Min2 Get: ({0}) and ({1}) Return: ({2})", n1, n2, Res));
            return Res;
        }
        public static string Max(string n1, string n2)
        {
            string Min = MyMath.Min(n1, n2);
            if (Min == n1)
                return n2;
            return n1;
        }
        public static string Mul(string n1, string n2)
        {
            string Res;
            //( xM + y )( z )
            if (n2.Contains(M))
            {
                string tmp = n1;
                n1 = n2;
                n2 = tmp;
            }

            //n1=( xM + y )  ,  n2=( z )
            if(n1.Contains(M))
            {
                string n1m, n1n;
                string Resm, Resn;
                n1m = n1n = "0";
                GetVarLike(n1, ref n1m, ref n1n);

                Resm = DivAndShrFun(Mul(n2, n1m));
                Resn = DivAndShrFun(Mul(n2, n1n));

                Res = PrintNumber(Resm, Resn);
            }
            else
            {
                // ratio * ratio
                double n11, n12, n21, n22;
                n11 = n12 = n21 = n22 = 1;
                GetNumDenom(n1,ref n11,ref n12);
                GetNumDenom(n2, ref n21, ref n22);

                n11 = n11 * n21;
                n21 = n12 * n22;

                Res = DivAndShrFun(n11, n21);
            }
            Debug.WriteLine(string.Format("Mul2 Get: ({0}) and ({1}) Return: ({2})", n1, n2, Res));
            return Res;
        }
        public static string Div(string n1,string n2)
        {
            double n21, n22;
            n21 = n22 = 1;
            GetNumDenom(n2, ref n21, ref n22);
            string Res = Mul(n1, string.Format("{0}/{1}", n22, n21));
            Debug.WriteLine(string.Format("Div2() Get: ({0}) and ({1}) Return : ({2})", n1, n2, Res));
            return Res;
        }
        public static void GetNumDenom(string s, ref double Num, ref double Denom)
        {
            
            if (!s.Contains("/"))
            {
                Num = double.Parse(s);
                Denom = 1;
            }
            else
            {
                string n1, n2;
                n1 = n2 = "1";

                string[] strlist = s.Split('/');
                n1 = strlist[0];
                n2 = strlist[1];

                if (string.IsNullOrEmpty(n1))
                    n1 = "1";
                if (string.IsNullOrEmpty(n2))
                    n2 = "1";

                Num = double.Parse(n1);
                Denom = double.Parse(n2);
            }
        }
        public static string DivAndShrFun(string s)
        {
            if (!s.Contains("/"))
                return s;

            double n1, n2;
            n1 = n2 = 1;
            GetNumDenom(s, ref n1, ref n2);

            return DivAndShrFun(n1, n2);
        }
        public static string DivAndShrFun(double n1, double n2)
        {
            Debug.WriteLine("DivAndShrFun(" + n1 + "," + n2 + ")");
            if (n2 == 0)
            {
                Debug.WriteLine("DivAndShrFun return (" + int.MaxValue.ToString() + ")");
                return int.MaxValue.ToString();
            }
            if (n1 == 0)
            {
                Debug.WriteLine("DivAndShrFun return (0)");
                return "0";
            }

            bool n1negat = false;
            bool n2negat = false;

            if (n1 < 0)
            {
                n1 *= (-1);
                n1negat = true;
            }
            if (n2 < 0)
            {
                n2 *= (-1);
                n2negat = true;
            }

            double GCD = GCDRecursive(n1, n2);
            n1 /= GCD;
            n2 /= GCD;

            if (n1negat)
                n1 *= (-1);
            if (n2negat)
                n1 *= (-1);
            string Res;
            if (n2 == 1)
            {
                Res = n1.ToString();
                Debug.WriteLine("DivAndShrFun return (" + Res + ")");
                return Res;
            }
            else
            {
                Res = string.Format("{0}/{1}", n1, n2);
                Debug.WriteLine("DivAndShrFun return (" + Res + ")");
                return Res;
            }
        }
        public static double GCDRecursive(double n1, double n2)
        {
            Debug.WriteLine("GCDRecursive(" + n1 + "," + n2 + ")");
            if (n1 == 0)
                return n2;
            if (n2 == 0)
                return n1;
            double Res;
            if (n1 > n2)
            {
                Res = GCDRecursive(n1 % n2, n2);
                Debug.WriteLine("GCDRecursive return (" + Res + ")");
                return Res;
            }
            else
            {
                Res = GCDRecursive(n1, n2 % n1);
                Debug.WriteLine("GCDRecursive return (" + Res + ")");
                return Res;
            }
        }
    }
}
