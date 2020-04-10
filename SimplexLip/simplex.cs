using System;
using System.Collections.Generic;
using System.Text;

namespace SimplexLib
{
    public class Simplex
    {
        public bool max;

        public string fun;
        public List<string> s_t;

        public string fun2;
        public List<string> s_t_2;

        public Simplex(bool max, string fun, List<string> s_t)
        {
            this.max = max;
            this.fun = fun;
            this.s_t = s_t;
        }

        public void Solve()
        {
            fun2 = Step1(fun);
            /*foreach (string s in s_t)
            {
                form2.Add(step1(s));
            }*/
        }

        string Step1(string s)
        {
        
            return null;
        }

        //"2x+4y" => x > 2 , y > 4

    }

    public class syntax
    {
        public string syn;//"2x+4y=3"
        public string symbol;//"<="
        public Dictionary<string, string> elements;//"x""2"

        private List<char> AsideSymb = new List<char>() { '<', '>', '=' };//"<"
        public static string Rev(string s)
        {
            if (s[0] == '-')
                s = s.Remove(0, 1);
            else
                s = "-" + s;
            return s;
        }
        public void GetElem()
        {
            if (string.IsNullOrEmpty(syn))
                throw new Exception("no syntas");

            List<string> tmp_elem = new List<string>();

            int i = 0;
            string t = "";
            bool AthSide = false;
            while(i<syn.Length)
            {
                if (syn[i] == '+')
                {
                    t = RealVal(t,AthSide);
                    tmp_elem.Add(t);
                    t = "";
                    i++;
                    continue;
                }
                else if (syn[i] == '-')
                {
                    t = RealVal(t, AthSide);
                    tmp_elem.Add(t);
                    t = "-";
                    i++;
                    continue;
                }
                else if (AsideSymb.Contains(syn[i]))
                {
                    AthSide = true;
                    if (!string.IsNullOrEmpty(t))
                    {
                        t = RealVal(t, AthSide);
                        tmp_elem.Add(t);
                    }
                    t = "";
                    i++;
                    continue;
                }
                else //number or var
                {
                    t += syn[i];
                    i++;
                    continue;
                }
            }

            t = RealVal(t, AthSide);
            tmp_elem.Add(t);

            foreach (string s in tmp_elem)
            {
                int ii = 0;
                string tmpN = "";
                string tmpV = "";
                while (ii<s.Length)
                {
                    if(((s[ii]>='a')&& (s[ii] <= 'z'))|| ((s[ii] >= 'A') && (s[ii] <= 'Z')))
                    {
                        //var
                        //if(!string.IsNullOrEmpty(tmpN))
                        tmpN += s[ii];
                    }
                    else
                    {
                        //non var
                        tmpV += s[ii];
                    }
                }
                if (tmpN.Length == 0)
                    tmpN = "1";
                if (tmpV.Length == 0)
                    tmpV = "1";
                elements.Add(tmpN, tmpV);
                tmpN = tmpV = "";
            }
            
        }
        public bool ChkuniSignisPos()
        {
            if (elements.ContainsKey("1"))
                if (elements["1"][0] != '-')
                {
                    return false;
                }
            return true;
        }
        public string RealVal(string s,bool af)
        {
            if(af)
                s = Rev(s);
            return s;
        }
    }
}
