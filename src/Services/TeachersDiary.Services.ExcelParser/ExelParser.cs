﻿using System.Collections.Generic;
using System.Data;
using System.IO;
using Excel;
using TeachersDiary.Data.Services.Contracts;
using TeachersDiary.Domain;
using TeachersDiary.Services.Contracts;

namespace TeachersDiary.Services.ExcelParser
{
    public class ExelParser : IExelParser
    {
        private readonly IClassService _classService;
        private readonly IEncryptingService _encryptingService;

        public ExelParser(IClassService classService, IEncryptingService encryptingService)
        {
            _classService = classService;
            _encryptingService = encryptingService;
        }

        public void CreateClassForUser(string filePath, string userId)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read);
            //TODO exel file extension validation

            //Choose one of either 1 or 2
            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
            //IExcelDataReader excelReader = ExcelReaderFactory.CreateBinaryReader(stream);

            //2. Reading from a OpenXml Excel file (2007 format; *.xlsx)
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);

            //Choose one of either 3, 4, or 5
            //3. DataSet - The result of each spreadsheet will be created in the result.Tables
            DataSet result = excelReader.AsDataSet();

            //4. DataSet - Create column names from first row
            //excelReader.IsFirstRowAsColumnNames = true;
            //DataSet result = excelReader.AsDataSet();

            //5. Data Reader methods
            var clases = new List<ClassDomain>();

            for (int i = 0; i < result.Tables.Count; i++)
            {
                var @class = new ClassDomain();
                @class.Name = result.Tables[i].TableName;
                @class.CreatedBy = userId;

                DataTable sheet = result.Tables[i];


                // skip first rows
                for (int j = 2; j < sheet.Rows.Count; j++)
                {
                    if (sheet.Rows[j].ItemArray[0].ToString() == "")
                    {
                        break;
                    }

                    var student = new StudentDomain
                    {
                        Number = int.Parse(sheet.Rows[j].ItemArray[0].ToString()),
                        FirstName = sheet.Rows[j].ItemArray[1].ToString(),
                        MiddleName = sheet.Rows[j].ItemArray[2].ToString(),
                        LastName = sheet.Rows[j].ItemArray[3].ToString()
                    };

                    var monthId = 1;

                    for (var k = 4; k <= sheet.Rows[j].ItemArray.Length; k += 2)
                    {
                        if (sheet.Rows[j].ItemArray[k].ToString() == "" && sheet.Rows[j].ItemArray[k + 1].ToString() == "")
                        {
                            break;
                        }

                        var absence = new AbsenceDomain();

                        double excusedAbsence;
                        string excusedAbsenceAsString = sheet.Rows[j].ItemArray[k].ToString();

                        if (double.TryParse(excusedAbsenceAsString, out excusedAbsence))
                        {
                            absence.Excused = excusedAbsence;
                        }
                        else
                        {
                            absence.Excused = 0;
                        }

                        double notExcusedAbsence;
                        string notExcusedAbsenceAsString = sheet.Rows[j].ItemArray[k + 1].ToString();

                        if (double.TryParse(notExcusedAbsenceAsString, out notExcusedAbsence))
                        {
                            absence.NotExcused = notExcusedAbsence;
                        }
                        else
                        {
                            absence.NotExcused = 0;
                        }

                        absence.MonthId = monthId;

                        student.Absences.Add(absence);
                        monthId++;
                    }

                    @class.Students.Add(student);
                }

                clases.Add(@class);
            }

            _classService.AddRange(clases);

            excelReader.Close();
        }
    }
}
