﻿namespace Common.ResponseModels
{
	public class PatientListResponseModel
	{
		public int Count { get; set; }
		public List<PatientResponseModel> patientResponseModels { get; set; } = new List<PatientResponseModel>();

	}
}
