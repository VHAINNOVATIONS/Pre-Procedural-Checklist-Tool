using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Security;
using System.Security.Cryptography;

namespace VAPPCT.DA
{
    /// <summary>
    /// This class contains helper methods for conversion and formatting 
    /// of values
    /// </summary>
    public static class CDataUtils
    {
        /// <summary>
        /// method
        /// truncates the passed in string at the specifed length - 3 and adds an ellipsis
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="nChars"></param>
        /// <returns></returns>
        public static string TruncateString(string strText, int nChars)
        {
            if (strText.Length <= nChars)
            {
                return strText;
            }

            return strText.Substring(0, nChars - 3) + "...";
        }

        /// <summary>
        /// breaks a string up and inserts a delimeter
        /// </summary>
        /// <param name="strText"></param>
        /// <param name="strDelimiter"></param>
        /// <param name="nChars"></param>
        /// <returns></returns>
        static public string DelimitString(
            string strText, 
            string strDelimiter, 
            int nChars)
        {
            //string we are going to work on
            string strWork = strText;
            string strPiece = string.Empty;

            //where to break the string
            int nBreak = nChars - strDelimiter.Length;

            //just return the string if its less than nchars
            string strDelimitString = String.Empty;
            if (strText.Length <= nChars)
            {
                strDelimitString = strText;
            }
            else
            {
                while (strWork.Length > nBreak)
                {
                    //get a piece but dont break on a non space
                    strPiece = strWork.Substring(0, nBreak);
                    int nPos = strPiece.IndexOf(" ");
                    if (nPos > -1)
                    {
                        while (strPiece.Substring(strPiece.Length - 1) != " ")
                        {
                            strPiece = strPiece.Substring(0, strPiece.Length - 1);
                        }
                    }

                    //add it to the full string we are returning
                    strDelimitString += strPiece;

                    //add the delimeter
                    strDelimitString += strDelimiter;

                    //pull off the piece from the working string
                    //strWork = strWork.Substring(nBreak);
                    
                    strWork = strWork.Substring(strPiece.Length);
                }

                //last piece
                strDelimitString += strWork;
            }

            return strDelimitString;
        }

        /// <summary>
        /// helper to get the number of rows in a dataset
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        static public int GetRowCount(DataSet ds)
        {
            if (ds == null)
            {
                return 0;
            }

            if (ds.Tables == null)
            {
                return 0;
            }

            int i = 0;
            foreach (DataTable table in ds.Tables)
            {
                i += table.Rows.Count;
            }

            return i;
        }

        /// <summary>
        /// We use 01/01/0001 to mean a "null" date.
        /// this will return a null date
        /// </summary>
        /// <returns></returns>
        static public DateTime GetNullDate()
        {
            return new DateTime(0001, 01, 01, 0, 0, 0);
        }

        /// <summary>
        /// is this string a numeric value?
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        static public bool IsNumeric(string strValue)
        {
            long lValue = 0;
            return long.TryParse(strValue, out lValue);
        }

