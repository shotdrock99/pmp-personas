using System;

namespace ModernizacionPersonas.Common.Utilities
{
    public class Number2WordConverter
    {

        public static string Convert(string value)
        {
            var number = System.Convert.ToDecimal(value);
            return Convert(number);
        }

        public static string Convert(decimal value)
        {
            string isNegative = "";
            try
            {
                var number = System.Convert.ToDouble(value).ToString();

                if (number.Contains("-"))
                {
                    isNegative = "Menos ";
                    number = number.Substring(1, number.Length - 1);
                }

                if (number == "0")
                {
                    return "Cero";
                }
                else
                {
                    return $"{isNegative + ConvertToWords(number)}";
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Number2WordConverter :: Convert", ex);
            }
        }

        private static string ones(string Number)
        {
            int _Number = System.Convert.ToInt32(Number);
            string name = "";
            switch (_Number)
            {

                case 1:
                    name = "Uno";
                    break;
                case 2:
                    name = "Dos";
                    break;
                case 3:
                    name = "Tres";
                    break;
                case 4:
                    name = "Cuatro";
                    break;
                case 5:
                    name = "Cinco";
                    break;
                case 6:
                    name = "Seis";
                    break;
                case 7:
                    name = "Siete";
                    break;
                case 8:
                    name = "Ocho";
                    break;
                case 9:
                    name = "Nueve";
                    break;
            }
            return name;
        }

        private static string tens(string Number)
        {
            int _Number = System.Convert.ToInt32(Number);
            string name = null;
            switch (_Number)
            {
                case 10:
                    name = "Diez";
                    break;
                case 11:
                    name = "Once";
                    break;
                case 12:
                    name = "Doce";
                    break;
                case 13:
                    name = "Trece";
                    break;
                case 14:
                    name = "Catorce";
                    break;
                case 15:
                    name = "Quince";
                    break;
                case 16:
                    name = "Dieciséis";
                    break;
                case 17:
                    name = "Diecisiete";
                    break;
                case 18:
                    name = "Dieciocho";
                    break;
                case 19:
                    name = "Diecinueve";
                    break;
                case 20:
                    name = "Veinte";
                    break;
                case 30:
                    name = "Treinta";
                    break;
                case 40:
                    name = "Cuarenta";
                    break;
                case 50:
                    name = "Cincuenta";
                    break;
                case 60:
                    name = "Sesenta";
                    break;
                case 70:
                    name = "Setenta";
                    break;
                case 80:
                    name = "Ochenta";
                    break;
                case 90:
                    name = "Noventa";
                    break;
                default:
                    if (_Number > 0)
                    {
                        name = tens(Number.Substring(0, 1) + "0") + " " + ones(Number.Substring(1));
                    }
                    break;
            }
            return name;
        }

        private static string ConvertWholeNumber(string Number)
        {
            string word = "";
            try
            {
                bool beginsZero = false;//tests for 0XX    
                bool isDone = false;//test if already translated    
                double dblAmt = (System.Convert.ToDouble(Number));
                //if ((dblAmt > 0) && number.StartsWith("0"))    
                if (dblAmt > 0)
                {//test for zero or digit zero in a nuemric    
                    beginsZero = Number.StartsWith("0");

                    int numDigits = Number.Length;
                    int pos = 0;//store digit grouping    
                    string place = "";//digit grouping name:hundres,thousand,etc...    
                    switch (numDigits)
                    {
                        case 1://ones' range    

                            word = ones(Number);
                            isDone = true;
                            break;
                        case 2://tens' range    
                            word = tens(Number);
                            isDone = true;
                            break;
                        case 3://hundreds' range    
                            pos = (numDigits % 3) + 1;
                            place = " Cientos ";
                            break;
                        case 4://thousands' range    
                        case 5:
                        case 6:
                            pos = (numDigits % 4) + 1;
                            place = " Miles ";
                            break;
                        case 7://millions' range    
                        case 8:
                        case 9:
                            pos = (numDigits % 7) + 1;
                            place = " Millones ";
                            break;
                        case 10://Billions's range    
                        case 11:
                        case 12:

                            pos = (numDigits % 10) + 1;
                            place = " Billones ";
                            break;
                        //add extra case options for anything above Billion...    
                        default:
                            isDone = true;
                            break;
                    }
                    if (!isDone)
                    {//if transalation is not done, continue...(Recursion comes in now!!)    
                        if (Number.Substring(0, pos) != "0" && Number.Substring(pos) != "0")
                        {
                            try
                            {
                                word = ConvertWholeNumber(Number.Substring(0, pos)) + place + ConvertWholeNumber(Number.Substring(pos));
                            }
                            catch { }
                        }
                        else
                        {
                            word = ConvertWholeNumber(Number.Substring(0, pos)) + ConvertWholeNumber(Number.Substring(pos));
                        }

                        //check for trailing zeros    
                        //if (beginsZero) word = " and " + word.Trim();    
                    }
                    //ignore digit grouping names    
                    if (word.Trim().Equals(place.Trim())) word = "";
                }
            }
            catch { }
            return word.Trim();
        }

        private static string ConvertToWords(string numb)
        {
            string val = "", wholeNo = numb, points = "", andStr = "", pointStr = "";
            string endStr = "";
            try
            {
                int decimalPlace = numb.IndexOf(".");
                if (decimalPlace > 0)
                {
                    wholeNo = numb.Substring(0, decimalPlace);
                    points = numb.Substring(decimalPlace + 1);
                    if (System.Convert.ToInt32(points) > 0)
                    {
                        andStr = "and";// just to separate whole numbers from points/cents    
                        endStr = "Paisa " + endStr;//Cents    
                        pointStr = ConvertDecimals(points);
                    }
                }
                val = string.Format("{0} {1}{2} {3}", ConvertWholeNumber(wholeNo).Trim(), andStr, pointStr, endStr);
            }
            catch { }
            return val;
        }

        private static string ConvertDecimals(string number)
        {
            string cd = "", digit = "", engOne = "";
            for (int i = 0; i < number.Length; i++)
            {
                digit = number[i].ToString();
                if (digit.Equals("0"))
                {
                    engOne = "Cero";
                }
                else
                {
                    engOne = ones(digit);
                }
                cd += " " + engOne;
            }
            return cd;
        }
    }
}
