using System;
using System.Collections.Generic;
using PatientRegistrationService.Models;

namespace PatientRegistrationService.Data
{
    public interface IPatientRepository
    {
        IEnumerable<Patient> GetAllPatients();
        Patient GetPatient(Guid id);
        void AddPatient(Patient patient);
        void UpdatePatient(Patient patient);
        void RemovePatient(Guid id);
    }
}
