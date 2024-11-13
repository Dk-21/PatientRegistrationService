using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using PatientRegistrationService.Models;

namespace PatientRegistrationService.Data
{
    public class PatientRepository : IPatientRepository
    {
        private readonly DataContext _context;

        public PatientRepository(DataContext context)
        {
            _context = context;
        }

        // Add a new patient to the database
        public void AddPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            _context.SaveChanges();
        }

        // Retrieve all patients from the database
        public List<Patient> GetAllPatients()
        {
            return _context.Patients.ToList();
        }

        // Retrieve a specific patient by their ID
        public Patient GetPatient(Guid id)
        {
            return _context.Patients.Find(id);
        }

        // Update an existing patient's details
        public void UpdatePatient(Patient patient)
        {
            _context.Patients.Update(patient);
            _context.SaveChanges();
        }

        // Remove a patient from the database by ID
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
