using System;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using OfficeOpenXml.Table;

namespace ExcelIO
{
    public class Excel
    {
        #region Member Variables
        private FileInfo excelFile;
        private ExcelWorksheets workSheets;
        private ExcelPackage excelApp;
        #endregion
        #region Properties
        public ExcelWorksheets WorkSheets { get => workSheets; set => workSheets = value; }
        public FileInfo ExcelFile { get => excelFile; set => excelFile = value; }
        public ExcelPackage ExcelApp { get => excelApp; set => excelApp = value; }
        #endregion
        #region Constructors
        public Excel(string path)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            excelFile = new FileInfo(path);
            excelApp = new ExcelPackage(excelFile);
            workSheets = excelApp.Workbook.Worksheets;

        }
        ~Excel()
        {
            excelApp.Dispose();
        }
        #endregion
        #region Methods
        public List<T> ReadFromTable<T>(string workSheetName, string tableName, Func<object[,], int, T> predicate)
        {
            List<T> result = new List<T>();
            var ws = this.WorkSheets[workSheetName];
            var table = ws.Tables[tableName];
            if (table == null)
            {
                throw new Exception("no table with this name");
            }

            object[,] values = table.Range.Value as object[,];
            int cols = values.GetLength(1);
            int rows = values.GetLength(0);
            for (int i = 1; i < rows; i++)
            {
                result.Add(predicate(values, i));
            }
            return result;
        }
        #endregion
    }
}
