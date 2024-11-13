using System;
using System.Collections.Generic;
using System.Linq;
using PatientRegistrationService.Models;
using Microsoft.EntityFrameworkCore;

namespace PatientRegistrationService.Data
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DataContext _context;

        public PatientRepository(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<Patient> GetAllPatients() => _context.Patients.ToList();

        public Patient GetPatient(Guid id) => _context.Patients.Find(id);

        public void AddPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        public void UpdatePatient(Patient patient)
        {
            _context.Patients.Update(patient);
            _context.SaveChanges();
        }

        public void RemovePatient(Guid id)
        {
            var patient = _context.Patients.Find(id);
            if (patient != null)
            {
                _context.Patients.Remove(patient);
                _context.SaveChanges();
            }
        }
    }
}
