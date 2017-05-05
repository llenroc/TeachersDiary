﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Bytes2you.Validation;

using TeachersDiary.Data.Contracts;
using TeachersDiary.Data.Domain;
using TeachersDiary.Data.Ef.Contracts;
using TeachersDiary.Data.Ef.Entities;
using TeachersDiary.Data.Services.Contracts;
using TeachersDiary.Services.Mapping.Contracts;

namespace TeachersDiary.Data.Services
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepository;
        private readonly IUnitOfWork _contextSaveChanges;
        private readonly IMappingService _mappingService;

        public ClassService(
            IClassRepository classRepository, 
            IUnitOfWork contextSaveChanges, 
            IMappingService mappingService)
        {
            Guard.WhenArgument(classRepository, nameof(classRepository)).IsNull().Throw();
            Guard.WhenArgument(contextSaveChanges, nameof(contextSaveChanges)).IsNull().Throw();
            Guard.WhenArgument(mappingService, nameof(mappingService)).IsNull().Throw();

            _classRepository = classRepository;
            _contextSaveChanges = contextSaveChanges;
            _mappingService = mappingService;
        }

        public async Task<ClassDomain> GetClassWithStudentsByClassIdAsync(int classId)
        {
            var claaEntity = await _classRepository.GetClassWithStudentsAndAbsencesByClassIdAsync(classId);

            var classDomain = _mappingService.Map<ClassDomain>(claaEntity);

            return classDomain;
        }

        public async Task<IEnumerable<ClassDomain>> GetAllAsync()
        {
            var classeEntities = await _classRepository.GetAllAsync();

            var classDomains = _mappingService.Map<IEnumerable<ClassDomain>>(classeEntities);

            return classDomains;
        }

        public void Add(ClassDomain classDomain)
        {
            Guard.WhenArgument(classDomain, nameof(classDomain)).IsNull().Throw();

            var classEntity = _mappingService.Map<ClassEntity>(classDomain);

            _classRepository.Add(classEntity);
            _contextSaveChanges.SaveChanges();
        }

        public void AddRange(List<ClassDomain> classDomains)
        {
            Guard.WhenArgument(classDomains, nameof(classDomains)).IsNull().Throw();

            var classEntities = _mappingService.Map<List<ClassEntity>>(classDomains);

            _classRepository.AddRange(classEntities);
            _contextSaveChanges.SaveChanges();
        }

        public async Task DeleteById(int classId)
        {
            var classEntity = await _classRepository.GetByIdAsync(classId);

            _classRepository.Delete(classEntity);
            _contextSaveChanges.SaveChanges();
        }
    }
}