using System;
using System.Configuration;

namespace GHMS.Helpers
{
    public static class Constants
    {
        public static string API_BASE_URL = Convert.ToString(ConfigurationManager.AppSettings["GHMS_API_BASE_URL"]);
        public static string PAEDIATRIC_AGE_LIMIT = "12";
        public static int NURSE_ENTRY_DELAY = -30;

        // Common end points
        public static readonly string API_AUTH_ENDPOINT = "Authentication/";
        public static readonly string API_DEPARTMENT_ENDPOINT = "Department/";
        public static readonly string API_EMPLOYEE_ENDPOINT = "Employee/";
        public static readonly string API_PATIENT_ENDPOINT = "Patient/";
        public static readonly string API_VISIT_ENDPOINT = "Visit/";
        public static readonly string API_MASTER_ENDPOINT = "Master/";
        public static readonly string API_EPISODE_SERVICE_ENDPOINT = "EpisodeService/";
        
        //Doctor related end points
        public static readonly string API_DOCTOR_DASHBOARD_ENDPOINT = "DoctorDashboard/";
        public static readonly string API_ASSESSMENT_ENDPOINT = "Assesment/";
        public static readonly string API_FOLLOWUP_ENDPOINT = "Followup/";
        public static readonly string API_SERVICE_ENDPOINT = "Service/";
        public static readonly string API_DRUG_ENDPOINT = "Drug/";
        public static readonly string API_DISCHARGE_ENDPOINT = "Discharge/";
        public static readonly string API_MEDICAL_DISCHARGE_ENDPOINT = "MedicalDischarge/";
        public static readonly string API_SUMMARY_TEMPLATE_ENDPOINT = "SummaryTemplate/";                
        public static readonly string API_CHEMO_ENDPOINT = "Chemotherapy/";
        public static readonly string API_CHEMO_ORDER_ENDPOINT = "Chemorder/";
        public static readonly string API_IPADMISSION_ENDPOINT = "IPAdmission/";
        public static readonly string API_IPINITIALASSESSMENT_ENDPOINT = "Ipinitialassessment/";
        public static readonly string API_TRANSFER_CONSULTANT_ENDPPOINT = "PatientTransferHistory/";
        public static readonly string API_MASTER_DISCHARGE_SUMMARY_ENDPOINT = "MasterDischargeSummary/";

        // Nurse related end points
        public static readonly string API_NURSE_DASHBOARD_ENDPOINT = "NurseDashboard/";
        public static readonly string API_NURSE_MONITORING_ENDPOINT = "Monitoring/";
        public static readonly string API_NURSE_ASSESSMENT_ENDPOINT = "NursingAssesment/";
        public static readonly string API_NURSE_CURRENT_DRUG_ENDPOINT = "NurseCurrentDrug/";
        public static readonly string API_NURSE_CHART_ENDPOINT = "Nursechart/";
        public static readonly string API_NURSE_TPRCHART_ENDPOINT = "NurseTPRChart/";
        public static readonly string API_NURSE_VITALSCHART_ENDPOINT = "NurseVitalsChart/"; 
        public static readonly string API_NURSE_PATIENTMOVEMENT_ENDPOINT = "Nursepatientmovement/";
        public static readonly string API_NURSE_DISCHARGE_SUMMARY_ENDPOINT = "PatientDischargeSummary/";
        public static readonly string API_NURSE_PAINASSESSMENT_SUMMARY_ENDPOINT = "NursePainAssesment/";
        public static readonly string API_NURSE_DRUG_RETURN = "DrugReturn/";
        public static readonly string API_NURSE_DRUG_ISSUE = "DrugIssue/";
        public static readonly string API_NURSE_DISCHARGE_CHECKLIST_SUMMARY_ENDPOINT = "DischargeCkecklist/";

        //Secretary related end points
        public static readonly string API_SECRETARY_DASHBOARD_ENDPOINT = "SecretaryDashboard/";

        // Lab related end points
        public static readonly string API_LAB_DASHBOARD_ENDPOINT = "LabDashboard/";
        public static readonly string API_LAB_SAMPLE_ENDPOINT = "LabSample/";
        public static readonly string API_LAB_CONFIGURATION_TESTPARAMETER = "TestParameter/";
        public static readonly string API_LAB_CONFIGURATION_RESULTPARAMETER = "ConfigParameters/";
        public static readonly string API_LAB_CONFIGURATION_TESTGROUP = "TestGroup/";

        //Lab Masters end points
        public static readonly string API_LAB_CONFIGURATION_UOM = "UnitMaster/";
        public static readonly string API_LAB_CONFIGURATION_CONTAINER_TYPE = "MasterContainerType/";      
        public static readonly string API_LAB_CONFIGURATION_SAMPLE_TYPE = "MasterSampleType/";
        public static readonly string API_LAB_CONFIGURATION_COLLECTION_ROUTE = "MasterCollectionRoute/";
        public static readonly string API_LAB_CONFIGURATION_COLLECTION_SITE = "MasterCollectionSite/";
        public static readonly string API_LAB_CONFIGURATION_COLLECTION_METHOD = "MasterCollectionMethod/";


     

        //Nuclear Medicine end points
        public static readonly string API_NUCLEAR_DASHBOARD_ENDPOINT = "NuclearDashboard/";
        public static readonly string API_NUCLEAR_MEDACK_ENDPOINT = "NuclearPatAck/";
        public static readonly string API_NUCLEAR_TEMPLATE_EDITOR = "NuclearEditor/";



        public static readonly string API_RESPONSE_SUCCESS = "SUCCESS";        
        public static readonly string CREATE_SUCCESS_MSG = "Successfully submitted";
        public static readonly string CREATE_ERROR_MSG = "Error encountered while creating.";
        public static readonly string UPDATE_SUCCESS_MSG = "Successfully updated";
        public static readonly string UPDATE_ERROR_MSG = "Error encountered while updating.";
    }

    public static class PATIENT_VISIT_STATUS
    {
        public static readonly string OPEN = "Open";
        public static readonly string CLOSED = "Close";
    }

    public enum DOCTOR_VISIT_STATUS
    {
        Waiting = 0,
        InProgress = 1,
        Complete = 2,
        Discharged = 3
    }
    
    public enum USER_ROLE
    {
        Admin = 0,
        Doctor = 1,
        Nurse = 2,
        Lab = 3,
        LabAdmin = 4
    }

    public enum YES_NO
    {
        Yes = 1,
        No = 0
    }

    public enum PATIENT_MOVEMENT_OPTS
    {
        //ICU = 0,
        //OT = 1,
        //Ward = 2
        Ward = 1,
        PACU = 2,
        Recovery = 3,
        ICU = 4,
        Mortuary = 5
    }
    public enum PATIENT_MOVEMENT_OPTS1
    {
        //ICU = 0,
        //OT = 1,
        //Ward = 2
        Ward = 1,
        PACU = 2,
        Recovery = 3,
        ICU = 4,
       
    }

}