using System;
using System.Collections.Generic;
using PatientRegistrationService.Models;

namespace PatientRegistrationService.Data
{
    public interface IPatientRepository
    {
        void AddPatient(Patient patient);
        List<Patient> GetAllPatients();
        Patient GetPatient(Guid id);
        void UpdatePatient(Patient patient);
        void RemovePatient(Guid id);
    }
}
