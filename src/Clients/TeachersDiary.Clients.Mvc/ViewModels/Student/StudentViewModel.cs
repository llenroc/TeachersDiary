﻿using System.Collections.Generic;

using TeachersDiary.Clients.Mvc.ViewModels.Absence;
using TeachersDiary.Data.Domain;
using TeachersDiary.Services.Mapping.Contracts;

namespace TeachersDiary.Clients.Mvc.ViewModels.Student
{
    public class StudentViewModel : IMapFrom<StudentDomain>, IMapTo<StudentDomain>
    {
        public StudentViewModel()
        {
            Absences = new List<AbsenceViewModel>();
        }

        public int Id { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public List<AbsenceViewModel> Absences { get; set; }

        public double TotalExcusedAbsences { get; set; }
        
        public double TotalNotExcusedAbsence { get; set; }

        public string TotalNotExcusedAbsenceAsString { get; set; }
    }
}