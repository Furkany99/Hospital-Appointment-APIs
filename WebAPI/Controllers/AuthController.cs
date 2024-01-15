using AutoMapper;
using Azure;
using Common.Dto;
using Common.Models.RequestModels.Patient;
using Common.Models.ResponseModels.Firebase;
using DataAccess.Contexts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FirebaseAdmin.Auth;
using static Common.Exceptions.ExceptionHandlingMiddleware;

namespace WebAPI.Controllers
{
	[ApiController]
	[Route("api/auth")]
	public class AuthController : Controller
	{
		private readonly HospitalAppointmentContext _context;
		private readonly IHttpClientFactory _httpClientFactory;
		private readonly AuthService _authService;
		private readonly PatientService _patientService;
		private readonly IMapper _mapper;
		private readonly ILogger<AuthController> _logger;

		public AuthController(HospitalAppointmentContext context, IHttpClientFactory httpClientFactory, AuthService authService, 
			PatientService patientService, IMapper mapper,ILogger<AuthController> logger)
		{
			_context = context;
			_httpClientFactory = httpClientFactory;
			_authService = authService;
			_patientService = patientService;
			_mapper = mapper;
			_logger = logger;
		}


		[HttpPost("login")]
		public async Task<IActionResult> Login(AccountDto accountDto)
		{
			// Firebase Authentication REST API URL
			var firebaseAuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key=AIzaSyCgnYKRl4l8mjgHQSsa_zLCVtPBFE-upr0";

			// Firebase Authentication için gerekli istek verisi
			var requestData = new
			{
				email = accountDto.Email,
				password = accountDto.Password,
				returnSecureToken = true

			};

			// HTTP isteği oluştur
			using (var client = _httpClientFactory.CreateClient())
			{
				var response = await client.PostAsJsonAsync(firebaseAuthUrl, requestData);

				if (response.IsSuccessStatusCode)
				{
					var content = await response.Content.ReadAsStringAsync();
					var firebaseResponse = JsonConvert.DeserializeObject<FirebaseLoginResponse>(content);
					FirebaseToken decoded = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(firebaseResponse.idToken);
					var userIdFromToken = decoded.Uid;
					var user = _authService.GetUserByFirebaseUid(userIdFromToken);
					



					if (user != null)
					{
						var claims = new Dictionary<string, object>();
						string roleClaim;

						switch (user.Role.Name)
						{
							case "Admin":
								roleClaim = "Admin";
								// Admin yetkisi
								break;

							case "Doctor":
								roleClaim = "Doctor";
								// Doktor yetkisi
								break;

							case "Patient":
								roleClaim = "Patient";
								// Hasta yetkisi
								break;

							default:
								roleClaim = string.Empty;
								break;
						}

						if (!string.IsNullOrEmpty(roleClaim))
						{
							claims.Add(ClaimTypes.Role, roleClaim);
							await FirebaseAuth.DefaultInstance.SetCustomUserClaimsAsync(userIdFromToken, claims);
						}

						return Ok(new
						{
							UserId = userIdFromToken,
							Role = user.Role.Name,
							Token = firebaseResponse.idToken,
							Claims = claims
						});
					}

				}
				
				var errorContent = await response.Content.ReadAsStringAsync();			
				throw new Exception();

			}

		}



		[HttpPost("register-patient")]
		[Authorize(Roles = "Admin,Patient")]
		public async Task<IActionResult> RegisterPatient(PatientCreateRequestModel patientCreate)
		{
			var patients = _mapper.Map<PatientDto>(patientCreate);
			var firebaseAuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyCgnYKRl4l8mjgHQSsa_zLCVtPBFE-upr0";

			var requestData = new
			{
				email = patients.Email,
				password = patients.Password,
				returnSecureToken = true
			};

			using (var client = _httpClientFactory.CreateClient())
			{
				try
				{
					var response = await client.PostAsJsonAsync(firebaseAuthUrl, requestData);

					if (response.IsSuccessStatusCode)
					{
						var content = await response.Content.ReadAsStringAsync();
						var firebaseResponse = JsonConvert.DeserializeObject<FirebaseRegisterResponse>(content);
						var tokenHandler = new JwtSecurityTokenHandler();
						var token = tokenHandler.ReadJwtToken(firebaseResponse.IdToken);
						var userId = token.Subject;



						_patientService.CreatePatient(patients, userId);


						var responseModel = new FirebaseRegisterResponse
						{
							IdToken = firebaseResponse.IdToken,
							Email = firebaseResponse.Email,
							FirebaseUid = userId
						};

						return Ok(responseModel);
					}
					else
					{
						var errorContent = await response.Content.ReadAsStringAsync();
						throw new Exception($"Firebase Hasta Kayıt Hatası: {errorContent}");
					}
				}
				catch (Exception ex)
				{
					throw new Exception($"Hasta kaydı sırasında bir hata oluştu: {ex.Message}");
				}
			}
		}

			//[HttpPost("register-doctor")]
			//public async Task<IActionResult> RegisterDoctor([FromBody] DoctorDto doctorDto)
			//{
			//	var firebaseAuthUrl = "https://identitytoolkit.googleapis.com/v1/accounts:signUp?key=AIzaSyCgnYKRl4l8mjgHQSsa_zLCVtPBFE-upr0";

			//	var requestData = new
			//	{
			//		email = doctorDto.Email,
			//		password = doctorDto.Password,
			//		returnSecureToken = true
			//	};

			//	using (var client = _httpClientFactory.CreateClient())
			//	{
			//		try
			//		{
			//			var response = await client.PostAsJsonAsync(firebaseAuthUrl, requestData);

			//			if (response.IsSuccessStatusCode)
			//			{
			//				var content = await response.Content.ReadAsStringAsync();
			//				var firebaseResponse = JsonConvert.DeserializeObject<FirebaseRegisterResponse>(content);

			//				var firebaseUID = firebaseResponse.FirebaseUid;

			//				_doctorService.CreateDoctor(doctorDto);

			//				return Ok("Doktor kaydı başarılı");
			//			}
			//			else
			//			{
			//				var errorContent = await response.Content.ReadAsStringAsync();
			//				return BadRequest($"Firebase Doktor Kayıt Hatası: {errorContent}");
			//			}
			//		}
			//		catch (Exception ex)
			//		{
			//			return StatusCode(500, $"Doktor kaydı sırasında bir hata oluştu: {ex.Message}");
			//		}
			//	}
			//}


		}
	} 