        /// <summary>
        /// We use 01/01/0001 to mean a "null" date.
        /// this will check to see if the date is null
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public bool IsDateNull(DateTime dt)
        {
            if (dt == null)
            {
                return true;
            }

            if (dt.Year == 0001)
            {
                if (dt.Month == 1)
                {
                    if (dt.Day == 1)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// -1 = invalid
        /// 0 = equal
        /// 1 = dt1 > dt2
        /// 2 = dt2 > dt1
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        static public k_COMPARE CompareDates(DateTime dt1, DateTime dt2)
        {
            //-1 invalid date range
            if (dt1 == null || dt2 == null)
            {
                return k_COMPARE.INVALID;
            }

            //strip time for the compare
            DateTime dt1Compare = new DateTime(dt1.Year, dt1.Month, dt1.Day, 0, 0, 0, 0);
            DateTime dt2Compare = new DateTime(dt2.Year, dt2.Month, dt2.Day, 0, 0, 0, 0);

            int nCompare = DateTime.Compare(dt1Compare, dt2Compare);

            //Less than zero
	        //t1 is earlier than t2.
            if (nCompare < 0)
            {
                return k_COMPARE.LESSTHAN;
            }

            //Zero
	        //t1 is the same as t2.
            if (nCompare == 0)
            {
                return k_COMPARE.EQUALTO;
            }

            //Greater than zero
            //t1 is later than t2. 
            if (nCompare > 0)
            {
                return k_COMPARE.GREATERTHAN;
            }

            return k_COMPARE.INVALID;        
        }

        /// <summary>
        /// -1 = invalid
        /// 0 = equal
        /// 1 = dt1 > dt2
        /// 2 = dt2 > dt1
        /// </summary>
        /// <param name="dt1"></param>
        /// <param name="dt2"></param>
        /// <returns></returns>
        static public k_COMPARE CompareDateTimes(DateTime dt1, DateTime dt2)
        {
            //-1 invalid date range
            if (dt1 == null || dt2 == null)
            {
                return k_COMPARE.INVALID;
            }

            //strip time for the compare
            DateTime dt1Compare = new DateTime(dt1.Year, dt1.Month, dt1.Day, dt1.Hour, dt1.Minute, dt1.Second, 0);
            DateTime dt2Compare = new DateTime(dt2.Year, dt2.Month, dt2.Day, dt2.Hour, dt2.Minute, dt1.Second, 0);

            int nCompare = DateTime.Compare(dt1Compare, dt2Compare);

            //Less than zero
            //t1 is earlier than t2.
            if (nCompare < 0)
            {
                return k_COMPARE.LESSTHAN;
            }

            //Zero
            //t1 is the same as t2.
            if (nCompare == 0)
            {
                return k_COMPARE.EQUALTO;
            }

            //Greater than zero
            //t1 is later than t2. 
            if (nCompare > 0)
            {
                return k_COMPARE.GREATERTHAN;
            }

            return k_COMPARE.INVALID;
        }

        /// <summary>
        /// gets a date time given a MDWS date formatted
        /// 20030428.000700
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <returns></returns>
        static public DateTime GetMDWSDate(string strDateTime)
        {
            if (strDateTime.Length < 15)
            {
                return GetNullDate();
            }

            if (strDateTime.IndexOf(".") == -1)
            {
                return GetNullDate();
            }

            string strYear = strDateTime.Substring(0, 4);
            string strMonth = strDateTime.Substring(4, 2);
            string strDay = strDateTime.Substring(6, 2);
            string strHH = strDateTime.Substring(9, 2);
            string strMM = strDateTime.Substring(11, 2);
            
            return new System.DateTime( Convert.ToInt32(strYear), 
                                        Convert.ToInt32(strMonth),
                                        Convert.ToInt32(strDay),
                                        Convert.ToInt32(strHH), 
                                        Convert.ToInt32(strMM), 
                                        0);
        }

        /// <summary>
        /// gets a date given a formmated datetime string mm/dd/yyyy
        /// and hh mm ss
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <returns></returns>
        static public DateTime GetDate(
            string strDateTime,
            int nHH,
            int nMM,
            int nSS)
        {
            DateTime dt = GetDate(strDateTime);
            if (IsDateNull(dt))
            {
                return dt;
            }

            return new System.DateTime(dt.Year, dt.Month, dt.Day, nHH, nMM, nSS);
        }
        
        /// <summary>
        /// gets a date given a formmated datetime string mm/dd/yyyy
        /// </summary>
        /// <param name="strDateTime"></param>
        /// <returns></returns>
        static public DateTime GetDate(string strDateTime)
        {
            string[] arstrDate = strDateTime.Split(new Char[] { '/' });
            if (arstrDate.Length < 3)
            {
                return new System.DateTime(0001, 01, 01, 0, 0, 0);
            }

            int nMonth = Convert.ToInt32(arstrDate[0]);
            int nDay = Convert.ToInt32(arstrDate[1]);
            int nYear = Convert.ToInt32(arstrDate[2]);

            return new System.DateTime(nYear, nMonth, nDay, 0, 0, 0);
        }

        /// <summary>
        /// convert a string to a long
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        static public long ToLong(string strValue)
        {
            long lValue = -1;

            try
            {
                lValue = Convert.ToInt32(strValue);
            }
            catch (Exception)
            {
                return -1;
            }

            return lValue;
        }

        /// <summary>
        /// converts a constant k_active to a long
        /// </summary>
        /// <param name="activeValue"></param>
        /// <returns></returns>
        static public long ToLong(k_ACTIVE activeValue)
        {
            long lValue = -1;

            try
            {
                lValue = Convert.ToInt32(activeValue);
            }
            catch (Exception)
            {
                return -1;
            }

            return lValue;
        }

        /// <summary>
        /// YYYYMMDD
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public string GetMilitaryDateAsString(DateTime dt)
        {
            string strYear = Convert.ToString(dt.Year);

            string strMonth = Convert.ToString(dt.Month);
            if (strMonth.Length < 2)
                strMonth = "0" + strMonth;

            string strDay = Convert.ToString(dt.Day);
            if (strDay.Length < 2)
                strDay = "0" + strDay;

            return strYear + strMonth + strDay;
        }

        /// <summary>
        /// used to retrieve a date in a string format for display
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public string GetDateAsString(DateTime dt)
        {
            string strMonth = Convert.ToString(dt.Month);
            if (strMonth.Length < 2)
                strMonth = "0" + strMonth;

            string strDay = Convert.ToString(dt.Day);
            if (strDay.Length < 2)
                strDay = "0" + strDay;

            string strYear = Convert.ToString(dt.Year);

            return strMonth + "/" + strDay + "/" + strYear;
        }

        /// <summary>
        /// used to retrieve a date/time in a string format for display
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        static public string GetDateTimeAsString(DateTime dt)
        {
            string strMonth = Convert.ToString(dt.Month);
            if (strMonth.Length < 2)
                strMonth = "0" + strMonth;

            string strDay = Convert.ToString(dt.Day);
            if (strDay.Length < 2)
                strDay = "0" + strDay;

            string strYear = Convert.ToString(dt.Year);

            string strHours = Convert.ToString(dt.Hour);
            if (strHours.Length < 2)
                strHours = "0" + strHours;

            string strMinutes = Convert.ToString(dt.Minute);
            if (strMinutes.Length < 2)
                strMinutes = "0" + strMinutes;

            return strMonth + "/" + strDay + "/" + strYear + " " + strHours + ":" + strMinutes;           
        }

        /// <summary>
        /// helper to determine if a dataset is empty
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        static public bool IsEmpty(DataSet ds)
        {
            if (ds == null)
            {
                return true;
            }

            if (ds.Tables == null)
            {
                return true;
            }

            foreach (DataTable table in ds.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// US:838
        /// gets a dataset as delimeted fields
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strField"></param>
        /// <param name="strData"></param>
        /// <returns></returns>
        static public CStatus GetDSDelimitedData(
            DataSet ds,
            string strField,
            string strDelimeter,
            out string strData)
        {
            strData = string.Empty;
            CStatus status = new CStatus();
            try
            {
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (!row.IsNull(strField))
                        {
                            string strValue = GetDSStringValue(row, strField);
                            if (!String.IsNullOrEmpty(strValue))
                            {
                                //add the value and delimeter to the list
                                strData += strValue;
                                strData += strDelimeter;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                status.Status = false;
                status.StatusCode = k_STATUS_CODE.Failed;
                status.StatusComment = e.Message;
            }

            return status;
        }
        
        /// <summary>
        /// helper to get one value from a dataset as a string
        /// we do this throughout the app so it makes sense to wrap it
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strField"></param>
        /// <returns></returns>
        static public long GetDSLongValue(DataSet ds, string strField)
        {
            long lValue = -1;
            if (IsEmpty(ds))
            {
                return -1;
            }

            string strValue = GetDSStringValue(ds, strField);
            try
            {
                lValue = Convert.ToInt32(strValue);
            }
            catch(Exception)
            {
                return -1;
            }

            return lValue;
        }

        /// <summary>
        /// helper to get a double value from a DS
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strField"></param>
        /// <returns></returns>
        static public double GetDSDoubleValue(DataSet ds, string strField)
        {
            double dblValue = -1;
            if (IsEmpty(ds))
            {
                return -1;
            }

            string strValue = GetDSStringValue(ds, strField);
            try
            {
                dblValue = Convert.ToDouble(strValue);
            }
            catch (Exception)
            {
                return -1;
            }

            return dblValue;
        }

        /// <summary>
        /// helper to get a double value from a data row
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="strField"></param>
        /// <returns></returns>
        static public double GetDSDoubleValue(DataRow dr, string strField)
        {
            double dblValue = -1;
            if (dr == null)
            {
                return -1;
            }

            try
            {
                dblValue = Convert.ToDouble(dr[strField].ToString());
            }
            catch (Exception)
            {
                return -1;
            }

            return dblValue;
        }

        /// <summary>
        /// helper to get a datetime value from a record
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strField"></param>
        /// <returns></returns>
        static public DateTime GetDSDateTimeValue(DataSet ds, string strField)
        {
            DateTime dt = new DateTime(0001,01,01,0,0,0);
            if (IsEmpty(ds))
            {
                return dt;
            }

            try
            {
                foreach (DataTable table in ds.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (!row.IsNull(strField))
                        {
                            dt = Convert.ToDateTime(row[strField]);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return dt;
            }

            return dt;
        }

        /// <summary>
        /// helper to get a value from a dataset row as a long
        /// do this throughout the app so it makes sense to wrap it
        /// </summary>
        /// <param name="dr"></param>
        /// <param name="strField"></param>
        /// <returns></returns>
        static public long GetDSLongValue(DataRow dr, string strField)
        {
            long lValue = -1;
            if (dr == null)
            {
                return -1;
            }

            try
            {
                lValue = Convert.ToInt32(dr[strField].ToString());
            }
            catch (Exception)
            {
                return -1;
            }

            return lValue;
        }

        /// <summary>
        /// helper to get one value from a dataset as a string
        /// we do this throughout the app so it makes sense to wrap it
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strField"></param>
        /// <returns></returns>
        static public string GetDSStringValue(DataRow dr, string strField)
        {
            if (dr == null)
            {
                return String.Empty;
            }

            if (!dr.IsNull(strField))
            {
                return Convert.ToString(dr[strField]);
            }

            return String.Empty;
        }

        /// <summary>
        /// helper to get a datetime from a data row
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strField"></param>
        /// <returns></returns>
        static public DateTime GetDSDateTimeValue(DataRow dr, string strField)
        {
            DateTime dt = new DateTime(0001, 01, 01,0,0,0);
            if (dr == null)
            {
                return dt;
            }

            try
            {
                if (!dr.IsNull(strField))
                {
                    dt = Convert.ToDateTime(dr[strField]);
                    return dt;
                }
            }
            catch (Exception)
            {
                return dt;
            }

            return dt;
        }

        /// <summary>
        /// helper to get one value from a dataset as a string
        /// we do this throughout the app so it makes sense to wrap it
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="strField"></param>
        /// <returns></returns>
        static public string GetDSStringValue(DataSet ds, string strField)
        {
            if (IsEmpty(ds))
            {
                return null;
            }

            foreach (DataTable table in ds.Tables)
            {
                foreach (DataRow row in table.Rows)
                {
                    if (!row.IsNull(strField))
                    {
                        return Convert.ToString(row[strField]);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// converts a string to a contstant k_ACTIVE
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        static public k_ACTIVE ToActiveID(string strValue)
        {
            return (strValue == "1") ? k_ACTIVE.ACTIVE : k_ACTIVE.INACTIVE;
        }

        /// <summary>
        /// converts an integer to a contstant k_ACTIVE
        /// </summary>
        /// <param name="nValue"></param>
        /// <returns></returns>
        static public k_ACTIVE ToActiveID(int nValue)
        {
            return (nValue == 1) ? k_ACTIVE.ACTIVE : k_ACTIVE.INACTIVE;
        }

        /// <summary>
        /// converts a long to a contstant k_ACTIVE
        /// </summary>
        /// <param name="lValue"></param>
        /// <returns></returns>
        static public k_ACTIVE ToActiveID(long lValue)
        {
            return (lValue == 1) ? k_ACTIVE.ACTIVE : k_ACTIVE.INACTIVE;
        }

        /// <summary>
        /// encrypt a string 
        /// </summary>
        /// <param name="strClear"></param>
        /// <returns></returns>
        static public string enc(
            string strClear, 
            string strEKey, 
            string strEInitVector)
        {
            //triple des
            TripleDES des = new TripleDESCryptoServiceProvider();

            //set the IV = to the IV passed in
            string strIV = strEInitVector;

            //get the key from the config file
            string strKey = strEKey;
                                    
            //set the key and vector
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            des.Key = enc.GetBytes(strKey);
            des.IV = enc.GetBytes(strIV);

            //encrypt
            ICryptoTransform ctEncrypt = des.CreateEncryptor();
            byte[] bCClear = System.Text.Encoding.Unicode.GetBytes(strClear);
            byte[] bCEncrypted = ctEncrypt.TransformFinalBlock(bCClear, 0, bCClear.Length);

            //convert the encrypted to string
            string strEnc = "";
            for (int i = 0; i < bCEncrypted.Length; i++)
            {
                string str = bCEncrypted[i].ToString();
                while (str.Length < 3)
                {
                    str = "0" + str;
                }

                strEnc += str;
            }

            //return the string
            return strEnc;
        }

        /// <summary>
        /// decrypt a string 
        /// </summary>
        /// <param name="strEnc"></param>
        /// <returns></returns>
        static public string dec(
            string strEnc, 
            string strEKey, 
            string strEInitVector)
        {
            string strEClear = "";

            byte[] bCEncrypted = new byte[strEnc.Length / 3];
            int noffset = 0;
            for (int j = 0; j < strEnc.Length; j += 3)
            {
                bCEncrypted[noffset++] = Convert.ToByte(strEnc.Substring(j, 3));
            }

            //if nothing to decrypt just return ""
            if (bCEncrypted.Length < 1)
            {
                return strEClear;
            }

            TripleDES des = new TripleDESCryptoServiceProvider();

            //set the IV = to the IV passed in
            string strIV = strEInitVector;

            //get the key from the config file
            string strKey = strEKey;

            //set the key and vector
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            des.Key = enc.GetBytes(strKey);
            des.IV = enc.GetBytes(strIV);

            //decrypt       
            ICryptoTransform ctDecrypt = des.CreateDecryptor();
            byte[] bEClear = ctDecrypt.TransformFinalBlock(bCEncrypted, 0, bCEncrypted.Length);
            strEClear = System.Text.Encoding.Unicode.GetString(bEClear);

            return strEClear;
        }
    }
}
