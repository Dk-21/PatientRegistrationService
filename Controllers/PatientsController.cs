using Microsoft.AspNetCore.Mvc;
using PatientRegistrationService.Data;
using PatientRegistrationService.DTOs;
using PatientRegistrationService.Models;
using System;
using System.Collections.Generic;

namespace PatientRegistrationService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _repository;

        public PatientsController(IPatientRepository repository)
        {
            _repository = repository;
        }

        // GET: api/patients
        [HttpGet]
        public IActionResult GetAllPatients()
        {
            var patients = _repository.GetAllPatients();
            return Ok(patients);
        }

        // GET: api/patients/{id}
        [HttpGet("{id}")]
        public IActionResult GetPatient(Guid id)
        {
            var patient = _repository.GetPatient(id);
            return patient == null ? NotFound(new { Message = $"Patient with ID {id} not found." }) : Ok(patient);
        }

        // POST: api/patients API Call
        [HttpPost]
        public IActionResult RegisterPatient([FromBody] CreatePatientDto patientDto)
        {
            // Validate model state for the DTO
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Map the DTO to the Patient model
            var patient = new Patient
            {
                Name = patientDto.Name,
                MedicalRecordNumber = patientDto.MedicalRecordNumber,
                Age = patientDto.Age,
                Gender = patientDto.Gender,
                Contacts = patientDto.Contacts,
                AdmittingDiagnosis = patientDto.AdmittingDiagnosis
            };

            // Assign AttendingPhysician and Department based on diagnosis
            if (patient.AdmittingDiagnosis.HasValue)
            {
                (patient.AttendingPhysician, patient.Department) = AssignPhysicianAndDepartment(patient.AdmittingDiagnosis.Value);
            }

            _repository.AddPatient(patient);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        // PUT: api/patients/{id}
        [HttpPut("{id}")]
        public IActionResult UpdatePatient(Guid id, [FromBody] UpdatePatientDto updatedPatientDto)
        {
            var existingPatient = _repository.GetPatient(id);
            if (existingPatient == null)
            {
                return NotFound(new { Message = $"Patient with ID {id} not found." });
            }

            // Update only the allowed fields from the DTO (demographic details)
            existingPatient.Name = updatedPatientDto.Name;
            existingPatient.MedicalRecordNumber = updatedPatientDto.MedicalRecordNumber;
            existingPatient.Age = updatedPatientDto.Age;
            existingPatient.Gender = updatedPatientDto.Gender;
            existingPatient.Contacts = updatedPatientDto.Contacts;

            // Save updated patient information
            _repository.UpdatePatient(existingPatient);
            return Ok(existingPatient);
        }

        // DELETE: api/patients/{id}
        [HttpDelete("{id}")]
        public IActionResult DeletePatient(Guid id)
        {
            var patient = _repository.GetPatient(id);
            if (patient == null)
            {
                return NotFound(new { Message = $"Patient with ID {id} not found." });
            }

            // Only allow deletion if AdmittingDiagnosis is null or Unspecified
            if (patient.AdmittingDiagnosis != null && patient.AdmittingDiagnosis != Diagnosis.Unspecified)
            {
                return BadRequest(new { Message = "Cannot delete a patient with an assigned diagnosis other than Unspecified." });
            }

            _repository.RemovePatient(id);
            return NoContent();
        }

        // Helper method to assign attending physician and department based on diagnosis
        private (string, string) AssignPhysicianAndDepartment(Diagnosis diagnosis)
        {
            return diagnosis switch
            {
                Diagnosis.BreastCancer or Diagnosis.LungCancer => ("Dr. Susan Jones", "Department J"),
                _ => ("Dr. Ben Smith", "Department S")
            };
        }
    }
}
