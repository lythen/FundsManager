﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Xml;
using DocumentFormat.OpenXml;
using System.Xml.Linq;

namespace Lythen.Common
{
    public class OpenXMLHelper
    {
        /// <summary>
        /// 将DataTable转化为XML输出
        /// </summary>
        /// <param name="dataSet">dataSet</param>
        /// <param name="fileName">文件名称</param>
        //public void DataTableToXML(DataTable dataTable, string fileName)
        public void DataSetToXML(DataSet dataSet, string filePath)
        {
            Dictionary<String, List<OpenXmlElement>> sets = ToSheets(dataSet);
            using (SpreadsheetDocument package = SpreadsheetDocument.Create(filePath, SpreadsheetDocumentType.Workbook))
            {
                WorkbookPart workbookpart = package.AddWorkbookPart();
                workbookpart.Workbook = new Workbook();

                Sheets sheets = workbookpart.Workbook.AppendChild(new Sheets());

                foreach (KeyValuePair<String, List<OpenXmlElement>> set in sets)
                {
                    WorksheetPart worksheetpart = workbookpart.AddNewPart<WorksheetPart>();
                    worksheetpart.Worksheet = new Worksheet(new SheetData(set.Value));
                    worksheetpart.Worksheet.Save();

                    Sheet sheet = new Sheet()
                    {
                        Id = workbookpart.GetIdOfPart(worksheetpart),
                        SheetId = (uint)(sheets.Count() + 1),
                        Name = set.Key
                    };
                    sheets.AppendChild(sheet);
                }
                workbookpart.Workbook.Save();
            }
        }
        Dictionary<string, List<OpenXmlElement>> ToSheets(DataSet ds)
        {
            return
                (from dt in ds.Tables.OfType<DataTable>()
                 select new
                 {
                     // Sheet Name
                     Key = dt.TableName,
                     Value = (
                         // Sheet Columns
                     new List<OpenXmlElement>(
                        new OpenXmlElement[] 
                {
                    new Row(
                        from d in dt.Columns.OfType<DataColumn>()
                        select (OpenXmlElement)new Cell()
                        {
                            CellValue = new CellValue(d.ColumnName),
                            DataType = CellValues.String
                        })
                })).Union
                         // Sheet Rows
                     ((from dr in dt.Rows.OfType<DataRow>()
                       select ((OpenXmlElement)new Row(from dc in dr.ItemArray
                                                       select (OpenXmlElement)new Cell()
                                                       {
                                                           CellValue = new CellValue(dc.ToString()),
                                                           DataType = CellValues.String
                                                       })))).ToList()
                 }).ToDictionary(p => p.Key, p => p.Value);
        }
        public List<OpenXmlElement> ToSheet(DataTable dt)
        {
            return
                // Sheet Columns
                     new List<OpenXmlElement>(
                        new OpenXmlElement[] 
                {
                    new Row(
                        from d in dt.Columns.OfType<DataColumn>()
                        select (OpenXmlElement)new Cell()
                        {
                            CellValue = new CellValue(d.ColumnName),
                            DataType = CellValues.String
                        })
                }).Union
                // Sheet Rows
                     ((from dr in dt.Rows.OfType<DataRow>()
                       select ((OpenXmlElement)new Row(from dc in dr.ItemArray
                                                       select (OpenXmlElement)new Cell()
                                                       {
                                                           CellValue = new CellValue(dc.ToString()),
                                                           DataType = CellValues.String
                                                       })))).ToList();
        }
        /// <summary>
        /// 将Excel多单一表转化为DataSet数据集对象
        /// </summary>
        /// <param name="filePath">Excel文件路径</param>
        /// <param name="sheetname">要取的表格名字，为空则取有内容的全部表格</param>
        /// <returns>转化的数据集</returns>
        public DataSet ExcelToDataSet(string filePath, string sheetname)
        {
            DataSet dataSet = new DataSet();
            try
            {
                using (SpreadsheetDocument spreadDocument = SpreadsheetDocument.Open(filePath, false))
                {
                    //指定WorkbookPart对象
                    WorkbookPart workBookPart = spreadDocument.WorkbookPart;
                    //获取Excel中SheetName集合
                    List<string> sheetNames = GetSheetNames(workBookPart);

                    foreach (string sheetName in sheetNames)
                    {
                        if (sheetname != "" && sheetname != sheetName) continue;
                        DataTable dataTable = WorkSheetToTable(workBookPart, sheetName);
                        if (dataTable != null)
                        {
                            dataSet.Tables.Add(dataTable);//将表添加到数据集
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                //throw new Exception("可能Excel正在打开中,请关闭重新操作！");
            }
            return dataSet;
        }

        /// <summary>
        /// 将Excel单一表转化为DataTable对象
        /// </summary>
        /// <param name="sheetName">SheetName</param>
        /// <param name="stream">Excel文件路径</param>
        /// <returns>DataTable对象</returns>
        public DataTable ExcelToDataTable(string sheetName, string filePath)
        {


            DataTable dataTable = new DataTable();
            //try
            //{
            //根据Excel流转换为spreadDocument对象
            using (SpreadsheetDocument spreadDocument = SpreadsheetDocument.Open(filePath, false))//Excel文档包
            {
                //Workbook workBook = spreadDocument.WorkbookPart.Workbook;//主文档部件的根元素
                //Sheets sheeets = workBook.Sheets;//块级结构（如工作表、文件版本等）的容器
                WorkbookPart workBookPart = spreadDocument.WorkbookPart;
                //获取Excel中SheetName集合
                List<string> sheetNames = GetSheetNames(workBookPart);
                if (sheetNames.Contains(sheetName))
                {
                    //根据WorkSheet转化为Table
                    dataTable = WorkSheetToTable(workBookPart, sheetName);
                }
            }
            //}
            //catch (Exception exp)
            //{
            //    //throw new Exception("可能Excel正在打开中,请关闭重新操作！");
            //}
            return dataTable;
        }

        /// <summary>
        /// 根据WorkbookPart获取所有SheetName
        /// </summary>
        /// <param name="workBookPart"></param>
        /// <returns>SheetName集合</returns>
        private List<string> GetSheetNames(WorkbookPart workBookPart)
        {
            List<string> sheetNames = new List<string>();
            Sheets sheets = workBookPart.Workbook.Sheets;
            foreach (Sheet sheet in sheets)
            {
                string sheetName = sheet.Name;
                if (!string.IsNullOrEmpty(sheetName))
                {
                    sheetNames.Add(sheetName);
                }
            }
            return sheetNames;
        }

        /// <summary>
        /// 根据WorkbookPart和sheetName获取该Sheet下所有Row数据
        /// </summary>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <param name="sheetName">SheetName</param>
        /// <returns>该SheetName下的所有Row数据</returns>
        public IEnumerable<Row> GetWorkBookPartRows(WorkbookPart workBookPart, string sheetName)
        {
            IEnumerable<Row> sheetRows = null;
            //根据表名在WorkbookPart中获取Sheet集合
            IEnumerable<Sheet> sheets = workBookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName);
            if (sheets.Count() == 0)
            {
                return null;//没有数据
            }

            WorksheetPart workSheetPart = workBookPart.GetPartById(sheets.First().Id) as WorksheetPart;
            //获取Excel中得到的行
            sheetRows = workSheetPart.Worksheet.Descendants<Row>();

            return sheetRows;
        }

        /// <summary>
        /// 根据WorkbookPart和sheetName获取该Sheet下所有Row数据
        /// </summary>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <param name="sheetName">SheetName</param>
        /// <returns>该SheetName下的所有Row数据</returns>
        public IEnumerable<Column> GetWorkBookPartColumns(WorkbookPart workBookPart, string sheetName)
        {
            IEnumerable<Column> sheetColumns = null;
            //根据表名在WorkbookPart中获取Sheet集合
            IEnumerable<Sheet> sheets = workBookPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName);
            if (sheets.Count() == 0)
            {
                return null;//没有数据
            }

            WorksheetPart workSheetPart = workBookPart.GetPartById(sheets.First().Id) as WorksheetPart;
            //获取Excel中得到的行
            sheetColumns = workSheetPart.Worksheet.Descendants<Column>();

            return sheetColumns;
        }

        /// <summary>
        /// 根据WorkbookPart和表名创建DataTable对象
        /// </summary>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <param name="tableName">表名</param>
        /// <returns>转化后的DataTable</returns>
        private DataTable WorkSheetToTable(WorkbookPart workBookPart, string sheetName)
        {
            //创建Table
            DataTable dataTable = new DataTable(sheetName);

            //根据WorkbookPart和sheetName获取该Sheet下所有行数据
            IEnumerable<Row> sheetRows = GetWorkBookPartRows(workBookPart, sheetName);
            IEnumerable<Column> sheetColumns = GetWorkBookPartColumns(workBookPart, sheetName);

            if (sheetRows == null || sheetRows.Count() <= 0)
            {
                return null;
            }

            foreach (Column col in sheetColumns)
            {
                uint str = col.Min;
            }

            //将数据导入DataTable,假定第一行为列名,第二行以后为数据
            int i = 0;
            foreach (Row row in sheetRows)
            {
                //获取Excel中的列头
                if (row.RowIndex == null && i == 0)
                {
                    i = 1;
                    List<DataColumn> listCols = GetDataColumn(row, workBookPart);
                    dataTable.Columns.AddRange(listCols.ToArray());
                }
                else if (row.RowIndex != null && row.RowIndex == 1 && i == 0)
                {
                    i = 1;
                    List<DataColumn> listCols = GetDataColumn(row, workBookPart);
                    dataTable.Columns.AddRange(listCols.ToArray());
                }
                else
                {
                    //Excel第二行同时为DataTable的第一行数据
                    DataRow dataRow = GetDataRow(row, dataTable, workBookPart);
                    if (dataRow != null)
                    {
                        dataTable.Rows.Add(dataRow);
                    }
                }
            }
            return dataTable;
        }

        /// <summary>
        /// 根据WorkbookPart获取NumberingFormats样式集合
        /// </summary>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <returns>NumberingFormats样式集合</returns>
        private List<string> GetNumberFormatsStyle(WorkbookPart workBookPart)
        {
            List<string> dicStyle = new List<string>();
            if (workBookPart.WorkbookStylesPart == null) return dicStyle;

            Stylesheet styleSheet = workBookPart.WorkbookStylesPart.Stylesheet;
            if (styleSheet.NumberingFormats == null) return dicStyle;
            OpenXmlElementList list = styleSheet.NumberingFormats.ChildElements;//获取NumberingFormats样式集合

            foreach (var element in list)//格式化节点
            {
                if (element.HasAttributes)
                {
                    using (OpenXmlReader reader = OpenXmlReader.Create(element))
                    {
                        if (reader.Read())
                        {
                            if (reader.Attributes.Count > 0)
                            {
                                string numFmtId = reader.Attributes[0].Value;//格式化ID
                                string formatCode = reader.Attributes[1].Value;//格式化Code
                                dicStyle.Add(formatCode);//将格式化Code写入List集合
                            }
                        }
                    }
                }
            }
            return dicStyle;
        }

        /// <summary>
        /// 根据行对象和WorkbookPart对象获取DataColumn集合
        /// </summary>
        /// <param name="row">Excel中行记录</param>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <returns>返回DataColumn对象集合</returns>
        private List<DataColumn> GetDataColumn(Row row, WorkbookPart workBookPart)
        {
            List<DataColumn> listCols = new List<DataColumn>();
            foreach (Cell cell in row)
            {
                string cellValue = GetCellValue(cell, workBookPart);
                DataColumn col = new DataColumn(cellValue);
                listCols.Add(col);
            }
            return listCols;
        }

        /// <summary>
        /// 根据Excel行\数据库表\WorkbookPart对象获取数据DataRow
        /// </summary>
        /// <param name="row">Excel中行对象</param>
        /// <param name="dateTable">数据表</param>
        /// <param name="workBookPart">WorkbookPart对象</param>
        /// <returns>返回一条数据记录</returns>
        private DataRow GetDataRow(Row row, DataTable dateTable, WorkbookPart workBookPart)
        {
            //读取Excel中数据,一一读取单元格,若整行为空则忽视该行
            DataRow dataRow = dateTable.NewRow();
            IEnumerable<Cell> cells = row.Elements<Cell>();

            int cellIndex = 0;//单元格索引
            int nullCellCount = cellIndex;//空行索引
            int cellCount = dataRow.ItemArray.Count();
            foreach (Cell cell in row)
            {
                string cellVlue = GetCellValue(cell, workBookPart);
                if (string.IsNullOrEmpty(cellVlue))
                {
                    nullCellCount++;
                }

                dataRow[cellIndex] = cellVlue;
                cellIndex++;
                if (cellIndex == cellCount) break;
            }
            if (nullCellCount == cellIndex)//剔除空行
            {
                dataRow = null;//一行中单元格索引和空行索引一样
            }
            return dataRow;
        }

        /// <summary>
        /// 根据Excel单元格和WorkbookPart对象获取单元格的值
        /// </summary>
        /// <param name="cell">Excel单元格对象</param>
        /// <param name="workBookPart">Excel WorkbookPart对象</param>
        /// <returns>单元格的值</returns>
        public string GetCellValue(Cell cell, WorkbookPart workBookPart)
        {
            string cellValue = string.Empty;
            if (cell.ChildElements.Count == 0)//Cell节点下没有子节点
            {
                return cellValue;
            }
            string cellRefId = "";
            if (cell.CellReference != null)
                cellRefId = cell.CellReference.InnerText;//获取引用相对位置
            string cellInnerText = cell.CellValue.InnerText;//获取Cell的InnerText
            cellValue = cellInnerText;//指定默认值(其实用来处理Excel中的数字)

            //获取WorkbookPart中NumberingFormats样式集合
            List<string> dicStyles = GetNumberFormatsStyle(workBookPart);
            //获取WorkbookPart中共享String数据
            SharedStringTable sharedTable = null;
            if (workBookPart.SharedStringTablePart != null)
                sharedTable = workBookPart.SharedStringTablePart.SharedStringTable;

            try
            {
                EnumValue<CellValues> cellType = cell.DataType;//获取Cell数据类型
                if (cellType != null)//Excel对象数据
                {
                    switch (cellType.Value)
                    {
                        case CellValues.SharedString://字符串
                            //获取该Cell的所在的索引
                            int cellIndex = int.Parse(cellInnerText);
                            //cellValue = sharedTable.ChildElements[cellIndex].InnerText;
                            cellValue = GetText(sharedTable.ChildElements[cellIndex].OuterXml);
                            break;
                        case CellValues.Boolean://布尔
                            cellValue = (cellInnerText == "1") ? "TRUE" : "FALSE";
                            break;
                        case CellValues.Date://日期
                            cellValue = Convert.ToDateTime(cellInnerText).ToString();
                            break;
                        case CellValues.Number://数字
                            cellValue = Convert.ToDecimal(cellInnerText).ToString();
                            break;
                        default: cellValue = cellInnerText; break;
                    }
                }
                else//格式化数据
                {
                    if (dicStyles.Count > 0 && cell.StyleIndex != null)//对于数字,cell.StyleIndex==null
                    {
                        int styleIndex = Convert.ToInt32(cell.StyleIndex.Value);
                        string cellStyle = dicStyles[styleIndex];//获取该索引的样式
                        if (cellStyle.Contains("yyyy") || cellStyle.Contains("h")
                            || cellStyle.Contains("dd") || cellStyle.Contains("ss"))
                        {
                            //如果为日期或时间进行格式处理,去掉“;@”
                            cellStyle = cellStyle.Replace(";@", "");
                            while (cellStyle.Contains("[") && cellStyle.Contains("]"))
                            {
                                int otherStart = cellStyle.IndexOf('[');
                                int otherEnd = cellStyle.IndexOf("]");

                                cellStyle = cellStyle.Remove(otherStart, otherEnd - otherStart + 1);
                            }
                            double doubleDateTime = double.Parse(cellInnerText);
                            DateTime dateTime = DateTime.FromOADate(doubleDateTime);//将Double日期数字转为日期格式
                            if (cellStyle.Contains("m")) { cellStyle = cellStyle.Replace("m", "M"); }
                            if (cellStyle.Contains("AM/PM")) { cellStyle = cellStyle.Replace("AM/PM", ""); }
                            // cellValue = dateTime.ToString(cellStyle);//不知道为什么Excel 2007中格式日期为yyyy/m/d
                            cellValue = dateTime.ToString();
                        }
                        else//其他的货币、数值
                        {
                            cellStyle = cellStyle.Substring(cellStyle.LastIndexOf('.') - 1).Replace("\\", "");
                            decimal decimalNum = decimal.Parse(cellInnerText);
                            cellValue = decimal.Parse(decimalNum.ToString(cellStyle)).ToString();
                        }
                    }
                }
            }
            catch (Exception exp)
            {
                //string expMessage = string.Format("Excel中{0}位置数据有误,请确认填写正确！", cellRefId);
                //throw new Exception(expMessage);
                //cellValue = "N/A";
            }
            return cellValue;
        }
        string GetText(string xml)
        {
            XDocument xdoc = XDocument.Parse(xml);
            XNamespace space = @"http://schemas.openxmlformats.org/spreadsheetml/2006/main";
            XElement element = xdoc.Element(space + "si");
            if (element == null) return "";
            StringBuilder sbText = new StringBuilder();
            string str = "";
            if (element.Element(space + "r") != null)
            {
                foreach (XElement xr in element.Elements(space + "r"))
                {
                    str = "";
                    if ((xr.Element(space + "t") != null))
                    {
                        str = xr.Element(space + "t").Value;
                        if (string.IsNullOrEmpty(str)) continue;
                        if (xr.Element(space + "rPr") != null)
                        {
                            XElement rpr = xr.Element(space + "rPr");
                            if (rpr.Element(space + "b") != null) str = "<b>" + str + "</b>";
                            if (rpr.Element(space + "u") != null) str = "<u>" + str + "</u>";
                            if (rpr.Element(space + "i") != null) str = "<i>" + str + "</i>";
                        }
                        else if (xr.Element(space + "rpr") != null)
                        {
                            XElement rpr = xr.Element(space + "rpr");
                            if (rpr.Element(space + "b") != null) str = "<b>" + str + "</b>";
                            if (rpr.Element(space + "u") != null) str = "<u>" + str + "</u>";
                            if (rpr.Element(space + "i") != null) str = "<i>" + str + "</i>";
                        }
                    }
                    sbText.Append(str);
                }
            }
            else if (element.Element(space + "t") != null) return element.Element(space + "t").Value;
            return sbText.ToString();
        }
        /// <summary>
        /// 获取Excel中多表的表名
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private List<string> GetExcelSheetNames(string filePath)
        {
            string sheetName = string.Empty;
            List<string> sheetNames = new List<string>();//所有Sheet表名
            using (SpreadsheetDocument spreadDocument = SpreadsheetDocument.Open(filePath, false))
            {
                WorkbookPart workBook = spreadDocument.WorkbookPart;
                Stream stream = workBook.GetStream(FileMode.Open);
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(stream);

                XmlNamespaceManager xmlNSManager = new XmlNamespaceManager(xmlDocument.NameTable);
                xmlNSManager.AddNamespace("default", xmlDocument.DocumentElement.NamespaceURI);
                XmlNodeList nodeList = xmlDocument.SelectNodes("//default:sheets/default:sheet", xmlNSManager);

                foreach (XmlNode node in nodeList)
                {
                    sheetName = node.Attributes["name"].Value;
                    sheetNames.Add(sheetName);
                }
            }
            return sheetNames;
        }

        #region SaveCell
        private void InsertTextCellValue(Worksheet worksheet, string column, uint row, string value)
        {
            Cell cell = ReturnCell(worksheet, column, row);
            CellValue v = new CellValue();
            v.Text = value;
            cell.AppendChild(v);
            cell.DataType = new EnumValue<CellValues>(CellValues.String);
            worksheet.Save();
        }
        private void InsertNumberCellValue(Worksheet worksheet, string column, uint row, string value)
        {
            Cell cell = ReturnCell(worksheet, column, row);
            CellValue v = new CellValue();
            v.Text = value;
            cell.AppendChild(v);
            cell.DataType = new EnumValue<CellValues>(CellValues.Number);
            worksheet.Save();
        }
        private static Cell ReturnCell(Worksheet worksheet, string columnName, uint row)
        {
            Row targetRow = ReturnRow(worksheet, row);

            if (targetRow == null)
                return null;

            return targetRow.Elements<Cell>().Where(c =>
               string.Compare(c.CellReference.Value, columnName + row,
               true) == 0).First();
        }
        private static Row ReturnRow(Worksheet worksheet, uint row)
        {
            return worksheet.GetFirstChild<SheetData>().
            Elements<Row>().Where(r => r.RowIndex == row).First();
        }
        #endregion
    }
}
