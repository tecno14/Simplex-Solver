using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Simplex0._1
{
    class Program
    {
        //
        public const string M = "M";
        public const string Space = " ";
        public const string Basis = "Basis";
        public const string Cb = "Cb";
        public const string RHS = "RHS";
        public const string Zj = "Zj";
        public const string Cj = "Cj";
        public const string Cj_Zj = Cj + "-" + Zj + "";
        public const string vbar = "|";
        public const string hbar = "=";//========//_//― 
        public const string CalculateNewTable = "Calculate New Table";
        public const string RepetedTable = "Repeted Table:";
        public const bool PrintRepetedTable = true;
        public const string unBounded = "unBounded";
        public const string Infesable = "Infesable";
        public const string Solution = "Solution";
        public const string EndOfSolve = "End Of Solve";
        public const string EndOfMain = "End Of Main";
        public const string Results = "Results";
        public const string plse_enter_valid = "please enter a valid value";
        public const int MinSpace = 5;
        public const string Maximize = "Maximize";
        public const string Minimize = "Minimize";

        //
        public const ConsoleColor BG = ConsoleColor.Black;

        public const int ResultMsg = 1;
        public const ConsoleColor ResultBC = ConsoleColor.DarkBlue;
        public const ConsoleColor ResultFC = ConsoleColor.White;

        public const int StepMsg = 2;
        public const ConsoleColor StepBC = ConsoleColor.DarkYellow;
        public const ConsoleColor StepFC = ConsoleColor.Black;

        public const int ErrorMsg = 3;
        public const ConsoleColor ErrorBC = ConsoleColor.DarkRed;
        public const ConsoleColor ErrorFC = ConsoleColor.White;

        public const int Mode4Msg = 4;
        public const ConsoleColor Mode4BC = ConsoleColor.DarkGreen;
        public const ConsoleColor Mode4FC = ConsoleColor.White;

        //
        public static List<List<string>> PreProblemEqs = PreProblemEqsGenerator();
        public static bool GoalMax = true;
        public static List<string> CurrentProblemTable = null;
        public static List<List<string>> eq = null;
        public static List<string> BasisList = null;
        public static List<string> CbList = null;
        public static List<List<List<string>>> Alltables = new List<List<List<string>>>();
        public static List<List<string>> table = null;
        public static List<string> AllResults = new List<string>();
        public static List<string> SavedVar = new List<string>() { M, "S", "A" };
        //
        static void Main(string[] args)
        {
            //cmd  ui
            Console.BackgroundColor = Program.BG;
            Console.Clear();
            Console.Title = "Simplex0.1";

            Reset();
            HomeLoop();
        }
        public static void Header()
        {
            Print("----------------------------------------------------", ResultMsg);
            Print(" Welcome to Simplex Solver 0.1", ResultMsg);
            Print(" -Wael Al-Haddad (wael.had.sy@gmail.com) 2020-", ResultMsg);
            Print(" enter your chose nunmber and hit 'Enter'", ResultMsg);
            Print();
        }
        public static void HomeLoop()
        {
            string Enter;
            Console.Clear();
            while (true)
            {
                PrintProblem();
                //Main Menu
                Header();
                Print("Main Menu :", ResultMsg);
                Print("  (1) - Load a pre-installed Example", ResultMsg);
                Print("  (2) - Enter / Modify the Problem", ResultMsg);
                Print("  (3) - Try Solve to the Problem", ResultMsg);
                Print();
                Print("  (4) - Reset", ResultMsg);
                Print("  (9) - Exit", ResultMsg);

                Enter = Console.ReadLine();

                Console.Clear();

                if (Enter == "1")
                    Load_Example_Menu();
                else if (Enter == "2")
                    Enter_Modify_Pr_Menu();
                else if (Enter == "3")//solve
                    TrySolve();
                else if (Enter == "4")
                    Reset();
                else if (Enter == "9")
                    return;
                else
                    Print("Error : " + plse_enter_valid + "(" + 6 + " Main Menu )", ErrorMsg);
                continue;
            }
        }
        public static void TrySolve()
        {
            try
            {
                PrintProblem();

                if (CurrentProblemTable == null)
                    throw new Exception("Please add problem to solve first");

                if (CurrentProblemTable.Count < 2)
                    throw new Exception("please add problem first");

                //covert current to eq
                Read_O_F_(CurrentProblemTable[0], false);

                for (int i = 1; i < CurrentProblemTable.Count; i++)
                    Read_S_T_(CurrentProblemTable[i], i, true);

                SelectBasisVar();

                BuildTable();

                if (!GoalMax)
                {
                    Print("We will solve the problem as (Maximize) and then Reverse the answer", StepMsg);//123
                    Print();
                    /*for (int i = 2; i < table[0].Count; i++)
                    {
                        //if (table[1][i].Contains(M)) continue;//123
                        table[1][i] = MyMath.Mul(table[1][i], "-1");
                    }*/
                }

                Solve(ref table, -10, true);

                Print("-------------------------------------");

                Print(Results, ResultMsg);

                for (int i = 0; i < AllResults.Count; i++)
                    Print(AllResults[i], ResultMsg);

                if (GoalMax)
                    Print("( Z = " + CalcZ(ref table) + " )", ResultMsg);
                else
                {
                    string Res = CalcZ(ref table);
                    Print("( -Z = " + Res + " => Z = " + MyMath.Mul("-1", Res) + " )", ResultMsg);
                }

                //Console.Beep();//123

                Print();

                Print("press any key to continue", StepMsg);
                Console.ReadKey();
                Print();
            }
            catch (Exception ex)
            {
                Print("Error :" + ex.Message, ErrorMsg);
            }
        }
        public static void Reset()
        {
            GoalMax = true;
            CurrentProblemTable = null;
            eq = null;
            BasisList = null;
            CbList = null;
            Alltables = new List<List<List<string>>>();
            table = null;
            AllResults = new List<string>();
        }
        public static void Enter_Modify_Pr_Menu()
        {
            string Enter;
            Console.Clear();
            while (true)
            {
                PrintProblem();
                //Enter_Edit Menu
                Header();
                Print("Enter / Modify Menu :", ResultMsg);
                Print(" (1) - Minimize / Maximize", ResultMsg);
                Print(" (2) - Set Objective-Function", ResultMsg);
                Print(" (3) - Add / Modify Subject-to Functions", ResultMsg);
                Print();
                Print(" (4) - Reset", ResultMsg);
                Print(" (9) - Save and go back Main Menu", ResultMsg);

                Enter = Console.ReadLine();

                Console.Clear();

                if (Enter == "1")//Max-Min
                    SetMaxMin();
                else if (Enter == "2")//o.f.
                    Set_O_F_();
                else if (Enter == "3")
                    Set_S_T_();
                else if (Enter == "4")
                    Reset();
                else if (Enter == "9")
                    return;
                else
                    Print("Error : " + plse_enter_valid + "(" + 5 + " \r\n Enter / Modify Menu)", ErrorMsg);
                continue;
            }
        }
        public static void Set_S_T_()//s.t.
        {
            bool Exit = false;
            try
            {
                if ((CurrentProblemTable == null) || (CurrentProblemTable.Count < 1))
                {
                    Exit = true;
                    throw new Exception("Enter Objective-Function first");
                }

                for (int i = 1; i < CurrentProblemTable.Count; i++)
                    Print(" (" + i + ") " + CurrentProblemTable[i], StepMsg);

                Print(" Add / Modify Subject-to Functions\r\n enter (0) to add New Or function number to Modyfy it \r\n or (-1) to go back", ResultMsg);

                string Enter = Console.ReadLine().Replace(" ", "").Replace("\t", "");
                Console.Clear();

                if (!int.TryParse(Enter, out int E))
                    throw new Exception(plse_enter_valid + "( 3 Add / Modify S.T.)");

                if (E == -1)
                    return;

                if ((E < 0) || (E >= CurrentProblemTable.Count))//error out
                    throw new Exception(plse_enter_valid + "(" + 4 + ")");

                Print("Enter Function or (-1) to go back", ResultMsg);
                if (E != 0)
                    Print(" or (-2) to delete this S.T.", ResultMsg);

                //0 new or 
                if (E==0)
                {
                    E = CurrentProblemTable.Count;
                    CurrentProblemTable.Add("");
                }
                
                Enter = Console.ReadLine();

                if (Enter == "-1")
                    return;
                if (Enter == "-2")
                {
                    CurrentProblemTable.RemoveAt(E);
                    Print("Deleted \r\n ", StepMsg);
                    return;
                }

                if (!ChkSyntax(Enter, true))
                    throw new Exception(plse_enter_valid + " (syntax not valid)");
                
                //if (!Read_S_T_(Enter, E)) 
                //   throw new Exception("");

                CurrentProblemTable[E] = Enter;
                
                Console.Clear();
                Print("Done", StepMsg);
                Print();
            }
            catch (Exception ex)
            {
                Print("Error : " + ex.Message, ErrorMsg);
                if (!Exit)
                    Set_S_T_();
            }
        }
        public static void Set_O_F_()//o.f.
        {
            string Enter;

            Print("Enter Objective-Function \r\n Or (9) to go back", ResultMsg);

            Enter = Console.ReadLine();
            Console.Clear();
            try
            {
                if (Enter.Contains(" "))
                    Enter = Enter.Replace(" ", "");
                if (Enter.Contains("\t"))
                    Enter = Enter.Replace("\t", "");

                if (Enter == "9")
                    return;
                else if (ChkSyntax(Enter))
                {
                    if (CurrentProblemTable == null)
                        CurrentProblemTable = new List<string>() { Enter };
                    else if (CurrentProblemTable.Count == 0)
                        CurrentProblemTable.Add(Enter);
                    else
                        CurrentProblemTable[0] = Enter;

                    Print("Done", StepMsg);
                    Print();
                }
                else
                    throw new Exception();
            }
            catch(Exception ex)
            {
                Print("Error : " + plse_enter_valid + "(2  O.F.) " + ex.Message, ErrorMsg);
                Set_O_F_();
            }
        }
        public static bool ChkSyntax(string str, bool S_T = false)
        {
            if (string.IsNullOrEmpty(str))
            {
                Print("Error : empty input !!!", ErrorMsg);
                return false;
            }
            
            for (int i = 0; i < SavedVar.Count; i++)
                if (str.Contains(SavedVar[i]))
                    throw new Exception("you cant use (" + SavedVar[i] + ")");

            if (str.Contains(" "))
                str = str.Replace(" ", "");
            if (str.Contains("\t"))
                str = str.Replace("\t", "");

            if (S_T)
            {
                //chk =
                if (!str.Contains("="))
                {
                    Print("Error : (=) is required", ErrorMsg);
                    return false;
                }
                /*
                string tmpvalue = S_T.Substring(i);
                if (!double.TryParse(tmpvalue, out double RHSValue))
                    throw new Exception("Error RHS of function");
                */
                if (!double.TryParse(str.Substring(str.IndexOf('=') + 1), out double t))
                {
                    Print("Error : RHS must be valid number (" + str + ")", ErrorMsg);
                    return false;
                }

                int f = str.Length;

                str = str.Replace("=", "");

                if (f != (str.Length + 1))
                {
                    Print("Error : only one (=) is allowed", ErrorMsg);
                    return false;
                }
                f = str.Length;

                //chk <>
                if (str.Contains(">"))
                    str = str.Replace(">", "");
                if (str.Contains("<"))
                    str = str.Replace("<", "");

                if (f > (str.Length + 1))
                {
                    Print("Error : one of (<) or (>) are allowed", ErrorMsg);
                    return false;
                }
            }
            //chk chars
            for (int i = 0; i < str.Length; i++)
            {
                if (
                     (str[i] == char.Parse(M))
                    ||
                     (str[i] == '/')
                    ||
                     (str[i] == '.')
                    ||
                     (str[i] == '-')
                    ||
                     (str[i] == '+')
                    ||
                     (
                        (str[i] >= '0') && (str[i] <= '9')
                     )
                    ||
                     (
                        (str[i] >= 'a') && (str[i] <= 'z')
                     )
                    ||
                     (
                        (str[i] >= 'A') && (str[i] <= 'Z')
                     )
                    )
                    continue;
                else
                {
                    Print(string.Format("Error at Reading ({0}) (char number {1} ) not valid", str[i], (i + 1)), ErrorMsg);//, if you think that is bug please send it to developer
                    return false;
                }
            }
            return true;

        }
        public static void SetMaxMin()
        {
            string Enter;

            Print("Enter Ma(x)imize Or Mi(n)imize \r\n Or (9) to go back", ResultMsg);

            Enter = Console.ReadLine();
            Console.Clear();
            Enter = Enter.ToLower();

            if (Enter == "n")
                GoalMax = false;
            else if ((Enter == "x"))// || (string.IsNullOrEmpty(Enter)))
                GoalMax = true;
            else if (Enter != "9")
            {
                Print("Error :" + plse_enter_valid + "\r\n MaxMin Set", ErrorMsg);
                SetMaxMin();
            }
        }
        public static void Load_Example_Menu()
        {
            try
            {
                if (PreProblemEqs.Count > 0)
                    Print(string.Format("Enter any number from (1) to ({0}) OR enter (-1) to exit", PreProblemEqs.Count), ResultMsg);
                else
                {
                    Print("there isn't any example ... ", ErrorMsg);
                    return;
                }

                string Enter = Console.ReadLine();
                Console.Clear();

                int SelectedEq = -1;

                if (!int.TryParse(Enter, out SelectedEq))
                    throw new Exception(plse_enter_valid);

                if (SelectedEq == -1)
                    return;

                else if ((SelectedEq > PreProblemEqs.Count) || (SelectedEq < 1))
                    throw new Exception(plse_enter_valid);

                CurrentProblemTable = PreProblemEqs[SelectedEq - 1];

                Print("Load example success", StepMsg);
            }
            catch (Exception ex)
            {
                Print("Error : " + ex.Message, ErrorMsg);
                Load_Example_Menu();
            }
        }   
        public static bool Read_O_F_(string O_F, bool PrintString = false)
        {
            if (string.IsNullOrEmpty(O_F))
                return false;

            for (int i = 0; i < SavedVar.Count; i++)
                if (O_F.Contains(SavedVar[i]))
                    throw new Exception("you cant use (" + SavedVar[i] + ")");

            if (O_F.Contains(" "))
                O_F = O_F.Replace(" ", "");
            if (O_F.Contains("\t"))
                O_F = O_F.Replace("\t", "");
            eq = null;
            List<List<string>> tmpEq = new List<List<string>> { new List<string>(), new List<string>() };
            /*
            if ((eq == null) || (eq.Count < 2))
                eq = new List<List<string>> { new List<string>(), new List<string>() };
            else
            {
                eq[0] = new List<string>();
                eq[1] = new List<string>();
            }
            */
            string number = "", var = "";

            //read goal fun
            for (int i = 0; i < O_F.Length; i++)
            {
                if (O_F[i] == '-')
                    number += '-';
                else if (O_F[i] == '+')
                    continue;
                else if (((O_F[i] >= '0') && (O_F[i] <= '9')) || (O_F[i] == '.') || (O_F[i] == '/') || (O_F[i] == char.Parse(M)))
                    number += O_F[i];
                else
                {
                    if (number == "-")
                        number = "-1";
                    else if (number == "")
                        number = "1";
                    tmpEq[1].Add(number);
                    number = "";
                    while (i < O_F.Length)
                    {
                        var += O_F[i];
                        if ((i + 1 == O_F.Length) || (O_F[i + 1] == '+') || (O_F[i + 1] == '-'))
                        {
                            tmpEq[0].Add(var);
                            var = "";
                            break;
                        }
                        else
                            i++;
                    }
                }
            }
            /*
            for (int i = 2; i < eq.Count; i++)
                while (eq[i].Count <= eq[0].Count)
                    eq[i].Add("0");
            */

            if (tmpEq[1].Count != tmpEq[0].Count)
                throw new Exception("error at read goal function");
            
            if(!GoalMax)
                for (int i = 0; i < tmpEq[1].Count; i++)
                {
                    //if (table[1][i].Contains(M)) continue;//123
                    tmpEq[1][i] = MyMath.Mul(tmpEq[1][i], "-1");
                }

            if (PrintString)
                PrintEqs(1, 2);

            eq = tmpEq;
            return true;
        }
        public static bool Read_S_T_(string S_T,int StId,bool PrintString = false)
        {
            if (eq == null)// || (eq[0] == null) || (eq[0].Count < 1))
                throw new Exception(" you need to add Goal Function first");

            for (int i = 0; i < SavedVar.Count; i++)
                if (S_T.Contains(SavedVar[i]))
                    throw new Exception("you cant use (" + SavedVar[i] + ")");

            List<List<string>> eqtmp = CopyTable(eq);

            if (S_T.Contains(" "))
                S_T = S_T.Replace(" ", "");
            if (S_T.Contains("\t"))
                S_T = S_T.Replace("\t", "");
            
            List<string> CurrentEq = new List<string>();

            StId++;

            if ((StId < 2) || (StId > eqtmp.Count))
                throw new Exception("For Dev. RunTimeError : (StId) invalid value (StId+1=" + StId + "+1 > eq.count=" + eqtmp.Count + ")");
            
            if (StId < eqtmp.Count)
            {
                eqtmp[StId] = CurrentEq;
            }
            else
            {
                eqtmp.Add(CurrentEq);
                StId = eqtmp.IndexOf(CurrentEq);
            }

            while (eqtmp[StId].Count<= eqtmp[0].Count)
                CurrentEq.Add("0");

            string number = "", var = "";

            List<char> tmpValid = new List<char>() { '<', '>', '=', '+', '-' };//'+', '-', '=' };// { '+', '-', '<', '>', '=' };
            
            //read goal fun
            for (int i = 0; i < S_T.Length; i++)
            {
                if (S_T[i] == '-')
                    number += '-';
                else if (S_T[i] == '+')
                    continue;
                else if (((S_T[i] >= '0') && (S_T[i] <= '9')) || (S_T[i] == '.') || (S_T[i] == '/') || (S_T[i] == char.Parse(M)))
                    number += S_T[i];
                else if (tmpValid.Contains(S_T[i]))
                {
                    //read RHS
                    string Svalue = "1";
                    if ((S_T[i] == '<') && (S_T[i + 1] == '='))
                    {
                        i++;
                    }
                    else
                    if ((S_T[i] == '>') && (S_T[i + 1] == '='))
                    {
                        i++;
                        Svalue = "-1";
                        int A = 0;
                        while (true)
                            if (!eqtmp[0].Contains("A" + ++A))
                                break;
                        int index = GetVarIndex("A" + A, eqtmp);
                        eqtmp[1][index] = "-" + M;
                        eqtmp[StId][index] = "1";
                    }
                    //else//if (S_T[i] == '=')
                    i++;
                    
                    //if (Svalue == "1")
                    {
                        int S = 0;
                        while (true)
                            if (!eqtmp[0].Contains("S" + ++S))
                                break;
                        int index = GetVarIndex("S" + S, eqtmp);
                        eqtmp[StId][index] = Svalue;
                    }

                    double RHSValue = double.Parse(S_T.Substring(i));

                    eqtmp[StId][eqtmp[0].Count] = RHSValue.ToString();
                    
                    if (RHSValue < 0)
                        for (int m = 0; m <= eqtmp[0].Count; m++)
                            eqtmp[StId][m] = MyMath.Mul("-1", eqtmp[StId][m]);
                    
                    if (PrintString)
                        PrintEqs(StId, StId + 1);

                    //sucess
                    break;
                    //return true;
                }
                else//var name
                {

                    if (number == "-")
                        number = "-1";
                    else if (number == "")
                        number = "1";

                    while (i < S_T.Length)
                    {
                        var += S_T[i];

                        if ((i + 1 == S_T.Length) || (tmpValid.Contains(S_T[i + 1])))
                        {
                            eqtmp[StId][GetVarIndex(var, eqtmp)] = number;

                            number = "";
                            var = "";
                            break;
                        }
                        else
                            i++;
                    }
                }
            }

            if ((eqtmp[1].Count != eqtmp[0].Count) || (CurrentEq.Count != eqtmp[0].Count + 1))
                throw new Exception("error at read S.T function");
            
            eq = eqtmp;
            return true;
        }
        public static int GetVarIndex(string var,List<List<string>> eqq)
        {
            //if variable exist ... set var 
            //else ... add var and set var

            int index = eqq[0].IndexOf(var);
            
            if (index == -1)
            {
                index = eqq[0].Count;
                eqq[0].Add(var);
                eqq[1].Add("0");
                string tmp;
                for (int i = 2; i < eqq.Count; i++)
                {
                    tmp = eqq[i][index];
                    eqq[i][index] = "0";
                    eqq[i].Add(tmp);
                }

                /*
                int size = eq[0].Count - 1;
                for (int p = 0; p < eq.Count; p++)
                {
                    string tmp = eq[0][size];
                    eq[p][size] = "0";
                }
                eq[0][size] = var;
                eq[StId][size] = number;
                */
            }
            return index;
        }
        public static void SelectBasisVar()
        {
            if(eq ==null)
                throw new Exception("Error : no st add");

            BasisList = new List<string>();
            CbList = new List<string>();

            bool flag = true;
            for (int i = eq.Count-1; i > 1; i--)//all eq from down
            {
                for (int j = eq[0].Count-1; j >= 0; j--)//all val from right//j=0;j<eq[0].count
                {
                    if (eq[i][j] == "1")
                    {
                        if ((eq[1][j] != "0") && (!eq[1][j].Contains(M)))
                            continue;

                        flag = true;
                        for (int k = 2; k < eq.Count; k++)//all eq chk that no one have that element
                        {
                            if (k == i)
                                continue;
                            if (eq[k][j] != "0")
                            {
                                flag = false;
                                break;
                            }
                        }
                        if (flag)
                        {
                            BasisList.Insert(0, eq[0][j]);
                            CbList.Insert(0, eq[1][j]);
                            break;
                        }
                        else throw new Exception("error at st function  " + (i + 1) + " : no basis var found");
                    }
                    else
                        continue;
                }
            }
            if (BasisList.Count != (eq.Count - 2))
                throw new Exception("Error: can't find  basis var equal to number of st functions");
        }
        public static string CalcZ(ref List<List<string>> table)
        {
            string Res = "0";

            for (int i = 2; i < table.Count - 2; i++)//row
                Res = MyMath.Add(Res, MyMath.Mul(table[i][table[0].Count], table[1][table[0].IndexOf(table[i][0])]));

            Debug.WriteLine("CalcZ Return: " + Res);
            return Res;
        }
        public static bool GetPlivotRow(ref List<List<string>> table, int Pc, ref int Pr)
        {
            string CurrentnResultt;
            string CurrentnPcValuee;
            string MinValuee = (int.MaxValue).ToString();
            for (int i = 2; i < table.Count - 2; i++)
            {
                CurrentnPcValuee = table[i][Pc];
             
                if (MyMath.Max(CurrentnPcValuee, "0") == "0")
                    continue;

                CurrentnResultt = MyMath.Div(table[i][table[2].Count - 1], table[i][Pc]);

                if (MyMath.Min(CurrentnResultt, MinValuee) != MinValuee)
                {
                    MinValuee = CurrentnResultt;
                    Pr = i;
                }
            }
            return MinValuee != (int.MaxValue).ToString();
        }
        public static bool IstherePlivot(ref List<List<string>> table, ref int Pc)
        {
            bool Isthere = false;
            string MaxValue = "0";
            string CurrentnValue;
            for (int i = 2; i < table[0].Count; i++)
            {
                CurrentnValue = table[table.Count - 1][i];

                if (MyMath.Max(CurrentnValue, MaxValue) != MaxValue)
                {
                    Isthere = true;
                    MaxValue = CurrentnValue;
                    Pc = i;
                }
            }
            return Isthere;
        }
        public static List<List<string>> NextTable(ref List<List<string>> OldTable, int Pc,int Pr)
        {
            //delet additional var
            List<List<string>> NewTable = CopyTable( OldTable);
            int Hidec = NewTable[0].IndexOf(NewTable[Pr][0]);
            if (!NewTable[1][Hidec].Contains(M))
                Hidec = -10;

            //get P 
            string PElementt = OldTable[Pr][Pc];
            
            NewTable[Pr][0] = OldTable[0][Pc];
            NewTable[Pr][1] = OldTable[1][Pc];
            Debug.WriteLine(string.Format("--- Update Body : Replase( {0} , {1} )", OldTable[0][Pc], OldTable[Pr][0]));
            
            //update body
            for (int i = 2; i < OldTable[0].Count + 1; i++)//col
            {
                if ((i == Pc)||(i == Hidec))
                    continue;
                for (int j = 2; j < OldTable.Count - 2; j++)//row
                {
                    if (j == Pr)
                        continue;
                    string m1 = OldTable[j][Pc];
                    string m2 = OldTable[Pr][i];
                    string tmp = OldTable[j][i];
                    Debug.Write(string.Format("--- Update Body  Get: ( {0} - {1} * {2} / {3} ) ", tmp, m1, m2, PElementt));
                    tmp = MyMath.Sub(tmp, (MyMath.Div(MyMath.Mul(m1, m2), PElementt)));
                    Debug.WriteLine("and Return: " + tmp);
                    NewTable[j][i] = tmp;
                }
            }

            //update col
            for (int i = 2; i < OldTable.Count - 2; i++)//row
            {
                NewTable[i][Pc] = "0";
            }
            //update row
            for (int i = 2; i < OldTable[0].Count + 1; i++)//col
            {
                if ((i == Pc) || (i == Hidec))
                    continue;
                NewTable[Pr][i] = MyMath.Div(OldTable[Pr][i], PElementt);//***
            }
            //update elem
            NewTable[Pr][Pc] = "1";

            RemoveCol(ref NewTable, Hidec);

            //update tail
            CalTableTail(ref NewTable, true);

            return NewTable;
        }
        public static void RemoveCol(ref List<List<string>> ctable, int Hidec)
        {
            if ((ctable[0].Count <= Hidec) || (0 > Hidec))
                return;
            for (int i = 0; i < ctable.Count; i++)//row
                ctable[i].RemoveAt(Hidec);
        }
        public static bool IsRepeatTable( List<List<string>> table,int table_Comp)
        {
            if (table_Comp == Alltables.Count)
                return false;
            if (Alltables[table_Comp].Count != table.Count)
            {
                table_Comp++;
                return IsRepeatTable( table, table_Comp);
            }

            for (int i = 0; i < table.Count; i++)
            {
                if (Alltables[table_Comp][i].Count != table[i].Count)
                {
                    table_Comp++;
                    return IsRepeatTable( table, table_Comp);
                }
                for (int j = 0; j < table[i].Count; j++)
                {
                    if (Alltables[table_Comp][i][j] != table[i][j])
                    {
                        table_Comp++;
                        return IsRepeatTable( table, table_Comp);
                    }
                }
            }
            return true;
        }
        public static List<List<string>> CopyTable( List<List<string>> Oldtable)
        {
            List<List<string>> NewTable = new List<List<string>>();
            foreach (List<string> OldRow in Oldtable)
            {
                List<string> NewRow = new List<string>();
                foreach (string s in OldRow)
                {
                    NewRow.Add(s);
                }
                NewTable.Add(NewRow);
            }
            return NewTable;
        }
        public static void Solve(ref List<List<string>> ctable,int Pc=-10,bool Format=false)
        {
            if(Format)
            {
                //table = null;
                AllResults = new List<string>();
                Alltables = new List<List<List<string>>>();
            }

            int Pr = -10;
            bool bool_IstherePlivot, bool_GetPlivotRow = false;
            if (Pc!=-10)
            {
                bool_IstherePlivot = true;
            }
            else
            {
                bool_IstherePlivot = IstherePlivot(ref ctable, ref Pc);
            }
            
            if (bool_IstherePlivot)
            {
                bool_GetPlivotRow = GetPlivotRow(ref ctable, Pc, ref Pr);//is there row
            }

            PrintTable(ref ctable, Pc, Pr);

            if (bool_IstherePlivot)//is there col
            {
                if (bool_GetPlivotRow)//is there row
                {
                    Print(CalculateNewTable, StepMsg);

                    ctable = NextTable(ref ctable, Pc, Pr);//ther is anouther table
                    table = ctable;
                    if (IsRepeatTable(ctable, 0))//table old)
                    {
                        Print(RepetedTable, ErrorMsg);
                        if (PrintRepetedTable)
                            PrintTable(ref ctable, -10, -10);
                        return;
                    }
                    Alltables.Add(ctable);

                    Solve(ref ctable);
                }
                else
                {
                    Print(unBounded, ResultMsg);
                    AllResults.Add(unBounded);
                    return;
                }
            }
            else
            {
                if(ctable[ctable.Count-2][ctable[0].Count].Contains(M))
                {
                    //unfesable
                    Print(Infesable, ResultMsg);
                    AllResults.Add(Infesable);
                }
                else
                {
                    Print(Solution, ResultMsg);
                    string tmp = PrintSolution(ref ctable);//, CalcZ(ref ctable));
                    Print(tmp, ResultMsg);
                    AllResults.Add(tmp);
                    //chk for more solutions
                    //get Basis Variables;
                    List<string> tBasis = new List<string>();
                    for (int i = 2; i < ctable.Count-2; i++)//rows
                        tBasis.Add(ctable[i][0]);

                    for (int i = 2; i < ctable[0].Count; i++)//cols
                    {
                        if(ctable[ctable.Count-1][i]=="0")
                        {
                            if(!tBasis.Contains(ctable[0][i]))
                            {
                                Solve(ref ctable, i);
                            }
                        }
                    }                   
                }

                //Console.WriteLine("Solved !!!");
                //Print(EndOfSolve, ResultMsg);
                return;
            }
        }
        public static void CalTableTail(ref List<List<string>> ctable, bool DeletLast2Line = false)
        {
            if (DeletLast2Line)
            {
                ctable.RemoveAt(ctable.Count - 1);
                ctable.RemoveAt(ctable.Count - 1);
            }
            
            //row before last
            ctable.Add(new List<string>() { Zj, Space });
            
            //row last
            ctable.Add(new List<string>() { Cj_Zj, Space });

            for (int k = 2; k < ctable[0].Count; k++)//col
            {
                string RR = "0";
                for (int m = 2; m < ctable.Count - 2; m++)//row
                {
                    RR = MyMath.Add(RR, MyMath.Mul(ctable[m][k], ctable[m][1])); // CbList[m - 2]));
                }
                ctable[ctable.Count - 2].Add(RR);
                ctable[ctable.Count - 1].Add(MyMath.Sub(ctable[1][k], RR));
            }
            ctable[ctable.Count - 2].Add(CalcZ(ref ctable));
        }
        public static void BuildTable()
        {
            table = new List<List<string>>
            {
                //row1
                new List<string>() { Space, Space },

                //row2:
                new List<string>() { Basis, Cb }
            };
            for (int i = 0; i < eq[0].Count; i++)//var count
            {
                table[0].Add(eq[0][i]);
                table[1].Add(eq[1][i]);
            }
            table[1].Add(RHS);
            
            //body of table
            for (int j = 2; j < eq.Count; j++)
            {
                table.Add(new List<string>());
                table[j].Add(BasisList[j - 2]);
                table[j].Add(CbList[j - 2]);
                for (int i = 0; i < eq[j].Count; i++)
                {
                    table[j].Add(eq[j][i]);
                }
            }

            //tail of table :
            CalTableTail(ref table);
        }
        public static List<List<string>> PreProblemEqsGenerator()
        {
            return new List<List<string>>
            {
                //eq1:
                    new List<string> {
                 "12x1  +   18x2    +   10x3", //like of var atmain function

                 "2x1   +   3x2     +   4x3   <=      50", //st1
                 "-x1   +   x2      +   x3    <=      0"  , //st2
                 "-x2   +   3/2x3   <=    0" },//st3
                
                //eq2:
                   new List<string> {
                "12x1  +   18x2  +   10x3",

                "2x1   +    3x2     +   4x3 <=  50" ,
                "x1    -    x2      -   x3  >=  0" ,
                "x2    -    3/2x3   >=  0"},
                
                //eq3://min
                   new List<string> {
                "2x1   +   3x2",

                "x1     >=  125" ,
                "x1     +   x2     >=  350" ,
                "2x1    +   x2     <=  600"},
                
                //eq4://infesable
                   new List<string> {
                "2x1    +    6x2",

                "4x1    +   3x2 <=  12" ,
                "2x1    +   x2  >=  8"},
                
                //eq5://unbounded//chked
                   new List<string> {
                "2x1    +   6x2",

                "4x1    +    3x2    >=  12" ,
                "2x1    +    x2   >=  8"},
                /*
                //eq6:
                   new List<string> {
                "2x1    +   4x2   +   6x3",

                "-x2    +    2K1  - K2 +    K3  =  2" ,
                "1/2x2  +   x3  +   K1  -   1/2K2   =  3" ,
                "x1     +   1/2x2   +   2K1 +   3/2K2   =  7" ,//****************************************
                "-1/2x2 +   2K1 +   5/2K2   +   K4  <=      9"},
                */
                //eq7://exam//min
                   new List<string> {
                "24y1   +   6y2 +   y3  +   2y4",

                "6y1    +   y2  -   y3   >= 5" ,
                "4y1    +   2y2 +   y3   +   y4 >=  4"}
            };
        }
        public static void PrintProblem()
        {
            if (CurrentProblemTable == null)
                return;

            Debug.WriteLine("PrintProblem;");

            Print();

            Print(GoalMax ? Maximize : Minimize, Mode4Msg);

            for (int i = 0; i < CurrentProblemTable.Count; i++)
                Print(CurrentProblemTable[i], Mode4Msg);

            Print();
        }
        public static void PrintEqs(int SEq = -1, int EndEq = -1, bool bBasis = false, bool bcb = false)
        {
            Print();
            string Res = "";
            if ((SEq != -1) && (SEq >= 0))
                for (int i = Math.Max(SEq, 1); i < Math.Min(EndEq, eq.Count); i++)
                {
                    Res = "";
                    //int Maxi = ((i >= 2) ? eq[0].Count : (eq[0].Count - 1));
                    for (int m = 0; m <= (eq[0].Count - 1); m++)
                    {
                        string tmp = eq[i][m];

                        if (tmp[0] != '-')
                            tmp = '+' + tmp;

                        Res += tmp + "(" + eq[0][m] + ") ";
                    }

                    if ((Res.Length > 0) && (Res[0] == '+'))
                        Res = Res.Substring(1);

                    if (i >= 2)
                        Res = string.Format("S.t. ({0}) : {1} = {2}", (i - 1), Res, eq[i][eq[0].Count]);
                    else
                        Res = string.Format("O.F. : {0}( {1} )", (GoalMax ? "MAX" : "MIN"), Res);
                    Print(Res, StepMsg);
                }

            if (((!bBasis) && (!bcb)) || (BasisList == null))
                return;

            Print();
            if (BasisList.Count > 0)
                Print("Basis  :", StepMsg);
            for (int i = 0; i < BasisList.Count; i++)
            {
                Res = string.Format(" {0}) ", (i + 1));
                if (bBasis)
                    Res += BasisList[i];
                if ((bBasis) && (bcb))
                    Res += " = ";
                if (bcb)
                    Res += CbList[i];
                Print(Res, StepMsg);
            }
            //basis var:
            //1) x = 0
            //2) a = M
            Print();
        }
        public static void PrintTable(ref List<List<string>> ctable, int Pc, int Pr)
        {
            Debug.WriteLine("PrintTable;");
            List<int> SpacesList = new List<int>();
            int MaxSpace = 0;
            for (int i = 0; i < ctable[0].Count; i++)//col
            {
                SpacesList.Add(MinSpace);
                for (int j = 0; j < ctable.Count; j++)//row
                    SpacesList[i] = Math.Max(SpacesList[i], ctable[j][i].Length);
            }
            SpacesList.Add(MinSpace);

            for (int i = 0; i < SpacesList.Count; i++)
                MaxSpace += SpacesList[i] + 2;

            Console.WriteLine();
            for (int i = 0; i < ctable.Count; i++)//row
            {
                for (int j = 0; j < ctable[i].Count; j++)//column
                {
                    if (((j == Pc) && (i > 1) && (i < ctable.Count - 2)) || ((i == Pr) && (j > 1)))
                        Console.BackgroundColor = ConsoleColor.DarkGray;

                    Console.Write(ctable[i][j]);
                    for (int k = -1; k < SpacesList[j] - ctable[i][j].Length; k++)
                        Console.Write(Space);

                    Console.BackgroundColor = BG;
                    if (((j == 1) || (j == (ctable[1].Count - 2)) || ((j == 0)) && (i < (ctable.Count - 2))))
                    {
                        Console.ForegroundColor = ConsoleColor.DarkCyan;
                        Console.Write(vbar);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                        Console.Write(Space);
                }
                if ((i == 1) || (i == (ctable.Count - 3)))
                {
                    Console.WriteLine();
                    Console.ForegroundColor = ConsoleColor.DarkCyan;
                    for (int d = 0; d < MaxSpace; d++)
                    {
                        Console.Write(hbar);
                    }
                    Console.ForegroundColor = ConsoleColor.White;

                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }
        public static void Print(string Message = "", int state = -1)
        {
            ConsoleColor BC = Console.BackgroundColor;
            ConsoleColor FC = Console.ForegroundColor;

            if (state == ResultMsg)
            {
                Console.BackgroundColor = ResultBC;
                Console.ForegroundColor = ResultFC;
            }
            else if (state == StepMsg)
            {
                Console.BackgroundColor = StepBC;
                Console.ForegroundColor = StepFC;
            }
            else if (state == ErrorMsg)
            {
                Console.BackgroundColor = ErrorBC;
                Console.ForegroundColor = ErrorFC;
            }
            else if (state == Mode4Msg)
            {
                Console.BackgroundColor = Mode4BC;
                Console.ForegroundColor = Mode4FC;
            }
            Console.WriteLine(Message);

            Console.BackgroundColor = BC;
            Console.ForegroundColor = FC;
        }
        public static string PrintSolution(ref List<List<string>> ctable, string Z = "")
        {
            Dictionary<string, string> AllVar = new Dictionary<string, string>();
            for (int i = 2; i < ctable[0].Count; i++)//col
                AllVar.Add(ctable[0][i], "0");
            

            for (int i = 2; i < ctable.Count - 2; i++)//row
                AllVar[ctable[i][0]] = ctable[i][ctable[0].Count];

            string Res = "";
            foreach (var item in AllVar)
                Res += string.Format(" {0} = {1} ,", item.Key, item.Value);// x1 = 4 ,
            if (!string.IsNullOrEmpty(Z))
                Res += string.Format(" And (Z) Equal: {0}", Z);
            return Res;
        }
    }
}
